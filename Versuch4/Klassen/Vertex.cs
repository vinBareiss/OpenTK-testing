using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Versuch4
{
    struct ColoredVertex
    {
        public const int Size = (3 + 4) * 4; //size in bytes

        private readonly Vector3 position;
        private readonly Color4 color;

        public ColoredVertex(Vector3 pos, Color4 col) {
            color = col;
            position = pos;
        }
    }

    sealed class VertexBuffer<TVertex>
        where TVertex : struct
    {
        private readonly int vertexSize;
        private TVertex[] vertecies = new TVertex[4];

        private int count;

        private readonly int handle;

        public VertexBuffer(int vertexSize) {
            this.vertexSize = vertexSize;
            this.handle = GL.GenBuffer();
        }

        public void AddVertex(TVertex v) {
            if (this.count == this.vertecies.Length) {
                Array.Resize(ref this.vertecies, this.count * 2);
            }
            this.vertecies[count] = v;
            this.count++;
        }

        public void Bind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.handle);
        }

        public void BufferData() {
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(this.vertexSize * this.count), this.vertecies, BufferUsageHint.StreamDraw);
        }

        public void Draw() {
            GL.DrawArrays(PrimitiveType.Triangles, 0, this.count);
        }
    }

    sealed class VertexArray<TVertex>
        where TVertex : struct
    {
        private readonly int handle;

        public VertexArray(VertexBuffer<TVertex> vertexBuffer, ShaderProgram program, params VertexAttribute[] attributes) {
            GL.GenVertexArrays(1, out this.handle);

            this.Bind();
            vertexBuffer.Bind();

            foreach (var attribute in attributes) {
                attribute.Set(program);
            }

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Bind() {
            GL.BindVertexArray(this.handle);
        }
    }

    sealed class VertexAttribute
    {
        private readonly string name;
        private readonly int size;
        private readonly VertexAttribPointerType type;
        private readonly bool normalize;
        private readonly int stride;
        private readonly int offset;

        public VertexAttribute(string name, int size, VertexAttribPointerType type, int stride, int offset, bool normalize = false) {
            this.name = name;
            this.size = size;
            this.type = type;
            this.stride = stride;
            this.offset = offset;
            this.normalize = normalize;
        }
        public void Set(ShaderProgram program) {
            int index = program.GetAttributeLocation(this.name);

            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, this.size, this.type, this.normalize, this.stride, this.offset);
        }
    }

    sealed class Matrix4Uniform
    {
        private readonly string name;
        private Matrix4 matrix;

        public Matrix4 Matrix
        {
            get { return this.matrix; }
            set { this.matrix = value; }
        }

        public Matrix4Uniform(string name) {
            this.name = name;
        }

        public void Set(ShaderProgram program) {
            var i = program.GetUniformLocation(this.name);
            GL.UniformMatrix4(i, false, ref this.matrix);
        }
    }


}


