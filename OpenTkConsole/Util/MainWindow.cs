using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Util
{
    public sealed class MainWindow : GameWindow
    {
        private int _program;
        private int _vertexArray;

        public MainWindow()
            : base(1280, // initial width
                    720, // initial height
                    GraphicsMode.Default,
                    "meinFenster",  // initial title
                    GameWindowFlags.Default,
                    DisplayDevice.Default,
                    4, // OpenGL major version
                    5, // OpenGL minor version
                    GraphicsContextFlags.ForwardCompatible) {
            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);
        }


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
            GL.DeleteProgram(_program);
            GL.DeleteVertexArrays(1, ref _vertexArray);
            base.Exit();
        }
        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, Width, Height);
        }



        protected override void OnUpdateFrame(FrameEventArgs e) {
            HandleKeyboard();
        }

        private double _time = 0;
        protected override void OnRenderFrame(FrameEventArgs e) {
           Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";
            _time += e.Time;

            Color4 backColor = new Color4()
            {
                A = 1.0f,
                R = 0.1f,
                G = 0.1f,
                B = 0.3f
            };

            GL.ClearColor(backColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(_program);
            //add attr


            GL.DrawArrays(PrimitiveType.Points, 0, 1);
            GL.PointSize(10);
            SwapBuffers();
        }

        private void HandleKeyboard() {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key.Escape)) {
                Exit();
            }
        }

        private int CompileShaders() {
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            string verShader = OpenTkConsole.Properties.Resources.vertexShader.ToString();
            GL.ShaderSource(vertexShader, verShader);
            GL.CompileShader(vertexShader);

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            string fagShader = OpenTkConsole.Properties.Resources.fragmentShader.ToString();
            GL.ShaderSource(fragmentShader, fagShader);
            GL.CompileShader(fragmentShader);
            var program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            string info = GL.GetShaderInfoLog(fragmentShader);
            Console.WriteLine(info);
            string info2 = GL.GetShaderInfoLog(vertexShader);
            Console.WriteLine(info2);
            string info3 = GL.GetProgramInfoLog(program);
            Console.WriteLine(info3);

            return program;
        }
    }
}