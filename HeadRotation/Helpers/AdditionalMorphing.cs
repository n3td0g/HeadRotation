using HeadRotation.Morphing;
using HeadRotation.Render;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeadRotation.Helpers
{
    public class AdditionalMorphing
    {
        public RenderMesh HeadMesh;
        public MorphTriangleType Type = MorphTriangleType.Left;
        public List<Vector2> Convex = new List<Vector2>();
        public List<uint> Indices = new List<uint>();
        public bool IsReversed = false;

        public int LastIndex = 0;
        public int FirstIndex = 0;

        public void Initialize(RenderMesh headMesh, ProjectedDots dots, HeadMorphing headMorphing)
        {
            HeadMesh = headMesh;

            MorphTriangleType realType = Type;
            if (IsReversed)
            {
                realType = Type == MorphTriangleType.Left ? MorphTriangleType.Right : MorphTriangleType.Left;
            }

            List<Vector2> points = new List<Vector2>();
            foreach (var part in HeadMesh.Parts)
            {
                foreach (var point in part.MorphPoints)
                {
                    if (point.TriangleType != realType || point.Position.Z < 0.0f)
                        continue;
                    if (IsReversed)
                        points.Add(point.ReversedWorldPosition.Xy);
                    else
                        points.Add(point.WorldPosition.Xy);
                }
            }

            Convex = Triangulate.ComputeConvexHull(points, (Type == MorphTriangleType.Left) == IsReversed);

            LastIndex = Convex.Count - 1;
            float prevY = Convex[LastIndex].Y;
            for (int i = LastIndex - 1; i >= 0; --i)
            {
                float y = Convex[i].Y;
                if (y < prevY)
                    break;
                prevY = y;
                FirstIndex = i;
            }
            Convex.RemoveRange(0, FirstIndex);
            if (Type == MorphTriangleType.Left)
            {
                Convex.Insert(0, GetPoint(dots.Points[52]));
                Convex.Insert(0, GetPoint(dots.Points[3]));
                Convex.Insert(0, GetPoint(dots.Points[58]));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[72].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[73].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[74].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[75].Xy));
            }
            else
            {
                Convex.Insert(0, GetPoint(dots.Points[53]));
                Convex.Insert(0, GetPoint(dots.Points[4]));
                Convex.Insert(0, GetPoint(dots.Points[59]));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[70].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[77].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[76].Xy));
                Convex.Insert(0, GetPoint(headMorphing.headPoints.Points[75].Xy));
            }
            FirstIndex = 7;
            LastIndex = Convex.Count - 1;

            var tempPoints = new List<Point>();
            for (int index = 0; index < Convex.Count; ++index)
            {
                var position = Convex[index];
                tempPoints.Add(new Point((uint)index, position.X, position.Y));
            }
            Indices.Clear();
            Indices.AddRange(Triangulate.Delaunay(tempPoints));
        }

        private Vector2 GetPoint(Vector2 point)
        {
            if (IsReversed)
                point.X = -point.X;
            return point;
        }

        public void ProcessPoints(ProjectedDots dots)
        {
            /*foreach (var part in HeadMesh.Parts)
            {
                foreach (var point in part.MorphPoints)
                {
                    point.AdditionalTriangle.TriangleIndex = -1;
                    if (point.Position.Z < 0.0f) //point.TriangleType != Type || 
                        continue;
                    for (int i = 0; i < Indices.Count; i += 3)
                    {
                        var index = i / 3;
                        var a = Convex[(int)Indices[i]];
                        var b = Convex[(int)Indices[i + 1]];
                        var c = Convex[(int)Indices[i + 2]];

                        point.AdditionalInitialize(ref a, ref b, ref c, index, IsReversed);
                    }
                }
            }*/

            int[] dotIndices = Type == MorphTriangleType.Right ?
                new int[] { 67, 69, 6, 8, 10, 11 } :
                new int[] { 66, 68, 5, 7, 9, 11 };

            var right = new Vector3(1.0f, 0.0f, 0.0f);
            right = HeadMesh.GetWorldPoint(right);

            var a = dots.Points[dotIndices.Last()];
            var b = Convex[LastIndex];

            var sa = Vector3.Dot(new Vector3(a.X, a.Y, 0.0f), right);
            var sb = Vector3.Dot(new Vector3(b.X, b.Y, 0.0f), right);

            var ds = sa / sb;

            foreach (var part in HeadMesh.Parts)
            {
                foreach (var point in part.MorphPoints)
                {
                    point.Position.X *= ds;

                    foreach (var index in point.Indices)
                    {
                        part.Vertices[index].Position = point.Position;
                    }
                }

                //part.UpdateBuffers();
            }


                    /*List<Vector2> targetPoints = new List<Vector2>();
                    foreach (var index in dotIndices)
                        targetPoints.Add(GetPoint(dots.Points[index]));

                    float targetLength = 0.0f;
                    for (int i = 1; i < targetPoints.Count; ++i)
                        targetLength += (targetPoints[i] - targetPoints[i - 1]).Length;

                    float sourceLength = 0.0f;
                    for (int i = FirstIndex; i < LastIndex; ++i)
                        sourceLength += (Convex[i + 1] - Convex[i]).Length;

                    List<Vector2> NewConvex = new List<Vector2>();
                    NewConvex.AddRange(Convex);
                    NewConvex[FirstIndex] = targetPoints[0];
                    NewConvex[LastIndex] = targetPoints[targetPoints.Count - 1];

                    float k = targetLength / sourceLength;
                    float sourceDistance = 0.0f;
                    float targetDistance = 0.0f;
                    int targetIndex = 0;
                    for (int i = FirstIndex + 1; i < LastIndex; ++i)
                    {
                        var point = Convex[i];
                        sourceDistance += (Convex[i - 1] - point).Length;
                        var td = sourceDistance * k;
                        var direction = targetPoints[targetIndex + 1] - targetPoints[targetIndex];
                        var distance = direction.Length;
                        while ((targetDistance + distance) < td)
                        {
                            targetDistance += distance;
                            targetIndex++;
                            direction = targetPoints[targetIndex + 1] - targetPoints[targetIndex];
                            distance = direction.Length;
                        }
                        direction.Normalize();
                        NewConvex[i] = targetPoints[targetIndex] + direction * (td - targetDistance);
                    }
                    Convex = NewConvex;

                    MorphPoints();*/
                }


        private void MorphPoints()
        {
            var start = Convex[FirstIndex];
            var end = Convex[LastIndex];
            var centerY = (start.Y + end.Y) * 0.5f;
            var height = Math.Abs((start.Y - end.Y)) * 0.5f;

            foreach (var part in HeadMesh.Parts)
            {
                foreach (var point in part.MorphPoints)
                {
                    if (point.AdditionalTriangle.TriangleIndex > -1)
                    {
                        var index = point.AdditionalTriangle.TriangleIndex * 3;
                        var i0 = (int)Indices[index];
                        var i1 = (int)Indices[index + 1];
                        var i2 = (int)Indices[index + 2];

                        var a = Convex[i0];
                        var b = Convex[i1];
                        var c = Convex[i2];

                        var worldPosition = point.AdditionalMorph(ref a, ref b, ref c, IsReversed);
                        var ky = (point.WorldPosition.Y - centerY) / height;
                        ky = 1.0f - Math.Min(ky * ky, 1.0f);
                        var kz = point.Position.Z / 2.0f;
                        kz = kz * kz;
                        var k = Math.Min(Math.Min(ky, kz), 1.0f);
                        var originalPoint = IsReversed ? HeadMesh.GetReversePositionFromWorld(worldPosition) : HeadMesh.GetPositionFromWorld(worldPosition);

                        point.Position = k * originalPoint + point.Position * (1.0f - k); // = point.Position;//
                    }

                    foreach (var index in point.Indices)
                    {
                        part.Vertices[index].Position = point.Position;
                    }
                }
                part.UpdateBuffers();
            }
        }
    }
}
