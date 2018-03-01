using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HeadRotation.Helpers;
using System.IO;
using OpenTK;

namespace HeadRotation.Controls
{
    public partial class PhotoControl : UserControl
    {
        public LuxandFaceRecognition Recognizer;

        private string templateImage;
        public string TemplateImage
        {
            get
            {
                return templateImage;
            }
        }

        public List<PointF> facialFeaturesTransformed = new List<PointF>();

        public int ImageTemplateWidth;
        public int ImageTemplateHeight;
        public int ImageTemplateOffsetX;
        public int ImageTemplateOffsetY;

        public PhotoControl()
        {
            InitializeComponent();
        }



        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            pictureTemplate.Refresh();
        }


        private Vector2 GetFrontWorldPoint(Vector3 value)
        {
            Vector2 v;
            var result = new Vector2();
            var imageWidth = pictureTemplate.Image.Width;
            var imageHeight = pictureTemplate.Image.Height;

            var width = Recognizer.FaceRectRelative.Width * imageWidth;
            var height = Recognizer.FaceRectRelative.Height * imageHeight;

            var x = Recognizer.FaceRectRelative.X * imageWidth;
            var y = Recognizer.FaceRectRelative.Y * imageHeight;

            v.X = ((value.X * imageWidth) - x) / width;
            v.Y = ((value.Y * imageHeight) - y) / height;

                result.X = v.X * ProgramCore.MainForm.RenderControl.HeadMesh.AABB.Size.X + ProgramCore.MainForm.RenderControl.HeadMesh.AABB.A.X;
                result.Y = v.Y * (-ProgramCore.MainForm.RenderControl.HeadMesh.AABB.Size.Y) + ProgramCore.MainForm.RenderControl.HeadMesh.AABB.B.Y;

                var centerX = ProgramCore.MainForm.RenderControl.HeadMesh.FaceCenterX;
                var angle = ProgramCore.MainForm.RenderControl.HeadMesh.HeadAngle;
                var noseDepth = ProgramCore.MainForm.RenderControl.HeadMesh.NoseDepth;
                float angleDegree = 180.0f * Math.Abs(angle) / (float)Math.PI;
                var depthScale = Math.Max(Math.Min((angleDegree - 10.0f) / 15.0f, 1.0f), 0.0f);
                depthScale = 1.0f + depthScale * 0.75f;
                noseDepth = noseDepth * depthScale;
                result.X = ((result.X - centerX) + (float)Math.Sin(angle) * value.Z * noseDepth) / (float)Math.Cos(angle);

            return result;
        }

        public void LoadPhoto()
        {
            using (var ofd = new OpenFileDialogEx("Select template file", "Image Files|*.jpg;*.png;*.jpeg;*.bmp"))
            {
                ofd.Multiselect = false;
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                templateImage = ofd.FileName;
                Recognizer = new LuxandFaceRecognition();
                if (!Recognizer.Recognize(ref templateImage, true))
                {
                    templateImage = string.Empty;
                    pictureTemplate.Image = null;

                    return;                     // это ОЧЕНЬ! важно. потому что мы во время распознавания можем создать обрезанную фотку и использовать ее как основную в проекте.
                }

                using (var ms = new MemoryStream(File.ReadAllBytes(templateImage))) // Don't use using!!
                {
                    var img = (Bitmap)Image.FromStream(ms);
                    pictureTemplate.Image = (Bitmap)img.Clone();
                    img.Dispose();
                }

                RecalcRealTemplateImagePosition();

                var eWidth = pictureTemplate.Width - 100;
                var TopEdgeTransformed = new RectangleF(pictureTemplate.Width / 2f - eWidth / 2f, 30, eWidth, eWidth);          // затычка. нужен будет подгон верхней части бошки - сделаю
                var minX = Recognizer.GetMinX();
                var topPoint = (TopEdgeTransformed.Y - ImageTemplateOffsetY) / ImageTemplateHeight;
                Recognizer.FaceRectRelative = new RectangleF(minX, topPoint, Recognizer.GetMaxX() - minX, Recognizer.BottomFace.Y - topPoint);

                var noseTip = GetFrontWorldPoint(Recognizer.FacialFeatures[2]);
                var noseTop = GetFrontWorldPoint(Recognizer.FacialFeatures[22]);
                var noseBottom = GetFrontWorldPoint(Recognizer.FacialFeatures[49]);
                ProgramCore.MainForm.RenderControl.HeadMesh.DetectFaceRotation(noseTip, noseTop, noseBottom);

                RenderTimer.Start();

                if (Math.Abs(Recognizer.RotatedAngle) > 20)
                    MessageBox.Show("The head rotated more than 20 degrees. Please select an other photo...");
            }
        }

        private void pictureTemplate_Paint(object sender, PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(templateImage))
                return;

            foreach (var point in facialFeaturesTransformed)
                e.Graphics.FillEllipse(DrawingTools.BlueSolidBrush, point.X - 2, point.Y - 2, 4, 4);

        }

        /// <summary> Пересчитать положение прямоугольника в зависимост от размера картинки на picturetemplate </summary>
        private void RecalcRealTemplateImagePosition()
        {
            var pb = pictureTemplate;
            if (pb.Image == null)
            {
                ImageTemplateWidth = ImageTemplateHeight = 0;
                ImageTemplateOffsetX = ImageTemplateOffsetY = -1;
                return;
            }

            if (pb.Width / (double)pb.Height < pb.Image.Width / (double)pb.Image.Height)
            {
                ImageTemplateWidth = pb.Width;
                ImageTemplateHeight = pb.Image.Height * ImageTemplateWidth / pb.Image.Width;
            }
            else if (pb.Width / (double)pb.Height > pb.Image.Width / (double)pb.Image.Height)
            {
                ImageTemplateHeight = pb.Height;
                ImageTemplateWidth = pb.Image.Width * ImageTemplateHeight / pb.Image.Height;
            }
            else // if ((double)pb.Width / (double)pb.Height == (double)pb.Image.Width / (double)pb.Image.Height)
            {
                ImageTemplateWidth = pb.Width;
                ImageTemplateHeight = pb.Height;
            }

            ImageTemplateOffsetX = (pb.Width - ImageTemplateWidth) / 2;
            ImageTemplateOffsetY = (pb.Height - ImageTemplateHeight) / 2;

            facialFeaturesTransformed.Clear();
            foreach (var point in Recognizer.FacialFeatures)
            {
                var pointTransformed = new PointF(point.X * ImageTemplateWidth + ImageTemplateOffsetX,
                                          point.Y * ImageTemplateHeight + ImageTemplateOffsetY);
                facialFeaturesTransformed.Add(pointTransformed);
            }


        }

        private void PhotoControl_Resize(object sender, EventArgs e)
        {
            RecalcRealTemplateImagePosition();
        }
    }
}
