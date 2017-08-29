using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 計數器繪圖物件
    /// </summary>
    public class DrawUICounterBar : DrawUI
    {
        /// <summary>
        /// 綁定計數器
        /// </summary>
        public CounterObject BindingCounter { get; set; }

        /// <summary>
        /// 線條粗細
        /// </summary>
        public int LineWidth { get; set; }

        /// <summary>
        /// 是否反向顯示
        /// </summary>
        public bool Reverse { get; set; }

        /// <summary>
        /// 新增能量條繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="lineWidth">線條粗細</param>
        /// <param name="reverse">是否反向顯示</param>
        /// <param name="bindObject">綁定物件</param>
        public DrawUICounterBar(Color color, int lineWidth, bool reverse, CounterObject bindingCounter = null)
        {
            Color = color;
            LineWidth = lineWidth;
            Reverse = reverse;
            BindingCounter = bindingCounter;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public override void Draw(Graphics g, Rectangle rectangle)
        {
            g.FillRectangle(Brushes.AliceBlue, rectangle);
            g.DrawRectangle(Pens.Black, rectangle);

            SolidBrush brush = GetBrush();
            if (BindingCounter != null && BindingCounter.Value > 0)
            {
                float ratio = Math.Min(BindingCounter.GetRatio(), 1);
                if (Reverse) ratio = 1 - ratio;
                int widthInside = (int)((rectangle.Width - LineWidth * 2) * ratio + 0.5F);
                if (widthInside > 0)
                {
                    g.FillRectangle(brush, rectangle.Left + LineWidth, rectangle.Top + LineWidth, widthInside, rectangle.Height - LineWidth * 2);
                }
            }
        }

        public override IDraw Copy()
        {
            return new DrawUICounterBar(Color, LineWidth, Reverse, BindingCounter);
        }
    }
}
