using System;
using OpenTK.Graphics.OpenGL;

namespace HeadRotation.Render
{
    public class OpenGlHelper
    {
        static public void CheckErrors()
        {
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                throw new Exception(error.ToString());
            }
        }
    }
}
