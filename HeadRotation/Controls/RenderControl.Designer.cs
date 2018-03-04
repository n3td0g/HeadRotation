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
            this.checkHand = new System.Windows.Forms.PictureBox();
            this.btnUnscale = new System.Windows.Forms.PictureBox();
            this.checkZoom = new System.Windows.Forms.PictureBox();
            this.checkArrow = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.checkHand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUnscale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkArrow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            // checkHand
            // 
            this.checkHand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkHand.BackColor = System.Drawing.SystemColors.Control;
            this.checkHand.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkHand.BackgroundImage")));
            this.checkHand.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.checkHand.Image = global::HeadRotation.Properties.Resources.btnHandNormal;
            this.checkHand.Location = new System.Drawing.Point(360, 3);
            this.checkHand.Name = "checkHand";
            this.checkHand.Size = new System.Drawing.Size(36, 36);
            this.checkHand.TabIndex = 8;
            this.checkHand.TabStop = false;
            this.checkHand.Tag = "2";
            this.checkHand.Click += new System.EventHandler(this.checkHand_Click);
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
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Image = global::HeadRotation.Properties.Resources.btnArrowNormal;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(36, 36);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Tag = "2";
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // RenderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkHand);
            this.Controls.Add(this.btnUnscale);
            this.Controls.Add(this.checkZoom);
            this.Controls.Add(this.checkArrow);
            this.Controls.Add(this.glControl);
            this.Name = "RenderControl";
            this.Size = new System.Drawing.Size(539, 452);
            ((System.ComponentModel.ISupportInitialize)(this.checkHand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUnscale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkArrow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        private OpenTK.GLControl glControl;

        #endregion

        public System.Windows.Forms.Timer RenderTimer;
        private System.Windows.Forms.PictureBox btnUnscale;
        internal System.Windows.Forms.PictureBox checkZoom;
        internal System.Windows.Forms.PictureBox checkArrow;
        internal System.Windows.Forms.PictureBox checkHand;
        internal System.Windows.Forms.PictureBox pictureBox1;
    }
}
