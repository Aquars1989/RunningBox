using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 多邊型繪圖物件
    /// </summary>
    public class DrawPolygon : DrawBase
    {
        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Colors.GetColor("Border"); }
        }

        /// <summary>
        /// 框線粗細
        /// </summary>
        public int BorderWidth { get; set; }

        /// <summary>
        /// 多邊形邊數,2為直線
        /// </summary>
        public int NumberOfSides { get; set; }

        /// <summary>
        /// 由繪圖工具管理物件新增畫筆繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="numberOfSides">多邊形邊數,2為直線</param>
        /// <param name="borderWidth">框線粗細</param>
        /// <param name="angle">旋轉角度</param>
        public DrawPolygon(DrawColors drawColor, int numberOfSides, int borderWidth, float angle)
            : base(drawColor)
        {
            BorderWidth = borderWidth;
            NumberOfSides = numberOfSides;
            Angle = angle;
        }

        /// <summary>
        /// 新增畫筆繪圖物件
        /// </summary>
        /// <param name="backColor">填滿顏色</param>
        /// <param name="borderColor">框架顏色</param>
        /// <param name="numberOfSides">多邊形邊數,2為直線</param>
        /// <param name="borderWidth">框線粗細</param>
        /// <param name="angle">旋轉角度</param>
        public DrawPolygon(Color backColor, Color borderColor, int numberOfSides, int borderWidth, float angle)
        {
            Colors.SetColor("Main", backColor);
            Colors.SetColor("Border", borderColor);
            BorderWidth = borderWidth;
            NumberOfSides = numberOfSides;
            Angle = angle;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            if (NumberOfSides < 2 || (NumberOfSides == 2 && BorderWidth < 1)) return;

            Rectangle drawRectangle = GetScaleRectangle(rectangle);

            int helfWidth = drawRectangle.Width / 2;
            int helfHeight = drawRectangle.Width / 2;
            int midX = drawRectangle.Left + helfWidth;
            int midY = drawRectangle.Top + helfHeight;

            Point[] pots = new Point[NumberOfSides];
            float partAngle = 360F / NumberOfSides;
            for (int i = 0; i < NumberOfSides; i++)
            {
                float angle = i * partAngle + Angle;
                int x = (int)(Math.Cos(angle / 180F * Math.PI) * helfWidth);
                int y = (int)(Math.Sin(angle / 180F * Math.PI) * helfHeight);
                pots[i] = new Point(midX + x, midY + y);
            }

            if (NumberOfSides > 2)
            {
                Brush brushBack = Colors.GetBrush("Main");
                g.FillPolygon(brushBack, pots);

                if (BorderWidth > 0)
                {
                    Pen penBorder = Colors.GetPen("Border", BorderWidth);
                    g.DrawPolygon(penBorder, pots);
                }
            }
            else
            {
                Pen penBorder = Colors.GetPen("Border", BorderWidth);
                g.DrawLine(penBorder, pots[0], pots[1]);
            }
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件,未綁定物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawPolygon(Colors.Copy(), NumberOfSides, BorderWidth, Angle)
            {
                Scale = this.Scale,
                Angle = this.Angle,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled
            };
        }
    }
}
