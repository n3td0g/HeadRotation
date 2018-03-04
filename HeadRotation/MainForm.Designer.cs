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
            this.btnImportVector = new System.Windows.Forms.Button();
            this.btnExportVector = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnLoadPhoto = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.renderControl = new HeadRotation.Controls.RenderControl();
            this.photoControl = new HeadRotation.Controls.PhotoControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnImportVector);
            this.panel1.Controls.Add(this.btnExportVector);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Controls.Add(this.btnLoadPhoto);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1067, 47);
            this.panel1.TabIndex = 0;
            // 
            // btnImportVector
            // 
            this.btnImportVector.Location = new System.Drawing.Point(458, 4);
            this.btnImportVector.Name = "btnImportVector";
            this.btnImportVector.Size = new System.Drawing.Size(75, 40);
            this.btnImportVector.TabIndex = 3;
            this.btnImportVector.Text = "Импорт точек";
            this.btnImportVector.UseVisualStyleBackColor = true;
            this.btnImportVector.Click += new System.EventHandler(this.btnImportVector_Click);
            // 
            // btnExportVector
            // 
            this.btnExportVector.Location = new System.Drawing.Point(553, 4);
            this.btnExportVector.Name = "btnExportVector";
            this.btnExportVector.Size = new System.Drawing.Size(75, 40);
            this.btnExportVector.TabIndex = 2;
            this.btnExportVector.Text = "Экспорт точек";
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
            this.splitter1.Location = new System.Drawing.Point(478, 47);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 585);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // renderControl
            // 
            this.renderControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderControl.Location = new System.Drawing.Point(481, 47);
            this.renderControl.Name = "renderControl";
            this.renderControl.Size = new System.Drawing.Size(586, 585);
            this.renderControl.TabIndex = 3;
            // 
            // photoControl
            // 
            this.photoControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.photoControl.Location = new System.Drawing.Point(0, 47);
            this.photoControl.Name = "photoControl";
            this.photoControl.Size = new System.Drawing.Size(478, 585);
            this.photoControl.TabIndex = 1;
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
    }
}

