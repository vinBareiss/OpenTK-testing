using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

using UtilLib;

namespace lightOOP_test
{
    class TexTestWindow : BaseGameWindow
    {
        string vertPath = "Shader/test/texTestVert.c";
        string fragPath = "Shader/test/texTestFrag.c";

        float[] vertices = {
            //pos               textCoord
            0.5f,  0.5f, 0.0f,  1.0f, 1.0f,   // Top Right
            0.5f, -0.5f, 0.0f,  1.0f, 0.0f,   // Bottom Right
           -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,   // Bottom Left
           -0.5f,  0.5f, 0.0f,  0.0f, 1.0f    // Top Left 
        };

        uint[] indices =
        {
            0,1,2,
            0,2,3
        };


        VertexArray vao;
        ShaderProgram prog;

        IndexBuffer ibo;
        VertexBuffer<float> vbo;

        Texture textureWall, textureContainer;
        protected override void OnLoad(EventArgs e) {
            prog = new ShaderProgram(Shader.FromFile(vertPath), Shader.FromFile(fragPath));

            textureWall = new Texture(System.Drawing.Image.FromFile("Data/wall.jpg"), 0, "wall");
            textureContainer = new Texture(System.Drawing.Image.FromFile("Data/container2.png"), 1, "container");



            vbo = new VertexBuffer<float>();
            vbo.Bind();
            vbo.BufferData(vertices);


            vao = new VertexArray();
            vao.Bind();

            ibo = new IndexBuffer();
            ibo.Bind();
            ibo.BufferData(indices);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * 4, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * 4, 3 * 4);
            GL.EnableVertexAttribArray(1);

            vao.Unbind();
            ibo.Unbind();
            vbo.Unbind();
            prog.Use();

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }


        float t;
        protected override void OnRenderFrame(FrameEventArgs e) {
            t += (float)e.Time;

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            prog.Use();


            textureWall.ActivateTextureUnit();
            textureWall.Bind();
            GL.Uniform1(prog.GetUniformLoc("ourTexture1"), textureWall.TextureUnitAsInt);

            textureContainer.ActivateTextureUnit();
            textureContainer.Bind();
            GL.Uniform1(prog.GetUniformLoc("ourTexture2"), textureContainer.TextureUnitAsInt);


            vao.Bind();
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);


            vao.Unbind();

            this.SwapBuffers();

        }
    }
}
