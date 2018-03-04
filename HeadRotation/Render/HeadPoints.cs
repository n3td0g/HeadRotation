using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System;

namespace HeadRotation.Render
{
    public class HeadPoints
    {
        #region Var

        public const float SelectionRadius = 10.0f;
        public List<Vector3> Points = new List<Vector3>();
        public Camera RenderCamera;

        #region Selection

        public int SelectedPoint = -1;

        private bool movingPoint = false;
        private float selectionDepth;

        #endregion

        #endregion

        public Vector3 GetSelectedPoint()
        {
            return PointIsValid(SelectedPoint) ? Points[SelectedPoint] : Vector3.Zero;
        }

        public void SetSelectedPoint(Vector3 value)
        {
            if (PointIsValid(SelectedPoint))
                Points[SelectedPoint] = value;
        }

        public void Initialize(int Count)
        {
            SelectedPoint = -1;
            Points.Clear();
            Random R = new Random();
            for (int i = 0; i<Count; ++i)
            {
                Points.Add(Vector3.Zero);
            }
        }

        public void Draw()
        {
            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);

            for (int i = 0; i < Points.Count; ++i)
            {
                if (i == SelectedPoint)
                    GL.Color3(1.0f, 0.0f, 0.0f);
                else
                    GL.Color3(0.0f, 1.0f, 0.0f);

                GL.Vertex3(Points[i]);               
            }

            GL.End();
            GL.PointSize(1.0f);
        }

        public void StartMoving(int x, int y)
        {
            float distance = float.MaxValue;
            Vector2 selectionPoint = new Vector2(x, y);
            if (IsPointSelected(selectionPoint, SelectedPoint, out distance))
            {
                selectionDepth = RenderCamera.GetPointDepth(Points[SelectedPoint]);
                movingPoint = true;
            }
        }

        public void MovePoint(int x, int y)
        {
            if (!movingPoint)
                return;
            SetSelectedPoint(RenderCamera.GetWorldPoint(x, y, selectionDepth));
        }

        public void StopMoving(int x, int y)
        {
            if (!movingPoint)
                return;

            MovePoint(x, y);
            movingPoint = false;
        }

        public void SelectPoint(int x, int y)
        {
            if (movingPoint)
                return;
            float minDistance = float.MaxValue;
            Vector2 selectionPoint = new Vector2(x, y);
            SelectedPoint = -1;
            for (int i = 0; i < Points.Count; ++i)
            {
                float distance = float.MaxValue;
                if(IsPointSelected(selectionPoint, i, out distance))
                {
                    if(distance < minDistance)
                        SelectedPoint = i;
                }
            }           
        }       
        
        public bool PointIsValid(int pointIndex)
        {
            return pointIndex >= 0 && pointIndex < Points.Count;
        }

        private bool IsPointSelected(Vector2 selectionPoint, int pointIndex, out float distance)
        {
            distance = float.MaxValue;
            if (!PointIsValid(pointIndex))
                return false;            
            var point = Points[pointIndex];
            var screenPoint = RenderCamera.GetScreenPoint(point);
            distance = (screenPoint - selectionPoint).Length;
            return distance < SelectionRadius;
        }
    }
}
