﻿using OpenTK;
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
            BeginUpdate();
            textX.Text = EditablePoint.X.ToString();
            textY.Text = EditablePoint.Y.ToString();
            textZ.Text = EditablePoint.Z.ToString();
            EndUpdate();
        }


        private void edit_TextChanged(object sender, EventArgs e)
        {
            if (IsLockUpdating())
                return;

            var x = Helpers.StringConverter.ToFloat(textX.Text);
            var y = Helpers.StringConverter.ToFloat(textY.Text);
            var z = Helpers.StringConverter.ToFloat(textZ.Text);
            if (ProgramCore.MainForm == null)
                return;

            if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z) || textX.Text.EndsWith(",") || textY.Text.EndsWith(",") || textZ.Text.EndsWith(","))
                return;

            ProgramCore.MainForm.RenderControl.HeadPoints.SetSelectedPoint(new Vector3(x, y, z));
        }
        private void EditPointForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            UpdateEditablePoint(Vector3.Zero);
            e.Cancel = true;
        }

        private int updater;
        private void BeginUpdate()
        {
            ++updater;
        }
        private void EndUpdate()
        {
            --updater;
        }
        private bool IsLockUpdating()
        {
            return updater > 0;
        }
    }
}
