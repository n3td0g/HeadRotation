using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using HeadRotation.Render;
using System.IO;
using HeadRotation.Properties;

namespace HeadRotation.Controls
{
    public partial class RenderControl : UserControl
    {
        #region var

        private bool UseTexture = true;
        public bool loaded = false;
        private ShaderController idleShader;

        public Camera camera = new Camera();
        public ScaleMode ScaleMode = ScaleMode.None;
        private int mX;
        private int mY;
        private bool leftMousePressed;

        public RenderMesh HeadMesh;
        public HeadPoints Points = new HeadPoints();

        public const int HeadPointsCount = 64;

        #endregion

        public RenderControl()
        {
            InitializeComponent();

            Toolkit.Init();
            
        }

        public void Initialize()
        {
            loaded = true;
            glControl.CreateControl();
            Points.Initialize(HeadPointsCount);
            Points.RenderCamera = camera;

            idleShader = new ShaderController("idle.vs", "idle.fs");
            idleShader.SetUniformLocation("u_UseTexture");
            idleShader.SetUniformLocation("u_Color");
            idleShader.SetUniformLocation("u_Texture");
            idleShader.SetUniformLocation("u_BrushMap");
            idleShader.SetUniformLocation("u_TransparentMap");
            idleShader.SetUniformLocation("u_World");
            idleShader.SetUniformLocation("u_WorldView");
            idleShader.SetUniformLocation("u_ViewProjection");
            idleShader.SetUniformLocation("u_LightDirection");

            var dir = Path.GetDirectoryName(Application.ExecutablePath);
            var fullPath = Path.Combine(dir, "Fem", "Fem.obj");
            HeadMesh = RenderMesh.LoadFromFile(fullPath);
            HeadMesh.OnBeforePartDraw += HeadMesh_OnBeforePartDraw;

            SetupViewport(glControl);


            RenderTimer.Start();
        }


        private void HeadMesh_OnBeforePartDraw(MeshPart part)
        {
            var transparent = UseTexture ? part.TransparentTexture : 0.0f;
            if (transparent > 0.0f)
                EnableTransparent();
            else
                DisableTransparent();

            var shader = idleShader;
            var useTextures = Vector3.Zero;

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, part.TransparentTexture);
            shader.UpdateUniform("u_TransparentMap", 1);
            //shader.UpdateUniform("u_UseTransparent", transparent);            
            useTextures.Y = transparent;

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, part.Texture);
            shader.UpdateUniform("u_Texture", 0);
            useTextures.X = UseTexture ? part.Texture : 0.0f;
            //shader.UpdateUniform("u_UseTexture", UseTexture ? part.Texture : 0.0f);
            shader.UpdateUniform("u_Color", part.Color);

            shader.UpdateUniform("u_UseTexture", useTextures);
        }

        private void EnableTransparent()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.DepthMask(false);
        }
        private void DisableTransparent()
        {
            GL.Disable(EnableCap.Blend);
            GL.DepthMask(true);
        }

        public void Render()
        {
            if (!loaded)  // whlie context not create
                return;
            GL.ClearColor(Color.White);

            // Clear the render canvas with the current color
            GL.Clear(
                ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit);

            camera.PutCamera();

            GL.PushMatrix();
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Normalize);
            GL.Disable(EnableCap.CullFace);

            idleShader.Begin();
            DrawHead();
            idleShader.End();

            GL.PopMatrix();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            DrawAxis();

            Points.Draw();

            glControl.SwapBuffers();
        }

        private void DrawHead()
        {
            idleShader.UpdateUniform("u_LightDirection", Vector3.Normalize(camera.Position));
            var worldMatrix = Matrix4.Identity;
            idleShader.UpdateUniform("u_World", worldMatrix);
            idleShader.UpdateUniform("u_WorldView", worldMatrix * camera.ViewMatrix);
            idleShader.UpdateUniform("u_ViewProjection", camera.ViewMatrix * camera.ProjectMatrix);

            HeadMesh.Draw(false);
        }

        private void DrawAxis()
        {
            GL.LineWidth(1.0f);
            GL.DepthMask(false);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(100.0, 0.0, 0.0);
            GL.Color3(Color.Blue);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 100.0, 0.0);
            GL.Color3(Color.Green);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, 100.0);
            GL.End();

            GL.Enable(EnableCap.LineStipple);
            GL.LineStipple(1, 255);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(-100.0, 0.0, 0.0);
            GL.Color3(Color.Blue);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, -100.0, 0.0);
            GL.Color3(Color.Green);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, -100.0);
            GL.End();
            GL.Disable(EnableCap.LineStipple);

            GL.DepthMask(true);
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            Render();
        }

        private void SetupViewport(GLControl c)
        {
            if (c.ClientSize.Height == 0)
                c.ClientSize = new Size(c.ClientSize.Width, 1);

            camera.UpdateViewport(c.ClientSize.Width, c.ClientSize.Height);
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;
            var c = sender as GLControl;
            SetupViewport(c);

        }

        #region ScaleTools

        private void btnUnscale_MouseUp(object sender, MouseEventArgs e)
        {
            btnUnscale.Image = Resources.btnUnscaleNormal;

            camera.ResetCamera(true, HeadMesh.HeadAngle);

            checkArrow.Tag = checkZoom.Tag = "2";
            checkArrow.Image = Resources.btnArrowNormal;
            checkZoom.Image = Resources.btnZoomNormal;
        }
        private void btnUnscale_MouseDown(object sender, MouseEventArgs e)
        {
            btnUnscale.Image = Resources.btnUnscalePressed;
        }

        private void ResetScaleModeTools()
        {
            if (checkZoom.Tag.ToString() == "1")
                checkZoom_Click(this, EventArgs.Empty);
            if (checkArrow.Tag.ToString() == "1")
                checkArrow_Click(this, EventArgs.Empty);
        }
        private void checkZoom_Click(object sender, EventArgs e)
        {
            if (checkZoom.Tag.ToString() == "2")
            {
                ResetScaleModeTools();
                ScaleMode = ScaleMode.Zoom;

                checkZoom.Tag = "1";
                checkArrow.Tag = "2";

                checkZoom.Image = Resources.btnZoomPressed;
                checkArrow.Image = Resources.btnArrowNormal;
            }
            else
            {
                checkZoom.Tag = "2";
                checkZoom.Image = Resources.btnZoomNormal;

                ScaleMode = ScaleMode.None;
            }
        }
        private void checkArrow_Click(object sender, EventArgs e)
        {
            if (checkArrow.Tag.ToString() == "2")
            {
                ResetScaleModeTools();
                ScaleMode = ScaleMode.Rotate;

                checkArrow.Tag = "1";
                checkZoom.Tag = "2";

                checkArrow.Image = Resources.btnArrowPressed;
                checkZoom.Image = Resources.btnZoomNormal;
            }
            else
            {
                checkArrow.Tag = "2";
                checkArrow.Image = Resources.btnArrowNormal;

                ScaleMode = ScaleMode.None;
            }
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            var newPoint = new Vector2(e.X, e.Y);
            if (leftMousePressed)
            {
                switch (ScaleMode)
                {
                    case ScaleMode.Rotate:
                        camera.LeftRight((e.Location.X - mX) * 1.0f / 150.0f);
                        break;
                    case ScaleMode.Move:
                        camera.dy -= (e.Location.Y - mY) * camera.Scale;
                        break;
                    case ScaleMode.Zoom:
                        camera.Wheel((e.Location.Y - mY) / 150f * camera.Scale);
                        break;
                    case ScaleMode.None:
                        Points.MovePoint(e.X, e.Y);
                        break;
                }
            }
            mX = e.Location.X;
            mY = e.Location.Y;
        }
        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftMousePressed = true;

                if(ScaleMode == ScaleMode.None)
                {
                    Points.StartMoving(e.X, e.Y);
                }
            }
        }
        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftMousePressed = false;

                if (ScaleMode == ScaleMode.None)
                {
                    Points.SelectPoint(e.X, e.Y);
                    Points.StopMoving(e.X, e.Y);
                }
            }
        }

        #endregion
    }
}
