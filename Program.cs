using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace RunningBox
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DeBugSet();
            Application.Run(new MainForm());
        }

        [Conditional("DEBUG")]
        static void DebugSet()
        {
            Global.DebugMode = true;
        }
    }
}
