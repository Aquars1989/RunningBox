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
    public class DrawPen : DrawBase
    {
        private Pen _Pen;

        /// <summary>
        /// 畫筆寬度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 繪製圖形
        /// </summary>
        public ShapeType DrawShape { get; set; }

        /// <summary>
        /// 新增畫筆繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="drawShape">繪製圖形</param>
        /// <param name="width">畫筆寬度</param>
        public DrawPen(Color color, ShapeType drawShape, int width)
        {
            Color = color;
            Width = width;
            DrawShape = drawShape;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public override void Draw(Graphics g, Rectangle rectangle)
        {
            if (Width < 1) return;

            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            GetPen(ref _Pen, Color, Opacity, RFix, GFix, BFix);
            _Pen.Width = Width;
            switch (DrawShape)
            {
                case RunningBox.ShapeType.Rectangle:
                    g.DrawRectangle(_Pen, drawRectangle);
                    break;
                case RunningBox.ShapeType.Ellipse:
                    g.DrawEllipse(_Pen, drawRectangle);
                    break;
            }
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawPen(Color, DrawShape, Width)
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
