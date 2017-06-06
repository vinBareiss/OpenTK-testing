using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace UtilLib
{
    //diffusemap

    public class Material
    {
        Dictionary<string, Texture> mTextures;
        Dictionary<string, float> mFloats;

        public Material() {
            mTextures = new Dictionary<string, Texture>();
            mFloats = new Dictionary<string, float>();
        }


        public void AddTexture(string name, Texture tex) {
            mTextures.Add(name, tex);
        }
        public void AddFloat(string name, float f) {
            mFloats.Add(name, f);
        }

        public void SetUniforms(string matName, ShaderProgram p) {
            foreach (KeyValuePair<string, float> item in mFloats) {
                GL.Uniform1(p.GetUniformLoc($"{matName}.{item.Key}"), item.Value);
            }
            foreach (KeyValuePair<string, Texture> item in mTextures) {
                item.Value.ActivateTextureUnit();
                item.Value.Bind();
                GL.Uniform1(p.GetUniformLoc($"{matName}.{item.Key}"), item.Value.TextureUnitAsInt);
            }
        }

    }
}
