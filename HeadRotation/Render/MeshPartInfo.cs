using OpenTK;
using System;
using System.Collections.Generic;

namespace HeadRotation.Render
{
    public class MeshPartInfo
    {

        public List<Vector3> VertexPositions = new List<Vector3>();
        public List<Vector2> TextureCoords = new List<Vector2>();
        public List<uint> VertexIndices = new List<uint>();
        public string PartName;
        public Vector4 Color;
        public int Texture = 0;
        public int TransparentTexture = 0;
        public string TextureName;
        public string TransparentTextureName;
        public string MaterialName;

        public void Clear()
        {
            TextureCoords.Clear();
            VertexPositions.Clear();
            VertexIndices.Clear();
            PartName = String.Empty;
        }
    }
}
