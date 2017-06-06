using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;


namespace lightingText
{
    class Program
    {
        static void Main(string[] args) {
            new GameWindow().Run(30);
        }
    }



    class GameWindow : OpenTK.GameWindow
    {

        int vaoHandle;
        Matrix4 proj = Matrix4.Identity;
        Matrix4 view = Matrix4.Identity;
        Matrix4 model = Matrix4.Identity;

        public GameWindow()
            : base(1280, 720, GraphicsMode.Default, "TestTitle", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible) {
            Console.WriteLine("OpenGl: " + GL.GetString(StringName.Version));
            Console.WriteLine("GLSL: " + GL.GetString(StringName.ShadingLanguageVersion));
        }

        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        #region data
        float[] vertices = {
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
        float[] test = { -0.5f, 0.0f, 0.0f,
                          0.5f, 0.0f, 0.0f,
                          0.0f, 0.5f, 0.0f
                         };

        string vertexSrc = @"#version 450

layout (location = 0) in vec3 pos;
layout (location = 2) uniform mat4 translate;

void main(){
    gl_Position =translate * vec4(pos,1.0);
}";
        string fragmentSrc = @"#version 450
out vec4 outColor;

uniform mat4 lighColor;
uniform mat4 objectColor;

void main(){
    outColor = vec4(lightColor * objectColor, 1.0);
}";
        #endregion


        protected override void OnLoad(EventArgs e) {


            GL.GenBuffers(1, out int vboHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * 4, vertices, BufferUsageHint.StaticDraw);

            GL.GenVertexArrays(1, out vaoHandle);
            GL.BindVertexArray(vaoHandle);

            #region shader
            int vertHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertHandle, vertexSrc);
            GL.CompileShader(vertHandle);

            int fragHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragHandle, fragmentSrc);
            GL.CompileShader(fragHandle);

            int progHandle = GL.CreateProgram();
            GL.AttachShader(progHandle, vertHandle);
            GL.AttachShader(progHandle, fragHandle);
            GL.LinkProgram(progHandle);
            GL.UseProgram(progHandle);
            Console.WriteLine("Shaderprogram: " + GL.GetProgramInfoLog(progHandle));
            GL.DetachShader(progHandle, vertHandle);
            GL.DetachShader(progHandle, fragHandle);

            #endregion

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * 4, 0);

            Debug.WriteLine(GL.GetUniformLocation(progHandle, "translate"));

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.GenVertexArrays(1, out int lightVao);
            GL.BindVertexArray(lightVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer,vboHandle);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * 4, 0);




            view = Matrix4.LookAt(new Vector3(1, 1, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            model = Matrix4.CreateScale(0.5f);
            proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), this.Width / this.Height, 0.1f, 100.0f);
        }



        protected override void OnRenderFrame(FrameEventArgs e) {
            GL.ClearColor(Color4.Aqua);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 transform = model * view * proj;

            GL.BindVertexArray(vaoHandle);
            GL.UniformMatrix4(2, false, ref transform);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);

            this.SwapBuffers();
        }
    }



}
