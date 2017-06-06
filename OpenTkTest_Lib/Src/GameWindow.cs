using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using System;
using System.Collections.Generic;

namespace OpenTkTest_Lib
{
    public class GameWindow : OpenTK.GameWindow
    {
        private double timeSinceStart;

        public GameWindow()
      : base(1280, 720, GraphicsMode.Default, "TestFenster2", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible) {
            Console.WriteLine("GL: " + GL.GetString(StringName.Version));
            Console.WriteLine("GLSL: " + GL.GetString(StringName.ShadingLanguageVersion));
        }

        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, Width, Height);
        }

        RenderObject<ColoredVertex> cube;
        protected override void OnLoad(EventArgs e) {
            //vorbereitung
            VertexBuffer<ColoredVertex> vbo = new VertexBuffer<ColoredVertex>();
            ElementBuffer ebo = new ElementBuffer();

            Shader vert = new Shader(ShaderType.VertexShader, Shader.LoadFile("vertex.c"));
            Shader frag = new Shader(ShaderType.FragmentShader, Shader.LoadFile("fragment.c"));
            ShaderProgram prog = new ShaderProgram(vert, frag);

            ShaderAttribute posAttrib = new ShaderAttribute("position", 3, VertexAttribPointerType.Float, ColoredVertex.SizeInBytes, 0);
            ShaderAttribute colAttrib = new ShaderAttribute("color", 4, VertexAttribPointerType.Float, ColoredVertex.SizeInBytes, 12);
            ShaderAttribute[] attribs = { posAttrib, colAttrib };

            cube = new RenderObject<ColoredVertex>(prog, attribs, vbo, ebo);

            cube.AddVertex(new ColoredVertex(new Vector3(0.5f, -0.5f, 0.5f), Color4.Red)); //urv   0
            cube.AddVertex(new ColoredVertex(new Vector3(0.5f, -0.5f, -0.5f), Color4.Green)); //urh  1
            cube.AddVertex(new ColoredVertex(new Vector3(-0.5f, -0.5f, 0.5f), Color4.Blue)); //ulv  2
            cube.AddVertex(new ColoredVertex(new Vector3(-0.5f, -0.5f, -0.5f), Color4.Red)); //ulh 3

            cube.AddVertex(new ColoredVertex(new Vector3(0.5f, 0.5f, 0.5f), Color4.Green)); //orv    4
            cube.AddVertex(new ColoredVertex(new Vector3(0.5f, 0.5f, -0.5f), Color4.Blue)); //orh   5
            cube.AddVertex(new ColoredVertex(new Vector3(-0.5f, 0.5f, 0.5f), Color4.Red)); //olv   6
            cube.AddVertex(new ColoredVertex(new Vector3(-0.5f, 0.5f, -0.5f), Color4.Green)); //olh  7

            uint[] elem = {
                            //unten
                            2,0,1, 2,1,3,
                            //vorn
                            2,0,4, 2,4,6,
                            //hinte
                            1,3,7, 1,7,5,
                            //rechts
                            1,4,0, 1,4,5,
                            //links
                            2,3,7, 2,6,7,
                            //oben
                            6,4,5, 6,5,7
                        };
            cube.SetElements(elem);

            cube.FinalizeObject();



            vbo.UploadData();
            ebo.UploadData();
            vbo.Unbind();
            ebo.Unbind();


            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            timeSinceStart += e.Time;

            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //draw
            Matrix4 rX = Matrix4.CreateRotationX((float)timeSinceStart);
            Matrix4 rY = Matrix4.CreateRotationY((float)-timeSinceStart);
            Matrix4 rZ = Matrix4.CreateRotationZ((float)timeSinceStart);


            Matrix4 view = Matrix4.LookAt(new Vector3(-4, -4, 2), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), Width / Height, 0.1f, 100f);
            cube.Draw((rX * rY * rZ) * view * proj);
           
            this.SwapBuffers();
        }

    }
}
