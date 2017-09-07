using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 文字繪圖物件
    /// </summary>
    public class DrawUIString : DrawBase
    {
        private GraphicsPath _BackFrame;
        private Rectangle _BackFrameRectangle;
        private SolidBrush _Brush;
        private SolidBrush _BrushBack;
        private Pen _PenBorder;

        private Color _BackColor;
        /// <summary>
        /// 背景顏色
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

        private Color _BorderColor;
        /// <summary>
        /// 框線顏色
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

        /// <summary>
        /// 框線粗細
        /// </summary>
        public int BorderWidtrh { get; set; }

        /// <summary>
        /// 繪製文字
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 繪製字型
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// 繪製設定
        /// </summary>
        public StringFormat DrawFormat { get; set; }

        /// <summary>
        /// 內部間距
        /// </summary>
        public Padding Padding { get; set; }

        /// <summary>
        /// 新增文字繪圖物件
        /// </summary>
        /// <param name="color">文字顏色</param>
        /// <param name="backColor">背景色</param>
        /// <param name="borderColor">框線顏色</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="text">文字</param>
        /// <param name="font">字型</param>
        /// <param name="drawFormat">繪製設定</param>
        public DrawUIString(Color color, Color backColor, Color borderColor, int borderWidtrh, string text, Font font, StringFormat drawFormat)
        {
            Text = text;
            Font = font;
            Color = color;
            BackColor = backColor;
            BorderColor = borderColor;
            BorderWidtrh = borderWidtrh;
            DrawFormat = drawFormat;
            Padding = new Padding(5, 5, 5, 5);
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public override void Draw(Graphics g, Rectangle rectangle)
        {
            Rectangle inside = new Rectangle(rectangle.Left + Padding.Left, rectangle.Top + Padding.Top, rectangle.Width - Padding.Horizontal, rectangle.Height - Padding.Vertical);
            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            Rectangle textRectangle = GetScaleRectangle(inside);
            GetBrush(ref _Brush, Color, Opacity, RFix, GFix, BFix);

            GraphicsPath backFrame = GetBackFrame(drawRectangle);
            if (BackColor != Color.Empty)
            {
                GetBrush(ref _BrushBack, BackColor, Opacity, RFix, GFix, BFix);
                g.FillPath(_BrushBack, backFrame);
            }

            if (BorderColor != Color.Empty && BorderWidtrh > 0)
            {
                GetPen(ref _PenBorder, BorderColor, Opacity, RFix, GFix, BFix);
                _PenBorder.Width = BorderWidtrh;
                g.DrawPath(_PenBorder, backFrame);
            }
            g.DrawString(Text, Font, _Brush, textRectangle, DrawFormat);
        }

        /// <summary>
        /// 產生圓角區域
        /// </summary>
        private GraphicsPath GetBackFrame(Rectangle rectangle)
        {
            if (_BackFrameRectangle != rectangle && _BackFrame != null)
            {
                _BackFrame.Dispose();
                _BackFrame = null;
            }

            if (_BackFrame == null)
            {
                _BackFrameRectangle = rectangle;
                _BackFrame = Function.GetRadiusFrame(rectangle, 8);
            }
            return _BackFrame;
        }

        public override DrawBase Copy()
        {
            return new DrawUIString(Color, BackColor, BorderColor, 2, Text, Font, DrawFormat)
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
            if (_BackFrame != null)
            {
                _BackFrame.Dispose();
                _BackFrame = null;
            }
            BackBrush(ref _Brush);
            BackBrush(ref _BrushBack);
            BackPen(ref _PenBorder);
        }
    }
}
