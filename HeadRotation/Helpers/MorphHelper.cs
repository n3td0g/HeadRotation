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

            ProcessHeadPoints();
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
    }
}
