using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using OpenTkTest_Lib;

namespace Open.gl_tutorial
{
    class GameWindow3 : OpenTK.GameWindow
    {
        public GameWindow3()
            : base(1280, 720, GraphicsMode.Default, "TestFenster2", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible) {
            System.Console.WriteLine("GL: " + GL.GetString(StringName.Version));
            System.Console.WriteLine("GLSL: " + GL.GetString(StringName.ShadingLanguageVersion));
        }

        float[] vertices =
            {
                1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
            };
        uint[] elements =
        {
            0,1,2
            };

        Vector3[] positions =
        {
            new Vector3(0,0,0),
            new Vector3(-1,-1,0)
    };


        int vaoHandle;
        ShaderProgram shaderProgram;

        protected override void OnLoad(EventArgs e) {
            //1. VAO
            GL.GenVertexArrays(1, out vaoHandle);
            GL.BindVertexArray(vaoHandle);

            //ebo / vbo
            int[] buffers = new int[2];
            GL.GenBuffers(2, buffers);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffers[0]);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffers[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, 4 * vertices.Length, vertices, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, elements.Length * sizeof(uint) , elements, BufferUsageHint.StaticDraw);


            #region shaders
            //src
            string vertSrc = @"#version 450
in vec3 position;
layout (location = 20) uniform mat4 trans;


void main(){
    gl_Position = trans * vec4(position, 1.0);
}
";
            string fragSrc = @"#version 450
out vec4 outColor;
void main(){
    outColor = vec4(1.0, 1.0, 0.0, 1.0);
}

";

            Shader vertexShader = new Shader(ShaderType.VertexShader, vertSrc);
            Shader fragShader = new Shader(ShaderType.FragmentShader, fragSrc);

            shaderProgram = new ShaderProgram(vertexShader,fragShader);         
            GL.BindFragDataLocation(shaderProgram, 0, "outColor");
            

            int loc = GL.GetAttribLocation(shaderProgram, "position");
            GL.EnableVertexAttribArray(loc);
            GL.VertexAttribPointer(loc, 3, VertexAttribPointerType.Float, false, 12, 0);
           
            #endregion

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            //draw
            for (int i = 0; i < positions.Length; i++) {
                Matrix4 trans = Matrix4.CreateTranslation(positions[i]);
                GL.BindVertexArray(vaoHandle);
                
                GL.UniformMatrix4(20, false, ref trans);

                //GL.DrawElements(BeginMode.Triangles, 3, DrawElementsType.UnsignedInt, 0);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

                GL.BindVertexArray(0);
            }
            this.SwapBuffers();

        }

        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, Width, Height);
        }

    }
}
