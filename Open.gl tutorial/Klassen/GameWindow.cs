using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Open.gl_tutorial
{
    class GameWindow : OpenTK.GameWindow
    {

        private int shaderProgHandle;

        public GameWindow()
            : base(1280, 720, GraphicsMode.Default, "TestTitle", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible) {
            Console.WriteLine("OpenGl: " + GL.GetString(StringName.Version));
            Console.WriteLine("GLSL: " + GL.GetString(StringName.ShadingLanguageVersion));
        }

        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        struct Vertex
        {
            public const int SIZE = (2 + 4 + 2) * 4; //bytesize

            private Vector2 mPos;
            private Color4 mCol;
            private Vector2 mTex;

            public Vertex(Vector2 pos, Color col, Vector2 tex) {
                mPos = pos;
                mCol = col;
                mTex = tex;
            }
        }

        uint vaoHandle;
        protected override void OnLoad(EventArgs e) {

            Vertex[] vertices =
            {
                new Vertex(new Vector2(-0.5f,  0.5f),  Color.Red,    new Vector2(0.0f, 0.0f)),
                new Vertex(new Vector2( 0.5f,  0.5f),  Color.Green,  new Vector2(1.0f, 0.0f)),
                new Vertex(new Vector2( 0.5f, -0.5f),  Color.Yellow, new Vector2(1.0f, 1.0f)),
                new Vertex(new Vector2(-0.5f, -0.5f),  Color.Blue,   new Vector2(0.0f, 1.0f))
            };

            uint[] elements =
            {
                0,1,2,
                2,3,0
            };

            //vbo
            GL.GenBuffers(1, out uint vboHandle); //speicher reservieren auf GK
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboHandle); //buffer "aktivieren"
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vertex.SIZE, vertices, BufferUsageHint.StaticDraw); //vertices array auf GK schreiben

            //vao
            GL.GenVertexArrays(1, out vaoHandle);
            GL.BindVertexArray(vaoHandle);

            //eao
            GL.GenBuffers(1, out uint eaoHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eaoHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, elements.Length * sizeof(uint), elements, BufferUsageHint.StaticDraw);

            //shaders
            #region shadercode
            string vertShaderSource = @"#version 450

in vec2 position;
in vec4 color;
in vec2 texCoord;

out vec4 Color;
out vec2 TexCoord;

void main(){
    Color = color;
    TexCoord = texCoord;
    gl_Position = vec4(position , 0.0, 1.0);
}";
            string fragShaderSource = @"#version 450
uniform sampler2D texKatze;
uniform sampler2D texPuppy;

in vec4 Color;
in vec2 TexCoord;

out vec4 outColor;
void main(){
    //outColor = color;
    vec4 colKatze = texture(texKatze, TexCoord);
    vec4 colPuppy = texture(texPuppy, TexCoord);

    outColor = mix(colPuppy,colKatze, 0.5) * (0.5* Color);
}";
            #endregion

            int vertShaderHandle = GL.CreateShader(ShaderType.VertexShader); //reserve some mem for shader
            GL.ShaderSource(vertShaderHandle, vertShaderSource); //load glsl code to shader 
            GL.CompileShader(vertShaderHandle); //compile shader
            Console.WriteLine("VertShader: " + GL.GetShaderInfoLog(vertShaderHandle)); //write log to output

            int fragShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragShaderHandle, fragShaderSource);
            GL.CompileShader(fragShaderHandle);
            Console.WriteLine("FragShader: " + GL.GetShaderInfoLog(fragShaderHandle));

            shaderProgHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgHandle, vertShaderHandle);
            GL.AttachShader(shaderProgHandle, fragShaderHandle);

            GL.BindFragDataLocation(shaderProgHandle, 0, "outColor");

            GL.LinkProgram(shaderProgHandle);
            GL.UseProgram(shaderProgHandle);

            GL.DetachShader(shaderProgHandle, vertShaderHandle);
            GL.DetachShader(shaderProgHandle, fragShaderHandle);

            //attributes
            int posAttrLoc = GL.GetAttribLocation(shaderProgHandle, "position"); //handle des attributs finden
            GL.EnableVertexAttribArray(posAttrLoc); //attribut aktivieren (abwehrkräfte)
            GL.VertexAttribPointer(posAttrLoc, 2, VertexAttribPointerType.Float, false, Vertex.SIZE, 0); //attribut "lesbar machen2

            int colAttrLoc = GL.GetAttribLocation(shaderProgHandle, "color");
            GL.EnableVertexAttribArray(colAttrLoc);
            GL.VertexAttribPointer(colAttrLoc, 4, VertexAttribPointerType.Float, false, Vertex.SIZE, 2 * sizeof(float));

            int texAttrLoc = GL.GetAttribLocation(shaderProgHandle, "texCoord");
            GL.EnableVertexAttribArray(texAttrLoc);
            GL.VertexAttribPointer(texAttrLoc, 2, VertexAttribPointerType.Float, false, Vertex.SIZE, 6 * sizeof(float));

            //texture
            uint[] textureHandles = new uint[2];
            GL.GenTextures(2, textureHandles);

            //katze
            GL.ActiveTexture(TextureUnit.Texture0);
            Bitmap bmpKatze = (Bitmap)Bitmap.FromFile("sample.png"); //load texture
            System.Drawing.Imaging.BitmapData dataKatze = bmpKatze.LockBits(new Rectangle(0, 0, bmpKatze.Width, bmpKatze.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, textureHandles[0]); //bind texture to operate on it
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpKatze.Width, bmpKatze.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dataKatze.Scan0);
            bmpKatze.UnlockBits(dataKatze);
            GL.Uniform1(GL.GetUniformLocation(shaderProgHandle, "texKatze"), 0);                                         //set texKatze to texture 0
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);            //set texture parameter
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

            //hund
            GL.ActiveTexture(TextureUnit.Texture1);
            Bitmap bmpHund = (Bitmap)Bitmap.FromFile("sample2.png"); //lade textur
            System.Drawing.Imaging.BitmapData dataHund = bmpHund.LockBits(new Rectangle(0, 0, bmpHund.Width, bmpHund.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, textureHandles[1]);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpHund.Width, bmpHund.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dataHund.Scan0);
            bmpHund.UnlockBits(dataHund);
            GL.Uniform1(GL.GetUniformLocation(shaderProgHandle, "texPuppy"), 1);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);            //set texture parameter
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

            Console.WriteLine(GL.GetError());
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(vaoHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            // GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);


            this.SwapBuffers();
        }
    }
}
