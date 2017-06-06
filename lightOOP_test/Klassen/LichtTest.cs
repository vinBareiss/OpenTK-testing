using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using UtilLib;

namespace lightOOP_test
{
    class LichtTest : BaseGameWindow
    {
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
        Vector3[] cubePos = {
            new Vector3( 0.0f,  0.0f,  0.0f),
    new Vector3( 2.0f,  5.0f, -15.0f),
    new Vector3(-1.5f, -2.2f, -2.5f),
    new Vector3(-3.8f, -2.0f, -12.3f),
    new Vector3( 2.4f, -0.4f, -3.5f),
    new Vector3(-1.7f,  3.0f, -7.5f),
    new Vector3( 1.3f, -2.0f, -2.5f),
    new Vector3( 1.5f,  2.0f, -2.5f),
    new Vector3( 1.5f,  0.2f, -1.5f),
    new Vector3(-1.3f,  1.0f, -1.5f)
        };
        Vector3[] lightPos = {
              new Vector3( 0.7f,  0.2f,  2.0f),
              new Vector3( 2.3f, -3.3f, -4.0f),
              new Vector3(-4.0f,  2.0f, -12.0f),
              new Vector3( 0.0f,  0.0f, -3.0f)
        };

        ShaderProgram lightingShader, lampShader;
        VertexArray boxVao, lampVao;
        Texture diffuseMapTexture, specularMapTexture;
        Kamera cam;

        Material containerMat;

        protected override void OnLoad(EventArgs e) {
            cam = new Kamera(this, new Vector3(0.0f, 0.0f, 3.0f), new Vector3(0, 0, 0), 0.1f, 0.05f);

            VertexBuffer<float> vbo = new VertexBuffer<float>();
            vbo.Bind();
            vbo.BufferData(vertices);

            boxVao = new VertexArray();
            boxVao.Bind();
            boxVao.SetVertexAttrib<float>(0, 3, VertexAttribPointerType.Float, false, 8, 0); //pos
            boxVao.SetVertexAttrib<float>(1, 3, VertexAttribPointerType.Float, false, 8, 3); //norm
            boxVao.SetVertexAttrib<float>(2, 2, VertexAttribPointerType.Float, false, 8, 6); //texCoord
            boxVao.Unbind();

            lampVao = new VertexArray();
            lampVao.Bind();
            vbo.Bind(); //müssen wir nochmal
            lampVao.SetVertexAttrib<float>(0, 3, VertexAttribPointerType.Float, false, 8, 0); //pos
            lampVao.SetVertexAttrib<float>(1, 3, VertexAttribPointerType.Float, false, 8, 3); //norm
            lampVao.SetVertexAttrib<float>(2, 2, VertexAttribPointerType.Float, false, 8, 6); //texCoord
            lampVao.Unbind();
            vbo.Unbind();


            lightingShader = new ShaderProgram(Shader.FromFile("Shader/vertexBase.c"), Shader.FromFile("Shader/fragmentBase.c"));
            lampShader = new ShaderProgram(Shader.FromFile("Shader/vertexBase.c"), Shader.FromFile("Shader/fragmentLamp.c"));

            diffuseMapTexture = new Texture("Data/container2.png", 0, "material.diffuse");
            specularMapTexture = new Texture("Data/container2_specular.png", 1, "material.specular");

            containerMat = new Material();
            containerMat.AddTexture("diffuse", diffuseMapTexture);
            containerMat.AddTexture("specular", specularMapTexture);
            containerMat.AddFloat("shininess", 32.0f);

            GL.Enable(EnableCap.DepthTest);
        }

        float t;
        protected override void OnRenderFrame(FrameEventArgs e) {

            Title = GetFps(e.Time);
            t += (float)e.Time;

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);



            #region Cube
            lightingShader.Use();

            //values
            Vector3 lampPos = new Vector3((float)Math.Sin(0.5f * t), (float)Math.Cos(t), (float)Math.Sin(t));
            Vector3 lightColor = new Vector3(1, 0.3f, 0.5f);



            //material
            containerMat.SetUniforms("material", lightingShader);

            //directional light
            Vector3 sunColor = Light.ColorToVector(Color4.White);
            DirectionalLight sunLight = new DirectionalLight(new Vector3(-0.2f, -1.0f, -0.3f),
                                                             new Vector3(sunColor * 0.05f),
                                                             new Vector3(sunColor * 0.4f),
                                                             new Vector3(sunColor * 0.5f));
            sunLight.SetUniforms("dirLight", lightingShader);


            int numLights = 4;
            //point lights
            for (int i = 0; i < numLights; i++) {
                PointLight lamp = new PointLight(lightPos[i],
                                                 new Vector3(0.05f),
                                                 new Vector3(0.8f),
                                                 new Vector3(1.0f),
                                                 PointLight.D20);
                //lamp.SetUniformsInArray(lightingShader, "pointLights", i);
            }

            PointLight camLight = new PointLight(cam.Position, new Vector3(0.05f), new Vector3(1), new Vector3(1.0f), PointLight.D20);
            //camLight.SetUniformsInArray(lightingShader, "pointLights", 4);

            //miscsss
            GL.Uniform3(lightingShader.GetUniformLoc("viewPos"), cam.Position);

            boxVao.Bind();
            Transformation cubeTransform;
            Matrix4 model;
            for (int i = 0; i < 10; i++) {
                float angle = 20.0f * i;
                model = Matrix4.CreateTranslation(cubePos[i]);
                model *= Matrix4.CreateRotationZ(angle);
                cubeTransform = new Transformation(model, cam);
                cubeTransform.SetUniforms(lightingShader);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
            model = Matrix4.CreateScale(0.2f);
            model *= Matrix4.CreateTranslation(lampPos);

            cubeTransform = new Transformation(model, cam);
            cubeTransform.SetUniforms(lightingShader);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            boxVao.Unbind();
            #endregion


            #region Lamp
            lampShader.Use();
            Transformation lampTransform;
            for (int i = 0; i < numLights; i++) {
                model = Matrix4.CreateScale(0.1f);
                model *= Matrix4.CreateTranslation(lightPos[i]);
                lampTransform = new Transformation(model, cam);
                lampTransform.SetUniforms(lampShader);
                lampVao.Bind();
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                lampVao.Unbind();
            }
            #endregion

            this.SwapBuffers();
        }
    }
}
