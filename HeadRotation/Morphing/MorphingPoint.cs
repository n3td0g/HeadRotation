using OpenTK;
using System.Collections.Generic;

namespace HeadRotation.Morphing
{
    public class TrinagleInfo
    {
        public int TriangleIndex = -1;
        public float U, V, W;
    }

    public class MorphingPoint
    {
        public static bool PointInTriangle(ref Vector2 a, ref Vector2 b, ref Vector2 c, Vector2 p)
        {
            var n1 = (b.Y - a.Y) * (p.X - a.X) - (b.X - a.X) * (p.Y - a.Y);
            var n2 = (c.Y - b.Y) * (p.X - b.X) - (c.X - b.X) * (p.Y - b.Y);
            var n3 = (a.Y - c.Y) * (p.X - c.X) - (a.X - c.X) * (p.Y - c.Y);
            return (n1 <= 0.0f && n2 <= 0.0f && n3 <= 0.0f) || (n1 >= 0.0f && n2 >= 0.0f && n3 >= 0.0f);
        }

        public Vector3 Position;
        public List<int> Indices = new List<int>();
        public TrinagleInfo FrontTriangle = new TrinagleInfo();
        public TrinagleInfo RightTriangle = new TrinagleInfo();

        public Vector3 MorphFront(ref Vector2 v1, ref Vector2 v2, ref Vector2 v3)
        {
            Vector3 result = Position;
            result.X = FrontTriangle.U * v1.X + FrontTriangle.V * v2.X + FrontTriangle.W * v3.X;
            result.Y = FrontTriangle.U * v1.Y + FrontTriangle.V * v2.Y + FrontTriangle.W * v3.Y;
            return result;
        }

        public Vector3 MorphRight(ref Vector2 v1, ref Vector2 v2, ref Vector2 v3)
        {
            Vector3 result = Position;
            result.Z = FrontTriangle.U * v1.X + FrontTriangle.V * v2.X + FrontTriangle.W * v3.X;
            result.Y = FrontTriangle.U * v1.Y + FrontTriangle.V * v2.Y + FrontTriangle.W * v3.Y;
            return result;
        }

        public void Initialize(ref Vector2 a, ref Vector2 b, ref Vector2 c, int triangleIndex, bool isFront)
        {
            var point = isFront ? Position.Xy : Position.Zy;

            if (PointInTriangle(ref a, ref b, ref c, point))
            {
                var triangle = isFront ? FrontTriangle : RightTriangle;

                var uv = Vector3.Cross(
                    new Vector3(c.X - a.X, b.X - a.X, a.X - point.X),
                    new Vector3(c.Y - a.Y, b.Y - a.Y, a.Y - point.Y));
                if (uv.Z == 0.0f)
                    triangle.U = triangle.V = triangle.W = 0.0f;
                else
                {
                    triangle.U = 1.0f - (uv.X + uv.Y) / uv.Z;
                    triangle.V = uv.Y / uv.Z;
                    triangle.W = uv.X / uv.Z;
                }
                triangle.TriangleIndex = triangleIndex;
            }
        }
    }
}
