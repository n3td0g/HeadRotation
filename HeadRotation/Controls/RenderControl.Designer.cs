using OpenTK;
namespace HeadRotation.Controls
{
    partial class RenderControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenderControl));
            this.glControl = new OpenTK.GLControl();
            this.RenderTimer = new System.Windows.Forms.Timer(this.components);
            this.btnUnscale = new System.Windows.Forms.PictureBox();
            this.checkZoom = new System.Windows.Forms.PictureBox();
            this.checkArrow = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnUnscale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkArrow)).BeginInit();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.AllowDrop = true;
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(539, 452);
            this.glControl.TabIndex = 3;
            this.glControl.VSync = false;
            this.glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseDown);
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseMove);
            this.glControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseUp);
            this.glControl.Resize += new System.EventHandler(this.glControl_Resize);
            // 
            // RenderTimer
            // 
            this.RenderTimer.Interval = 40;
            this.RenderTimer.Tick += new System.EventHandler(this.RenderTimer_Tick);
            // 
            // btnUnscale
            // 
            this.btnUnscale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnscale.BackColor = System.Drawing.SystemColors.Control;
            this.btnUnscale.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnUnscale.Image = global::HeadRotation.Properties.Resources.btnUnscaleNormal;
            this.btnUnscale.Location = new System.Drawing.Point(486, 3);
            this.btnUnscale.Name = "btnUnscale";
            this.btnUnscale.Size = new System.Drawing.Size(36, 36);
            this.btnUnscale.TabIndex = 7;
            this.btnUnscale.TabStop = false;
            this.btnUnscale.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnUnscale_MouseDown);
            this.btnUnscale.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnUnscale_MouseUp);
            // 
            // checkZoom
            // 
            this.checkZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkZoom.BackColor = System.Drawing.SystemColors.Control;
            this.checkZoom.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkZoom.BackgroundImage")));
            this.checkZoom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.checkZoom.Image = global::HeadRotation.Properties.Resources.btnZoomNormal;
            this.checkZoom.Location = new System.Drawing.Point(444, 3);
            this.checkZoom.Name = "checkZoom";
            this.checkZoom.Size = new System.Drawing.Size(36, 36);
            this.checkZoom.TabIndex = 6;
            this.checkZoom.TabStop = false;
            this.checkZoom.Tag = "2";
            this.checkZoom.Click += new System.EventHandler(this.checkZoom_Click);
            // 
            // checkArrow
            // 
            this.checkArrow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkArrow.BackColor = System.Drawing.SystemColors.Control;
            this.checkArrow.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkArrow.BackgroundImage")));
            this.checkArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.checkArrow.Image = global::HeadRotation.Properties.Resources.btnArrowNormal;
            this.checkArrow.Location = new System.Drawing.Point(402, 3);
            this.checkArrow.Name = "checkArrow";
            this.checkArrow.Size = new System.Drawing.Size(36, 36);
            this.checkArrow.TabIndex = 5;
            this.checkArrow.TabStop = false;
            this.checkArrow.Tag = "2";
            this.checkArrow.Click += new System.EventHandler(this.checkArrow_Click);
            // 
            // RenderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnUnscale);
            this.Controls.Add(this.checkZoom);
            this.Controls.Add(this.checkArrow);
            this.Controls.Add(this.glControl);
            this.Name = "RenderControl";
            this.Size = new System.Drawing.Size(539, 452);
            ((System.ComponentModel.ISupportInitialize)(this.btnUnscale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkArrow)).EndInit();
            this.ResumeLayout(false);

        }

        private OpenTK.GLControl glControl;

        #endregion

        public System.Windows.Forms.Timer RenderTimer;
        private System.Windows.Forms.PictureBox btnUnscale;
        internal System.Windows.Forms.PictureBox checkZoom;
        internal System.Windows.Forms.PictureBox checkArrow;
    }
}
