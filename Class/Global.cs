using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    class Global
    {
        public static SQLobject SQL = new SQLobject("server=104.199.150.7;uid=client;Password=playforfun;database=Survive100");
        public static string RegistryAddr = "SOFTWARE\\Survive100\\";

        public static bool DebugMode = false;
        public static Random Rand = new Random(Guid.NewGuid().GetHashCode());

        public static Font CommandFont = new System.Drawing.Font("微軟正黑體", 20, FontStyle.Bold);
        public static StringFormat CommandFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        public static int DefaultIntervalOfRound = 20;
        public static int DefaultIntervalOfWave = 1500;
        public static int DefaultEndDelayLimit = 1000;
        public static Padding DefaultMainRectanglePadding = new Padding(80, 80, 80, 80);
        public static int DefaultWaveMax = 60;
        public static int DefaultScoreMax = 1000000;
        public static int DefaultEnergyLimit = 10000;
        public static int DefaultEnergyGetPerSec = 1500;

        public static string PlayerName;
        public static string UUID;

        static Global()
        {
            ManagementObjectCollection instances = new ManagementClass("Win32_ComputerSystemProduct").GetInstances();
            foreach (ManagementObject mo in instances)
            {
                UUID = mo.Properties["UUID"].Value.ToString();
                return;
            }
        }
    }
}
