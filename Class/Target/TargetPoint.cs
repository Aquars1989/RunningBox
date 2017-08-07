using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class TargetPoint : ITarget
    {
        public float X
        {
            get { return Targer.X; }
        }

        public float Y
        {
            get { return Targer.Y; }
        }

        public PointF Targer { get; set; }

        public TargetPoint(int x, int y)
        {
            Targer = new PointF(x, y);
        }

        public TargetPoint(PointF point)
        {
            Targer = point;
        }

        public PointF GetPoint()
        {
            return Targer;
        }
    }
}
