using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace UtilLib
{
    public abstract class OGLHandle
    {
        public int Handle { get; private set; }

        public OGLHandle(int handle) {
            this.Handle = handle;
        }
        public static implicit operator int(OGLHandle h) {
            return h.Handle;
        }

    }

    public abstract class Buffer : OGLHandle
    {
        public Buffer(int handle) : base(handle) {
        }

        public abstract void Bind();
        public abstract void Unbind();
    }

    public class VertexBuffer<T> : Buffer
      where T : struct
    {
        int mVertexSize;
        public VertexBuffer() : base(GL.GenBuffer()) {
            //initialisation
            mVertexSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        }

        public void BufferData(T[] data) {
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * mVertexSize, data, BufferUsageHint.StaticDraw);
        }

        public override void Bind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this);
        }

        public override void Unbind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
    public class VertexArray : Buffer
    {
        public VertexArray() : base(GL.GenVertexArray()) {

        }

        public void SetVertexAttrib<T>(int index, int size, VertexAttribPointerType type, bool normalize, int stride, int offset) {
            int sizeOfType = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            
            GL.VertexAttribPointer(index, size, type, normalize, stride * sizeOfType, offset * sizeOfType);
            GL.EnableVertexAttribArray(index);
        }

        public override void Bind() {
            GL.BindVertexArray(this);
        }

        public override void Unbind() {
            GL.BindVertexArray(0);
        }
    }
    public class IndexBuffer : Buffer
    {
        public IndexBuffer() : base(GL.GenBuffer()) { }

        public void BufferData(uint[] data) {
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.StaticDraw);
        }

        public override void Bind() {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this);
        }

        public override void Unbind() {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
