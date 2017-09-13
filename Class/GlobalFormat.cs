using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    class GlobalFormat
    {
        public static StringFormat TopLeft = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
        public static StringFormat MiddleCenter = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        public static StringFormat MiddleLeft = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
    }
}
