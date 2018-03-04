using HeadRotation.Controls;
using HeadRotation.Helpers;
using Luxand;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HeadRotation
{
    public partial class MainForm : Form
    {
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
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLoadPhoto_Click(object sender, EventArgs e)
        {
            photoControl.LoadPhoto();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            renderControl.Initialize();
        }

        private void btnImportVector_Click(object sender, EventArgs e)
        {
            var vectors = VectorEx.ImportVector();
        }
        private void btnExportVector_Click(object sender, EventArgs e)
        {
            var vectors = new List<Vector3>();
            vectors.Add(new Vector3(1, 2, 3));
            vectors.Add(new Vector3(4, 5, 6));
            vectors.Add(new Vector3(7, 8, 9));

            VectorEx.ExportVector(vectors);
        }

        private void btnEditPoint_Click(object sender, EventArgs e)
        {
            if (frmEditPoint.Visible)
            {
                frmEditPoint.Hide();
                frmEditPoint.UpdateEditablePoint(Vector3.Zero);
            }
            else
                frmEditPoint.Show(this);
        }
    }
}
