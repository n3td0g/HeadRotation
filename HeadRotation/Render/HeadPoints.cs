using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Text;
using OpenTK.Graphics;
using System.Globalization;

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

        #region Render
        public List<TextRender> TextRenderList = null;
        public Font TextFont = new Font(new FontFamily(GenericFontFamilies.SansSerif), 20, GraphicsUnit.Pixel);
        #endregion

        #endregion

        public Vector3 GetSelectedPoint()
        {
            return PointIsValid(SelectedPoint) ? Points[SelectedPoint] : Vector3.Zero;
        }

        public void SetSelectedPoint(Vector3 value)
        {
            if (PointIsValid(SelectedPoint))
            {
                Points[SelectedPoint] = value;
                ProgramCore.MainForm.frmEditPoint.UpdateEditablePoint(value);
            }
        }

        public void Initialize(int Count)
        {
            SelectedPoint = -1;
            Points.Clear();
            Random R = new Random();
            for (int i = 0; i < Count; ++i)
            {
                Points.Add(Vector3.Zero);
            }
        }

        public void Draw()
        {
            const float scale = 0.7f;
            float textScale = scale * RenderCamera.Scale;
            InitializeTextRender();

            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);

            for (int i = 0; i < Points.Count; ++i)
            {
                if (i == SelectedPoint)
                    GL.Color3(1.0f, 0.0f, 0.0f);
                else
                    GL.Color3(0.0f, 1.0f, 0.0f);

                GL.Vertex3(Points[i]);
                TextRenderList[i].Position = Points[i];
                TextRenderList[i].Scale = textScale;
            }

            GL.End();
            GL.PointSize(1.0f);

            foreach (var text in TextRenderList)
            {
                GL.Translate(text.Position);
                text.Render();
                GL.Translate(-text.Position);
            }

            GL.Disable(EnableCap.Texture2D);

        }

        private void InitializeTextRender()
        {           
            if (TextRenderList == null)
            {
                TextRenderList = new List<TextRender>();
                for (var i = 0; i < Points.Count; i++)
                    TextRenderList.Add(new TextRender(TextFont, Color4.Black,
                        i.ToString(CultureInfo.InvariantCulture))
                    {
                        Scale = 1.0f,
                        Position = Points[i]
                    });
            }
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
            {
                return;
            }
            float minDistance = float.MaxValue;
            Vector2 selectionPoint = new Vector2(x, y);
            SelectedPoint = -1;
            for (int i = 0; i < Points.Count; ++i)
            {
                float distance = float.MaxValue;
                if (IsPointSelected(selectionPoint, i, out distance))
                {
                    if (distance < minDistance)
                    {
                        SelectedPoint = i;
                        ProgramCore.MainForm.frmEditPoint.UpdateEditablePoint(Points[SelectedPoint]);
                    }
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
