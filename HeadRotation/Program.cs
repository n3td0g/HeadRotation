using System;
using System.Windows.Forms;

namespace HeadRotation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ProgramCore.MainForm = new MainForm();
            Application.Run(ProgramCore.MainForm);
        }
    }
}
