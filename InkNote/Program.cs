using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;

namespace InkNote
{
    static class Program
    {
        static public string s_appDataDir = string.Empty;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string name = Process.GetCurrentProcess().ProcessName;
            Process[] result = Process.GetProcessesByName(name);
            if (result.Length > 1)
            {
                MessageBox.Show("There is already a instance running.", "Information");
                System.Environment.Exit(0);
            }

            string dataDir = System.Configuration.ConfigurationManager.AppSettings["DataDir"];
            if (dataDir != null && dataDir.Length > 0)
            {
                if (System.IO.Directory.Exists(dataDir))
                    s_appDataDir = dataDir;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MdiContainer());
        }
    }
}
