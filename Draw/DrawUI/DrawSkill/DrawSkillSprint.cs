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
    public class DrawSkillSprint : DrawSkillBase
    {
        private SolidBrush _Brush;

        /// <summary>
        /// 動畫進度
        /// </summary>
        public int Animation { get; set; }

        /// <summary>
        /// 新增技能:衝刺繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="bindingSkill">綁定技能</param>
        public DrawSkillSprint(Color color, SkillBase bindingSkill = null)
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
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = GetScaleRectangle(rectangle);

            int aniMax = 8;
            int criMax = 32;
            if (Animation >= aniMax)
            {
                Animation %= aniMax;
            }

            int ani = Animation;
            float drawX = drawRectangle.Left + (drawRectangle.Width * 0.1F); //原始位置
            float drawY = drawRectangle.Top + (drawRectangle.Height * 0.1F); //原始位置
            float size = drawRectangle.Width * 0.3F; //原始大小

            GetBrush(ref _Brush, Color, Opacity, RFix, GFix, BFix);
            g.FillEllipse(_Brush, drawX, drawY, size, size);


            while (true)
            {
                float ratio = ani / (float)criMax;
                size -= ratio * drawRectangle.Width * 0.3F;
                drawX += ratio * drawRectangle.Width * 0.7F;
                drawY += ratio * drawRectangle.Height * 0.7F;
                if (size > 0)
                {
                    g.FillEllipse(_Brush, drawX, drawY, size, size);
                }
                else break;
                ani = aniMax;
            }

            Animation++;
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawSkillSprint(Color, BindingSkill)
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
            BackBrush(ref _Brush);
            base.OnColorChanged();
        }

        protected override void OnColorFixChanged()
        {
            BackBrush(ref _Brush);
            base.OnColorFixChanged();
        }

        protected override void OnDispose()
        {
            BackBrush(ref _Brush);
        }
    }
}
