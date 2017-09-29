﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
            AppDomain.CurrentDomain.AssemblyResolve += (x, e) =>
            {
                string resourceName = "RunningBox.Assemblies." + new AssemblyName(e.Name).Name + ".dll";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SetDebug();

            RegistryKey oRegistryKey = Registry.CurrentUser.CreateSubKey(Global.RegistryAddr + "\\PlayerInfo");
            Global.PlayerName = oRegistryKey.GetValue("PlayerName", "") as string;

            GlobalScenes.Scenes.AllReadRegistry();
            Application.Run(new MainForm());
        }

        [Conditional("DEBUG")]
        static void SetDebug()
        {
            Global.DebugMode = true;
        }
    }
}
