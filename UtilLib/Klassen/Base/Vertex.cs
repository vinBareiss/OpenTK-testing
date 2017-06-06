using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
namespace UtilLib
{
    interface IVertex
    {
        void SetupVertexPointer();
    }

    struct Vertex3P2UV3N : IVertex
    {
        int sizeInBytes;
        Vector3 pos;
        Vector2 uv;
        Vector3 norm;

        public Vertex3P2UV3N(Vector3 pos, Vector2 uv, Vector3 norm) {
            this.pos = pos;
            this.uv = uv;
            this.norm = norm;
            sizeInBytes = (3 + 2 + 3) * 4;
        }

        public void SetupVertexPointer() {
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeInBytes, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeInBytes, 3 * 4);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, sizeInBytes, 5 * 4);
        }
    }
}
