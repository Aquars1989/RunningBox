using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能:護盾繪圖物件
    /// </summary>
    public class DrawSkillShield : DrawSkillBase
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
        /// 由繪圖工具管理物件新增技能:護盾繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="bindingSkill">綁定技能</param>
        public DrawSkillShield(DrawColors drawColor, SkillBase bindingSkill = null)
            : base(drawColor)
        {
            Animation = 0;
            BindingSkill = bindingSkill;
        }

        /// <summary>
        /// 新增技能:護盾繪圖物件
        /// </summary>
        /// <param name="iconColor">繪製顏色</param>
        /// <param name="bindingSkill">綁定技能</param>
        public DrawSkillShield(Color iconColor, SkillBase bindingSkill = null)
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
            if (Animation >= 120)
            {
                Animation %= 120;
            }

            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            int left = drawRectangle.Left;
            int top = drawRectangle.Top;
            int width = drawRectangle.Width;
            int height = drawRectangle.Height;
            int centerWidth = (int)(width * 0.25F + 0.5F);
            int centerHeight = (int)(height * 0.25F + 0.5F);
            int shieldWidth = (int)(width * 0.7F + 0.5F);
            int shieldHeight = (int)(height * 0.7F + 0.5F);
            Rectangle centerRect = new Rectangle(left + (width - centerWidth) / 2, top + (height - centerHeight) / 2, centerWidth, centerHeight);
            Rectangle shieldRect = new Rectangle(left + (width - shieldWidth) / 2, top + (height - shieldHeight) / 2, shieldWidth, shieldHeight);

            SolidBrush brushIcon = Colors.GetBrush("Icon");
            Pen penIcon = Colors.GetPen("Icon");
            if (BindingSkill != null && BindingSkill.Status != SkillStatus.Channeled)
            {
                penIcon.Width = 1;
                penIcon.DashStyle = DashStyle.Custom;
                penIcon.DashPattern = new float[] { 1, 1 };
            }
            else
            {
                penIcon.Width = (Animation / 5) % 2 + 1;
            }

            int helfWidth = shieldRect.Width / 2;
            int helfHeight = shieldRect.Width / 2;
            int midX = shieldRect.Left + helfWidth;
            int midY = shieldRect.Top + helfHeight;

            Point[] pots = new Point[6];
            float partAngle = 360F / 6;
            for (int i = 0; i < 6; i++)
            {
                float angle = 180 - i * partAngle + Animation * 3;
                int x = (int)(Math.Sin(angle / 180F * Math.PI) * helfWidth);
                int y = (int)(Math.Cos(angle / 180F * Math.PI) * helfHeight);
                pots[i] = new Point(midX + x, midY + y);
            }

            g.DrawPolygon(penIcon, pots);
            penIcon.DashStyle = DashStyle.Solid;

            g.FillEllipse(brushIcon, centerRect);
            Animation++;
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
