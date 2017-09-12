using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 雜訊繪圖物件
    /// </summary>
    public class DrawNoise : DrawBase
    {
        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Colors.GetColor("Back"); }
        }

        /// <summary>
        /// 最大畫筆寬度
        /// </summary>
        public int MaxBorderWidth { get; set; }

        /// <summary>
        /// 使用繪圖工具管理物件新增雜訊繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="maxBorderWidth">線條最大寬度</param>
        public DrawNoise(DrawColors drawColor, int maxBorderWidth)
            : base(drawColor)
        {
            MaxBorderWidth = maxBorderWidth;
        }

        /// <summary>
        /// 新增雜訊繪圖物件
        /// </summary>
        /// <param name="borderColor">雜訊顏色1</param>
        /// <param name="borderColor2">雜訊顏色2</param>
        /// <param name="maxBorderWidth">線條最大寬度</param>
        public DrawNoise(Color borderColor, Color borderColor2, int maxBorderWidth)
        {
            Colors.SetColor("Border", borderColor);
            Colors.SetColor("Border2", borderColor2);
            MaxBorderWidth = maxBorderWidth;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            if (MaxBorderWidth < 1) return;

            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            Pen penBorder1 = Colors.GetPen("Border");
            Pen penBorder2 = Colors.GetPen("Border2");
            penBorder1.Width = 1;
            penBorder2.Width = 1;
            int lineHeight = drawRectangle.Height / 4;
            for (int i = 0; i <= drawRectangle.Width; )
            {
                int drawX = drawRectangle.Left + i;
                int drawY1 = drawRectangle.Top + Global.Rand.Next(0, drawRectangle.Height - lineHeight);
                int drawY2 = drawY1 + lineHeight;

                Pen drawPen = (Global.Rand.Next(2) == 1) ? penBorder1 : penBorder2;
                if (MaxBorderWidth > 1)
                    drawPen.Width = Global.Rand.Next(1, MaxBorderWidth);
                g.DrawLine(drawPen, drawX, drawY1, drawX, drawY2);

                i += (int)drawPen.Width + Global.Rand.Next(0, 2);
            }
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawNoise(Colors.Copy(), MaxBorderWidth)
            {
                Scene = this.Scene,
                Owner = this.Owner,
                Scale = this.Scale
            };
        }
    }
}
