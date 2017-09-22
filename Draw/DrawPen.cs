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
        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Colors.GetColor("Border"); }
        }

        /// <summary>
        /// 畫筆寬度
        /// </summary>
        public int BorderWidth { get; set; }

        /// <summary>
        /// 繪製圖形
        /// </summary>
        public ShapeType DrawShape { get; set; }

        /// <summary>
        /// 使用繪圖工具管理物件新增畫筆繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="drawShape">繪製圖形</param>
        /// <param name="borderWidth">畫筆寬度</param>
        public DrawPen(DrawColors drawColor, ShapeType drawShape, int borderWidth)
            : base(drawColor)
        {
            DrawShape = drawShape;
            BorderWidth = borderWidth;
        }

        /// <summary>
        /// 新增畫筆繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="drawShape">繪製圖形</param>
        /// <param name="borderWidth">畫筆寬度</param>
        public DrawPen(Color color, ShapeType drawShape, int borderWidth)
        {
            Colors.SetColor("Border", color);
            DrawShape = drawShape;
            BorderWidth = borderWidth;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            if (BorderWidth < 1) return;

            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            Pen penBorder = Colors.GetPen("Border");
            penBorder.Width = BorderWidth;
            switch (DrawShape)
            {
                case RunningBox.ShapeType.Rectangle:
                    if (Angle != 0)
                    {
                        float baseX = drawRectangle.Left + drawRectangle.Width / 2;
                        float baseY = drawRectangle.Top + drawRectangle.Height / 2;
                        var oldTransform = g.Transform;
                        g.TranslateTransform(baseX, baseY);
                        g.RotateTransform(Angle);
                        g.TranslateTransform(-baseX, -baseY);
                        g.DrawRectangle(penBorder, drawRectangle);
                        g.Transform.Dispose();
                        g.Transform = oldTransform;
                    }
                    else
                    {
                        g.DrawRectangle(penBorder, drawRectangle);
                    }
                    break;
                case RunningBox.ShapeType.Ellipse:
                    g.DrawEllipse(penBorder, drawRectangle);
                    break;
            }
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件,未綁定物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawPen(Colors.Copy(), DrawShape, BorderWidth)
            {
                Scale = this.Scale,
                Angle = this.Angle,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled
            };
        }
    }
}
