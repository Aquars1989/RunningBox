using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            float moveX = originX + (float)(Math.Cos(angle / 180 * Math.PI) * distance);
            float moveY = originY + (float)(Math.Sin(angle / 180 * Math.PI) * distance);
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
        /// 取得物件AB是否碰撞
        /// </summary>
        /// <param name="layou1">物件配置A</param>
        /// <param name="layout2">物件配置B</param>
        /// <returns>是否碰撞</returns>
        public static bool IsCollison(Layout layou1, Layout layout2)
        {
            return IsCollison(layou1.CollisonShape, layou1.Rectangle, layout2.CollisonShape, layout2.Rectangle);
        }

        /// <summary>
        /// 取得物件AB是否碰撞
        /// </summary>
        /// <param name="type1">物件A類型</param>
        /// <param name="rect1">物件A區域</param>
        /// <param name="type2">物件B類型</param>
        /// <param name="rect2">物件B區域</param>
        /// <returns>是否碰撞</returns>
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

        /// <summary>
        /// 取得圓角區域
        /// </summary>
        /// <param name="rectangle">區域</param>
        /// <param name="radius">圓角大小</param>
        /// <returns>圓角區域</returns>
        public static GraphicsPath GetRadiusFrame(Rectangle rectangle, int radius)
        {
            GraphicsPath result = new GraphicsPath();
            if (radius == 0)
            {
                result.AddRectangle(rectangle);
            }
            else
            {
                int width = rectangle.Width;
                int height = rectangle.Height;

                //頂端
                result.AddLine(rectangle.Left + radius, rectangle.Top, rectangle.Right - radius, rectangle.Top);
                //右上角
                result.AddArc(rectangle.Right - radius, rectangle.Top, radius, radius, 270, 90);
                //右邊
                result.AddLine(rectangle.Right, rectangle.Top + radius, rectangle.Right, rectangle.Bottom - radius);
                //右下角
                result.AddArc(rectangle.Right - radius, rectangle.Bottom - radius, radius, radius, 0, 90);
                //底邊
                result.AddLine(rectangle.Right - radius, rectangle.Bottom, rectangle.Left + radius, rectangle.Bottom);
                //左下角
                result.AddArc(rectangle.Left, rectangle.Bottom - radius, radius, radius, 90, 90);
                //左邊
                result.AddLine(rectangle.Left, rectangle.Bottom - radius, rectangle.Left, rectangle.Top + radius);
                //左上角
                result.AddArc(rectangle.Left, rectangle.Top, radius, radius, 180, 90);
                result.CloseAllFigures();
            }
            return result;
        }


        private static string[] _NameA = { "堅忍的", "優雅的", "孤獨的", "國家英雄" , "覺醒的", "真．", "恐怖的", "閃亮的", "莊嚴的", "潛伏的","粗黑的","粉色的",
                                           "神選者", "性感的", "無名的", "奇蹟之人", "脫俗的" , "Mr.", "極致的" ,"記憶中的", "亮眼的","飛奔的","纖細的","得道的",
                                           "不可思議","駭人的","天降的","幻影之","隔壁村的","時速破百的","傳說中的","毀天滅地","高大的","深情的","江湖人稱",
                                           "帥氣","龐克","冷酷","怪力","變態","陰沉", "粉紅", "裸體", "煞氣" ,"狂暴","兇惡", "柔情", "飛天","粗勇"};
        private static string[] _NameB = { "大屁", "帥氣", "獨眼", "龐克頭", "假面", "冷酷", "怪力", "變態", "陰沉", "跛腳", "臃腫", "肌肉", "裸體","劈磚",
                                           "手刀","甩砲","黏稠","煞氣" , "狂暴", "兇惡","柔情", "菜刀", "飛天","貧血","吊帶襪","眼鏡","鎧甲","帶刺","粗勇",
                                           "魔棒","蒟蒻","軟Q","少女心","熱舞","電音","搖滾"};
        private static string[] _NameC = { "吉娃娃", "狂猴", "男", "殺手", "老頭", "禿頭", "遊俠", "騎士", "猛男", "書生", "捏麵人", "兄貴", "張飛" ,"未亡人",
                                           "槍手" ,"魔人", "舞者", "哥哥","弟弟","姐姐","妹妹","屁孩","公公","大媽","大嬸","二叔公","技安","狼人", "巨人","阿伯",
                                           "超人","戰士","法師","小學生" ,"黑熊","領主","歐巴桑","大四生","男高音","哥布林","精靈","魔術師","大師","博士"};
        public static string GetRandName()
        {
            string nameA = _NameA[Global.Rand.Next(_NameA.Length)];
            string nameC = _NameC[Global.Rand.Next(_NameC.Length)];
            string nameB;
            int lenLimit = 10 - nameA.Length - nameC.Length;
            do
            {
                nameB = _NameB[Global.Rand.Next(_NameB.Length)];
            }
            while (nameB == nameA || nameB.Length > lenLimit);
            return nameA + nameB + nameC;
        }
    }
}
