using System;
using System.Drawing;
using OpenTK.Graphics;
using OpenTK;

namespace OpenTkTest_Lib
{
    public interface IVertex
     {
          int Size { get; }
    }

    public struct ColoredVertex : IVertex
    {

        public const int SizeInBytes = (3 + 4) * 4;
        public int Size { get { return SizeInBytes; } }

        Vector3 pos;
        Color4 col;
        public ColoredVertex(Vector3 pos, Color4 col) {
            this.pos = pos;
            this.col = col;
        }

        public override string ToString() {
            return pos.ToString();
        }
    }
}
