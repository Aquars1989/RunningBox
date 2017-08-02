using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    class Function
    {
        public static double PointRotation(PointF PotA, PointF PotB)
        {
            float Dx = PotB.X - PotA.X;
            float Dy = PotB.Y - PotA.Y;
            double DRoation = Math.Atan2(Dy, Dx);
            double WRotation = DRoation / Math.PI * 180;
            return WRotation;
        }

        public static double PointRotation(float X1, float Y1, float X2, float Y2)
        {
            float Dx = X2 - X1;
            float Dy = Y2 - Y1;
            double DRoation = Math.Atan2(Dy, Dx);
            double WRotation = DRoation / Math.PI * 180;
            return WRotation;
        }
    }
}
