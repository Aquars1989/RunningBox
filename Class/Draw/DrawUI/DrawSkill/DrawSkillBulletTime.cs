﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能:時間減緩繪圖物件
    /// </summary>
    public class DrawSkillBulletTime : DrawUI, IDrawSkill
    {
        /// <summary>
        /// 綁定技能
        /// </summary>
        public SkillBase BindingSkill { get; set; }

        /// <summary>
        /// 動畫進度
        /// </summary>
        public int Animation { get; set; }

        /// <summary>
        /// 新增技能:時間減緩繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="bindingSkill">綁定技能</param>
        public DrawSkillBulletTime(Color color, SkillBase bindingSkill = null)
        {
            Color = color;
            Animation = 0;
            BindingSkill = bindingSkill;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public override void Draw(Graphics g, Rectangle rectangle)
        {
            int width = rectangle.Width;
            int height = rectangle.Height;
            int paddingX = (int)(width * 0.1F);
            int paddingY = (int)(height * 0.1F);
            Rectangle clockRect = new Rectangle(rectangle.Left + paddingX, rectangle.Top + paddingY, width - paddingX * 2, height - paddingY * 2);

            Pen pen = GetPen();
            pen.Width = 1;
            g.DrawEllipse(pen, clockRect);

            if (Animation > 2880)
            {
                Animation %= 2880;
            }
            int ani = Animation / 2;
            float h = ani / 60F;
            int m = ani % 60;

            PointF point1 = new PointF(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
            float directionH = (h * 15) - 180;
            float lengthH = (rectangle.Width + rectangle.Height) / 4 * 0.5F;
            float moveHX = (float)Math.Cos(directionH / 180 * Math.PI) * lengthH;
            float moveHY = (float)Math.Sin(directionH / 180 * Math.PI) * lengthH;
            PointF pointH = new PointF(point1.X + moveHX, point1.Y + moveHY);
            g.DrawLine(pen, point1, pointH);

            float directionM = (m * 6) - 180;
            float lengthM = (rectangle.Width + rectangle.Height) / 4 * 0.7F;
            float moveMX = (float)Math.Cos(directionM / 180 * Math.PI) * lengthM;
            float moveMY = (float)Math.Sin(directionM / 180 * Math.PI) * lengthM;
            PointF pointM = new PointF(point1.X + moveMX, point1.Y + moveMY);
            g.DrawLine(pen, point1, pointM);

            Animation += BindingSkill != null && BindingSkill.Status == SkillStatus.Channeled ? 1 : 3;
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override IDraw Copy()
        {
            return new DrawSkillBulletTime(Color, BindingSkill);
        }
    }
}