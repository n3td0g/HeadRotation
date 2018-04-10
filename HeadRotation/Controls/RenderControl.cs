using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using HeadRotation.Render;
using System.IO;
using HeadRotation.Properties;
using HeadRotation.Helpers;
using HeadRotation.Morphing;
using System.Collections;
using System.Linq;
using OpenTK.Graphics;
using OpenTK.Platform;
using HeadRotation.ObjFile;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace HeadRotation.Controls
{
    public partial class RenderControl : UserControl
    {
        #region var

        private bool UseTexture = true;
        private int headTextureId = 0;
        public bool loaded = false;
        private ShaderController idleShader;
        private ShaderController blendShader;
        private LuxandFaceRecognition recognizer;

        public Camera camera = new Camera();
        public ProjectedDots ProjectedPoints = new ProjectedDots();
        public MorphHelper morphHelper = new MorphHelper();
        public HeadMorphing headMorphing = new HeadMorphing();
        public ScaleMode ScaleMode = ScaleMode.None;
        private int mX;
        private int mY;
        private bool leftMousePressed;

        public RenderMesh HeadMesh;
        public HeadPoints HeadPoints = new HeadPoints();

        public const int HeadPointsCount = 70;

        bool drawDots = false;
        bool drawSpheres = false;
        bool drawPoints = false;
        bool drawTriangles = false;
        bool drawAABB = false;
        bool drawAxis = false;
        bool drawRotation = false;

        public List<int> smoothedTextures = new List<int>();

        private readonly Panel renderPanel = new Panel();
        private GraphicsContext graphicsContext;
        private IWindowInfo windowInfo;

        #endregion

        public RenderControl()
        {
            InitializeComponent();

            Toolkit.Init();
        }

        private void LoadModel()
        {
            var dir = Path.GetDirectoryName(Application.ExecutablePath);
            var fullPath = Path.Combine(dir, "Models", "FemWithSmile", "Fem.obj");
            HeadMesh = RenderMesh.LoadFromFile(fullPath);
            HeadMesh.OnBeforePartDraw += HeadMesh_OnBeforePartDraw;
            HeadPoints.HeadMesh = HeadMesh;
        }
        internal void ReloadModel()
        {
            if (HeadMesh != null)
            {
                HeadMesh.OnBeforePartDraw -= HeadMesh_OnBeforePartDraw;
                foreach (var part in HeadMesh.Parts)
                {
                    if (part.TransparentTexture != 0)
                    {
                        GL.DeleteTexture(part.TransparentTexture);
                    }
                    if (part.Texture != 0)
                    {
                        GL.DeleteTexture(part.Texture);
                    }
                }

                TextureHelper.ReloadTextures();
            }

            LoadModel();

            var blendingInfo = new List<BlendingInfo>();
            blendingInfo.Add(new BlendingInfo { Position = new Vector2(-0.01f, -4.46f), Radius = 3.2f });           // центр лица
            blendingInfo.Add(new BlendingInfo { Position = new Vector2(-3.20f, 0.20f), Radius = 3.0f });            // левый глаз
            blendingInfo.Add(new BlendingInfo { Position = new Vector2(3.20f, 0.20f), Radius = 3.0f });            // правый глаз
            blendingInfo.Add(new BlendingInfo { Position = new Vector2(0f, -8.6f), Radius = 2.0f });                  // подбородок
            HeadMesh.CalculateBlendingWeights(blendingInfo);

            camera.ResetCamera(true);
        }

        public void ImportPoints()
        {
            HeadPoints.Points.Clear();
            HeadPoints.Points.AddRange(VectorEx.ImportVector());
            HeadPoints.IsVisible.AddRange(Enumerable.Repeat(true, HeadPoints.Points.Count));

            //headMorphing.Initialize(HeadPoints);
        }

        public void PhotoLoaded(LuxandFaceRecognition recognizer, string photoPath)
        {
            this.recognizer = recognizer;
            headTextureId = TextureHelper.GetTexture(photoPath);

            ProjectedPoints.Initialize(recognizer, HeadPoints);
            headMorphing.Initialize(recognizer, HeadPoints);
            morphHelper.ProcessPoints(ProjectedPoints, HeadPoints);
           // headMorphing.Morph();

            //ApplySmoothedTextures();              // Для автоматического текстурирования раскомментить эту строку. А так - подвесил на кнопку.

            ResetCamera();
        }

        public void Initialize()
        {
            loaded = true;
            glControl.CreateControl();
            HeadPoints.Initialize(HeadPointsCount);
            HeadPoints.RenderCamera = camera;
            HeadPoints.GenerateSphere(0.3f, 5, 5);

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

            blendShader = new ShaderController("blending.vs", "blending.fs");
            blendShader.SetUniformLocation("u_Texture");
            blendShader.SetUniformLocation("u_BaseTexture");
            blendShader.SetUniformLocation("u_BlendDirectionX");

            LoadModel();

            SetupViewport(glControl);

            windowInfo = Utilities.CreateWindowsWindowInfo(renderPanel.Handle);
            graphicsContext = new GraphicsContext(GraphicsMode.Default, windowInfo);
            renderPanel.Resize += (sender, args) => graphicsContext.Update(windowInfo);
            glControl.Context.MakeCurrent(glControl.WindowInfo);

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

            if (drawSpheres)
            {
                HeadPoints.DrawSpheres();
            }

            GL.PopMatrix();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);

            if(drawRotation)
            {
                DrawMeshRotation();
            }            

            if (drawAxis)
            {
                DrawAxis();
            }

            if (drawDots)
            {
                HeadPoints.DrawDots();
            }

            if (drawPoints)
            {
                ProjectedPoints.Draw();
            }

            if (drawTriangles)
            {
                headMorphing.Draw(useProfileTriangles);
                EnableTransparent();
                headMorphing.DrawTriangles(useProfileTriangles);
                DisableTransparent();
            }

            glControl.SwapBuffers();
        }

        private void DrawHead()
        {
            idleShader.UpdateUniform("u_LightDirection", Vector3.Normalize(camera.Position));
            var worldMatrix = HeadMesh.RotationMatrix;
            idleShader.UpdateUniform("u_World", worldMatrix);
            idleShader.UpdateUniform("u_WorldView", worldMatrix * camera.ViewMatrix);
            idleShader.UpdateUniform("u_ViewProjection", camera.ViewMatrix * camera.ProjectMatrix);

            HeadMesh.Draw(drawAABB);
        }

        private void DrawMeshRotation()
        {
            GL.PushMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
            var WorldViweMatrix = HeadMesh.RotationMatrix * camera.ViewMatrix;
            GL.LoadMatrix(ref WorldViweMatrix);

            var Offset = HeadMesh.AABB.Center;
            Offset.X = 0.0f;
            Offset.Z = 0.0f;
            GL.LineWidth(3.0f);
            GL.DepthMask(false);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(10.0, 0.0, 0.0);
            GL.Color3(Color.Blue);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 10.0, 0.0);
            GL.Color3(Color.Green);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, 10.0);
            GL.End();
            
            GL.DepthMask(true);

            GL.PopMatrix();
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

        private void DrawQuad(float r, float g, float b, float a)
        {
            GL.Color4(r, g, b, a);
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(-1.0f, -1.0f);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(1.0f, -1.0f);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(1.0f, 1.0f);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(-1.0f, 1.0f);

            GL.End();
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

        #region RenderToTexture

        public void ApplySmoothedTextures()
        {
            if(smoothedTextures.Count == 0)
            {
                foreach(var part in HeadMesh.Parts)
                {
                    if (!smoothedTextures.Contains(part.Texture) && part.Texture > 0)
                        smoothedTextures.Add(part.Texture);
                }
            }

            foreach (var smoothTex in smoothedTextures)
            {
                var bitmap = RenderToTexture(smoothTex);
                TextureHelper.SetTexture(smoothTex, bitmap);
            }
        }

        public Bitmap RenderToTexture(int textureId)
        {
            int textureWidth;
            int textureHeight;
            var texPath = TextureHelper.GetTexturePath(textureId);
            using (var img = new Bitmap(texPath))
            {
                textureWidth = img.Width;
                textureHeight = img.Height;
            }
            return RenderToTexture(textureId, textureWidth, textureHeight, blendShader);
        }

        public Bitmap RenderToTexture(int textureId, int textureWidth, int textureHeight, ShaderController shader, bool useAlpha = false)
        {
            graphicsContext.MakeCurrent(windowInfo);
            renderPanel.Size = new Size(textureWidth, textureHeight);
            GL.Viewport(0, 0, textureWidth, textureHeight);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Enable(EnableCap.Texture2D);
            GL.DepthMask(false);

            DrawToTexture(shader, textureId);

            GL.DepthMask(true);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            var result = GrabScreenshot(string.Empty, textureWidth, textureHeight, useAlpha);
            result.Save(textureId.ToString() + ".png");
            glControl.Context.MakeCurrent(glControl.WindowInfo);
            SetupViewport(glControl);
            return result;
        }

        private bool DrawToTexture(ShaderController shader, int textureId)
        {
            //GL.BindTexture(TextureTarget.Texture2D, oldTextureId);
            DrawQuad(1f, 1f, 1f, 1f);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            shader.Begin();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, headTextureId);
            shader.UpdateUniform("u_Texture", 0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            shader.UpdateUniform("u_BaseTexture", 1);

            shader.UpdateUniform("u_BlendDirectionX", HeadMesh.HeadAngle >= 0 ? 1.0f : -1.0f);

            HeadMesh.DrawToTexture(textureId);

            shader.End();
            GL.Disable(EnableCap.Blend);
            return true;
        }

        public Bitmap GrabScreenshot(string filePath, int width, int height, bool useAlpha = false)
        {
            var bmp = new Bitmap(width, height);
            var rect = new Rectangle(0, 0, width, height);
            var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, useAlpha ? System.Drawing.Imaging.PixelFormat.Format32bppArgb : System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, width, height, useAlpha ? OpenTK.Graphics.OpenGL.PixelFormat.Bgra : OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            GL.Finish();
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            if (!string.IsNullOrEmpty(filePath))
                bmp.Save(filePath, ImageFormat.Jpeg);
            return bmp;
        }

        #endregion

        #region ScaleTools

        public void ResetCamera()
        {
            if (recognizer != null)
            {
                const int indexA = 22;
                const int indexB = 11;

                var pA = ProgramCore.MainForm.PhotoControl.facialFeaturesTransformed[indexA];
                var pB = ProgramCore.MainForm.PhotoControl.facialFeaturesTransformed[indexB];
                var pointA = new Vector2(pA.X, pA.Y);
                var pointB = new Vector2(pB.X, pB.Y);

                var pointA1 = camera.GetScreenPoint(HeadPoints.Points[indexA]);
                var pointB1 = camera.GetScreenPoint(HeadPoints.Points[indexB]);

                camera.SetupCamera(pointA - pointB, pointA1 - pointB1);

                var worldPointB = camera.GetWorldPoint((int)pointB1.X, (int)pB.Y, 0.0f);
                var worldPointB1 = HeadPoints.Points[indexB];
                camera.dy = (worldPointB1.Y - worldPointB.Y);
                camera.PutCamera();
            }
            else
            {
                camera.ResetCamera(true);
            }
        }

        private void btnUnscale_MouseUp(object sender, MouseEventArgs e)
        {
            btnUnscale.Image = Resources.btnUnscaleNormal;
            ResetCamera();

            ScaleMode = ScaleMode.None;

            checkArrow.Tag = checkZoom.Tag = "2";
            checkArrow.Image = Resources.btnArrowNormal;
            checkZoom.Image = Resources.btnZoomNormal;
        }
        private void btnUnscale_MouseDown(object sender, MouseEventArgs e)
        {
            btnUnscale.Image = Resources.btnUnscalePressed;
        }

        private void btnFront_MouseDown(object sender, MouseEventArgs e)
        {
            btnFront.Image = Resources.btnUnscalePressed;
        }

        private void btnFront_MouseUp(object sender, MouseEventArgs e)
        {
            btnFront.Image = Resources.btnUnscaleNormal;

            camera.ResetCamera(true, -HeadMesh.HeadAngle);
            ScaleMode = ScaleMode.None;

            checkArrow.Tag = checkZoom.Tag = "2";
            checkArrow.Image = Resources.btnArrowNormal;
            checkZoom.Image = Resources.btnZoomNormal;
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
                checkArrow.Tag = checkHand.Tag = "2";

                checkZoom.Image = Resources.btnZoomPressed;
                checkArrow.Image = Resources.btnArrowNormal;
                checkHand.Image = Resources.btnHandNormal;
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
                checkZoom.Tag = checkHand.Tag = "2";

                checkArrow.Image = Resources.btnArrowPressed;
                checkZoom.Image = Resources.btnZoomNormal;
                checkHand.Image = Resources.btnHandNormal;
            }
            else
            {
                checkArrow.Tag = "2";
                checkArrow.Image = Resources.btnArrowNormal;

                ScaleMode = ScaleMode.None;
            }
        }
        private void checkHand_Click(object sender, EventArgs e)
        {
            if (checkHand.Tag.ToString() == "2")
            {
                ResetScaleModeTools();
                ScaleMode = ScaleMode.Move;

                checkHand.Tag = "1";
                checkArrow.Tag = checkZoom.Tag = "2";

                checkHand.Image = Resources.btnHandPressed;
                checkArrow.Image = Resources.btnArrowNormal;
                checkZoom.Image = Resources.btnZoomNormal;
            }
            else
            {
                checkHand.Tag = "2";
                checkHand.Image = Resources.btnHandNormal;

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
                        HeadPoints.MovePoint(e.X, e.Y);
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

                if (ScaleMode == ScaleMode.None)
                {
                    HeadPoints.StartMoving(e.X, e.Y);
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
                    HeadPoints.SelectPoint(e.X, e.Y);
                    HeadPoints.StopMoving(e.X, e.Y);
                }
            }
        }

        #endregion

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            camera.LeftRight(Math.PI / 2f);
        }

        private bool useProfileTriangles = false;
        private void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                drawAABB = !drawAABB;
            }
            if (e.KeyCode == Keys.D)
            {
                drawDots = !drawDots;
            }
            if (e.KeyCode == Keys.S)
            {
                drawSpheres = !drawSpheres;
            }
            if (e.KeyCode == Keys.F)
            {
                drawPoints = !drawPoints;
            }
            if (e.KeyCode == Keys.T)
            {
                drawTriangles = !drawTriangles;
            }
            if (e.KeyCode == Keys.R)
            {
                pictureBox1_MouseUp(null, null);
            }
            if (e.KeyCode == Keys.Q)
            {
                drawRotation = !drawRotation;
            }
            if (e.KeyCode == Keys.G)
            {
                drawAxis = !drawAxis;
            }
            if (e.KeyCode == Keys.P)
            {
                useProfileTriangles = !useProfileTriangles;
            }

            if (e.KeyCode == Keys.Space)
            {
                UseTexture = !UseTexture;
            }
            if (e.KeyCode == Keys.Delete)
            {
                if (HeadPoints.SelectedPoint != -1)
                    HeadPoints.IsVisible[HeadPoints.SelectedPoint] = false;
            }
            if(e.KeyCode == Keys.Z)
            {
                HeadMesh.SetMorphPercent(1.0f);
            }
            if (e.KeyCode == Keys.X)
            {
                HeadMesh.SetMorphPercent(0.0f);
            }
        }
    }
}
