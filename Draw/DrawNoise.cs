using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 畫筆繪圖物件
    /// </summary>
    public class DrawNoise : DrawBase
    {
        private Pen _Pen;
        private Pen _Pen2;

        /// <summary>
        /// 最大畫筆寬度
        /// </summary>
        public int MaxWidth { get; set; }

        private Color _Color2;
        /// <summary>
        /// 外框顏色
        /// </summary>
        public Color Color2
        {
            get { return _Color2; }
            set
            {
                if (_Color2 == value) return;
                _Color2 = value;
                OnColor2Changed();
            }
        }

        /// <summary>
        /// 新增畫筆繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="color2">繪製顏色2</param>
        /// <param name="maxWidth">畫筆寬度</param>
        public DrawNoise(Color color, Color color2, int maxWidth)
        {
            Color = color;
            Color2 = color2;
            MaxWidth = maxWidth;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            if (MaxWidth < 1) return;

            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            GetPen(ref _Pen, Color, Opacity, RFix, GFix, BFix);
            GetPen(ref _Pen2, Color2, Opacity, RFix, GFix, BFix);
            _Pen.Width = 1;
            _Pen2.Width = 1;
            int lineHeight = drawRectangle.Height / 4;
            for (int i = 0; i <= drawRectangle.Width; )
            {
                int drawX = drawRectangle.Left + i;
                int drawY1 = drawRectangle.Top + Global.Rand.Next(0, drawRectangle.Height - lineHeight);
                int drawY2 = drawY1 + lineHeight;

                Pen drawPen = (Global.Rand.Next(2) == 1) ? _Pen : _Pen2;
                if (MaxWidth > 1)
                    drawPen.Width = Global.Rand.Next(1, MaxWidth);
                g.DrawLine(Global.Rand.Next(2) == 1 ? _Pen : _Pen2, drawX, drawY1, drawX, drawY2);

                i += (int)drawPen.Width + Global.Rand.Next(0, 2);
            }
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawNoise(Color, Color2, MaxWidth)
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

        protected void OnColor2Changed()
        {
            BackPen(ref _Pen2);
            base.OnColorChanged();
        }

        protected override void OnColorFixChanged()
        {
            BackPen(ref _Pen);
            BackPen(ref _Pen2);
            base.OnColorFixChanged();
        }

        protected override void OnDispose()
        {
            BackPen(ref _Pen);
            BackPen(ref _Pen2);
        }
    }
}
