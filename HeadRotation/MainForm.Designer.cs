using HeadRotation.Controls;

namespace HeadRotation
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.trackMorphing = new System.Windows.Forms.TrackBar();
            this.btnApplyTextures = new System.Windows.Forms.Button();
            this.btnEditPoint = new System.Windows.Forms.Button();
            this.btnImportVector = new System.Windows.Forms.Button();
            this.btnExportVector = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnLoadPhoto = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.renderControl = new HeadRotation.Controls.RenderControl();
            this.photoControl = new HeadRotation.Controls.PhotoControl();
            this.btnMirror = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackMorphing)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnMirror);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.trackMorphing);
            this.panel1.Controls.Add(this.btnApplyTextures);
            this.panel1.Controls.Add(this.btnEditPoint);
            this.panel1.Controls.Add(this.btnImportVector);
            this.panel1.Controls.Add(this.btnExportVector);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Controls.Add(this.btnLoadPhoto);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1067, 55);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(380, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Morphing:";
            // 
            // trackMorphing
            // 
            this.trackMorphing.Enabled = false;
            this.trackMorphing.LargeChange = 10;
            this.trackMorphing.Location = new System.Drawing.Point(441, 7);
            this.trackMorphing.Maximum = 100;
            this.trackMorphing.Name = "trackMorphing";
            this.trackMorphing.Size = new System.Drawing.Size(104, 45);
            this.trackMorphing.SmallChange = 10;
            this.trackMorphing.TabIndex = 6;
            this.trackMorphing.TickFrequency = 10;
            this.trackMorphing.Value = 100;
            this.trackMorphing.ValueChanged += new System.EventHandler(this.trackMorphing_ValueChanged);
            // 
            // btnApplyTextures
            // 
            this.btnApplyTextures.Enabled = false;
            this.btnApplyTextures.Location = new System.Drawing.Point(253, 12);
            this.btnApplyTextures.Name = "btnApplyTextures";
            this.btnApplyTextures.Size = new System.Drawing.Size(97, 23);
            this.btnApplyTextures.TabIndex = 5;
            this.btnApplyTextures.Text = "Apply textures";
            this.btnApplyTextures.UseVisualStyleBackColor = true;
            this.btnApplyTextures.Click += new System.EventHandler(this.btnApplyTextures_Click);
            // 
            // btnEditPoint
            // 
            this.btnEditPoint.Location = new System.Drawing.Point(855, 12);
            this.btnEditPoint.Name = "btnEditPoint";
            this.btnEditPoint.Size = new System.Drawing.Size(75, 23);
            this.btnEditPoint.TabIndex = 4;
            this.btnEditPoint.Text = "Edit point";
            this.btnEditPoint.UseVisualStyleBackColor = true;
            this.btnEditPoint.Click += new System.EventHandler(this.btnEditPoint_Click);
            // 
            // btnImportVector
            // 
            this.btnImportVector.Location = new System.Drawing.Point(642, 12);
            this.btnImportVector.Name = "btnImportVector";
            this.btnImportVector.Size = new System.Drawing.Size(75, 23);
            this.btnImportVector.TabIndex = 3;
            this.btnImportVector.Text = "Import points";
            this.btnImportVector.UseVisualStyleBackColor = true;
            this.btnImportVector.Click += new System.EventHandler(this.btnImportVector_Click);
            // 
            // btnExportVector
            // 
            this.btnExportVector.Location = new System.Drawing.Point(737, 12);
            this.btnExportVector.Name = "btnExportVector";
            this.btnExportVector.Size = new System.Drawing.Size(75, 23);
            this.btnExportVector.TabIndex = 2;
            this.btnExportVector.Text = "Export points";
            this.btnExportVector.UseVisualStyleBackColor = true;
            this.btnExportVector.Click += new System.EventHandler(this.btnExportVector_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(980, 12);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnLoadPhoto
            // 
            this.btnLoadPhoto.Location = new System.Drawing.Point(12, 12);
            this.btnLoadPhoto.Name = "btnLoadPhoto";
            this.btnLoadPhoto.Size = new System.Drawing.Size(79, 23);
            this.btnLoadPhoto.TabIndex = 0;
            this.btnLoadPhoto.Text = "Load photo";
            this.btnLoadPhoto.UseVisualStyleBackColor = true;
            this.btnLoadPhoto.Click += new System.EventHandler(this.btnLoadPhoto_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(478, 55);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 577);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // renderControl
            // 
            this.renderControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderControl.Location = new System.Drawing.Point(481, 55);
            this.renderControl.Name = "renderControl";
            this.renderControl.Size = new System.Drawing.Size(586, 577);
            this.renderControl.TabIndex = 3;
            // 
            // photoControl
            // 
            this.photoControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.photoControl.Location = new System.Drawing.Point(0, 55);
            this.photoControl.Name = "photoControl";
            this.photoControl.Size = new System.Drawing.Size(478, 577);
            this.photoControl.TabIndex = 1;
            // 
            // btnMirror
            // 
            this.btnMirror.Enabled = false;
            this.btnMirror.Location = new System.Drawing.Point(141, 12);
            this.btnMirror.Name = "btnMirror";
            this.btnMirror.Size = new System.Drawing.Size(97, 23);
            this.btnMirror.TabIndex = 8;
            this.btnMirror.Text = "Mirror face shape";
            this.btnMirror.UseVisualStyleBackColor = true;
            this.btnMirror.Click += new System.EventHandler(this.btnMirror_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 632);
            this.Controls.Add(this.renderControl);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.photoControl);
            this.Controls.Add(this.panel1);
            this.Name = "MainForm";
            this.Text = "Head rotation tests";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackMorphing)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnLoadPhoto;
        private PhotoControl photoControl;
        private System.Windows.Forms.Splitter splitter1;
        private RenderControl renderControl;
        private System.Windows.Forms.Button btnImportVector;
        private System.Windows.Forms.Button btnExportVector;
        private System.Windows.Forms.Button btnEditPoint;
        private System.Windows.Forms.Button btnApplyTextures;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackMorphing;
        private System.Windows.Forms.Button btnMirror;
    }
}

