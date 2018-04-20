using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace HeadRotation
{
    public static class ProgramCore
    {
        /// <summary> Какую модель грузим по дефолту - с улыбкой или нет. Нужно для дебага, когда еще не загрузили фотку. </summary>
        public static bool DefaultIsSmile = true;
        public static MainForm MainForm;

        static ProgramCore()
        {
            LogFolder = "log"; // determine the name of the folder that will be used for logging
            logFileName = String.Format("log_{0}.txt", DateTime.Now.ToShortDateString().Replace(".", "_").Replace("/", "_")); // determine log filename. It has prefix "log_" and current date in short format (28.03.2014).
        }

        #region Echo To Log

        /// <summary> File, where will log writing. </summary>
        private static readonly string logFileName;

        /// <summary> Folder, where all log files storaging </summary>
        private static string logFolder;

        /// <summary> Folder, where all log files storaging </summary>
        private static string LogFolder
        {
            set
            {
                try
                {
                    if (value.Trim() == "") // just check. TRIM() funtion for string - remove all spaces from start and end. If our value is empty - just set folder by default. it's- logValue
                        value = "log";
                    //Application.StartupPath
                    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Abalone");
                    value = Path.Combine(path, value); // Path.Combine - concatinating path to directory, where our program and filename.
                    Directory.CreateDirectory(value); // check and create folder 
                    logFolder = value;
                }
                catch
                {
                }
            }
        }

        /// <summary> Private method. Write message to log file </summary>
        /// <param name="message"> Message, that will writing to log file</param>
        private static void WriteMessageToLog(string message)
        {
            try
            {
                var fileName = Path.Combine(Application.StartupPath, Path.Combine(logFolder, logFileName)); // get actual filename for logfile
                using (var streamW = new StreamWriter(fileName, true, Encoding.Default)) // open stream for writing. using statement allow not use "reader.Close()".
                    streamW.WriteLine(message); // and save our message to file.
            }
            catch
            {
            }
        }

        /// <summary> Public method for posting message in logFile </summary>
        /// <param name="text">Message for saving</param>
        /// <param name="messageType">Message type (error, warning or information)</param>
        /// <param name="showMessage">Show messagebox to user</param>
        public static void EchoToLog(string text, EchoMessageType messageType, bool showMessage = false)
        {
            var date = DateTime.Now;
            WriteMessageToLog(date.ToLongDateString() + " " + date.ToLongTimeString() + " " + date.Millisecond + " " + // get current date and time
                              messageType.GetTitle() + " " + // get message type caption
                              text); // Such precision in time will facilitate program debugging, in case of errors

            if (showMessage)
                MessageBox.Show(text, messageType.GetTitle(), MessageBoxButtons.OK);
        }

        /// <summary> Write error to logFile </summary>
        /// <param name="ex">Occured exception</param>
        /// <param name="showMessage">Show messagebox to user</param>
        public static void EchoToLog(Exception ex, bool showMessage = false)
        {
            var date = DateTime.Now;
            WriteMessageToLog(date.ToLongDateString() + " " + date.ToLongTimeString() + " " + date.Millisecond + " " + // get current date and time   
                              EchoMessageType.Error + " " +
                              ex.Message +
                              (ex.InnerException == null || String.IsNullOrEmpty(ex.InnerException.Message) ? "" : "\n" + ex.InnerException.Message)); // get error message ( facilitate program debugging)

            if (showMessage)
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
        }

        #endregion
    }
}
