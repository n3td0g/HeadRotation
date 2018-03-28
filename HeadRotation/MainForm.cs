using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using HeadRotation.Controls;
using HeadRotation.Helpers;
using Luxand;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HeadRotation
{
    public partial class MainForm : Form
    {
        public PhotoControl PhotoControl { get { return photoControl; } }
        public RenderControl RenderControl { get { return renderControl; } }
        public EditPointForm frmEditPoint = new EditPointForm();


        public MainForm()
        {
            InitializeComponent();

            // Enable double duffering to stop flickering.
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, false);
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            if (FSDK.FSDKE_OK != FSDK.ActivateLibrary("DWysHuomlBcczVM2MQfiz/3WraXb7r+fM0th71X5A9z+gsHn2kpGOgWrVh9D/9sQWlPXO00CFmGMvetl9A+VEr9Y5GVBIccyV32uaZutZjKYH5KB2k87NJAAw6NPkzK0DSQ5b5W7EO0yg2+x4HxpWzPogGyIIYcAHIYY11/YGsU="))
            {
                MessageBox.Show(@"Please run the License Key Wizard (Start - Luxand - FaceSDK - License Key Wizard)", @"Error activating FaceSDK", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            FSDK.InitializeLibrary();
            FSDK.SetFaceDetectionParameters(true, true, 384);

            btnImportVector.Visible = btnExportVector.Visible = btnEditPoint.Visible = false;   // служебные кнопки. для показа старику не нужны.
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLoadPhoto_Click(object sender, EventArgs e)
        {
            photoControl.Reset();
            RenderControl.ReloadModel();
            RenderControl.ImportPoints();

            photoControl.LoadPhoto();
            btnApplyTextures.Enabled = true;
            trackMorphing.Enabled = true;
            btnMirror.Enabled = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            renderControl.Initialize();
            btnImportVector_Click(null, EventArgs.Empty);
        }

        private void btnImportVector_Click(object sender, EventArgs e)
        {
            ProgramCore.MainForm.RenderControl.ImportPoints();
        }
        private void btnExportVector_Click(object sender, EventArgs e)
        {
            VectorEx.ExportVector(ProgramCore.MainForm.RenderControl.HeadPoints.Points);
        }

        private void btnEditPoint_Click(object sender, EventArgs e)
        {
            if (frmEditPoint.Visible)
            {
                frmEditPoint.Hide();
                frmEditPoint.UpdateEditablePoint(OpenTK.Vector3.Zero);
            }
            else
                frmEditPoint.Show(this);
        }

        private void btnApplyTextures_Click(object sender, EventArgs e)
        {
            renderControl.ApplySmoothedTextures();
        }

        private void trackMorphing_ValueChanged(object sender, EventArgs e)
        {
            var value = trackMorphing.Value / 100f;
            renderControl.HeadMesh.SetMorphPercent(value);
        }

        private void btnMirror_Click(object sender, EventArgs e)
        {
            renderControl.morphHelper.MirrorPoints();
            renderControl.headMorphing.Morph();
        }
    }
}
