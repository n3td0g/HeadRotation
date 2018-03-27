using OpenTK;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HeadRotation.Render
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex3d
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector4 Color;
        public Vector4 AutodotsTexCoord;
        public Vector3 OriginalPosition;
        public float BlendWeight;
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

}
