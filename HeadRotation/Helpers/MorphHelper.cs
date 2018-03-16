using HeadRotation.Render;
using OpenTK;
using System;
using System.Collections.Generic;

namespace HeadRotation.Helpers
{
    public class MorphHelper
    {
        public ProjectedDots projectedDots;
        public HeadPoints headPoints;
        private float rightPower = 0.0f;

        private Vector3 forwardVector;
        private Vector3 rightVector;
        private Vector3 topVector = new Vector3(0.0f, 1.0f, 0.0f);

        static List<int> headIndices = new List<int>();
        static List<int> mirroredPoints = new List<int>
        {
            12, 15,
            18, 21,
            16, 17,
            19, 20,
            13, 14,
            23, 26,
            35, 40,
            28, 32,
            36, 39,
            24, 25,
            38, 41,
            27, 31,
            37, 42,
            0, 1,
            66, 67,
            68, 69,
            5, 6,
            7, 8,
            9, 10,
            43, 44,
            45, 46,
            47, 48,
            50, 51,
            52, 53,
            56, 57,
            3, 4,
            60, 62,
            63, 65,
            58, 59
        };

        //new List<int> { 66, 67, 68, 69, 5, 6, 7, 8, 9, 10, 11 };
        private Matrix4 RotationMatrix;

        public void ProcessPoints(ProjectedDots dots, HeadPoints points)
        { 
            projectedDots = dots;
            headPoints = points;

            headIndices.Clear();
            for (int i = 0; i < dots.Points.Count; ++i)
                headIndices.Add(i);

            Matrix4.CreateRotationY(-headPoints.HeadMesh.HeadAngle, out RotationMatrix);

            rightPower = 1.0f - Math.Abs(headPoints.HeadMesh.HeadAngle) * 2.0f / (float)Math.PI;
            rightPower = Math.Min(1.0f, Math.Max(rightPower, 0.0f));

            rightVector = headPoints.GetWorldPoint(new Vector3(1.0f, 0.0f, 0.0f));
            forwardVector = headPoints.GetWorldPoint(new Vector3(0.0f, 0.0f, 1.0f));

            //ProcessHeadPoints();

            //MirrorPoints(headPoints.HeadMesh.HeadAngle > 0.0f);
        }

        private void ProcessHeadPoints()
        {
            foreach(int index in headIndices)
            {
                Vector2 targetPoint = projectedDots.Points[index];
                Vector3 result = headPoints.GetWorldPoint(index);
                result.Y = targetPoint.Y;
                float dist = targetPoint.X - result.X;
                Vector3 a = result + (dist / rightVector.X) * rightVector;
                Vector3 b = result + (dist / forwardVector.X) * forwardVector;
                Vector3 c = (a * rightPower) + (b * (1.0f - rightPower));
                result.X = c.X;
                result.Z = c.Z;
                headPoints.Points[index] = Vector4.Transform(new Vector4(result), RotationMatrix).Xyz;
            }
        }

        private void MirrorPoints(bool leftToRight)
        {
            for(int i = 0; i < mirroredPoints.Count; i += 2)
            {
                int a = mirroredPoints[leftToRight ? i : i + 1];
                int b = mirroredPoints[leftToRight ? i + 1 : i];
                var vectorA = headPoints.Points[a];
                var vectorB = headPoints.Points[b];
                vectorB = vectorA;
                vectorB.X = -vectorB.X;
                headPoints.Points[b] = vectorB;
            }
        }
    }
}
