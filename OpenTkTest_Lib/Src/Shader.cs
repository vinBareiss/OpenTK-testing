using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace OpenTkTest_Lib
{
    public class Shader
    {
        private readonly int mHandle;
        private readonly string mSrc;

        public ShaderType Type { get; private set; }

        public Shader(ShaderType type, string src) {
            mHandle = GL.CreateShader(type);
            mSrc = src;
            this.Type = type;

            GL.ShaderSource(mHandle, src);
            GL.CompileShader(mHandle);

            string status = GL.GetShaderInfoLog(mHandle);
            if (status != "")
                Console.WriteLine(type.ToString() + " " + status);
            //throw new ApplicationException(status);
            else
                Console.WriteLine(type.ToString() + " erfolgreich");
        }

        public static string LoadFile(string path) {
            using (StreamReader sr = new StreamReader(path)) {
                return sr.ReadToEnd();
            }
        }

        public static implicit operator int(Shader s) {
            return s.mHandle;
        }

    }

    public class ShaderAttribute
    {
        private string mName;
        private int mSize;
        private VertexAttribPointerType mType;
        private bool mNorm;
        private int mStride;
        private int mOffset;

        public ShaderAttribute(string name, int size, VertexAttribPointerType type, int stride, int offset, bool normalize = false) {
            mName = name;
            mSize = size;
            mType = type;
            mStride = stride;
            mOffset = offset;
            mNorm = normalize;
        }

        public void Set(ShaderProgram program) {
            // get location of attribute from shader program
            int index = program.GetAttribLoc(this.mName);

            // enable and set attribute
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, this.mSize, this.mType, this.mNorm, this.mStride, this.mOffset);
        }
    }

    public class ShaderProgram
    {
        private int mHandle;

        public ShaderProgram(Shader vertexShader, Shader fragmentShader) {
            mHandle = GL.CreateProgram();
            if (vertexShader.Type == ShaderType.VertexShader)
                GL.AttachShader(mHandle, vertexShader);
            else
                throw new ApplicationException("VertexShader not of type Vertexshader");

            if (fragmentShader.Type == ShaderType.FragmentShader)
                GL.AttachShader(mHandle, fragmentShader);
            else
                throw new ApplicationException("FragmentShader not of type Fragmentshader");

            GL.LinkProgram(mHandle);
            string status = GL.GetProgramInfoLog(mHandle);
            if (status != "")
                //throw new ApplicationException(status);
                Console.WriteLine(status);
            else
                Console.WriteLine("Programm erfolgreich");
            GL.DetachShader(mHandle, vertexShader);
            GL.DetachShader(mHandle, fragmentShader);
        }

        public ShaderProgram(string vertexSrc, string fragmentSrc) :this(
            new Shader(ShaderType.VertexShader, vertexSrc),
            new Shader(ShaderType.FragmentShader, fragmentSrc)) {} 

        public void Use() {
            GL.UseProgram(mHandle);
        }

        public int GetAttribLoc(string name) {
            return GL.GetAttribLocation(mHandle, name);
        }

        public int GetUniformLoc(string name) {
            return GL.GetUniformLocation(mHandle, name);
        }

        public static void UnUse() {
            GL.UseProgram(0);
        }

        public static implicit operator int(ShaderProgram p) {
            return p.mHandle;
        }
    }
}
