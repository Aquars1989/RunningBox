
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public interface ITarget
    {
        float X { get; }
        float Y { get; }
        PointF GetPoint();
    }
}
