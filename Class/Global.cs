using System;
using System.Collections.Generic;
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
    }
}
