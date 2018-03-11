using HeadRotation.Helpers;
using HeadRotation.Render;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace HeadRotation.Morphing
{
    public class MorphTriangle
    {
        public int A;
        public int B;
        public int C;
    }

    public class HeadMorphing
    {
        public List<MorphTriangle> TrianglesFront = new List<MorphTriangle>();
        public List<MorphTriangle> TrianglesRight = new List<MorphTriangle>();
        public HeadPoints headPoints;

        public void Initialize(HeadPoints hPoints)
        {
            headPoints = hPoints;

            var headMesh = headPoints.HeadMesh;
            var a = headMesh.AABB.A;
            var b = headMesh.AABB.B;
            var b1 = new Vector3(a.X, b.Y, b.Z);
            var b2 = new Vector3(a.X, a.Y, b.Z);
            var b3 = new Vector3(b.X, a.Y, b.Z);

            headPoints.Points.Add(b);
            headPoints.Points.Add((b + b1) * 0.5f);
            headPoints.Points.Add(b1);
            headPoints.Points.Add((b1 + b2) * 0.5f);
            headPoints.Points.Add(b2);
            headPoints.Points.Add((b2 + b3) * 0.5f);
            headPoints.Points.Add(b3);
            headPoints.Points.Add((b3 + b) * 0.5f);

            TrianglesFront.Add(new MorphTriangle { A = 66, B = 52, C = 68 });
        }

        public void Draw()
        {
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Begin(PrimitiveType.Lines);

            foreach (var triangle in TrianglesFront)
            {
                RenderHelper.DrawLine(headPoints.Points[triangle.A], headPoints.Points[triangle.B]);
                RenderHelper.DrawLine(headPoints.Points[triangle.B], headPoints.Points[triangle.C]);
                RenderHelper.DrawLine(headPoints.Points[triangle.A], headPoints.Points[triangle.C]);
            }

            GL.End();
        }

        public void UpdateMorphing()
        {

        }
    }
}
