﻿using System;
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
        /// 取得位移後位置
        /// </summary>
        /// <param name="originX">原點座標X</param>
        /// <param name="originY">原點座標Y</param>
        /// <param name="angle">角度</param>
        /// <param name="speed">距離</param>
        /// <returns>位移後點位置</returns>
        public static PointF GetOffsetPoint(float originX, float originY, double angle, double distance)
        {
            float moveX = originX + (float)(Math.Cos(angle / 180 * Math.PI * distance));
            float moveY = originY + (float)(Math.Sin(angle / 180 * Math.PI * distance));
            return new PointF(moveX, moveY);
        }

        /// <summary>
        /// 取得兩點間的距離
        /// </summary>
        /// <param name="point1">點A</param>
        /// <param name="point2">點B</param>
        /// <returns>距離</returns>
        public static double GetDistance(PointF point1, PointF point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }

        /// <summary>
        /// 取得兩點間的距離
        /// </summary>
        /// <param name="X1">點A座標X</param>
        /// <param name="Y1">點A座標Y</param>
        /// <param name="X2">點B座標X</param>
        /// <param name="Y2">點B座標Y</param>
        /// <returns>距離</returns>
        public static double GetDistance(float X1, float Y1, float X2, float Y2)
        {
            return Math.Sqrt(Math.Pow(X1 - X2, 2) + Math.Pow(Y1 - Y2, 2));
        }

        /// <summary>
        /// 取得兩點間的距離
        /// </summary>
        /// <param name="X1">點1X座標</param>
        /// <param name="Y1">點1Y座標</param>
        /// <param name="X2">點2X座標</param>
        /// <param name="Y2">點2Y座標</param>
        /// <returns>距離</returns>
        public static bool IsCollison(Layout layou1, Layout layout2)
        {
            return IsCollison(layou1.CollisonShape, layou1.Rectangle, layout2.CollisonShape, layout2.Rectangle);
        }

        public static bool IsCollison(ShapeType type1, Rectangle rect1, ShapeType type2, Rectangle rect2)
        {
            if (!rect1.IntersectsWith(rect2)) return false; //如果矩形未相交,任意形狀一定不相交

            if (type1 == type2)//同類型檢測
            {
                switch (type1)
                {
                    case ShapeType.Ellipse:  //圓型碰撞檢測
                        float halfWidth1 = rect1.Width / 2F;
                        float halfHeight1 = rect1.Height / 2F;
                        float halfWidth2 = rect2.Width / 2F;
                        float halfHeight2 = rect2.Height / 2F;
                        float centerX1 = rect1.Left + halfWidth1;
                        float centerY1 = rect1.Top + halfHeight1;
                        float centerX2 = rect2.Left + halfWidth2;
                        float centerY2 = rect2.Top + halfHeight2;
                        float distanceX = centerX2 - centerX1;
                        float distanceY = centerY2 - centerY1;
                        double distance = Math.Sqrt(distanceX * distanceX + distanceY * distanceY); //兩中心距離

                        double roation = Math.Atan2(distanceY, distanceX);
                        double radius1 = halfWidth1 == halfHeight1 ? halfWidth1 : Math.Sqrt(Math.Pow(halfWidth1 * Math.Cos(roation), 2) + Math.Pow(halfHeight1 * Math.Sin(roation), 2)); //圓半徑1
                        double radius2 = halfWidth2 == halfHeight2 ? halfWidth2 : Math.Sqrt(Math.Pow(halfWidth2 * Math.Cos(roation), 2) + Math.Pow(halfHeight2 * Math.Sin(roation), 2)); //圓半徑2
                        return distance < radius1 + radius2;
                    case ShapeType.Rectangle://矩形碰撞檢測,已做
                        return true;
                    default:
                        return true;
                }
            }
            else //混合碰撞檢測
            {
                Rectangle rectangle, ellipse;

                if (type1 == ShapeType.Rectangle)
                {
                    rectangle = rect1;
                    ellipse = rect2;
                }
                else
                {
                    rectangle = rect2;
                    ellipse = rect1;
                }

                float halfWidth1 = rectangle.Width / 2F;
                float halfHeight1 = rectangle.Height / 2F;
                float halfWidth2 = ellipse.Width / 2F;
                float halfHeight2 = ellipse.Height / 2F;
                float centerX1 = rectangle.Left + halfWidth1;
                float centerY1 = rectangle.Top + halfHeight1;
                float centerX2 = ellipse.Left + halfWidth2;
                float centerY2 = ellipse.Top + halfHeight2;
                float distanceX = centerX2 - centerX1;
                float distanceY = centerY2 - centerY1;

                float dX = Math.Abs(distanceX);
                float dY = Math.Abs(distanceY);
                float uX = dX > halfWidth1 ? dX - halfWidth1 : 0;
                float uY = dY > halfHeight1 ? dY - halfHeight1 : 0;

                double roation = Math.Atan2(distanceY, distanceX);
                double radius2 = halfWidth2 == halfHeight2 ? halfWidth2 : Math.Sqrt(Math.Pow(halfWidth2 * Math.Cos(roation), 2) + Math.Pow(halfHeight2 * Math.Sin(roation), 2)); //圓半徑2
                return (uX * uX + uY * uY) < (radius2 * radius2);
            }
        }
    }
}
