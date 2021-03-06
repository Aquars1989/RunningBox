﻿using System;
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
        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Colors.GetColor("Main"); }
        }

        /// <summary>
        /// 繪製圖形
        /// </summary>
        public ShapeType DrawShape { get; set; }

        /// <summary>
        /// 使用繪圖工具管理物件新增筆刷繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="drawShape">繪製圖形</param>
        public DrawBrush(DrawColors drawColor, ShapeType drawShape)
            : base(drawColor)
        {
            DrawShape = drawShape;
        }

        /// <summary>
        /// 新增筆刷繪圖物件
        /// </summary>
        /// <param name="backColor">繪製顏色</param>
        /// <param name="drawShape">繪製圖形</param>
        public DrawBrush(Color backColor, ShapeType drawShape)
        {
            Colors.SetColor("Main", backColor);
            DrawShape = drawShape;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            Brush brushBack = Colors.GetBrush("Main");
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
                        g.FillRectangle(brushBack, drawRectangle);
                        g.Transform.Dispose();
                        g.Transform = oldTransform;
                    }
                    else
                    {
                        g.FillRectangle(brushBack, drawRectangle);
                    }
                    break;
                case RunningBox.ShapeType.Ellipse:
                    g.FillEllipse(brushBack, drawRectangle);
                    break;
            }
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件,未綁定物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawBrush(Colors.Copy(), DrawShape)
            {
                Scale = this.Scale,
                Angle = this.Angle,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled
            };
        }
    }
}
