using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;


namespace OpenTkTest_Lib
{
    class RenderObject<TVertex> where TVertex : struct {
        private bool finalized = false;

        private ShaderProgram mProg;

        private VertexBuffer<TVertex> mVbo;
        private ElementBuffer mEbo;
        private VertexArray<TVertex> mVao;

        private List<TVertex> mVertices;
        private int mVerticesOffset;
        private List<uint> mElements;
        private int mElementsOffset;

        public RenderObject(ShaderProgram prog, ShaderAttribute[] attrib, VertexBuffer<TVertex> vbo = null, ElementBuffer ebo = null) {
            //vorbereitung
            mProg = prog;
            mVbo = vbo;
            mEbo = ebo;
            mVertices = new List<TVertex>();
            mElements = new List<uint>();

            //vao
            mVao = new VertexArray<TVertex>(vbo, ebo, prog, attrib);
        }

        public void AddVertex(TVertex vert) {
            mVertices.Add(vert);
        }
        public void AddVertices(IEnumerable<TVertex> vertices) {
            mVertices.AddRange(vertices);
        }

        public void SetElements(uint[] elements) {
            mElements.AddRange(elements);
        }

        public void FinalizeObject() {
            finalized = true;
            mVerticesOffset = mVbo.Length;
            mVbo.AddData(mVertices);
            mElementsOffset = mEbo.Length;
            mEbo.AddElements(mElements);
        }

        public void Draw(Matrix4 model) {
            int uniloc = mProg.GetUniformLoc("transform");
            mVao.Bind();
            GL.UniformMatrix4(uniloc, false, ref model);
            GL.DrawElements(BeginMode.Triangles, mElements.Count, DrawElementsType.UnsignedInt, 0);
            mVao.Unbind();
        }




    }
}
