using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    class Function
    {
        /// <summary>
        /// 取得角度值(+-180)
        /// </summary>
        /// <param name="PotA">點A</param>
        /// <param name="PotB">點B</param>
        /// <returns>角度值(+-180)</returns>
        public static double PointRotation(PointF PotA, PointF PotB)
        {
            float Dx = PotB.X - PotA.X;
            float Dy = PotB.Y - PotA.Y;
            double DRoation = Math.Atan2(Dy, Dx);
            double WRotation = DRoation / Math.PI * 180;
            return WRotation;
        }

        /// <summary>
        /// 取得角度值(+-180)
        /// </summary>
        /// <param name="X1">點A座標X</param>
        /// <param name="Y1">點A座標Y</param>
        /// <param name="X2">點B座標X</param>
        /// <param name="Y2">點B座標Y</param>
        /// <returns>角度值(+-180)</returns>
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
