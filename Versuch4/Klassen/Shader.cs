using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Versuch4
{
    sealed class Shader
    {
        private readonly int handle;

        public int Handle { get { return this.handle; } }

        public Shader(ShaderType type, string code) {
            this.handle = GL.CreateShader(type);

            GL.ShaderSource(this.handle, code);
            GL.CompileShader(this.handle);
        }
    }

    sealed class ShaderProgram
    {
        private readonly int handle;

        public ShaderProgram(params Shader[] shaders) {
            this.handle = GL.CreateProgram();

            foreach (var shader in shaders) {
                GL.AttachShader(this.handle, shader.Handle);
            }
            GL.LinkProgram(this.handle);

            foreach (var shader in shaders) {
                GL.DetachShader(this.handle, shader.Handle);
            }
        }

        public void Use() {
            GL.UseProgram(this.handle);
        }

        public int GetAttributeLocation(string name) {
            return GL.GetAttribLocation(this.handle, name);
        }

        public int GetUniformLocation(string name) {
            return GL.GetUniformLocation(this.handle, name);
        }
    }
}
