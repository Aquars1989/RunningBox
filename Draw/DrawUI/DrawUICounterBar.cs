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
        private Pen _PenBorder;
        private SolidBrush _Brush;
        private SolidBrush _BrushBack;

        /// <summary>
        /// 綁定計數器
        /// </summary>
        public CounterObject BindingCounter { get; set; }

        /// <summary>
        /// 線條粗細
        /// </summary>
        public int BorderWidth { get; set; }

        private Color _BorderColor;
        /// <summary>
        /// 外框顏色
        /// </summary>
        public Color BorderColor
        {
            get { return _BorderColor; }
            set
            {
                if (_BorderColor == value) return;
                _BorderColor = value;
                OnBorderColorChanged();
            }
        }

        private Color _BackColor;
        /// <summary>
        /// 底色
        /// </summary>
        public Color BackColor
        {
            get { return _BackColor; }
            set
            {
                if (_BackColor == value) return;
                _BackColor = value;
                OnBackColorChanged();
            }
        }

        /// <summary>
        /// 是否反向顯示
        /// </summary>
        public bool Reverse { get; set; }

        /// <summary>
        /// 新增能量條繪圖物件
        /// </summary>
        /// <param name="color">條棒顏色</param>
        /// <param name="borderColor">外框顏色</param>
        /// <param name="backColor">底色</param>
        /// <param name="borderWidth">框線粗細</param>
        /// <param name="reverse">是否反向顯示</param>
        /// <param name="bindObject">綁定物件</param>
        public DrawUICounterBar(Color color, Color borderColor, Color backColor, int borderWidth, bool reverse, CounterObject bindingCounter = null)
        {
            Color = color;
            BackColor = backColor;
            BorderColor = borderColor;
            BorderWidth = borderWidth;
            Reverse = reverse;
            BindingCounter = bindingCounter;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public override void Draw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            GetBrush(ref _Brush, Color, Opacity, RFix, GFix, BFix);
            GetBrush(ref _BrushBack, BackColor, Opacity, RFix, GFix, BFix);
            GetPen(ref _PenBorder, BorderColor, Opacity, RFix, GFix, BFix);

            g.FillRectangle(_BrushBack, drawRectangle);
            g.DrawRectangle(_PenBorder, drawRectangle);

            if (BindingCounter != null && BindingCounter.Value > 0)
            {
                float ratio = Math.Min(BindingCounter.GetRatio(), 1);
                if (Reverse) ratio = 1 - ratio;
                int widthInside = (int)((drawRectangle.Width - BorderWidth * 2) * ratio + 0.5F);
                if (widthInside > 0)
                {
                    g.FillRectangle(_Brush, drawRectangle.Left + BorderWidth, drawRectangle.Top + BorderWidth, widthInside, drawRectangle.Height - BorderWidth * 2);
                }
            }
        }

        public override DrawBase Copy()
        {
            return new DrawUICounterBar(Color, BorderColor, BackColor, BorderWidth, Reverse, BindingCounter)
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

        protected void OnBorderColorChanged()
        {
            BackPen(ref _PenBorder);
            base.OnColorChanged();
        }

        protected void OnBackColorChanged()
        {
            BackBrush(ref _BrushBack);
            base.OnColorChanged();
        }

        protected override void OnColorFixChanged()
        {
            BackBrush(ref _Brush);
            BackBrush(ref _BrushBack);
            BackPen(ref _PenBorder);
            base.OnColorFixChanged();
        }

        protected override void OnDispose()
        {
            BackBrush(ref _Brush);
            BackBrush(ref _BrushBack);
            BackPen(ref _PenBorder);
        }
    }
}
