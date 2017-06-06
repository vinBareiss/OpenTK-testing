using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

namespace UtilLib
{
    public class BaseGameWindow : OpenTK.GameWindow
    {
        public BaseGameWindow()
            : base(1280, 720, GraphicsMode.Default, "BaseGameWindow", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible) {
            Debug.WriteLine("--------------------------------------------------------------");
            Debug.WriteLine("OpenGl: " + GL.GetString(StringName.Version));
            Debug.WriteLine("GLSL: " + GL.GetString(StringName.ShadingLanguageVersion));
        }
        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, Width, Height);
        }

        public string GetFps(double deltaTime) {
            return $"FPS: {1f / deltaTime:0}";
        }
    }
}
