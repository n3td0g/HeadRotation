using OpenTK;

namespace HeadRotation.ObjFile
{
    public class ObjMaterial
    {
        #region Usable items

        private Vector4 diffuseColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        public Vector4 DiffuseColor
        {
            get
            {
                return diffuseColor;
            }
            set
            {
                diffuseColor = value;
            }
        }
        public bool IsTransparent
        {
            get { return diffuseColor.W < 1.0f || TransparentTexture != 0; }
        }

        private string transparentTextureMap;
        public string TransparentTextureMap
        {
            get
            {
                return transparentTextureMap;
            }
            set
            {
                transparentTextureMap = value;
#if (WEB_APP)
                TransparentTexture = string.IsNullOrEmpty(transparentTextureMap) ? 0 : ProgramCore.Project.RenderMainHelper.GetTexture(transparentTextureMap);
#else
                TransparentTexture = string.IsNullOrEmpty(transparentTextureMap) ? 0 : TextureHelper.GetTexture(transparentTextureMap);
#endif
            }
        }
        public int TransparentTexture
        {
            get;
            set;
        }

        private string diffuseTextureMap;
        public string DiffuseTextureMap
        {
            get
            {
                return diffuseTextureMap;
            }
            set
            {
                diffuseTextureMap = value;
#if (WEB_APP)
                if (ProgramCore.Project != null)
                    Texture = string.IsNullOrEmpty(diffuseTextureMap) ? 0 : ProgramCore.Project.RenderMainHelper.GetTexture(diffuseTextureMap);
#else
                Texture = string.IsNullOrEmpty(diffuseTextureMap) ? 0 : TextureHelper.GetTexture(diffuseTextureMap);
#endif
            }
        }
        public int Texture
        {
            get;
            set;
        }

        #endregion

        #region Non-usable items

        public Vector3 AmbientColor
        {
            get;
            set;
        }
        public Vector3 SpecularColor
        {
            get;
            set;
        }
        public float SpecularCoefficient
        {
            get;
            set;
        }
        public float OpticalDensity
        {
            get;
            set;
        }

        public float Transparency
        {
            get;
            set;
        }

        public int IlluminationModel
        {
            get;
            set;
        }

        public string AmbientTextureMap
        {
            get;
            set;
        }

        public string SpecularTextureMap
        {
            get;
            set;
        }
        public string SpecularHighlightTextureMap
        {
            get;
            set;
        }

        public string BumpMap
        {
            get;
            set;
        }
        public string DisplacementMap
        {
            get;
            set;
        }
        public string StencilDecalMap
        {
            get;
            set;
        }

        #endregion

        public ObjMaterial(string materialName)
        {
            Name = materialName;
            DiffuseColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            Transparency = 1f;
        }

        public string Name
        {
            get;
            set;
        }
    }
}
