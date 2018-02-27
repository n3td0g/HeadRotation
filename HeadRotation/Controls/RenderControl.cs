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
using HeadRotation.Render;
using System.IO;

namespace HeadRotation.Controls
{
    public partial class RenderControl : UserControl
    {
        private bool UseTexture = true;
        public bool loaded = false;
        private ShaderController idleShader;
        public Camera Camera = new Camera();
        public RenderMesh HeadMesh;

        public RenderControl()
        {
            InitializeComponent();

            Toolkit.Init();
         
        }

        public void Initialize()
        {
            loaded = true;
            glControl.CreateControl();

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
            GL.Viewport(this.Location.X, this.Location.Y, Width, Height);
            GL.ClearColor(Color.White);

            // Clear the render canvas with the current color
            GL.Clear(
                ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit);

            Camera.PutCamera();

            GL.PushMatrix();
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Normalize);
            GL.Disable(EnableCap.CullFace);

            idleShader.Begin();
            DrawHead();
            idleShader.End();
            
            glControl.SwapBuffers();
        }

        private void DrawHead()
        {
            idleShader.UpdateUniform("u_LightDirection", Vector3.Normalize(Camera.Position));
            var worldMatrix = Matrix4.Identity;
            //Иначе берем Matrix4.Identity
            idleShader.UpdateUniform("u_World", worldMatrix);
            idleShader.UpdateUniform("u_WorldView", worldMatrix * Camera.ViewMatrix);
            idleShader.UpdateUniform("u_ViewProjection", Camera.ViewMatrix * Camera.ProjectMatrix);

            HeadMesh.Draw(false);
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            Render();
        }
    }
}
