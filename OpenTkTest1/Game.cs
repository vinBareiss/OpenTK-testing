using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.IO;

namespace OpenTkTest1
{
    class Game : GameWindow
    {
        int pgmID;
        int vsID;
        int fsID;
        int attribute_vcol;
        int attribute_vpos;
        int uniform_mview;
        int vbo_position;
        int vbo_color;
        int vbo_mview;
        Vector3[] vertdata;
        Vector3[] coldata;
        Matrix4[] mviewdata;

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            initProgram();

            vertdata = new Vector3[] { new Vector3(-0.8f, -0.8f, 0f),
                new Vector3( 0.8f, -0.8f, 0f),
                new Vector3( 0f,  0.8f, 0f)};


            coldata = new Vector3[] { new Vector3(1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f,  1f, 0f)};


            mviewdata = new Matrix4[]{
                Matrix4.Identity
            };



            Title = "Hello OpenTK!";
            GL.ClearColor(Color.CornflowerBlue);
            GL.PointSize(5f);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {

            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SwapBuffers();
        }

        private void initProgram() {
            pgmID = GL.CreateProgram();
            loadShader("vs.glsl", ShaderType.VertexShader, pgmID, out vsID);
            loadShader("fs.glsl", ShaderType.FragmentShader, pgmID, out fsID);
            GL.LinkProgram(pgmID);

            Console.WriteLine(GL.GetProgramInfoLog(pgmID));
            attribute_vpos = GL.GetAttribLocation(pgmID, "vPosition");
            attribute_vcol = GL.GetAttribLocation(pgmID, "vColor");
            uniform_mview = GL.GetUniformLocation(pgmID, "modelview");

            if (attribute_vpos == -1 || attribute_vcol == -1 || uniform_mview == -1) {
                Console.WriteLine("Error binding attributes");
            }
            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out vbo_color);
            GL.GenBuffers(1, out vbo_mview);

        }

        void loadShader(String filename, ShaderType type, int program, out int address) {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename)) {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }



    }
}
