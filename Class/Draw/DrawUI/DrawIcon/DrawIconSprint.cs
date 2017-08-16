using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能:衝刺繪圖物件
    /// </summary>
    public class DrawIconSprint : DrawIconBase
    {
        /// <summary>
        /// 動畫進度
        /// </summary>
        public int Animation { get; set; }

        /// <summary>
        /// 新增技能:衝刺繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        public DrawIconSprint(Color color, EnumSkillButton drawButton)
        {
            Color = color;
            Animation = 0;
            DrawButton = drawButton;
        }

        /// <summary>
        /// 繪製圖示
        /// </summary>
        /// <param name="g">畫筆物件</param>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void DrawIcon(Pen pen, SolidBrush brush, Graphics g, Rectangle rectangle)
        {
            if (Animation > 32)
            {
                Animation %= 32;
            }

            int ani = Animation / 2 % 4;

            float drawX = rectangle.Left + (rectangle.Width * 0.1F), drawY = rectangle.Top + (rectangle.Height * 0.1F);
            float size = rectangle.Width * 0.3F;
            g.FillEllipse(brush, drawX, drawY, size, size);


            do
            {
                size -= ani * rectangle.Width * 0.3F / 16F;
                drawX += ani * rectangle.Width * 0.7F / 16F;
                drawY += ani * rectangle.Width * 0.7F / 16F;
                if (size > 0)
                {
                    g.FillEllipse(brush, drawX, drawY, size, size);
                }
                ani = 4;
            } while (size > 0);
            Animation++;
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override IDraw Copy()
        {
            return new DrawIconSprint(Color, DrawButton);
        }
    }
}
