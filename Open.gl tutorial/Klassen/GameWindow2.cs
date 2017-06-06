using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Open.gl_tutorial
{
    public struct Vertex
    {
        public const int SIZE = (3 + 4) * sizeof(float);

        Vector3 mPos;
        Color4 mCol;

        public Vertex(Vector3 pos, Color4 col) {
            mPos = pos;
            mCol = col;
        }
    }


    public class GameWindow2 : OpenTK.GameWindow
    {
        public GameWindow2()
            : base(1280, 720, GraphicsMode.Default, "TestFenster2", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible) {
            System.Console.WriteLine("GL: " + GL.GetString(StringName.Version));
            System.Console.WriteLine("GLSL: " + GL.GetString(StringName.ShadingLanguageVersion));
        }
        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        private int progHandle;

        private Matrix4 view, projection;


        protected override void OnLoad(EventArgs e) {

            Vertex[] vertices =
            {
                new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), Color.Red), //ol
                new Vertex(new Vector3(0.5f, -0.5f, 0.0f), Color.Red), //or
                new Vertex(new Vector3(-0.5f, 0.5f, 0.0f), Color.Red), //ul
                new Vertex(new Vector3(0.5f, 0.5f, 0.0f), Color.Red) //ur
            };
            uint[] elements =
            {
                0,1,3,
                0,3,2

            };
            float[] cubeVert = {
                        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
                         0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
                         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

                        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                         0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                         0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                        -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
                        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

                        -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                        -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                        -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                         0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                         0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                         0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                         0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
                         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

                        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                        -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
                        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
                    };


            //1. Create VBO
            GL.CreateBuffers(1, out int vboHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, cubeVert.Length * sizeof(float), cubeVert, BufferUsageHint.StaticDraw);

            //2. Create VAO
            GL.GenVertexArrays(1, out int vaoHandle);
            GL.BindVertexArray(vaoHandle);
            //2. Create EBO
            GL.CreateBuffers(1, out int eaoHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eaoHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, elements.Length * sizeof(uint), elements, BufferUsageHint.StaticDraw);

            //4. Shader
            #region code
            string vertexSrc = @"#version 450

in vec3 position;
in vec2 texCoord;

out vec2 TexCoord;

uniform mat4 translate;

void main(){
    gl_Position = translate * vec4(position, 1.0);
    TexCoord = texCoord;
}
";
            string fragSrc = @"#version 450
in vec2 TexCoord;

uniform sampler2D texSampler;

out vec4 outColor;
void main(){
    vec4 texCol = texture(texSampler, TexCoord);
    outColor = texCol;
}
";
            #endregion

            int vertHandle = GL.CreateShader(ShaderType.VertexShader); //create Vertexshader and compile
            GL.ShaderSource(vertHandle, vertexSrc);
            GL.CompileShader(vertHandle);
            Console.WriteLine("Vertex: " + GL.GetShaderInfoLog(vertHandle));

            int fragHandle = GL.CreateShader(ShaderType.FragmentShader); //create Fragmentshader and compile
            GL.ShaderSource(fragHandle, fragSrc);
            GL.CompileShader(fragHandle);
            Console.WriteLine("Fragment: " + GL.GetShaderInfoLog(fragHandle));

            progHandle = GL.CreateProgram();
            GL.AttachShader(progHandle, vertHandle);
            GL.AttachShader(progHandle, fragHandle);
            GL.BindFragDataLocation(progHandle, 0, "outColor");
            GL.LinkProgram(progHandle);
            GL.UseProgram(progHandle);
            GL.DetachShader(progHandle, vertHandle);
            GL.DetachShader(progHandle, fragHandle);


            //5. Shader Bindings
            int posAttrHandle = GL.GetAttribLocation(progHandle, "position");
            GL.EnableVertexAttribArray(posAttrHandle);
            GL.VertexAttribPointer(posAttrHandle, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int colAttrHandle = GL.GetAttribLocation(progHandle, "texCoord");
            GL.EnableVertexAttribArray(colAttrHandle);
            GL.VertexAttribPointer(colAttrHandle, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            Console.WriteLine(GL.GetError());

            //6. Textures;
            GL.GenTextures(1, out int textureHandle);
            Bitmap bmpKatze = (Bitmap)Bitmap.FromFile("wall.jpg"); //load texture
            System.Drawing.Imaging.BitmapData dataKatze = bmpKatze.LockBits(new Rectangle(0, 0, bmpKatze.Width, bmpKatze.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle); //bind texture to operate on it
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpKatze.Width, bmpKatze.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dataKatze.Scan0);

            bmpKatze.UnlockBits(dataKatze);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);            //set texture parameter
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);


            //7. final settings
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Enable(EnableCap.DepthTest);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, this.Width / this.Height, 0.1f, 100.0f);
            keys = new bool[1024];

            //8. unbind for clean
            /* GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
               GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
               GL.BindTexture(TextureTarget.Texture2D, 0);
               GL.BindVertexArray(0);
           */
        }

        float t;

        protected override void OnUpdateFrame(FrameEventArgs e) {
            t += (float)e.Time;
            this.Title = t.ToString();
        }


        Vector3 camPos = new Vector3(0, 0, 3);
        Vector3 camFront = new Vector3(0, 0, -1.0f);
        Vector3 camUp = new Vector3(0, 1, 0);
        bool[] keys;

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            keys[e.ScanCode] = true;
            Console.WriteLine(e.ScanCode);
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            keys[e.ScanCode] = false;
        }
        void HandleMove() {
            float camSpeed = 0.1f;
            if (keys[(int)Key.W] == true)
                camPos += camFront * camSpeed;
            if (keys[(int)Key.S] == true)
                camPos -= camFront * camSpeed;
            if (keys[(int)Key.A] == true)
                camPos -= Vector3.Normalize(Vector3.Cross(camFront, camUp))* camSpeed;
            if (keys[(int)Key.D] == true)
                camPos += Vector3.Normalize(Vector3.Cross(camFront, camUp)) * camSpeed;
            if (keys[(int)Key.F] == true)
                camPos += camUp * camSpeed;
            if (keys[(int)Key.C] == true)
                camPos -= camUp * camSpeed;
        }

        float yaw, pitch;
        protected override void OnMouseMove(MouseMoveEventArgs e) {
            if (!e.Mouse.IsAnyButtonDown)
                return;

            float sensitivity = 0.1f;

            yaw -= e.XDelta * sensitivity;
            pitch -= e.YDelta * sensitivity;

            if (pitch > 89.0f)
                pitch = 89.0f;
            if (pitch < -89.0f)
                pitch = -89.0f;

            Vector3 front = new Vector3()
            {
                X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float) Math.Cos(MathHelper.DegreesToRadians(yaw)),
                Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch)),
                Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw))
            };

            camFront = Vector3.Normalize(front);
        }

        float fov = 45;
        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            if (fov >= 1.0f && fov <= 45.0f)
                fov -= e.DeltaPrecise;
            if (fov <= 1.0f)
                fov = 1.0f;
            if (fov >= 45.0f)
                fov = 45.0f;

            Console.WriteLine(fov);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), this.Width / this.Height, 0.1f, 100.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            HandleMove();
            view = Matrix4.LookAt(camPos, camPos + camFront, camUp);

            Vector3[] pos =
            {
            new Vector3(0,0,0),
            new Vector3(0,0,-2),
            new Vector3(2,2,0)
            };
            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);



            Matrix4 translate;
            int translLoc = GL.GetUniformLocation(progHandle, "translate");


            foreach (Vector3 vec in pos) {
                //build model matrix


                Matrix4 scl = Matrix4.CreateScale(1);

                Matrix4 rX = Matrix4.CreateRotationX(0);
                Matrix4 rY = Matrix4.CreateRotationY(0);
                Matrix4 rZ = Matrix4.CreateRotationZ(0);

                Matrix4 trans = Matrix4.CreateTranslation(vec);

                Matrix4 model = scl * (rX * rY * rZ) * trans;



                translate = model * view * projection;
                GL.UniformMatrix4(translLoc, false, ref translate);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
            //  GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            this.SwapBuffers();


            
        }

    }
}
