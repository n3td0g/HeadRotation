using System.Runtime.InteropServices;
using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System;

namespace HeadRotation.Render
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex3d
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector4 Color;
        public static readonly int Stride = Marshal.SizeOf(default(Vertex3d));
    }

    public class VertexInfo
    {
        public Vector3 Position;
        public Vector2 TexCoords;
    }

    public class VectorEqualityComparer :
        IEqualityComparer<Vector3>,
        IEqualityComparer<Vector2>,
        IEqualityComparer<VertexInfo>
    {
        private const float Delta = 0.00001f;   

        public static bool EqualsVector3(Vector3 a, Vector3 b)
        {
            return Math.Abs(a.X - b.X) < Delta && Math.Abs(a.Y - b.Y) < Delta && Math.Abs(a.Z - b.Z) < Delta;
        }

        public bool Equals(Vector3 a, Vector3 b)
        {
            return EqualsVector3(a, b);
        }
        public bool Equals(Vector2 a, Vector2 b)
        {
            return Math.Abs(a.X - b.X) < Delta && Math.Abs(a.Y - b.Y) < Delta;
        }
        public bool Equals(VertexInfo a, VertexInfo b)
        {
            return EqualsVector3(a.Position, b.Position) && Equals(a.TexCoords, b.TexCoords);
        }

        public int GetHashCode(Vector3 a)
        {
            return (int)((a.X * a.X + a.Y * a.Y + a.Z * a.Z) * 10000);
        }
        public int GetHashCode(Vector2 a)
        {
            return (int)((a.X * a.X + a.Y * a.Y) * 10000);
        }
        public int GetHashCode(VertexInfo a)
        {
            return GetHashCode(a.Position) * GetHashCode(a.TexCoords);
        }
    }

    public class MeshPart
    {
        public List<uint> Indices = new List<uint>();
        public Vertex3d[] Vertices = null;

        public int Texture = 0;
        public int TransparentTexture = 0;
        public Vector4 Color = Vector4.One;

        public int IndexBuffer, VertexBuffer = 0;
        public int CountIndices
        {
            get;
            set;
        }

        public void UpdateBuffers()
        {
            UpdateVertexBuffer();
            UpdateIndexBuffer();
        }

        public void Destroy()
        {
            if (VertexBuffer != 0)
            {
                GL.DeleteBuffers(1, ref VertexBuffer);
                VertexBuffer = 0;
            }

            if (IndexBuffer != 0)
            {
                GL.DeleteBuffers(1, ref IndexBuffer);
                IndexBuffer = 0;
            }
        }

        private void UpdateVertexBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * Vertex3d.Stride), Vertices, BufferUsageHint.StreamDraw);
            OpenGlHelper.CheckErrors();
        }

        public void UpdateIndexBuffer()
        {
            UpdateIndexBuffer(Indices);
        }

        public void UpdateIndexBuffer(List<uint> indices)
        {
            CountIndices = indices.Count;
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(CountIndices * sizeof(uint)), indices.ToArray(), BufferUsageHint.DynamicDraw);
            OpenGlHelper.CheckErrors();
        }

        public bool Create(MeshPartInfo info)
        {
            if (info.VertexPositions.Count == 0)
                return false;
           // Guid = Guid.NewGuid();
            Color = info.Color;
            Texture = info.Texture;
            TransparentTexture = info.TransparentTexture;
            //DefaultTextureName = TextureName = info.TextureName;
           // TransparentTextureName = info.TransparentTextureName;

            Indices.Clear();
            var positions = new List<Vector3>();
            var texCoords = new List<Vector2>();

            var positionsDict = new Dictionary<VertexInfo, uint>(new VectorEqualityComparer());
            var pointsIndicesDict = new Dictionary<int, int>();
            for (var i = 0; i < info.VertexPositions.Count; i++)
            {
                var vertexInfo = new VertexInfo
                {
                    Position = info.VertexPositions[i],
                    TexCoords = info.TextureCoords[i]
                };
                if (!positionsDict.ContainsKey(vertexInfo))
                {
                    var index = (uint)positions.Count;
                    positionsDict.Add(vertexInfo, index);
                    Indices.Add(index);
                    positions.Add(vertexInfo.Position);
                    texCoords.Add(vertexInfo.TexCoords);                   
                }
                else
                    Indices.Add(positionsDict[vertexInfo]);
            }

            CountIndices = Indices.Count;
            Vertices = new Vertex3d[positions.Count];

            var normals = Normal.CalculateNormals(positions, Indices);
            for (var i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position = positions[i];
                Vertices[i].Normal = normals[i];
                Vertices[i].TexCoord = texCoords[i];
                Vertices[i].Color = Vector4.One;
            }
            
            Destroy();
            GL.GenBuffers(1, out VertexBuffer);
            GL.GenBuffers(1, out IndexBuffer);

            return true;
        }
    }
}
