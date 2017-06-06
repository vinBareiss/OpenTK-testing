using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
//using OpenTK.Graphics.OpenGL4;

namespace Versuch4
{
    sealed class GameWindow : OpenTK.GameWindow
    {
        private VertexBuffer<ColoredVertex> vertexBuffer;
        private ShaderProgram shaderProgram;
        private VertexArray<ColoredVertex> vertexArray;
        private Matrix4Uniform projectionMatrix;

        //private Matrix4 projectionMatrix;

        private Vector3 cameraUp = Vector3.UnitY; // which way is up for the camera

        public GameWindow()
            : base(1280, 720, GraphicsMode.Default, "TestTitle", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.ForwardCompatible) {
            Console.WriteLine($"GL version: {GL.GetString(StringName.Version)}");
            Console.WriteLine($"Shader Version: {GL.GetString(StringName.ShadingLanguageVersion)}");
        }


        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        protected override void OnLoad(EventArgs e) {
            this.vertexBuffer = new VertexBuffer<ColoredVertex>(ColoredVertex.Size);

            this.vertexBuffer.AddVertex(new ColoredVertex(new Vector3(-1, -1, -1.5f), Color4.Lime));
            this.vertexBuffer.AddVertex(new ColoredVertex(new Vector3(1, 1, -1.5f), Color4.Red));
            this.vertexBuffer.AddVertex(new ColoredVertex(new Vector3(1, -1, -1.5f), Color4.Blue));



            // load shaders
            #region Shaders

            var vertexShader = new Shader(ShaderType.VertexShader,
@"#version 130
// a projection transformation to apply to the vertex' position
uniform mat4 projectionMatrix;
// attributes of our vertex
in vec3 vPosition;
in vec4 vColor;
out vec4 fColor; // must match name in fragment shader
void main()
{
    // gl_Position is a special variable of OpenGL that must be set
	gl_Position = projectionMatrix * vec4(vPosition, 1.0);
	fColor = vColor;
}"
                );
            var fragmentShader = new Shader(ShaderType.FragmentShader,
@"#version 130
in vec4 fColor; // must match name in vertex shader
out vec4 fragColor; // first out variable is automatically written to the screen
void main()
{
    fragColor = fColor;
}"
                );

            #endregion

            this.shaderProgram = new ShaderProgram(vertexShader, fragmentShader);
            this.vertexArray = new VertexArray<ColoredVertex>(this.vertexBuffer, this.shaderProgram,
                new VertexAttribute("vPosition", 3, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, ColoredVertex.Size, 0),
                new VertexAttribute("vColor", 4, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, ColoredVertex.Size, 12)
            );
            this.projectionMatrix = new Matrix4Uniform("projectionMatrix");
            projectionMatrix.Matrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 16f / 9, 0.1f, 100f);

          /*  Vector3 test = new Vector3(-0.5f, 0f, 0f);
            Matrix4 posMat = Matrix4.CreateTranslation(test);

            this.projectionMatrix.Matrix *= posMat;
*/       }

        float t;
        protected override void OnUpdateFrame(FrameEventArgs e) {
            t += (float)e.Time;
            this.Title = t.ToString() ;
            if(t > 1) {
                t = 0;
                Vector3 test = new Vector3(0.1f, 0.0f, 0.0f);
                Matrix4 posMat = Matrix4.CreateScale(test);

                this.projectionMatrix.Matrix *= posMat;

            }
        }



        
        protected override void OnRenderFrame(FrameEventArgs e) {
            //t += (float) e.Time;

           

            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            this.shaderProgram.Use();
            this.projectionMatrix.Set(this.shaderProgram);

            vertexBuffer.Bind();
            vertexArray.Bind();

            vertexBuffer.BufferData();
            vertexBuffer.Draw();

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.UseProgram(0);


            this.SwapBuffers();
        }
    }
}
