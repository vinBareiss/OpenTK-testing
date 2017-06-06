using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using OpenTK.Input;

using UtilLib;

namespace lightOOP_test
{
    class Gamewindow : BaseGameWindow
    {
        const string basicVertexSrc = "Shader/basic_vertex.c";
        const string lampFragSrc = "Shader/lamp_frag.c";
        const string lightingFragSrc = "Shader/lighting_frag.c";

      
        Kamera cam;

        float[] vertices = {
            // Positions          // Normals           // Texture Coords
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,
         0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  0.0f,
         0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
         0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,

        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,
         0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  0.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,

        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
        -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

         0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
         0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
         0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
         0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
         0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
         0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,
         0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  1.0f,
         0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
         0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,

        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f,
         0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  1.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f
        };

        ShaderProgram lightingShader, lampShader;
        VertexArray containerVao, lampVao;
        Texture diffuseMap, specularMap;


        protected override void OnLoad(EventArgs e) {
            cam = new Kamera(this, new Vector3(0,0,2), new Vector3(0), 0.2f,0.1f);

            lightingShader = new ShaderProgram(Shader.FromFile(basicVertexSrc), Shader.FromFile(lightingFragSrc));
            lampShader = new ShaderProgram(Shader.FromFile(basicVertexSrc), Shader.FromFile(lampFragSrc));

            diffuseMap = new Texture("Data/container2.png", 0, "diffuse");
            specularMap = new Texture("Data/container2_specular.png", 1, "specular");

            VertexBuffer<float> vbo = new VertexBuffer<float>();
            vbo.Bind();
            vbo.BufferData(vertices);

            //container vao
            containerVao = new VertexArray();
            containerVao.Bind();
            int vertexStride = (3 + 3 + 2) * 4;
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexStride, 0); //0 = pos in basic_vertex.c
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, vertexStride, 12);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, vertexStride, 24);
            GL.EnableVertexAttribArray(2);
            containerVao.Unbind();

            lampVao = new VertexArray();
            lampVao.Bind();
            vbo.Bind(); //so vao knows to use this
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexStride, 0); //0 = pos in basic_vertex.c
            GL.EnableVertexAttribArray(0);
            lampVao.Unbind();
            GL.Enable(EnableCap.DepthTest);
        }

        float t;
        protected override void OnRenderFrame(FrameEventArgs e) {
            t += (float)e.Time;
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            Vector3 lightPos = new Vector3(0, 1, -1);
            Vector3 cubePos = new Vector3(0);

            Matrix4 proj = cam.Projection;
            Matrix4 view = cam.View;


            #region oldCube
            /*
             lightingShader.Use();
            int viewPosLoc = lightingShader.GetUniformLoc("viewPos");
            GL.Uniform3(viewPosLoc, cam.Position.X, cam.Position.Y, cam.Position.Z);


            //material uniforms
            //int matAmbientLoc = lightingShader.GetUniformLoc("material.ambient");
            int matDiffuseLoc = lightingShader.GetUniformLoc("material.diffuse");
            int matSpecularLoc = lightingShader.GetUniformLoc("material.specular");
            int matShinyLoc = lightingShader.GetUniformLoc("material.shiny");

            diffuseMap.ActivateTextureUnit();
            diffuseMap.Bind();
            GL.Uniform1(matDiffuseLoc, diffuseMap.TextureUnitAsInt);
            GL.Uniform3(matSpecularLoc, 0.5f, 0.5f, 0.5f);
            GL.Uniform1(matShinyLoc, 32.0f);
               
            


            //light colors / intensity
            Vector3 lightColor = new Vector3(1);//new Vector3((float) Math.Sin(t + 2), (float)Math.Sin(t-0.2), (float)Math.Sin(t-1));
            Vector3 diffColor = lightColor * 0.5f;
            Vector3 ambColor = diffColor * 0.2f;

            //light uniforms
            int lightAmbientLoc = lightingShader.GetUniformLoc("light.ambient");
            int lightDiffuseLoc = lightingShader.GetUniformLoc("light.diffuse");
            int lightSpecularLoc = lightingShader.GetUniformLoc("light.specular");
            int lightPosLoc = lightingShader.GetUniformLoc("light.pos");
            GL.Uniform3(lightAmbientLoc, ref ambColor);
            GL.Uniform3(lightDiffuseLoc, ref diffColor);
            GL.Uniform3(lightSpecularLoc, ref lightColor);
            GL.Uniform3(lightPosLoc, ref lightPos);
             
             */
            #endregion


            //draw cube
            lightingShader.Use();

            int matDiffuseLoc = lightingShader.GetUniformLoc("material.diffuse");
            int matSpecularLoc = lightingShader.GetUniformLoc("material.specular");
            int matShinyLoc = lightingShader.GetUniformLoc("material.shiny");

            diffuseMap.ActivateTextureUnit();
            diffuseMap.Bind();
            GL.Uniform1(matDiffuseLoc, diffuseMap.TextureUnitAsInt);
            GL.Uniform3(matSpecularLoc, 0.5f, 0.5f, 0.5f);
            GL.Uniform1(matShinyLoc, 32.0f);


            //light colors / intensity
            Vector3 lightColor = new Vector3(1);//new Vector3((float) Math.Sin(t + 2), (float)Math.Sin(t-0.2), (float)Math.Sin(t-1));
            Vector3 diffColor = lightColor * 0.5f;
            Vector3 ambColor = lightColor * 0.1f;

            //light uniforms
            int lightAmbientLoc = lightingShader.GetUniformLoc("light.ambient");
            int lightDiffuseLoc = lightingShader.GetUniformLoc("light.diffuse");
            int lightSpecularLoc = lightingShader.GetUniformLoc("light.specular");
            int lightPosLoc = lightingShader.GetUniformLoc("light.pos");
            GL.Uniform3(lightAmbientLoc, ref ambColor);
            GL.Uniform3(lightDiffuseLoc, ref diffColor);
            GL.Uniform3(lightSpecularLoc, ref lightColor);
            GL.Uniform3(lightPosLoc, ref lightPos);



            Matrix4 model = Matrix4.CreateTranslation(cubePos);
            Transformation cubeTransformation = new Transformation(model, cam);
            cubeTransformation.SetUniforms(lightingShader);
            containerVao.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            containerVao.Unbind();



            //draw light
            lampShader.Use();

            model = Matrix4.CreateScale(0.1f);
            model *= Matrix4.CreateTranslation(lightPos);
            Transformation lamTransform = new Transformation(model, cam);
            lamTransform.SetUniforms(lampShader);

            lampVao.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            lampVao.Unbind();

            this.SwapBuffers();
            base.OnRenderFrame(e);
        }

        Vector3 camPos = new Vector3(4);

    }
}
