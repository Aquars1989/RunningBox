using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 無效的目標物件
    /// </summary>
    public class TargetNull : ITarget
    {
        public static TargetNull Value = new TargetNull();

        private TargetNull() { }
        public float X { get { return 0; } }
        public float Y { get { return 0; } }
        public PointF GetPoint() { return PointF.Empty; }
    }
}
