using System;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;


namespace UtilLib
{
    public class Texture : OGLHandle
    {
        TextureUnit mTexUnit;
        int mTexUnitAsInt;
        public int TextureUnitAsInt { get { return mTexUnitAsInt; } }

        string mName;

        public Texture(Image img, int textureUnit, string name) : base(GL.GenTexture()) {
            mName = name;

            mTexUnit = TextureUnit.Texture0 + textureUnit;
            mTexUnitAsInt = textureUnit;

           using (Bitmap bmp = (Bitmap)img) {
                BitmapData data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size),
                               ImageLockMode.ReadOnly,
                               System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.ActiveTexture(mTexUnit);

                this.Bind();
                GL.TexImage2D(  TextureTarget.Texture2D, 
                                0,
                                PixelInternalFormat.Rgba,
                                bmp.Width, bmp.Height, 
                                0,
                                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                                PixelType.UnsignedByte, 
                                data.Scan0
                              );
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);            //set texture parameter
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);


                bmp.UnlockBits(data);
                this.Unbind();
            }
        }

        public Texture(string path, int textureUnit, string name) : this(Image.FromFile(path), textureUnit, name) { }

        public void ActivateTextureUnit() {
            GL.ActiveTexture(mTexUnit);
        }

        public void SetUniforms(ShaderProgram p) {
            GL.Uniform1(p.GetUniformLoc(mName), TextureUnitAsInt);
        }

        public void Bind() { GL.BindTexture(TextureTarget.Texture2D, this); }
        public void Unbind() { GL.BindTexture(TextureTarget.Texture2D, 0); }
    }
}
