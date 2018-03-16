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


        public bool isSetted;
        private void button1_Click(object sender, EventArgs e)
        {
            var focalLength = photoControl.ImageTemplateWidth;
            var imagePoints = new List<PointF>();
            imagePoints.Add(new PointF(photoControl.Recognizer.RealPoints[66].X, photoControl.Recognizer.RealPoints[66].Y));        // уши
            imagePoints.Add(new PointF(photoControl.Recognizer.RealPoints[67].X, photoControl.Recognizer.RealPoints[67].Y));
            imagePoints.Add(new PointF(photoControl.Recognizer.RealPoints[0].X, photoControl.Recognizer.RealPoints[0].Y));       // глаза центры
            imagePoints.Add(new PointF(photoControl.Recognizer.RealPoints[1].X, photoControl.Recognizer.RealPoints[1].Y));
            imagePoints.Add(new PointF(photoControl.Recognizer.RealPoints[3].X, photoControl.Recognizer.RealPoints[3].Y));       // левый-правый угол рта
            imagePoints.Add(new PointF(photoControl.Recognizer.RealPoints[4].X, photoControl.Recognizer.RealPoints[4].Y));
            imagePoints.Add(new PointF(photoControl.Recognizer.RealPoints[2].X, photoControl.Recognizer.RealPoints[2].Y));       // центр носа

            var modelPoints = new List<MCvPoint3D32f>();
            modelPoints.Add(new MCvPoint3D32f(RenderControl.HeadPoints.Points[66].X, RenderControl.HeadPoints.Points[66].Y, RenderControl.HeadPoints.Points[66].Z));
            modelPoints.Add(new MCvPoint3D32f(RenderControl.HeadPoints.Points[67].X, RenderControl.HeadPoints.Points[67].Y, RenderControl.HeadPoints.Points[67].Z));
            modelPoints.Add(new MCvPoint3D32f(RenderControl.HeadPoints.Points[0].X, RenderControl.HeadPoints.Points[0].Y, RenderControl.HeadPoints.Points[0].Z));
            modelPoints.Add(new MCvPoint3D32f(RenderControl.HeadPoints.Points[1].X, RenderControl.HeadPoints.Points[1].Y, RenderControl.HeadPoints.Points[1].Z));
            modelPoints.Add(new MCvPoint3D32f(RenderControl.HeadPoints.Points[3].X, RenderControl.HeadPoints.Points[3].Y, RenderControl.HeadPoints.Points[3].Z));
            modelPoints.Add(new MCvPoint3D32f(RenderControl.HeadPoints.Points[4].X, RenderControl.HeadPoints.Points[4].Y, RenderControl.HeadPoints.Points[4].Z));
            modelPoints.Add(new MCvPoint3D32f(RenderControl.HeadPoints.Points[2].X, RenderControl.HeadPoints.Points[2].Y, RenderControl.HeadPoints.Points[2].Z));

            #region CamMatrix

            var img = CvInvoke.Imread(photoControl.TemplateImage);
            var max_d = Math.Max(img.Rows, img.Cols);
            var camMatrix = new Emgu.CV.Matrix<double>(3, 3);
            camMatrix[0, 0] = max_d;
            camMatrix[0, 1] = 0;
            camMatrix[0, 2] = img.Cols / 2.0;
            camMatrix[1, 0] = 0;
            camMatrix[1, 1] = max_d;
            camMatrix[1, 2] = img.Rows / 2.0;
            camMatrix[2, 0] = 0;
            camMatrix[2, 1] = 0;
            camMatrix[2, 2] = 1;

            #endregion

            var rvec = new Mat(3, 1, DepthType.Default, 1);
            var tvec = new Mat(3, 1, DepthType.Default, 1);

            var distMatrix = new Matrix<double>(1, 4);      // не используемый коэф.
            distMatrix[0, 0] = 0;
            distMatrix[0, 1] = 0;
            distMatrix[0, 2] = 0;
            distMatrix[0, 3] = 0;

            Emgu.CV.CvInvoke.SolvePnP(modelPoints.ToArray(), imagePoints.ToArray(), camMatrix, distMatrix, rvec, tvec, false, Emgu.CV.CvEnum.SolvePnpMethod.EPnP);      // решаем проблему PNP

            var translationVector = new Matrix<float>(tvec.Rows, tvec.Cols, tvec.DataPointer);

            var rotM = new Mat(3, 3, DepthType.Cv64F, 1);

            CvInvoke.Rodrigues(rvec, rotM);
            var rotationMatrix = new Matrix<float>(rotM.Rows, rotM.Cols, rotM.DataPointer);         // МАТРИЦА ПОВОРОТА!


            isSetted = true;
        }
    }
}
