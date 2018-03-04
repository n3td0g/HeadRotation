using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                textX.Text = value.X.ToString();
                textY.Text = value.Y.ToString();
                textZ.Text = value.Z.ToString();
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
            btnApply.Enabled =textX.Enabled = textY.Enabled = textZ.Enabled = point != Vector3.Zero;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            var x = Helpers.StringConverter.ToFloat(textX.Text);
            var y = Helpers.StringConverter.ToFloat(textY.Text);
            var z = Helpers.StringConverter.ToFloat(textZ.Text);
            EditablePoint = new Vector3(x, y, z);
        }
    }
}
