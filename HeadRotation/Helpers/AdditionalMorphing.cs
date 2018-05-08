﻿using HeadRotation.Morphing;
using HeadRotation.Render;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadRotation.Helpers
{
    public class AdditionalMorphing
    {
        public RenderMesh HeadMesh;
        public MorphTriangleType Type = MorphTriangleType.Left;
        public List<Vector2> Convex = new List<Vector2>();
        public List<uint> Indices = new List<uint>();

        public int LastIndex = 0;
        public int FirstIndex = 0;

        public void Initialize(RenderMesh headMesh, ProjectedDots dots)
        {
            HeadMesh = headMesh;

            List<MorphingPoint> points = new List<MorphingPoint>();
            foreach (var part in HeadMesh.Parts)
            {
                foreach(var point in part.MorphPoints)
                {
                    if (point.TriangleType != Type || point.Position.Z < 0.0f)
                        continue;

                    points.Add(point);
                }
            }

            Convex = Triangulate.ComputeConvexHull(points);

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
            Convex.Insert(0, dots.Points[52]);
            Convex.Insert(0, dots.Points[3]);
            Convex.Insert(0, dots.Points[58]);
            FirstIndex = 3;
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

        
        public void ProcessPoints(ProjectedDots dots)
        {
            List<MorphingPoint> points = new List<MorphingPoint>();
            foreach (var part in HeadMesh.Parts)
            {
                foreach (var point in part.MorphPoints)
                {
                    if (point.TriangleType != Type || point.Position.Z < 0.0f)
                        continue;
                    for (int i = 0; i < Indices.Count; i += 3)
                    {
                        var index = i / 3;
                        var a = Convex[(int)Indices[i]];
                        var b = Convex[(int)Indices[i + 1]];
                        var c = Convex[(int)Indices[i + 2]];

                        point.AdditionalInitialize(ref a, ref b, ref c, index);
                    }
                }
            }

            int[] dotIndices = new int[]{ 66, 68, 5, 7, 9, 11 };

            float targetLength = 0.0f;
            for (int i = 1; i < dotIndices.Length; ++i)
                targetLength += (dots.Points[dotIndices[i]] - dots.Points[dotIndices[i - 1]]).Length;

            float sourceLength = 0.0f;
            for (int i = FirstIndex; i < LastIndex; ++i)
                sourceLength += (Convex[i + 1] - Convex[i]).Length;

            List<Vector2> NewConvex = new List<Vector2>();
            NewConvex.AddRange(Convex);
            NewConvex[FirstIndex] = dots.Points[dotIndices[0]];
            NewConvex[LastIndex] = dots.Points[dotIndices.Last()];

            float k = targetLength / sourceLength;
            float sourceDistance = 0.0f;
            float targetDistance = 0.0f;
            int targetIndex = 0;
            for (int i = FirstIndex + 1; i < LastIndex; ++i)
            {
                var point = Convex[i];
                sourceDistance += (Convex[i - 1] - point).Length;
                var td = sourceDistance * k;
                var direction = dots.Points[dotIndices[targetIndex + 1]] - dots.Points[dotIndices[targetIndex]];
                var distance = direction.Length;
                while ((targetDistance + distance) < td)
                {
                    targetDistance += distance;
                    targetIndex++;
                    direction = dots.Points[dotIndices[targetIndex + 1]] - dots.Points[dotIndices[targetIndex]];
                    distance = direction.Length;
                }
                direction.Normalize();
                NewConvex[i] = dots.Points[dotIndices[targetIndex]] + direction * (td - targetDistance);
            }
            Convex = NewConvex;

            MorphPoints();
        }


        private void MorphPoints()
        {
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

                        var worldPosition = point.AdditionalMorph(ref a, ref b, ref c);
                        point.Position = HeadMesh.GetPositionFromWorld(worldPosition);
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
