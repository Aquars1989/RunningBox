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
        private Pen _Pen;

        /// <summary>
        /// 動畫進度
        /// </summary>
        public int Animation { get; set; }

        /// <summary>
        /// 新增技能:護盾繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="bindingSkill">綁定技能</param>
        public DrawSkillShield(Color color, SkillBase bindingSkill = null)
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
            if (Animation >= 120)
            {
                Animation %= 120;
            }

            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            int width = drawRectangle.Width;
            int height = drawRectangle.Height;
            int paddingCenterX = (int)(width * 0.6F);
            int paddingCenterY = (int)(height * 0.6F);
            int paddingX = (int)(width * 0.2F);
            int paddingY = (int)(height * 0.2F);
            Rectangle centerRect = new Rectangle(drawRectangle.Left + paddingCenterX, drawRectangle.Top + paddingCenterY, width - paddingCenterX * 2, height - paddingCenterY * 2);
            Rectangle shieldRect = new Rectangle(drawRectangle.Left + paddingX, drawRectangle.Top + paddingY, width - paddingX * 2, height - paddingY * 2);

            GetPen(ref _Pen, Color, Opacity, RFix, GFix, BFix);
            _Pen.Width = 1;
            if (BindingSkill != null && BindingSkill.Status != SkillStatus.Channeled)
            {
                _Pen.DashStyle = DashStyle.Custom;
                _Pen.DashPattern = new float[] { 1,1 };
            }

            int helfWidth = shieldRect.Width / 2;
            int helfHeight = shieldRect.Width / 2;
            int midX = shieldRect.Left + helfWidth;
            int midY = shieldRect.Top + helfHeight;

            Point[] pots = new Point[6];
            float partAngle = 360F / 6;
            for (int i = 0; i < 6; i++)
            {
                float angle = 180 - i * partAngle +Animation * 3;
                int x = (int)(Math.Sin(angle / 180F * Math.PI) * helfWidth);
                int y = (int)(Math.Cos(angle / 180F * Math.PI) * helfHeight);
                pots[i] = new Point(midX + x, midY + y);
            }

            g.DrawPolygon(_Pen, pots);
            _Pen.DashStyle = DashStyle.Solid;
            g.DrawEllipse(_Pen, centerRect);
            Animation++;
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawSkillBulletTime(Color, BindingSkill)
            {
                Scene = this.Scene,
                Owner = this.Owner,
                Opacity = this.Opacity,
                RFix = this.RFix,
                GFix = this.GFix,
                BFix = this.BFix,
                Scale = this.Scale
            };
        }

        protected override void OnColorChanged()
        {
            BackPen(ref _Pen);
            base.OnColorChanged();
        }

        protected override void OnColorFixChanged()
        {
            BackPen(ref _Pen);
            base.OnColorFixChanged();
        }

        protected override void OnDispose()
        {
            BackPen(ref _Pen);
        }
    }
}
