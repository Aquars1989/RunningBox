using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 引導能量條
    /// </summary>
    public class DrawUIChanneledBar : DrawUI
    {
        /// <summary>
        /// 綁定技能物件
        /// </summary>
        public SkillBase BindingSkill { get; set; }

        /// <summary>
        /// 線條粗細
        /// </summary>
        public int LineWidth { get; set; }

        /// <summary>
        /// 新增能量條繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="lineWidth">線條粗細</param>
        /// <param name="bindObject">綁定技能物件</param>
        public DrawUIChanneledBar(Color color, int lineWidth, SkillBase bindingSkill = null)
        {
            Color = color;
            LineWidth = lineWidth;
            BindingSkill = bindingSkill;
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
            if (BindingSkill != null && BindingSkill.CooldownLimit > 0)
            {
                float ratio = (BindingSkill.ChanneledLimit - BindingSkill.ChanneledTicks) / (float)BindingSkill.ChanneledLimit;
                int widthInside = (int)((rectangle.Width - LineWidth * 2) * Math.Min(ratio, 1));
                g.FillRectangle(brush, rectangle.Left + LineWidth, rectangle.Top + LineWidth, widthInside, rectangle.Height - LineWidth * 2);
            }
        }

        public override IDraw Copy()
        {
            return new DrawUIChanneledBar(Color, LineWidth, BindingSkill);
        }
    }
}
