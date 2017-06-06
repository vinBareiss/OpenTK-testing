using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;


namespace UtilLib
{
    public abstract class Light
    {
        private string mAmbientName = "ambient", mDiffuseName = "diffuse", mSpecularName = "specular";
        private Vector3 mAmbient, mDiffuse, mSpecular;
        public Light(Vector3 ambient, Vector3 diffuse, Vector3 specular) {
            mAmbient = ambient;
            mDiffuse = diffuse;
            mSpecular = specular;
        }

        /// <summary>
        /// Sets all the Uniforms
        /// </summary>
        /// <param name="p">Collection of shaders to calculate theese lights</param>
        /// <param name="structName">Name of the Struct that represents this light in the Fragment shader (name.light)</param>
        public virtual void SetUniforms(string structName, ShaderProgram p) {
            GL.Uniform3(p.GetUniformLoc($"{structName}.{mAmbientName}"), mAmbient);
            GL.Uniform3(p.GetUniformLoc($"{structName}.{mDiffuseName}"), mDiffuse);
            GL.Uniform3(p.GetUniformLoc($"{structName}.{mSpecularName}"), mSpecular);
        }
        public void SetUniformsInArray(ShaderProgram p, string arrayName, int index) {
            SetUniforms($"{arrayName}[{index}]", p);
        }
        
        //yeah, im lazy
        public static Vector3 ColorToVector(Color4 col) {
            return new Vector3(col.R, col.G, col.B);
        }
    }


    public class DirectionalLight : Light
    {
        string mDirectionName = "direction";
        Vector3 mDirection;

        /// <summary>
        /// Creates a directional light
        /// </summary>
        /// <param name="direction">From where is the light coming?</param>
        /// <param name="ambient">Ambient component (general Illumination)</param>
        /// <param name="diffuse">Diffuse component</param>
        /// <param name="specular">Specular component</param>
        public DirectionalLight(Vector3 direction, Vector3 ambient, Vector3 diffuse, Vector3 specular) : base(ambient, diffuse, specular) {
            mDirection = direction;
        }

        public override void SetUniforms(string structName, ShaderProgram p) {
            GL.Uniform3(p.GetUniformLoc($"{structName}.{this.mDirectionName}"), mDirection);

            base.SetUniforms(structName, p);
        }
    }

    public class PointLight : Light
    {
        public struct AnnuationDetails
        {
            public float constant, linear, quadratic;
            public AnnuationDetails(float l, float q) {
                constant = 1.0f;
                linear = l;
                quadratic = q;
            }
        }

        public static AnnuationDetails D3250 = new AnnuationDetails(0.0014f, 0.000007f);
        public static AnnuationDetails D600 = new AnnuationDetails(0.007f, 0.0002f);
        public static AnnuationDetails D325 = new AnnuationDetails(0.014f, 0.0007f);
        public static AnnuationDetails D200 = new AnnuationDetails(0.022f, 0.0019f);
        public static AnnuationDetails D160 = new AnnuationDetails(0.027f, 0.0028f);
        public static AnnuationDetails D100 = new AnnuationDetails(0.045f, 0.0075f);
        public static AnnuationDetails D65 = new AnnuationDetails(0.07f, 0.017f);
        public static AnnuationDetails D50 = new AnnuationDetails(0.09f, 0.032f);
        public static AnnuationDetails D32 = new AnnuationDetails(0.14f, 0.07f);
        public static AnnuationDetails D20 = new AnnuationDetails(0.22f, 0.20f);
        public static AnnuationDetails D13 = new AnnuationDetails(0.35f, 0.44f);
        public static AnnuationDetails D7 = new AnnuationDetails(0.7f, 1.8f);

        string mPositionName = "position";
        Vector3 mPosition;

        string mConstantName = "constant", mLinearName = "linear", mQuadraticName = "quadratic";
        float mConstant, mLinear, mQuadratic;

        public PointLight(Vector3 position, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic) : base(ambient, diffuse, specular) {
            mPosition = position;
            mConstant = constant;
            mLinear = linear;
            mQuadratic = quadratic;
        }
        public PointLight(Vector3 position, Vector3 ambient, Vector3 diffuse, Vector3 specular, AnnuationDetails aDetails) :
            this(position, ambient, diffuse, specular, aDetails.constant, aDetails.linear, aDetails.quadratic) { }

        public override void SetUniforms(string structName, ShaderProgram p) {
            GL.Uniform3(p.GetUniformLoc($"{structName}.{mPositionName}"), mPosition);

            GL.Uniform1(p.GetUniformLoc($"{structName}.{mConstantName}"), mConstant);
            GL.Uniform1(p.GetUniformLoc($"{structName}.{mLinearName}"), mLinear);
            GL.Uniform1(p.GetUniformLoc($"{structName}.{mQuadraticName}"), mQuadratic);

   
            base.SetUniforms(structName, p);
        }

        
    }
}
