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
        public static double GetAngle(PointF PotA, PointF PotB)
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
        public static double GetAngle(float X1, float Y1, float X2, float Y2)
        {
            float Dx = X2 - X1;
            float Dy = Y2 - Y1;
            double DRoation = Math.Atan2(Dy, Dx);
            double WRotation = DRoation / Math.PI * 180;
            return WRotation;
        }

        /// <summary>
        /// 取得兩點間的距離
        /// </summary>
        /// <param name="point1">點1</param>
        /// <param name="point2">點2</param>
        /// <returns>距離</returns>
        public static double GetDistance(PointF point1, PointF point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }

        /// <summary>
        /// 取得兩點間的距離
        /// </summary>
        /// <param name="X1">點1X座標</param>
        /// <param name="Y1">點1Y座標</param>
        /// <param name="X2">點2X座標</param>
        /// <param name="Y2">點2Y座標</param>
        /// <returns>距離</returns>
        public static double GetDistance(float X1, float Y1, float X2, float Y2)
        {
            return Math.Sqrt(Math.Pow(X1 - X2, 2) + Math.Pow(Y1 - Y2, 2));
        }

        /// <summary>
        /// 取得兩物件間的距離
        /// </summary>
        /// <param name="point1">物件1</param>
        /// <param name="point2">物件2</param>
        /// <param name="deductSize">距離是否扣除兩物件半徑</param>
        /// <returns>距離</returns>
        public static double GetDistance(ObjectBase object1, ObjectBase object2, bool deductSize)
        {
            double result = Math.Sqrt(Math.Pow(object1.X - object2.X, 2) + Math.Pow(object1.Y - object2.Y, 2));
            if (deductSize)
            {
                result -= object1.Size + object2.Size;
            }
            return result;
        }
    }
}
