using HeadRotation.Helpers;
using HeadRotation.Render;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;

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

            float centerX = (a.X + b.X) * 0.5f;
            float centerZ = (a.Z + b.Z) * 0.5f;

            var a1 = new Vector3(centerX, a.Y, centerZ);
            var a2 = new Vector3(centerX, b.Y, centerZ);

            headPoints.Points.Add(b); //70
            headPoints.Points.Add((b + b1) * 0.5f); //71
            headPoints.Points.Add(b1); //72
            headPoints.Points.Add((b1 + b2) * 0.5f); //73
            headPoints.Points.Add(b2); //74
            headPoints.Points.Add((b2 + b3) * 0.5f); //75
            headPoints.Points.Add(b3); //76
            headPoints.Points.Add((b3 + b) * 0.5f); //77

            headPoints.Points.Add(a1); //78
            headPoints.Points.Add(a2); //79

            headPoints.IsVisible.AddRange(Enumerable.Repeat(true, 10));

            #region TrianglesFront
            // left oval
            TrianglesFront.Add(new MorphTriangle { A = 52, B = 3, C = 68 });
            TrianglesFront.Add(new MorphTriangle { A = 52, B = 66, C = 68 });
            TrianglesFront.Add(new MorphTriangle { A = 66, B = 73, C = 68 });
            TrianglesFront.Add(new MorphTriangle { A = 66, B = 73, C = 12 });
            TrianglesFront.Add(new MorphTriangle { A = 5, B = 3, C = 68 });
            TrianglesFront.Add(new MorphTriangle { A = 5, B = 74, C = 68 });
            TrianglesFront.Add(new MorphTriangle { A = 5, B = 3, C = 7 });
            TrianglesFront.Add(new MorphTriangle { A = 5, B = 74, C = 7 });
            TrianglesFront.Add(new MorphTriangle { A = 3, B = 58, C = 7 });
            TrianglesFront.Add(new MorphTriangle { A = 9, B = 58, C = 7 });
            TrianglesFront.Add(new MorphTriangle { A = 9, B = 58, C = 55 });
            TrianglesFront.Add(new MorphTriangle { A = 9, B = 55, C = 11 });
            TrianglesFront.Add(new MorphTriangle { A = 9, B = 7, C = 74 });
            TrianglesFront.Add(new MorphTriangle { A = 9, B = 11, C = 75 });
            TrianglesFront.Add(new MorphTriangle { A = 45, B = 66, C = 43 });
            TrianglesFront.Add(new MorphTriangle { A = 23, B = 66, C = 43 });
            TrianglesFront.Add(new MorphTriangle { A = 72, B = 73, C = 12 });
            TrianglesFront.Add(new MorphTriangle { A = 72, B = 71, C = 16 });
            TrianglesFront.Add(new MorphTriangle { A = 66, B = 12, C = 23 });
            TrianglesFront.Add(new MorphTriangle { A = 66, B = 52, C = 45 });
            TrianglesFront.Add(new MorphTriangle { A = 73, B = 74, C = 68 });
            TrianglesFront.Add(new MorphTriangle { A = 74, B = 75, C = 9 });

            //right oval
            TrianglesFront.Add(new MorphTriangle { A = 4, B = 53, C = 69 });
            TrianglesFront.Add(new MorphTriangle { A = 67, B = 53, C = 69 });
            TrianglesFront.Add(new MorphTriangle { A = 67, B = 77, C = 69 });
            TrianglesFront.Add(new MorphTriangle { A = 67, B = 77, C = 15 });
            TrianglesFront.Add(new MorphTriangle { A = 6, B = 4, C = 69 });
            TrianglesFront.Add(new MorphTriangle { A = 6, B = 76, C = 69 });
            TrianglesFront.Add(new MorphTriangle { A = 6, B = 4, C = 8 });
            TrianglesFront.Add(new MorphTriangle { A = 6, B = 76, C = 8 });
            TrianglesFront.Add(new MorphTriangle { A = 4, B = 59, C = 8 });
            TrianglesFront.Add(new MorphTriangle { A = 10, B = 59, C = 8 });
            TrianglesFront.Add(new MorphTriangle { A = 10, B = 59, C = 55 });
            TrianglesFront.Add(new MorphTriangle { A = 10, B = 55, C = 11 });
            TrianglesFront.Add(new MorphTriangle { A = 10, B = 8, C = 76 });
            TrianglesFront.Add(new MorphTriangle { A = 10, B = 11, C = 75 });

            TrianglesFront.Add(new MorphTriangle { A = 46, B = 67, C = 44 });
            TrianglesFront.Add(new MorphTriangle { A = 26, B = 67, C = 44 });
            TrianglesFront.Add(new MorphTriangle { A = 75, B = 10, C = 76 });
            TrianglesFront.Add(new MorphTriangle { A = 46, B = 53, C = 67 });
            TrianglesFront.Add(new MorphTriangle { A = 76, B = 77, C = 69 });
            TrianglesFront.Add(new MorphTriangle { A = 70, B = 77, C = 15 });
            TrianglesFront.Add(new MorphTriangle { A = 71, B = 70, C = 17 });
            TrianglesFront.Add(new MorphTriangle { A = 26, B = 67, C = 15 });

            //left eye
            TrianglesFront.Add(new MorphTriangle { A = 23, B = 35, C = 37 });
            TrianglesFront.Add(new MorphTriangle { A = 0, B = 35, C = 37 });
            TrianglesFront.Add(new MorphTriangle { A = 28, B = 35, C = 0 });
            TrianglesFront.Add(new MorphTriangle { A = 28, B = 36, C = 0 });
            TrianglesFront.Add(new MorphTriangle { A = 36, B = 24, C = 38 });
            TrianglesFront.Add(new MorphTriangle { A = 36, B = 0, C = 38 });
            TrianglesFront.Add(new MorphTriangle { A = 27, B = 0, C = 38 });
            TrianglesFront.Add(new MorphTriangle { A = 27, B = 0, C = 37 });
            //left eyelash
            TrianglesFront.Add(new MorphTriangle { A = 23, B = 12, C = 18 });
            TrianglesFront.Add(new MorphTriangle { A = 18, B = 35, C = 23 });
            TrianglesFront.Add(new MorphTriangle { A = 18, B = 35, C = 16 });
            TrianglesFront.Add(new MorphTriangle { A = 28, B = 35, C = 16 });
            TrianglesFront.Add(new MorphTriangle { A = 28, B = 19, C = 16 });
            TrianglesFront.Add(new MorphTriangle { A = 28, B = 19, C = 36 });
            TrianglesFront.Add(new MorphTriangle { A = 13, B = 19, C = 36 });
            TrianglesFront.Add(new MorphTriangle { A = 24, B = 22, C = 13 });
            TrianglesFront.Add(new MorphTriangle { A = 13, B = 36, C = 24 });
            TrianglesFront.Add(new MorphTriangle { A = 38, B = 24, C = 43 });
            TrianglesFront.Add(new MorphTriangle { A = 37, B = 27, C = 43 });
            TrianglesFront.Add(new MorphTriangle { A = 37, B = 23, C = 43 });

            //right eye
            TrianglesFront.Add(new MorphTriangle { A = 26, B = 40, C = 42 });
            TrianglesFront.Add(new MorphTriangle { A = 1, B = 40, C = 42 });
            TrianglesFront.Add(new MorphTriangle { A = 32, B = 40, C = 1 });
            TrianglesFront.Add(new MorphTriangle { A = 32, B = 39, C = 1 });
            TrianglesFront.Add(new MorphTriangle { A = 39, B = 25, C = 41 });
            TrianglesFront.Add(new MorphTriangle { A = 39, B = 1, C = 41 });
            TrianglesFront.Add(new MorphTriangle { A = 31, B = 1, C = 41 });
            TrianglesFront.Add(new MorphTriangle { A = 31, B = 1, C = 42 });
            //right eyelash
            TrianglesFront.Add(new MorphTriangle { A = 26, B = 15, C = 21 });
            TrianglesFront.Add(new MorphTriangle { A = 21, B = 40, C = 26 });
            TrianglesFront.Add(new MorphTriangle { A = 21, B = 40, C = 17 });
            TrianglesFront.Add(new MorphTriangle { A = 32, B = 40, C = 17 });
            TrianglesFront.Add(new MorphTriangle { A = 32, B = 20, C = 17 });
            TrianglesFront.Add(new MorphTriangle { A = 32, B = 20, C = 39 });
            TrianglesFront.Add(new MorphTriangle { A = 14, B = 20, C = 39 });
            TrianglesFront.Add(new MorphTriangle { A = 25, B = 22, C = 14 });
            TrianglesFront.Add(new MorphTriangle { A = 14, B = 25, C = 39 });
            TrianglesFront.Add(new MorphTriangle { A = 25, B = 41, C = 44 });
            TrianglesFront.Add(new MorphTriangle { A = 31, B = 42, C = 44 });
            TrianglesFront.Add(new MorphTriangle { A = 26, B = 42, C = 44 });

            //middle face
            TrianglesFront.Add(new MorphTriangle { A = 14, B = 22, C = 13 });

            // Upper part
            TrianglesFront.Add(new MorphTriangle { A = 12, B = 18, C = 72 });
            TrianglesFront.Add(new MorphTriangle { A = 16, B = 18, C = 72 });
            TrianglesFront.Add(new MorphTriangle { A = 16, B = 19, C = 71 });
            TrianglesFront.Add(new MorphTriangle { A = 13, B = 19, C = 71 });
            TrianglesFront.Add(new MorphTriangle { A = 13, B = 14, C = 71 });
            TrianglesFront.Add(new MorphTriangle { A = 14, B = 20, C = 71 });
            TrianglesFront.Add(new MorphTriangle { A = 17, B = 20, C = 71 });
            TrianglesFront.Add(new MorphTriangle { A = 17, B = 21, C = 70 });
            TrianglesFront.Add(new MorphTriangle { A = 15, B = 21, C = 70 });

            // nose
            TrianglesFront.Add(new MorphTriangle { A = 43, B = 22, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 43, B = 45, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 47, B = 45, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 44, B = 22, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 44, B = 46, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 46, B = 48, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 48, B = 49, C = 2 });
            TrianglesFront.Add(new MorphTriangle { A = 47, B = 49, C = 2 });
            // nose-eyes
            TrianglesFront.Add(new MorphTriangle { A = 25, B = 22, C = 44 });
            TrianglesFront.Add(new MorphTriangle { A = 44, B = 41, C = 31 });
            TrianglesFront.Add(new MorphTriangle { A = 24, B = 22, C = 43 });
            TrianglesFront.Add(new MorphTriangle { A = 43, B = 38, C = 27 });

            // left mouth
            TrianglesFront.Add(new MorphTriangle { A = 3, B = 52, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 45, B = 52, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 45, B = 47, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 49, B = 47, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 49, B = 54, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 3, B = 60, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 3, B = 60, C = 63 });
            TrianglesFront.Add(new MorphTriangle { A = 3, B = 58, C = 63 });
            TrianglesFront.Add(new MorphTriangle { A = 64, B = 58, C = 55 });
            TrianglesFront.Add(new MorphTriangle { A = 63, B = 61, C = 64 });
            TrianglesFront.Add(new MorphTriangle { A = 60, B = 54, C = 61 });
            TrianglesFront.Add(new MorphTriangle { A = 60, B = 54, C = 56 });
            TrianglesFront.Add(new MorphTriangle { A = 63, B = 64, C = 58 });
            TrianglesFront.Add(new MorphTriangle { A = 63, B = 60, C = 61 });
            // right mouth
            TrianglesFront.Add(new MorphTriangle { A = 4, B = 53, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 46, B = 53, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 46, B = 48, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 49, B = 48, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 49, B = 54, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 4, B = 62, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 4, B = 62, C = 65 });
            TrianglesFront.Add(new MorphTriangle { A = 4, B = 59, C = 65 });
            TrianglesFront.Add(new MorphTriangle { A = 64, B = 59, C = 55 });
            TrianglesFront.Add(new MorphTriangle { A = 65, B = 61, C = 64 });
            TrianglesFront.Add(new MorphTriangle { A = 62, B = 54, C = 61 });
            TrianglesFront.Add(new MorphTriangle { A = 62, B = 54, C = 57 });
            TrianglesFront.Add(new MorphTriangle { A = 65, B = 64, C = 59 });
            TrianglesFront.Add(new MorphTriangle { A = 65, B = 62, C = 61 });

            #endregion

            #region Triangles right

            TrianglesRight.Add(new MorphTriangle { A = 5, B = 7, C = 78 });
            TrianglesRight.Add(new MorphTriangle { A = 9, B = 7, C = 78 });
            TrianglesRight.Add(new MorphTriangle { A = 9, B = 11, C = 78 });
            TrianglesRight.Add(new MorphTriangle { A = 11, B = 75, C = 78 });
            TrianglesRight.Add(new MorphTriangle { A = 5, B = 3, C = 68 });
            TrianglesRight.Add(new MorphTriangle { A = 66, B = 12, C = 79 });
            TrianglesRight.Add(new MorphTriangle { A = 66, B = 43, C = 45 });
            TrianglesRight.Add(new MorphTriangle { A = 66, B = 45, C = 68 });
            TrianglesRight.Add(new MorphTriangle { A = 50, B = 45, C = 68 });
            TrianglesRight.Add(new MorphTriangle { A = 50, B = 52, C = 68 });
            TrianglesRight.Add(new MorphTriangle { A = 68, B = 52, C = 3 });
            TrianglesRight.Add(new MorphTriangle { A = 5, B = 7, C = 3 });
            TrianglesRight.Add(new MorphTriangle { A = 7, B = 63, C = 3 });
            TrianglesRight.Add(new MorphTriangle { A = 55, B = 75, C = 11 });
            TrianglesRight.Add(new MorphTriangle { A = 55, B = 58, C = 11 });
            TrianglesRight.Add(new MorphTriangle { A = 9, B = 58, C = 11 });
            TrianglesRight.Add(new MorphTriangle { A = 9, B = 63, C = 7 });
            TrianglesRight.Add(new MorphTriangle { A = 63, B = 58, C = 9 });

            TrianglesRight.Add(new MorphTriangle { A = 12, B = 79, C = 18 });
            TrianglesRight.Add(new MorphTriangle { A = 72, B = 79, C = 18 });
            TrianglesRight.Add(new MorphTriangle { A = 72, B = 16, C = 18 });
            TrianglesRight.Add(new MorphTriangle { A = 72, B = 16, C = 73 });
            TrianglesRight.Add(new MorphTriangle { A = 73, B = 13, C = 16 });
            TrianglesRight.Add(new MorphTriangle { A = 73, B = 13, C = 22 });
            TrianglesRight.Add(new MorphTriangle { A = 73, B = 2, C = 22 });

            TrianglesRight.Add(new MorphTriangle { A = 2, B = 49, C = 47 });
            TrianglesRight.Add(new MorphTriangle { A = 2, B = 43, C = 47 });
            TrianglesRight.Add(new MorphTriangle { A = 45, B = 43, C = 47 });
            TrianglesRight.Add(new MorphTriangle { A = 45, B = 49, C = 47 });
            TrianglesRight.Add(new MorphTriangle { A = 50, B = 49, C = 45 });
            TrianglesRight.Add(new MorphTriangle { A = 50, B = 52, C = 56 });
            TrianglesRight.Add(new MorphTriangle { A = 3, B = 52, C = 56 });

            TrianglesRight.Add(new MorphTriangle { A = 2, B = 43, C = 22 });
            TrianglesRight.Add(new MorphTriangle { A = 37, B = 43, C = 22 });
            TrianglesRight.Add(new MorphTriangle { A = 37, B = 43, C = 66 });
            TrianglesRight.Add(new MorphTriangle { A = 37, B = 23, C = 66 });
            TrianglesRight.Add(new MorphTriangle { A = 12, B = 23, C = 66 });
            TrianglesRight.Add(new MorphTriangle { A = 35, B = 23, C = 12 });
            TrianglesRight.Add(new MorphTriangle { A = 35, B = 23, C = 37 });
            TrianglesRight.Add(new MorphTriangle { A = 35, B = 22, C = 37 });
            TrianglesRight.Add(new MorphTriangle { A = 35, B = 13, C = 12 });
            TrianglesRight.Add(new MorphTriangle { A = 35, B = 13, C = 22 });
            TrianglesRight.Add(new MorphTriangle { A = 18, B = 13, C = 12 });
            TrianglesRight.Add(new MorphTriangle { A = 18, B = 13, C = 16 });

            TrianglesRight.Add(new MorphTriangle { A = 49, B = 50, C = 56 });
            TrianglesRight.Add(new MorphTriangle { A = 49, B = 61, C = 56 });
            TrianglesRight.Add(new MorphTriangle { A = 3, B = 61, C = 56 });
            TrianglesRight.Add(new MorphTriangle { A = 3, B = 60, C = 63 });

            TrianglesRight.Add(new MorphTriangle { A = 58, B = 64, C = 63 });
            TrianglesRight.Add(new MorphTriangle { A = 58, B = 64, C = 55 });

            TrianglesRight.Add(new MorphTriangle { A = 49, B = 2, C = 75 });            // сомнительные губищи
            TrianglesRight.Add(new MorphTriangle { A = 49, B = 61, C = 75 });
            TrianglesRight.Add(new MorphTriangle { A = 64, B = 61, C = 75 });
            TrianglesRight.Add(new MorphTriangle { A = 64, B = 55, C = 75 });

            TrianglesRight.Add(new MorphTriangle { A = 60, B = 61, C = 63 });
            TrianglesRight.Add(new MorphTriangle { A = 61, B = 64, C = 63 });

            #endregion

            InitializeMorphin();
        }

        private void InitializeMorphin()
        {
            var headMesh = headPoints.HeadMesh;

            foreach (var part in headMesh.Parts)
            {
                foreach (var point in part.MorphPoints)
                {
                    for (int index = 0; index < TrianglesFront.Count; ++index)
                    {
                        var triangle = TrianglesFront[index];
                        var a = headPoints.Points[triangle.A].Xy;
                        var b = headPoints.Points[triangle.B].Xy;
                        var c = headPoints.Points[triangle.C].Xy;

                        point.Initialize(ref a, ref b, ref c, index, true);
                    }

                    for (int index = 0; index < TrianglesRight.Count; ++index)
                    {
                        var triangle = TrianglesRight[index];
                        var a = headPoints.Points[triangle.A].Zy;
                        var b = headPoints.Points[triangle.B].Zy;
                        var c = headPoints.Points[triangle.C].Zy;

                        point.Initialize(ref a, ref b, ref c, index, false);
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
                    bool hasFrontPoint = false;
                    if (point.FrontTriangle.TriangleIndex > -1 )
                    {
                        var triangle = TrianglesFront[point.FrontTriangle.TriangleIndex];
                        var a = headPoints.Points[triangle.A].Xy;
                        var b = headPoints.Points[triangle.B].Xy;
                        var c = headPoints.Points[triangle.C].Xy;

                        point.Position = point.MorphFront(ref a, ref b, ref c);
                        hasFrontPoint = true;
                    }

                    if (point.RightTriangle.TriangleIndex > -1)
                    {
                        var triangle = TrianglesRight[point.RightTriangle.TriangleIndex];
                        var a = headPoints.Points[triangle.A].Zy;
                        var b = headPoints.Points[triangle.B].Zy;
                        var c = headPoints.Points[triangle.C].Zy;

                        /*point.Position = point.MorphRight(ref a, ref b, ref c);*/

                        Vector3 rightPoisition = point.MorphRight(ref a, ref b, ref c);
                        point.Position.Y = hasFrontPoint ? (rightPoisition.Y + point.Position.Y) * 0.5f : rightPoisition.Y;
                        point.Position.Z = rightPoisition.Z;
                    }

                    foreach (var index in point.Indices)
                    {
                        part.Vertices[index].Position = point.Position;
                    }
                }
                part.UpdateBuffers();
            }
        }

        public void DrawTriangles(bool useProfilePoints)
        {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color4(0.0f, 1.0f, 0.0f, 0.3f);

        

            var points = useProfilePoints ? TrianglesRight : TrianglesFront;
            foreach (var triangle in points)
            {
                var a = headPoints.GetWorldPoint(triangle.A);
                var b = headPoints.GetWorldPoint(triangle.B);
                var c = headPoints.GetWorldPoint(triangle.C);
                GL.Vertex3(a);
                GL.Vertex3(b);
                GL.Vertex3(c);
            }
            GL.End();
        }

        public void Draw(bool useProfilePoints)
        {
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Begin(PrimitiveType.Lines);

            var points = useProfilePoints ? TrianglesRight : TrianglesFront;
            foreach (var triangle in points)
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
