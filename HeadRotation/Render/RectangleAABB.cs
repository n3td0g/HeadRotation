using HeadRotation.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace HeadRotation.Render
{
    public class RectangleAABB
    {
        public float Width
        {
            get { return B.X - A.X; }
        }

        public float Height
        {
            get { return B.Y - A.Y; }
        }

        public void Draw()
        {
            var a1 = new Vector3(a.X, a.Y, b.Z);
            var a2 = new Vector3(b.X, a.Y, a.Z);
            var a3 = new Vector3(b.X, a.Y, b.Z);

            var b1 = new Vector3(a.X, b.Y, b.Z);
            var b2 = new Vector3(b.X, b.Y, a.Z);
            var b3 = new Vector3(a.X, b.Y, a.Z);

            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Begin(PrimitiveType.Lines);

            RenderHelper.DrawLine(a, a1);
            RenderHelper.DrawLine(a1, a2);
            RenderHelper.DrawLine(a2, a3);
            RenderHelper.DrawLine(a3, a);

            RenderHelper.DrawLine(b, b1);
            RenderHelper.DrawLine(b1, b2);
            RenderHelper.DrawLine(b2, b3);
            RenderHelper.DrawLine(b3, b);

            RenderHelper.DrawLine(a1, b1);
            RenderHelper.DrawLine(a2, b2);
            RenderHelper.DrawLine(a, b3);
            RenderHelper.DrawLine(b, a3);            

            GL.End();
        }

        

        public RectangleAABB()
        {
            A = new Vector3(99999.0f, 99999.0f, 99999.0f);
            B = new Vector3(-99999.0f, -99999.0f, -99999.0f);
        }

        public void AddPoint(Vector3 point)
        {
            a.X = Math.Min(A.X, point.X);
            a.Y = Math.Min(A.Y, point.Y);
            a.Z = Math.Max(A.Z, point.Z);

            b.X = Math.Max(B.X, point.X);
            b.Y = Math.Max(B.Y, point.Y);
            b.Z = Math.Max(B.Z, point.Z);
        }

        public void AddPoint(Vector2 point)
        {
            a.X = Math.Min(a.X, point.X);
            a.Y = Math.Min(a.Y, point.Y);

            b.X = Math.Max(b.X, point.X);
            b.Y = Math.Max(b.Y, point.Y);
        }

        private Vector3 a;
        public Vector3 A
        {
            get
            {
                return a;
            }
            set
            {
                a = value;
                UpdateSize();
            }
        }

        private Vector3 b;
        public Vector3 B
        {
            get
            {
                return b;
            }
            set
            {
                b = value;
                UpdateSize();
            }
        }

        private Vector3 size = Vector3.Zero;
        public Vector3 Size
        {
            get
            {
                return size;
            }
        }

        public Vector3 Center
        {
            get
            {
                return (a + b) * 0.5f;
            }
        }

        private void UpdateSize()
        {
            size = b - a;
        }
    }
}
