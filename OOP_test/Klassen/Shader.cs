using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OOP_test.Klassen
{
    class Shader : IDisposable
    {
        private int mHandle;
        public int Handle
        {
            get { return mHandle; }
            private set { mHandle = value; }
        }

        private ShaderType mType;

        private readonly string mSrc;

        public static implicit operator int(Shader s) {
            return s.mHandle;
        }

        public Shader(ShaderType type, string src) {
            mSrc = src;
            mType = type;
            //tell oGL to create shader
            mHandle = GL.CreateShader(type);

            GL.ShaderSource(this, mSrc);
            GL.CompileShader(this);

            //check for error
            GL.GetShader(this, ShaderParameter.CompileStatus, out int statusCode);
            if (statusCode != 1) {
                GL.GetShaderInfoLog(this, out string info);
                throw new ApplicationException(info);
            }
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }

  
    public class ShaderProgram
    {
        /// <summary>
        /// Constructor for <see cref="Name"/>
        /// </summary>
        public ShaderProgram() {

        }
    }

    public class NAME
    {
        #region
        #endregion
    }
}
