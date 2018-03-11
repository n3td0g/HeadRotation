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

            TrianglesFront.Clear();

            TrianglesFront.Add(new MorphTriangle { A = 66, B = 52, C = 68 });

            InitializeMorphin();
        }

        private void InitializeMorphin()
        {
            var headMesh = headPoints.HeadMesh;
            for (int index = 0; index < TrianglesFront.Count; ++index)
            {
                var triangle = TrianglesFront[index];
                var a = headPoints.Points[triangle.A].Xy;
                var b = headPoints.Points[triangle.B].Xy;
                var c = headPoints.Points[triangle.C].Xy;

                foreach (var part in headMesh.Parts)
                {
                    foreach (var point in part.MorphPoints)
                    {
                        point.Initialize(ref a, ref b, ref c, index, true);
                    }
                }
            }
        }

        public void Morph()
        {
            var headMesh = headPoints.HeadMesh;
            foreach (var part in headMesh.Parts)
            {
                foreach (var point in part.MorphPoints)
                {
                    if(point.FrontTriangle.TriangleIndex > -1 )
                    {
                        var triangle = TrianglesFront[point.FrontTriangle.TriangleIndex];
                        var a = headPoints.Points[triangle.A].Xy;
                        var b = headPoints.Points[triangle.B].Xy;
                        var c = headPoints.Points[triangle.C].Xy;

                        point.Morph(ref a, ref b, ref c);

                        foreach(var index in point.Indices)
                        {
                            part.Vertices[index].Position = point.Position;
                        }
                    }
                }
                part.UpdateBuffers();
            }
        }        

        public void Draw()
        {
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Begin(PrimitiveType.Lines);

            foreach (var triangle in TrianglesFront)
            {
                var a = headPoints.GetWorldPoint(triangle.A);
                var b = headPoints.GetWorldPoint(triangle.B);
                var c = headPoints.GetWorldPoint(triangle.C);
                RenderHelper.DrawLine(a, b);
                RenderHelper.DrawLine(b, c);
                RenderHelper.DrawLine(c, a);
            }

            GL.End();
        }

        public void UpdateMorphing()
        {

        }
    }
}
