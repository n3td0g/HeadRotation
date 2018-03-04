using OpenTK;
using System;
using System.Windows.Forms;

namespace HeadRotation.Controls
{
    public partial class EditPointForm : Form
    {
        private Vector3 editablePoint;
        private Vector3 EditablePoint
        {
            get
            {
                return editablePoint;
            }
            set
            {
                editablePoint = value;
                UpdateEditors();
            }
        }

        public EditPointForm()
        {
            InitializeComponent();
            UpdateEditablePoint(Vector3.Zero);
        }

        public void UpdateEditablePoint(Vector3 point)
        {
            EditablePoint = point;
        }
        private void UpdateEditors()
        {
            textX.Text = EditablePoint.X.ToString();
            textY.Text = EditablePoint.Y.ToString();
            textZ.Text = EditablePoint.Z.ToString();
        }


        private void edit_TextChanged(object sender, EventArgs e)
        {
            var x = Helpers.StringConverter.ToFloat(textX.Text);
            var y = Helpers.StringConverter.ToFloat(textY.Text);
            var z = Helpers.StringConverter.ToFloat(textZ.Text);
            if (ProgramCore.MainForm == null)
                return;

            if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z) || textX.Text.EndsWith(",") || textY.Text.EndsWith(",") || textZ.Text.EndsWith(","))
                return;

            ProgramCore.MainForm.RenderControl.Points.SetSelectedPoint(new Vector3(x, y, z));
        }

        private void EditPointForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            UpdateEditablePoint(Vector3.Zero);
            e.Cancel = true;
        }
    }
}
