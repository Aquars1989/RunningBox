using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    class Global
    {
        public static bool DebugMode = false;
        public static Random Rand = new Random();
        public static int DefaultIntervalOfRound = 20;
        public static int DefaultIntervalOfWave = 1500;
        public static int DefaultEndDelayLimit = 1000;
        public static Padding DefaultMainRectanglePadding = new Padding(80, 80, 80, 80);

        public static Font CommandFont = new System.Drawing.Font("微軟正黑體", 22);
        public static StringFormat CommandFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        public static int DefaultWaveMax = 60;
        public static int DefaultScoreMax = 1000000;
        public static int DefaultEnergyLimit = 10000;
        public static int DefaultEnergyGetPerSec = 1500;
    }
}
