using System;
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
    public class DrawSkillBulletTime : DrawSkillBase
    {
        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Colors.GetColor("Icon"); }
        }

        /// <summary>
        /// 動畫進度
        /// </summary>
        public int Animation { get; set; }

        /// <summary>
        /// 由繪圖工具管理物件新增技能:時間減緩繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="bindingSkill">綁定技能</param>
        public DrawSkillBulletTime(DrawColors drawColor, SkillBase bindingSkill = null)
            : base(drawColor)
        {
            Animation = 0;
            BindingSkill = bindingSkill;
        }

        /// <summary>
        /// 新增技能:時間減緩繪圖物件
        /// </summary>
        /// <param name="iconColor">繪製顏色</param>
        /// <param name="bindingSkill">綁定技能</param>
        public DrawSkillBulletTime(Color iconColor, SkillBase bindingSkill = null)
        {
            Colors.SetColor("Icon", iconColor);
            Animation = 0;
            BindingSkill = bindingSkill;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            int width = drawRectangle.Width;
            int height = drawRectangle.Height;
            int paddingX = (int)(width * 0.1F);
            int paddingY = (int)(height * 0.1F);
            Rectangle clockRect = new Rectangle(drawRectangle.Left + paddingX, drawRectangle.Top + paddingY, width - paddingX * 2, height - paddingY * 2);

            Pen penIcon = Colors.GetPen("Icon");
            penIcon.Width = 1;
            g.DrawEllipse(penIcon, clockRect);

            if (Animation > 2880)
            {
                Animation %= 2880;
            }
            int ani = Animation / 2;
            float h = ani / 60F;
            int m = ani % 60;

            PointF point1 = new PointF(drawRectangle.Left + drawRectangle.Width / 2, drawRectangle.Top + drawRectangle.Height / 2);
            float directionH = (h * 15) - 180;
            float lengthH = (drawRectangle.Width + drawRectangle.Height) / 4 * 0.5F;
            float moveHX = (float)Math.Cos(directionH / 180 * Math.PI) * lengthH;
            float moveHY = (float)Math.Sin(directionH / 180 * Math.PI) * lengthH;
            PointF pointH = new PointF(point1.X + moveHX, point1.Y + moveHY);
            g.DrawLine(penIcon, point1, pointH);

            float directionM = (m * 6) - 180;
            float lengthM = (drawRectangle.Width + drawRectangle.Height) / 4 * 0.7F;
            float moveMX = (float)Math.Cos(directionM / 180 * Math.PI) * lengthM;
            float moveMY = (float)Math.Sin(directionM / 180 * Math.PI) * lengthM;
            PointF pointM = new PointF(point1.X + moveMX, point1.Y + moveMY);
            g.DrawLine(penIcon, point1, pointM);

            Animation += BindingSkill != null && BindingSkill.Status == SkillStatus.Channeled ? 1 : 3;
        }

        /// <summary>
        /// 複製繪圖物件,未綁定物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawSkillBulletTime(Colors.Copy(), BindingSkill)
            {
                Scale = this.Scale,
                Angle = this.Angle,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled
            };
        }
    }
}
