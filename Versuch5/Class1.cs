using System;
using OpenTK;
//this namespace allows us to use only OpenGL 4, with all deprecated items removed.
using OpenTK.Graphics.OpenGL4;


namespace Tutorial
{
    class GL4Window : GameWindow
    {
        private Matrix4 ProjectionMatrix;
        private Matrix4 WorldMatrix;
        private Matrix4 ModelviewMatrix;

        private Vector3 CameraPosition;

        private Shader shader;
        private VertexFloatBuffer buffer;

        public GL4Window(int width = 800, int height = 600)
            : base(width, height,
            OpenTK.Graphics.GraphicsMode.Default,
            "Tutorial GL4 Window",
            GameWindowFlags.Default,
            DisplayDevice.Default,
            //Major M inor implicitly assigned to 4.0
            //It's best to set to your version of GL
            //so look atthe method below for help.
            //**do not set to a version above your own
            4, 0,
            //Make sure that we are only using 4.0 related stuff.
            OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible) {

        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            #region GL_VERSION
            //this will return your version of opengl
            GL.GetInteger(GetPName.MajorVersion, out int major);
            GL.GetInteger(GetPName.MinorVersion, out int minor);
            Console.WriteLine("Major {0}\nMinor {1}", major, minor);
            //you can also get your GLSL version, although not sure if it varies from the above
            Console.WriteLine("GLSL {0}", GL.GetString(StringName.ShadingLanguageVersion));
            #endregion

            //set clear color [black]
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            //setup projection this tutorial is for 3D ill make another about 2D
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.5f, 10000.0f);
            WorldMatrix = new Matrix4();
            ModelviewMatrix = new Matrix4();

            CameraPosition = new Vector3(0.5f, 0.5f, 0);

            //setup the shader
            //To use the Shader class you will have to implicitly type
            //the layout location of each attribute
            //else the vbo wont know where they go when binding
            //here i only used 2 attributes since thats all i need for this example
            //but you can always write more to these, or atleast give it a go
            //it can be really fun :D
            string vertex_source =
            @"#version 400
            layout (location = 0) in vec3 vertex_position;
            layout (location = 1) in vec4 vertex_color;

            uniform mat4 mvp_matrix;

            out vec4 color;

            void main(void)
            {
                color = vertex_color;
                //ref line 124
                gl_Position = mvp_matrix * vec4(vertex_position, 1.0);
            }";

            //during the rasterization process each pixel that will be processed (excluding the glclearcolor)
            //to the viewport will go through this pixel shader
            //initially any processed pixel does not have an actual color
            //you can set it from here but since this example uses vertex coloring
            //the color is passed from the [vertex shader] out vec4 color -> [pixelshader] in vec4 color
            //and then output through frag_color
            string fragment_source =
            @"#version 400

            layout (location = 0) out vec4 frag_color;

            in vec4 color;

            void main(void)
            {
	            frag_color = color;
            }";

            shader = new Shader(ref vertex_source, ref fragment_source);

            //setup the vertex buffer [vbo]
            buffer = new VertexFloatBuffer(VertexFormat.XYZ_COLOR, 3);
            //just a triangle with full r g b
            buffer.AddVertex(0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f);
            buffer.AddVertex(0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f);
            buffer.AddVertex(1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f);

            buffer.IndexFromLength();
            buffer.Load();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            //prepare data to shader

            //set the world matrix
            WorldMatrix = Matrix4.CreateTranslation(-CameraPosition);

            //set out triangle position with the modelview matrix
            ModelviewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -2.0f);

            //combine all matrices
            //the different between GL and GLSL with matrix order
            //GL   modelview * worldview * projection;
            //GLSL projection * worldview * modelview;
            Matrix4 MVP_Matrix = ModelviewMatrix * WorldMatrix * ProjectionMatrix;

            //send to shader
            GL.UseProgram(shader.Program);
            //will return -1 without useprogram
            int mvp_matrix_location = GL.GetUniformLocation(shader.Program, "mvp_matrix");
            GL.UniformMatrix4(mvp_matrix_location, false, ref MVP_Matrix);
            GL.UseProgram(0);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //render
            //this useprogram is redundant and not needed in 'this' example
            //with one shader and one object, but good practice for expanding
            //to multiple objects and shaders
            GL.UseProgram(shader.Program);
            buffer.Bind(shader);
            GL.UseProgram(0);

            SwapBuffers();
        }

        #region Shader.cs

        //This will hold our shader code in a nice clean class
        //this example only uses a shader with position and color
        //but didnt want to leave out the other bits for the shader
        //so you could practice writing a shader on your own :P
        public class Shader
        {
            public string VertexSource { get; private set; }
            public string FragmentSource { get; private set; }

            public int VertexID { get; private set; }
            public int FragmentID { get; private set; }

            public int Program { get; private set; }

            public int PositionLocation { get; set; }
            public int NormalLocation { get; set; }
            public int TexCoordLocation { get; set; }
            public int ColorLocation { get; set; }

            public Shader(ref string vs, ref string fs) {
                VertexSource = vs;
                FragmentSource = fs;

                Build();
            }

            private void Build() {

                VertexID = GL.CreateShader(ShaderType.VertexShader);
                FragmentID = GL.CreateShader(ShaderType.FragmentShader);

                // Compile vertex shader
                GL.ShaderSource(VertexID, VertexSource);
                GL.CompileShader(VertexID);
                GL.GetShaderInfoLog(VertexID, out string info);
                GL.GetShader(VertexID, ShaderParameter.CompileStatus, out int status_code);

                if (status_code != 1)
                    throw new ApplicationException(info);

                // Compile fragment shader
                GL.ShaderSource(FragmentID, FragmentSource);
                GL.CompileShader(FragmentID);
                GL.GetShaderInfoLog(FragmentID, out info);
                GL.GetShader(FragmentID, ShaderParameter.CompileStatus, out status_code);

                if (status_code != 1)
                    throw new ApplicationException(info);

                Program = GL.CreateProgram();
                GL.AttachShader(Program, FragmentID);
                GL.AttachShader(Program, VertexID);

                GL.LinkProgram(Program);

                GL.UseProgram(Program);
                //layout dependent locations
                PositionLocation = GL.GetAttribLocation(Program, "vertex_position");
                NormalLocation = GL.GetAttribLocation(Program, "vertex_normal");
                TexCoordLocation = GL.GetAttribLocation(Program, "vertex_texcoord");
                ColorLocation = GL.GetAttribLocation(Program, "vertex_color");

                if (PositionLocation >= 0)
                    GL.BindAttribLocation(Program, PositionLocation, "vertex_position");
                if (NormalLocation >= 0)
                    GL.BindAttribLocation(Program, NormalLocation, "vertex_normal");
                if (TexCoordLocation >= 0)
                    GL.BindAttribLocation(Program, TexCoordLocation, "vertex_texcoord");
                if (ColorLocation >= 0)
                    GL.BindAttribLocation(Program, ColorLocation, "vertex_color");

                GL.UseProgram(0);
            }

            public void Dispose() {
                if (Program != 0)
                    GL.DeleteProgram(Program);
                if (FragmentID != 0)
                    GL.DeleteShader(FragmentID);
                if (VertexID != 0)
                    GL.DeleteShader(VertexID);
            }
        }

        #endregion

        #region VertexFloatBuffer.cs

        public enum VertexFormat
        {
            XY,
            XY_COLOR,
            XY_UV,
            XY_UV_COLOR,

            XYZ,
            XYZ_COLOR,
            XYZ_UV,
            XYZ_UV_COLOR,
            XYZ_NORMAL_UV,
            XYZ_NORMAL_UV_COLOR
        }

        //Unlike the last vertex buffer, this one is an update
        //i ran into some problems with speed and found out it was the buffer class
        //so i rewrote to this which is x100 faster
        //if you have a hard time understanding what a VBO is you can lookat
        //http://deathbyalgorithm.blogspot.com/2013/10/opentk-vertex-buffer-object-vbo.html

        public class VertexFloatBuffer
        {
            public VertexFormat Format { get; private set; }
            public int Stride { get; private set; }
            public int AttributeCount { get; private set; }
            public int TriangleCount { get { return index_data.Length / 3; } }
            public int VertexCount { get { return vertex_data.Length / AttributeCount; } }
            public bool IsLoaded { get; private set; }
            public BufferUsageHint UsageHint { get; set; }
            public BeginMode DrawMode { get; set; }

            public int VBO { get { return id_vbo; } }
            public int EBO { get { return id_ebo; } }

            private int id_vbo;
            private int id_ebo;

            private int vertex_position;
            private int index_position;

            protected float[] vertex_data;
            protected uint[] index_data;

            public VertexFloatBuffer(VertexFormat format, int limit = 1024) {
                Format = format;
                SetStride();
                UsageHint = BufferUsageHint.StreamDraw;
                DrawMode = BeginMode.Triangles;

                vertex_data = new float[limit * AttributeCount];
                index_data = new uint[limit];
            }

            public void Clear() {
                vertex_position = 0;
                index_position = 0;
            }

            public void SetFormat(VertexFormat format) {
                Format = format;
                SetStride();
                Clear();
            }

            private void SetStride() {
                switch (Format) {
                    case VertexFormat.XY:
                        Stride = 8;
                        break;
                    case VertexFormat.XY_COLOR:
                        Stride = 24;
                        break;
                    case VertexFormat.XY_UV:
                        Stride = 16;
                        break;
                    case VertexFormat.XY_UV_COLOR:
                        Stride = 32;
                        break;
                    case VertexFormat.XYZ:
                        Stride = 12;
                        break;
                    case VertexFormat.XYZ_COLOR:
                        Stride = 28;
                        break;
                    case VertexFormat.XYZ_UV:
                        Stride = 20;
                        break;
                    case VertexFormat.XYZ_UV_COLOR:
                        Stride = 36;
                        break;
                    case VertexFormat.XYZ_NORMAL_UV:
                        Stride = 32;
                        break;
                    case VertexFormat.XYZ_NORMAL_UV_COLOR:
                        Stride = 48;
                        break;
                }

                AttributeCount = Stride / sizeof(float);
            }

            public void Set(float[] vertices, uint[] indices) {
                Clear();
                vertex_data = vertices;
                index_data = indices;
            }

            /// <summary>
            /// Load vertex buffer into a VBO in OpenGL
            /// :: store in memory
            /// </summary>
            public void Load() {
                if (IsLoaded) return;

                //VBO
                GL.GenBuffers(1, out id_vbo);
                GL.BindBuffer(BufferTarget.ArrayBuffer, id_vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertex_position * sizeof(float)), vertex_data, UsageHint);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.GenBuffers(1, out id_ebo);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, id_ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(index_position * sizeof(uint)), index_data, UsageHint);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                IsLoaded = true;
            }

            /// <summary>
            /// Reload the buffer data without destroying the buffers pointer to OpenGL
            /// </summary>
            public void Reload() {
                if (!IsLoaded) return;

                GL.BindBuffer(BufferTarget.ArrayBuffer, id_vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertex_position * sizeof(float)), vertex_data, UsageHint);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, id_ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(index_position * sizeof(uint)), index_data, UsageHint);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            /// <summary>
            /// Unload vertex buffer from OpenGL
            /// :: release memory
            /// </summary>
            public void Unload() {
                if (!IsLoaded) return;

                GL.BindBuffer(BufferTarget.ArrayBuffer, id_vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertex_position * sizeof(float)), IntPtr.Zero, UsageHint);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, id_ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(index_position * sizeof(uint)), IntPtr.Zero, UsageHint);

                GL.DeleteBuffers(1, ref id_vbo);
                GL.DeleteBuffers(1, ref id_ebo);

                IsLoaded = false;
            }

            public void Bind(Shader shader) {
                if (!IsLoaded) return;

                GL.BindBuffer(BufferTarget.ArrayBuffer, id_vbo);

                switch (Format) {
                    case VertexFormat.XY:
                        GL.EnableVertexAttribArray(shader.PositionLocation);
                        GL.VertexAttribPointer(shader.PositionLocation, 2, VertexAttribPointerType.Float, false, Stride, 0);
                        break;
                    case VertexFormat.XY_COLOR:
                        GL.EnableVertexAttribArray(shader.PositionLocation);
                        GL.EnableVertexAttribArray(shader.ColorLocation);
                        GL.VertexAttribPointer(shader.PositionLocation, 2, VertexAttribPointerType.Float, false, Stride, 0);
                        GL.VertexAttribPointer(shader.ColorLocation, 4, VertexAttribPointerType.Float, false, Stride, 8);
                        break;
                    case VertexFormat.XY_UV:
                        GL.EnableVertexAttribArray(shader.PositionLocation);
                        GL.EnableVertexAttribArray(shader.TexCoordLocation);
                        GL.VertexAttribPointer(shader.PositionLocation, 2, VertexAttribPointerType.Float, false, Stride, 0);
                        GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 8);
                        break;
                    case VertexFormat.XY_UV_COLOR:
                        GL.EnableVertexAttribArray(shader.PositionLocation);
                        GL.EnableVertexAttribArray(shader.TexCoordLocation);
                        GL.EnableVertexAttribArray(shader.ColorLocation);
                        GL.VertexAttribPointer(shader.PositionLocation, 2, VertexAttribPointerType.Float, false, Stride, 0);
                        GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 8);
                        GL.VertexAttribPointer(shader.ColorLocation, 4, VertexAttribPointerType.Float, false, Stride, 16);
                        break;
                    case VertexFormat.XYZ:
                        GL.EnableVertexAttribArray(shader.PositionLocation);
                        GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                        break;
                    case VertexFormat.XYZ_COLOR:
                        GL.EnableVertexAttribArray(shader.PositionLocation);
                        GL.EnableVertexAttribArray(shader.ColorLocation);
                        GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                        GL.VertexAttribPointer(shader.ColorLocation, 4, VertexAttribPointerType.Float, false, Stride, 12);
                        break;
                    case VertexFormat.XYZ_UV:
                        GL.EnableVertexAttribArray(shader.PositionLocation);
                        GL.EnableVertexAttribArray(shader.TexCoordLocation);
                        GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                        GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 12);
                        break;
                    case VertexFormat.XYZ_UV_COLOR:
                        GL.EnableVertexAttribArray(shader.PositionLocation);
                        GL.EnableVertexAttribArray(shader.TexCoordLocation);
                        GL.EnableVertexAttribArray(shader.ColorLocation);
                        GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                        GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 12);
                        GL.VertexAttribPointer(shader.ColorLocation, 4, VertexAttribPointerType.Float, false, Stride, 20);
                        break;
                    case VertexFormat.XYZ_NORMAL_UV:
                        GL.EnableVertexAttribArray(shader.PositionLocation);
                        GL.EnableVertexAttribArray(shader.NormalLocation);
                        GL.EnableVertexAttribArray(shader.ColorLocation);
                        GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                        GL.VertexAttribPointer(shader.NormalLocation, 3, VertexAttribPointerType.Float, false, Stride, 12);
                        GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 24);
                        break;
                    case VertexFormat.XYZ_NORMAL_UV_COLOR:
                        GL.EnableVertexAttribArray(shader.PositionLocation);
                        GL.EnableVertexAttribArray(shader.NormalLocation);
                        GL.EnableVertexAttribArray(shader.TexCoordLocation);
                        GL.EnableVertexAttribArray(shader.ColorLocation);
                        GL.VertexAttribPointer(shader.PositionLocation, 3, VertexAttribPointerType.Float, false, Stride, 0);
                        GL.VertexAttribPointer(shader.NormalLocation, 3, VertexAttribPointerType.Float, false, Stride, 12);
                        GL.VertexAttribPointer(shader.TexCoordLocation, 2, VertexAttribPointerType.Float, false, Stride, 24);
                        GL.VertexAttribPointer(shader.ColorLocation, 4, VertexAttribPointerType.Float, false, Stride, 32);
                        break;
                }

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, id_ebo);
                GL.DrawElements(DrawMode, index_position, DrawElementsType.UnsignedInt, 0);

                GL.DisableVertexAttribArray(0);
                GL.DisableVertexAttribArray(1);
                GL.DisableVertexAttribArray(2);
                GL.DisableVertexAttribArray(3);
            }

            public void Dispose() {
                Unload();
                Clear();
                vertex_data = null;
                index_data = null;
            }

            /// <summary>
            /// Add indices in order of vertices length,
            /// this is if you dont want to set indices and just index from vertex-index
            /// </summary>
            public void IndexFromLength() {
                int count = vertex_position / AttributeCount;
                index_position = 0;
                for (uint i = 0; i < count; i++) {
                    index_data[index_position++] = i;
                }
            }

            public void AddIndex(uint indexA, uint indexB, uint indexC) {
                index_data[index_position++] = indexA;
                index_data[index_position++] = indexB;
                index_data[index_position++] = indexC;
            }

            public void AddIndices(uint[] indices) {
                for (int i = 0; i < indices.Length; i++) {
                    index_data[index_position++] = indices[i];
                }
            }

            public void AddVertex(float x, float y) {
                if (Format != VertexFormat.XY)
                    throw new FormatException("vertex must be of the same format type as buffer");

                vertex_data[vertex_position++] = x;
                vertex_data[vertex_position++] = y;
            }

            public void AddVertex(float x, float y, float r, float g, float b, float a) {
                if (Format != VertexFormat.XY_COLOR)
                    throw new FormatException("vertex must be of the same format type as buffer");

                vertex_data[vertex_position++] = x;
                vertex_data[vertex_position++] = y;
                vertex_data[vertex_position++] = r;
                vertex_data[vertex_position++] = g;
                vertex_data[vertex_position++] = b;
                vertex_data[vertex_position++] = a;
            }

            public void AddVertex(float x, float y, float z) {
                if (Format != VertexFormat.XYZ)
                    throw new FormatException("vertex must be of the same format type as buffer");

                vertex_data[vertex_position++] = x;
                vertex_data[vertex_position++] = y;
                vertex_data[vertex_position++] = z;
            }

            public void AddVertex(float x, float y, float z, float r, float g, float b, float a) {
                if (Format != VertexFormat.XYZ_COLOR)
                    throw new FormatException("vertex must be of the same format type as buffer");

                vertex_data[vertex_position++] = x;
                vertex_data[vertex_position++] = y;
                vertex_data[vertex_position++] = z;
                vertex_data[vertex_position++] = r;
                vertex_data[vertex_position++] = g;
                vertex_data[vertex_position++] = b;
                vertex_data[vertex_position++] = a;
            }

            public void AddVertex(float x, float y, float z, float u, float v) {
                if (Format != VertexFormat.XYZ_UV)
                    throw new FormatException("vertex must be of the same format type as buffer");

                vertex_data[vertex_position++] = x;
                vertex_data[vertex_position++] = y;
                vertex_data[vertex_position++] = z;
                vertex_data[vertex_position++] = u;
                vertex_data[vertex_position++] = v;
            }

            public void AddVertex(float x, float y, float z, float u, float v, float r, float g, float b, float a) {
                if (Format != VertexFormat.XYZ_UV_COLOR)
                    throw new FormatException("vertex must be of the same format type as buffer");

                vertex_data[vertex_position++] = x;
                vertex_data[vertex_position++] = y;
                vertex_data[vertex_position++] = z;
                vertex_data[vertex_position++] = u;
                vertex_data[vertex_position++] = v;
                vertex_data[vertex_position++] = r;
                vertex_data[vertex_position++] = g;
                vertex_data[vertex_position++] = b;
                vertex_data[vertex_position++] = a;
            }

            public void AddVertex(float x, float y, float z, float nx, float ny, float nz, float u, float v) {
                if (Format != VertexFormat.XYZ_NORMAL_UV)
                    throw new FormatException("vertex must be of the same format type as buffer");

                vertex_data[vertex_position++] = x;
                vertex_data[vertex_position++] = y;
                vertex_data[vertex_position++] = z;
                vertex_data[vertex_position++] = nx;
                vertex_data[vertex_position++] = ny;
                vertex_data[vertex_position++] = nz;
                vertex_data[vertex_position++] = u;
                vertex_data[vertex_position++] = v;
            }

            public void AddVertex(float x, float y, float z, float nx, float ny, float nz, float u, float v, float r, float g, float b, float a) {
                if (Format != VertexFormat.XYZ_NORMAL_UV_COLOR)
                    throw new FormatException("vertex must be of the same format type as buffer");

                vertex_data[vertex_position++] = x;
                vertex_data[vertex_position++] = y;
                vertex_data[vertex_position++] = z;
                vertex_data[vertex_position++] = nx;
                vertex_data[vertex_position++] = ny;
                vertex_data[vertex_position++] = nz;
                vertex_data[vertex_position++] = u;
                vertex_data[vertex_position++] = v;
                vertex_data[vertex_position++] = r;
                vertex_data[vertex_position++] = g;
                vertex_data[vertex_position++] = b;
                vertex_data[vertex_position++] = a;
            }
        }

        #endregion
    }
}
