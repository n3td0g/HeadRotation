using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace HeadRotation.Controls
{
    public partial class RenderControl : UserControl
    {
        public bool loaded = false;
        //    public readonly Camera camera = new Camera();
        private GraphicsContext graphicsContext;
        private IWindowInfo windowInfo;

        public RenderControl()
        {
            InitializeComponent();

            OpenTK.Toolkit.Init();
         
        }

        public void Initialize()
        {
            loaded = true;
            glControl.CreateControl();

            RenderTimer.Start();
        }

        public void Render()
        {
            if (!loaded)  // whlie context not create
                return;
            GL.Viewport(this.Location.X, this.Location.Y, Width, Height);
            GL.ClearColor(Color.Red);

            // Clear the render canvas with the current color
            GL.Clear(
                ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit);

            GL.Flush();
 
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            Render();
        }
    }
}
