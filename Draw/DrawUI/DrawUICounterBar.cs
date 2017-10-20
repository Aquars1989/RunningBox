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
    public class DrawUICounterBar : DrawBase
    {
        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Colors.GetColor("Icon"); }
        }

        /// <summary>
        /// 綁定計數器
        /// </summary>
        public CounterObject BindingCounter { get; set; }

        /// <summary>
        /// 線條粗細
        /// </summary>
        public int BorderWidth { get; set; }

        /// <summary>
        /// 是否反向顯示
        /// </summary>
        public bool Reverse { get; set; }

        /// <summary>
        /// 新增能量條繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="borderWidth">框線粗細</param>
        /// <param name="reverse">是否反向顯示</param>
        /// <param name="bindObject">綁定物件</param>
        public DrawUICounterBar(DrawColors drawColor, int borderWidth, bool reverse, CounterObject bindingCounter = null)
            : base(drawColor)
        {
            BorderWidth = borderWidth;
            Reverse = reverse;
            BindingCounter = bindingCounter;
        }

        /// <summary>
        /// 新增能量條繪圖物件
        /// </summary>
        /// <param name="mainColor">條棒顏色</param>
        /// <param name="borderColor">外框顏色</param>
        /// <param name="backColor">底色</param>
        /// <param name="borderWidth">框線粗細</param>
        /// <param name="reverse">是否反向顯示</param>
        /// <param name="bindObject">綁定物件</param>
        public DrawUICounterBar(Color mainColor, Color borderColor, Color backColor, int borderWidth, bool reverse, CounterObject bindingCounter = null)
        {
            Colors.SetColor("Main", mainColor);
            Colors.SetColor("Border", borderColor);
            Colors.SetColor("Back", backColor);
            BorderWidth = borderWidth;
            Reverse = reverse;
            BindingCounter = bindingCounter;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            Brush brushMain = Colors.GetBrush("Main");
            Brush brushBack = Colors.GetBrush("Back");
            Pen penBorder = Colors.GetPen("Border", BorderWidth);
            g.FillRectangle(brushBack, drawRectangle);
            g.DrawRectangle(penBorder, drawRectangle);

            if (BindingCounter != null && BindingCounter.Value > 0)
            {
                float ratio = Math.Min(BindingCounter.GetRatio(), 1);
                if (Reverse) ratio = 1 - ratio;
                int widthInside = (int)((drawRectangle.Width - BorderWidth * 2) * ratio + 0.5F);
                if (widthInside > 0)
                {
                    g.FillRectangle(brushMain, drawRectangle.Left + BorderWidth, drawRectangle.Top + BorderWidth, widthInside, drawRectangle.Height - BorderWidth * 2);
                }
            }
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件,未綁定物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawUICounterBar(Colors.Copy(), BorderWidth, Reverse, BindingCounter)
            {
                Scale = this.Scale,
                Angle = this.Angle,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled
            };
        }
    }
}
