using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace UtilLib
{
    public class Transformation
    {
        Matrix4 mModel, mView, mProjection;
       

        string modelUniformName, viewUniformName, projectionUniformName;

        public Transformation(Matrix4 model, Kamera cam) {
            mModel = model;
            mView = cam.View;
            mProjection = cam.Projection;


            modelUniformName = "model";
            viewUniformName = "view";
            projectionUniformName = "proj";
        }

        public void SetUniforms(ShaderProgram prog) {
            if (!prog.IsUsed)
                throw new ApplicationException("Probatly should use this first... fuckboi");

            GL.UniformMatrix4(prog.GetUniformLoc(modelUniformName), false, ref mModel);
            GL.UniformMatrix4(prog.GetUniformLoc(viewUniformName), false, ref mView);
            GL.UniformMatrix4(prog.GetUniformLoc(projectionUniformName), false, ref mProjection);
        }
    }
}
