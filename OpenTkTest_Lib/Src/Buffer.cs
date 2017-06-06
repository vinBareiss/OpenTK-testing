using System;
using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace OpenTkTest_Lib
{ 
    /// <summary>
    /// 
    /// </summary>
    public abstract class Buffer
    {
        /// <summary>
        /// OpenGL handle für den Buffer
        /// </summary>
        int mHandle;
        public int Handle { get { return mHandle; } }

        public Buffer(int handle) {
            mHandle = handle;
        }

        public abstract void Bind();
        public abstract void Unbind();

        /// <summary>
        /// Komfortfunktion, erlaubt this = mHandle
        /// </summary>
        /// <param name="b"></param>
        public static implicit operator int(Buffer b) {
            return b.mHandle;
        }
    }
    

    /// <summary>
    /// 
    /// </summary>
    public class VertexBuffer <TVertex> : Buffer
        where TVertex : struct
    {
        /// <summary>
        /// Länge der zu dem VBO upzulodenden Daten zu diesem Moment
        /// </summary>
        public int Length { get { return verticesArray.Count; } }

        /// <summary>
        /// CPU seitiger Array, wird in VBO hochgeladen
        /// </summary>
        private List<TVertex> verticesArray;

        /// <summary>
        /// Constructor
        /// </summary>
        public VertexBuffer() : base(GL.GenBuffer()) {
            verticesArray = new List<TVertex>();
        }

        /// <summary>
        /// Läd die in dem CPU seitigen Array vorhandenen Daten auf die GPU hoch
        /// </summary>
        public void UploadData() {
            this.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, verticesArray.Count * ColoredVertex.SizeInBytes, verticesArray.ToArray(), BufferUsageHint.StreamDraw);
            this.Unbind();
        }

        /// <summary>
        /// Fügt eine Vertex zu dem CPU seitigen Array hinzu
        /// </summary>
        /// <param name="vert"></param>
        public void AddData(TVertex vert) {
            verticesArray.Add(vert);
        }
        /// <summary>
        /// Fügt mehrere Vertices zu dem CPU seitegen Array hinzu, !Läd nicht hoch!
        /// </summary>
        /// <param name="data">Hinzuzufügende Daten</param>
        public void AddData(IEnumerable<TVertex> data) {
            verticesArray.AddRange(data);
        }

        /// <summary>
        /// Binded dieses VBO
        /// </summary>
        public override void Bind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this);
        }
        /// <summary>
        /// Setzt gebundenes VBO auf 0
        /// </summary>
        public override void Unbind() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }


    /// <summary>
    /// TODO: DOC
    /// </summary>
    public class ElementBuffer : Buffer    
    {

        public int Length { get { return elementArray.Count; } }
        private List<uint> elementArray;

        public ElementBuffer() : base(GL.GenBuffer()) {
            elementArray = new List<uint>();
        }

        public void UploadData() {
            this.Bind();
            GL.BufferData(BufferTarget.ElementArrayBuffer, elementArray.Count * sizeof(uint), elementArray.ToArray(), BufferUsageHint.StaticDraw);
            this.Unbind();
        }

        public void AddElement(uint element) {
            elementArray.Add(element);
        }

        public void AddElements(IEnumerable<uint> elements) {
            elementArray.AddRange(elements);
        }
        
        public override void Bind() {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this);
        }
        public override void Unbind() {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }


   /// <summary>
   /// 
   /// </summary>
    public class VertexArray <TVertex> : Buffer where TVertex : struct
    {
        /// <summary>
        /// Zusammenfassung der verwendeten Shader
        /// </summary>
        ShaderProgram mProg;

        /// <summary>
        /// Constructor, binded die ShaderAttribute
        /// </summary>
        /// <param name="vbo">Ein VBO um Vertices zu speichern</param>
        /// <param name="ebo">Ein EVO zum schnelleren Abruf des VBO</param>
        /// <param name="prog">Zusammenfassung der Shader</param>
        /// <param name="attribs">Alle Shader Attribute</param>
        public VertexArray(VertexBuffer<TVertex> vbo, ElementBuffer ebo, ShaderProgram prog, params ShaderAttribute[] attribs) : base(GL.GenVertexArray()) {
            mProg = prog;
            this.Bind();
            vbo.Bind();
            ebo.Bind();

            foreach (ShaderAttribute attrib in attribs) {
                attrib.Set(prog);
            }

            this.Unbind();
            vbo.Unbind();
            ebo.Unbind();
        }

        /// <summary>
        /// Bindet das VAO (und EAO, VBO) und aktiviert Program zum zeichen
        /// </summary>
        public override void Bind() {
            mProg.Use();
            GL.BindVertexArray(this);
        }

        /// <summary>
        /// Setzt VAO und Program auf 0
        /// </summary>
        public override void Unbind() {
            ShaderProgram.UnUse();
            GL.BindVertexArray(0);
        }
    }
}