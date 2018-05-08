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
        private Vector3 upVector;

        
        static List<int> headIndices = new List<int>();
        #region Mirrored indices
        public static List<int> mirroredPoints = new List<int>
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
        #endregion

        #region Lips indices
        public static List<int> LipsIndiecs = new List<int>
        {
            3, 56, 54, 57, 60, 61, 62, 4,
            63, 64, 65, 58, 55, 59
        };

        public static List<int> LipsCenterIndiecs = new List<int>
        {
            54, 61, 64, 55
        };
        #endregion

        //new List<int> { 66, 67, 68, 69, 5, 6, 7, 8, 9, 10, 11 };
        private Matrix4 RotationMatrix;

        public void ProcessPoints(ProjectedDots dots, HeadPoints points)
        {
            projectedDots = dots;
            headPoints = points;

            rightVector = headPoints.GetWorldPoint(new Vector3(1.0f, 0.0f, 0.0f));
            forwardVector = headPoints.GetWorldPoint(new Vector3(0.0f, 0.0f, 1.0f));
            upVector = headPoints.GetWorldPoint(new Vector3(0.0f, 1.0f, 0.0f));

            headIndices.Clear();
            for (int i = 0; i < dots.Points.Count; ++i)
                headIndices.Add(i);

            var rotationMatrix = headPoints.HeadMesh.RotationMatrix;
            Matrix4.Invert(ref rotationMatrix, out RotationMatrix);

            ProcessHeadPoints();

            FixLipsPoints();
        }

       /* public Vector3 Point0;
        public Vector3 Point1;

        public Vector3 Point2;
        public Vector3 Point3;*/

        private void ProcessHeadPoints()
        {
            foreach (int index in headIndices)
            {
                Vector2 targetPoint = projectedDots.Points[index];
                Vector3 current = headPoints.GetWorldPoint(index);

                var point0 = new Vector3(targetPoint.X, targetPoint.Y, -1000.0f);
                var point1 = new Vector3(targetPoint.X, targetPoint.Y, 1000.0f);

                point0 = Vector4.Transform(new Vector4(point0), RotationMatrix).Xyz;
                point1 = Vector4.Transform(new Vector4(point1), RotationMatrix).Xyz;               

                var p = point1 - point0;
                var t = current - point0;
                p.Normalize();
                var dot = Vector3.Dot(t, p);

                var point3 = point0 + p * dot;

                /*if (index == headIndices[44])
                {
                    Point0 = headPoints.GetWorldPoint(point0);
                    Point1 = headPoints.GetWorldPoint(point1);

                    Point2 = current;
                    Point3 = headPoints.GetWorldPoint(point3);
                }*/

                headPoints.Points[index] = point3;
            }
        }

        private void FixLipsPoints()
        {
            float minX = 99999.0f;
            float maxX = -99999.0f;

            foreach(var index in LipsCenterIndiecs)
            {
                var headPoint = headPoints.Points[index];
                minX = Math.Min(minX, headPoint.X);
                maxX = Math.Max(maxX, headPoint.X);
            }

            var lipsCenter = (maxX + minX) * 0.5f;

            const int topNose = 22;
            const int bottomNose = 2;

            var noseCenter = (headPoints.Points[topNose].X + headPoints.Points[bottomNose].X) * 0.5f;

            foreach (var index in LipsIndiecs)
            {
                var headPoint = headPoints.Points[index];
                headPoint.X += noseCenter - lipsCenter;
                headPoints.Points[index] = headPoint;
            }
        }

//=================================== OLD ===================================

        public void ProcessPoints_old(ProjectedDots dots, HeadPoints points)
        { 
            projectedDots = dots;
            headPoints = points;

            headIndices.Clear();
            for (int i = 0; i < dots.Points.Count; ++i)
                headIndices.Add(i);

            var rotaionMatrix = headPoints.HeadMesh.RotationMatrix;
            Matrix4.Invert(ref rotaionMatrix, out RotationMatrix);
            
            rightPower = 1.0f - Math.Abs(headPoints.HeadMesh.HeadAngle) * 2.0f / (float)Math.PI;
            //rightPower = Math.Min(1.0f, Math.Max(rightPower, 0.0f));
            // (float)Math.Cos(headPoints.HeadMesh.HeadAngle);

            rightVector = headPoints.GetWorldPoint(new Vector3(1.0f, 0.0f, 0.0f));
            forwardVector = headPoints.GetWorldPoint(new Vector3(0.0f, 0.0f, 1.0f));            

            ProcessHeadPoints_old();

            //SpecialAlignment();         // выравниваем точки рта-глаз-носа по центру лица
          //  MirrorPoints(headPoints.HeadMesh.HeadAngle > 0.0f);             // отразить форму лица зеркально
        }

        public void MirrorPoints()
        {
            MirrorPoints(headPoints.HeadMesh.HeadAngle > 0.0f);
        }

        private void ProcessHeadPoints_old()
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

        /// <summary> Выравнивание отдельных частей лица по центру (рот, нос) </summary>
        private void SpecialAlignment()
        {
            var leftPoint = headPoints.Points[68];                                     // крайняя левая-правай боковые точки лица на моделе (вдоль носа)
            var rightPoint = headPoints.Points[69];
            var centerXPos = leftPoint.X + ((rightPoint.X - leftPoint.X) / 2f);        // центр лица по крайним боковым точкам..
            var diff = centerXPos - headPoints.Points[2].X;                            // смещение основных частей лица относительно центра. на основе кончика носа

            MovePoint(2, diff);                            // основной нос
            MovePoint(43, diff);
            MovePoint(45, diff);
            MovePoint(47, diff);
            MovePoint(49, diff);
            MovePoint(48, diff);
            MovePoint(46, diff);
            MovePoint(44, diff);

            MovePoint(3, diff);         // рот
            MovePoint(56, diff); 
            MovePoint(60, diff);
            MovePoint(54, diff);
            MovePoint(61, diff);
            MovePoint(57, diff);
            MovePoint(62, diff);
            MovePoint(4, diff);
            MovePoint(65, diff);
            MovePoint(59, diff);
            MovePoint(64, diff);
            MovePoint(55, diff);
            MovePoint(63, diff);
            MovePoint(58, diff);

            MovePoint(52, diff);         // носо-рот.
            MovePoint(50, diff);
            MovePoint(53, diff);
            MovePoint(51, diff);

            leftPoint = headPoints.Points[66];                                     // крайняя левая-правай боковые точки лица на моделе (вдоль глаз)
            rightPoint = headPoints.Points[67];
            centerXPos = leftPoint.X + ((rightPoint.X - leftPoint.X) / 2f);        // центр лица по крайним боковым точкам..
            diff = centerXPos - headPoints.Points[22].X;                            // смещение основных частей лица относительно центра. на основе переносицы

            MovePoint(22, diff);                        // переносица

            MovePoint(23, diff);                        // левый глаз
            MovePoint(35, diff);
            MovePoint(28, diff);
            MovePoint(36, diff);
            MovePoint(24, diff);
            MovePoint(38, diff);
            MovePoint(27, diff);
            MovePoint(37, diff);
            MovePoint(29, diff);
            MovePoint(0, diff);
            MovePoint(30, diff);

            MovePoint(25, diff);                        // правый глаз
            MovePoint(39, diff);
            MovePoint(32, diff);
            MovePoint(40, diff);
            MovePoint(26, diff);
            MovePoint(42, diff);
            MovePoint(31, diff);
            MovePoint(41, diff);
            MovePoint(33, diff);
            MovePoint(1, diff);
            MovePoint(34, diff);

            MovePoint(12, diff);                     // левая бровь
            MovePoint(18, diff);
            MovePoint(16, diff);
            MovePoint(19, diff);
            MovePoint(13, diff);

            MovePoint(14, diff);                     // правая бровь
            MovePoint(20, diff);
            MovePoint(17, diff);
            MovePoint(21, diff);
            MovePoint(15, diff);

        }
        private void MovePoint(int index, float diff)
        {
            var point = headPoints.Points[index];
            headPoints.Points[index] = new Vector3(point.X + diff, point.Y, point.Z);
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
