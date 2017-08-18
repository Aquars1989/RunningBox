using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 能量條繪圖物件
    /// </summary>
    public abstract class DrawUiEnergyBar : DrawUI
    {
        /// <summary>
        /// 綁定物件
        /// </summary>
        public ObjectActive BindingObject { get; set; }

        /// <summary>
        /// 新增能量條繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="bindObject">綁定物件</param>
        public DrawUiEnergyBar(Color color, ObjectActive bindObject = null)
        {
            Color = color;
            BindingObject = bindObject;
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
            if (BindingObject != null && BindingObject.EnergyMax > 0)
            {
                float ratio = BindingObject.Energy / (float)BindingObject.EnergyMax;
                int widthInside = (int)((rectangle.Width - 4) * Math.Min(ratio, 1));
                g.FillRectangle(brush, rectangle.Left + 2, rectangle.Top + 2, widthInside, rectangle.Height - 4);
            }
        }
    }
}
