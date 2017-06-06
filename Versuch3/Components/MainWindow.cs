using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System.IO;

namespace Versuch3
{
    public sealed class MainWindow : GameWindow
    {
        private string _title = "MeinFenster";

        public MainWindow()
    : base(1280, // initial width
        720, // initial height
        GraphicsMode.Default,
        "dreamstatecoding",  // initial title
        GameWindowFlags.Default,
        DisplayDevice.Default,
        4, // OpenGL major version
        0, // OpenGL minor version
        GraphicsContextFlags.ForwardCompatible) {
            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
        }

        private double _time;
        protected override void OnRenderFrame(FrameEventArgs e) {
            _time += e.Time;
            Title = $"{_title}: (Vsync: {VSync}) FPS: {1f / e.Time:0}";
            Color4 backColor;
            backColor.A = 1.0f;
            backColor.R = 0.1f;
            backColor.G = 0.1f;
            backColor.B = 0.3f;
            GL.ClearColor(backColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(_program);

            // add shader attributes here
            GL.VertexAttrib1(0, _time);
            Vector4 position;
            position.X = (float)Math.Sin(_time) * 0.5f;
            position.Y = (float)Math.Cos(_time) * 0.5f;
            position.Z = 0.0f;
            position.W = 1.0f;
            GL.VertexAttrib4(1, position);
         


            GL.DrawArrays(PrimitiveType.Points, 0, 1);
            GL.PointSize(10);
            SwapBuffers();
        }



        private int CompileShaders() {
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, File.ReadAllText(@"Components\Shaders\vertexShader.vert"));
            GL.CompileShader(vertexShader);

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, File.ReadAllText(@"Components\Shaders\fragmentShader.frag"));
            GL.CompileShader(fragmentShader);

            var program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            return program;
        }

        private int _program;
        private int _vertexArray;

        protected override void OnLoad(EventArgs e) {
            CursorVisible = true;
            _program = CompileShaders();
            GL.GenVertexArrays(1, out _vertexArray);
            GL.BindVertexArray(_vertexArray);
            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs eventArgs) {
            Exit();
        }

        public override void Exit() {
            GL.DeleteVertexArrays(1, ref _vertexArray);
            GL.DeleteProgram(_program);
            base.Exit();
        }


    }


}