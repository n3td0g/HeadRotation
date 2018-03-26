using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace HeadRotation.ObjFile
{
    public class TextureInfo
    {
        public int Texture;
        public int Width;
        public int Height;
    }

    public class TextureHelper
    {
        private static Dictionary<string, TextureInfo> textures = new Dictionary<string, TextureInfo>();

        public static void ReloadTextures()
        {
            textures.Clear();
        }
        public TextureInfo FindTexture(string textureName)
        {
            TextureInfo result;
            textures.TryGetValue(textureName, out result);
            return result;
        }

        public static int GetTexture(string textureName)
        {
            var textureId = 0;
            if (textureName != string.Empty && File.Exists(textureName))
            {

                if (textures.ContainsKey(textureName))
                    return textures[textureName].Texture;

                Bitmap bitmap;
                using (var ms = new MemoryStream(File.ReadAllBytes(textureName)))
                    bitmap = (Bitmap)Image.FromStream(ms);

                textureId = GetTexture(bitmap);
                textures.Add(textureName, new TextureInfo
                {
                    Texture = textureId,
                    Width = bitmap.Width,
                    Height = bitmap.Height
                });
            }
            return textureId;
        }

        public static int GetTexture(Bitmap bitmap)
        {
            try
            {
                int textureId;
                GL.GenTextures(1, out textureId);

                SetTexture(textureId, bitmap);
                return textureId;
            }
            catch
            {
                return 0;
            }
        }

        public static string GetTexturePath(int id)
        {
            if (id == 0)
                return string.Empty;
            foreach (var t in textures)
                if (t.Value.Texture == id)
                    return t.Key;
            return string.Empty;
        }

        public static void SetTexture(int textureId, Bitmap bitmap)
        {
            try
            {
                GL.BindTexture(TextureTarget.Texture2D, textureId);

                var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            }
            finally
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

    }
}
