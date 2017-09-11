using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能:誘餌繪圖物件
    /// </summary>
    public class DrawSkillBait : DrawSkillBase
    {
        private SolidBrush _Brush;
        private Pen _Pen;

        /// <summary>
        /// 動畫進度
        /// </summary>
        public int Animation { get; set; }

        /// <summary>
        /// 新增技能:誘餌繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="bindingSkill">綁定技能</param>
        public DrawSkillBait(Color color, SkillBase bindingSkill = null)
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
            int aniMax = 40;
            if (Animation >= aniMax)
            {
                Animation %= aniMax;
            }

            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            int left = drawRectangle.Left;
            int top = drawRectangle.Top;
            int width = drawRectangle.Width;
            int height = drawRectangle.Height;
            int centerWidth = (int)(width * 0.25F + 0.5F);
            int centerHeight = (int)(height * 0.25F + 0.5F);
            int centLeft = left + (width - centerWidth) / 2;
            int centTop = top + (height - centerHeight) / 2;

            int aniStart = aniMax / 3;
            float offsetWidth = (width * 0.2F);
            if (Animation <= aniStart)
            {
                GetBrush(ref _Brush, Color, Opacity, RFix, GFix, BFix);
                g.FillEllipse(_Brush, centLeft, centTop, centerWidth, centerHeight);
            }
            else
            {
                int ani = Animation - aniStart;
                int aniStartMaxHalf = (aniMax - aniStart) / 2;
                float offsetX = (1 - (Math.Abs(aniStartMaxHalf - ani) / (float)aniStartMaxHalf)) * 2F;
                if (offsetX > 1) offsetX = 1;

                GetPen(ref _Pen, Color, Opacity, RFix, GFix, BFix);
                GetBrush(ref _Brush, Color, Opacity, RFix, GFix, BFix);

                g.DrawEllipse(_Pen, centLeft + offsetX * offsetWidth, centTop, centerWidth, centerHeight);
                g.FillEllipse(_Brush, centLeft - offsetX * offsetWidth, centTop, centerWidth, centerHeight);

                float noiseCenterX = centLeft + offsetX * offsetWidth + centerWidth / 2;
                float noiseCenterY = centTop + centerHeight / 2;
                int lineLengthMin = centerHeight / 6;
                int lineLengthMax = centerHeight / 4;
                for (int i = 0; i < 6; i++)
                {
                    double angle = Global.Rand.NextDouble() * 360;
                    PointF drawPoint = Function.GetOffsetPoint(noiseCenterX, noiseCenterY, angle, centerWidth / 2);
                    int lineLength = Global.Rand.Next(lineLengthMin, lineLengthMax);
                    g.DrawLine(_Pen, drawPoint.X, drawPoint.Y - lineLength, drawPoint.X, drawPoint.Y + lineLength + 1);
                }
            }
            Animation++;
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawSkillBait(Color, BindingSkill)
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
            BackPen(ref _Pen);
            base.OnColorChanged();
        }

        protected override void OnColorFixChanged()
        {
            BackBrush(ref _Brush);
            BackPen(ref _Pen);
            base.OnColorFixChanged();
        }

        protected override void OnDispose()
        {
            BackBrush(ref _Brush);
            BackPen(ref _Pen);
        }
    }
}
