using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 筆刷繪圖物件
    /// </summary>
    public class DrawBrush : DrawBase
    {
        private SolidBrush _Brush;

        /// <summary>
        /// 繪製圖形
        /// </summary>
        public ShapeType DrawShape { get; set; }

        /// <summary>
        /// 新增筆刷繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="drawShape">繪製圖形</param>
        public DrawBrush(Color color, ShapeType drawShape)
        {
            Color = color;
            DrawShape = drawShape;
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
            switch (DrawShape)
            {
                case RunningBox.ShapeType.Rectangle:
                    g.FillRectangle(_Brush, drawRectangle);
                    break;
                case RunningBox.ShapeType.Ellipse:
                    g.FillEllipse(_Brush, drawRectangle);
                    break;
            }
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawBrush(Color, DrawShape)
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
