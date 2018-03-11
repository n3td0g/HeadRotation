using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HeadRotation.Helpers
{
    public class RenderHelper
    {
        static public void DrawLine(Vector3 a, Vector3 b)
        {
            GL.Vertex3(a);
            GL.Vertex3(b);
        }
    }
}
