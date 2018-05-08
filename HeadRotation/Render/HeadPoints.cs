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
        public List<bool> IsVisible = new List<bool>();
        public Camera RenderCamera;
        public RenderMesh HeadMesh;

        #region Selection

        public int SelectedPoint = -1;

        private bool movingPoint = false;
        private float selectionDepth;

        #endregion

        #region Render
        public List<TextRender> TextRenderList = null;
        public Font TextFont = new Font(new FontFamily(GenericFontFamilies.SansSerif), 20, GraphicsUnit.Pixel);

        public int IndexBuffer, VertexBuffer = 0;
        public List<uint> Indices = new List<uint>();
        public Vertex3d[] Vertices = null;
        #endregion

        #endregion

        public void GenerateSphere(float radius, int rings, int sectors)
        {
            Destroy();

            float R = 1.0f / (rings - 1);
            float S = 1.0f / (sectors - 1);

            Vertices = new Vertex3d[rings * sectors];
            int index = 0;
            for (int r = 0; r < rings; r++)
            {
                for (int s = 0; s < sectors; s++)
                {
                    float x = (float)(Math.Cos(2.0 * Math.PI * s * S) * Math.Sin(Math.PI * r * R));
                    float y = (float)(Math.Sin(2.0 * Math.PI * s * S) * Math.Sin(Math.PI * r * R));
                    float z = (float)Math.Cos(Math.PI * r * R);

                    var vertex = new Vertex3d();

                    vertex.TexCoord.X = s * S;
                    vertex.TexCoord.Y = r * R;

                    vertex.Position.X = x * radius;
                    vertex.Position.Y = y * radius;
                    vertex.Position.Z = z * radius;

                    vertex.Normal.X = x;
                    vertex.Normal.X = y;
                    vertex.Normal.X = z;

                    vertex.Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);

                    Vertices[index++] = vertex;
                }
            }

            index = 0;
            for (int r = 0; r < rings; r++)
            {
                for (int s = 0; s < sectors; s++)
                {
                    Indices.Add((uint)(r * sectors + s));
                    Indices.Add((uint)(r * sectors + (s + 1)));
                    Indices.Add((uint)((r + 1) * sectors + (s + 1)));
                    Indices.Add((uint)((r + 1) * sectors + s));
                }
            }

            GL.GenBuffers(1, out VertexBuffer);
            GL.GenBuffers(1, out IndexBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * Vertex3d.Stride), Vertices, BufferUsageHint.StreamDraw);
            OpenGlHelper.CheckErrors();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Count * sizeof(uint)), Indices.ToArray(), BufferUsageHint.DynamicDraw);
            OpenGlHelper.CheckErrors();
        }

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

        public void Destroy()
        {
            if (VertexBuffer != 0)
            {
                GL.DeleteBuffers(1, ref VertexBuffer);
                VertexBuffer = 0;
            }

            if (IndexBuffer != 0)
            {
                GL.DeleteBuffers(1, ref IndexBuffer);
                IndexBuffer = 0;
            }
        }

        public void DrawSpheres()
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);

            GL.VertexPointer(3, VertexPointerType.Float, Vertex3d.Stride, new IntPtr(0));
            GL.NormalPointer(NormalPointerType.Float, Vertex3d.Stride, new IntPtr(Vector3.SizeInBytes));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes));
            GL.ColorPointer(4, ColorPointerType.Float, Vertex3d.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vector2.SizeInBytes));

            foreach (var p in Points)
            {
                var point = GetWorldPoint(p);
                GL.Translate(point);
                GL.DrawRangeElements(PrimitiveType.Quads, 0, Indices.Count, Indices.Count, DrawElementsType.UnsignedInt, new IntPtr(0));
                GL.Translate(-point);
            }


            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.ColorArray);
        }

        public Vector3 GetWorldPoint(int pointIndex)
        {
            return GetWorldPoint(Points[pointIndex]);
        }

        public Vector3 GetWorldPoint(Vector3 point)
        {
            return HeadMesh.GetWorldPoint(point);
        }

        public void DrawDots()
        {

            const float scale = 0.7f;
            float textScale = scale * RenderCamera.Scale;
            InitializeTextRender();

            GL.PointSize(5.0f);
            GL.Begin(PrimitiveType.Points);

            for (int i = 0; i < Points.Count; ++i)
            {
                if (!IsVisible[i])
                    continue;

                if (i == SelectedPoint)
                    GL.Color3(1.0f, 0.0f, 0.0f);
                else
                    GL.Color3(0.0f, 1.0f, 0.0f);

                var point = GetWorldPoint(Points[i]);
                GL.Vertex3(point);
                TextRenderList[i].Position = point;
                TextRenderList[i].Scale = textScale;
            }

            GL.End();
            GL.PointSize(1.0f);

            for (var i = 0; i < TextRenderList.Count; i++)
            {
                var text = TextRenderList[i];
                if (!IsVisible[i])
                    continue;

                float cameraAngle = -(float)(RenderCamera.beta); //(float)(RenderCamera.beta);
                cameraAngle *= 180.0f;
                cameraAngle /= (float)Math.PI;
                cameraAngle += 90.0f;


                GL.Translate(text.Position);
                GL.Rotate(cameraAngle, 0.0f, 1.0f, 0.0f);
                text.Render();
                GL.Rotate(cameraAngle, 0.0f, -1.0f, 0.0f);
                GL.Translate(-text.Position);

            }

            GL.Disable(EnableCap.Texture2D);

        }

        private void InitializeTextRender()
        {
            if (TextRenderList == null || TextRenderList.Count != Points.Count)
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
            Vector2 selectionPoint = new Vector2(x, y);
            if (IsPointSelected(selectionPoint, SelectedPoint))
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
            Vector2 selectionPoint = new Vector2(x, y);
            SelectedPoint = -1;
            for (var i = Points.Count - 1; i >= 0; i--)
            {
                if (IsPointSelected(selectionPoint, i))
                {
                    SelectedPoint = i;
                    ProgramCore.MainForm.frmEditPoint.UpdateEditablePoint(Points[SelectedPoint]);
                    break;
                }
            }
        }

        public bool PointIsValid(int pointIndex)
        {
            return pointIndex >= 0 && pointIndex < Points.Count;
        }

        private bool IsPointSelected(Vector2 selectionPoint, int pointIndex)
        {
            if (!PointIsValid(pointIndex))
                return false;
            var point = GetWorldPoint(Points[pointIndex]);
            var screenPoint = RenderCamera.GetScreenPoint(point);
            float distance = (screenPoint - selectionPoint).Length;
            return distance < SelectionRadius;
        }
    }
}
