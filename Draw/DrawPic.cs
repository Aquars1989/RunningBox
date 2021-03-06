﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 圖片繪圖物件
    /// </summary>
    public class DrawPic : DrawBase
    {
        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Colors.GetColor("Back"); }
        }

        /// <summary>
        /// 繪製圖片
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// 使用繪圖工具管理物件新增圖片繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="image">繪製圖片</param>
        /// <param name="angle">旋轉角度</param>
        public DrawPic(DrawColors drawColor, Image image, float angle)
            : base(drawColor)
        {
            Image = image;
            Angle = angle;
        }

        /// <summary>
        /// 新增圖片繪圖物件
        /// </summary>
        /// <param name="backColor">繪製顏色(供技能/特效使用)</param>
        /// <param name="image">繪製圖片</param>
        /// <param name="angle">旋轉角度</param>
        public DrawPic(Color backColor, Image image, float angle)
        {
            Colors.SetColor("Back", backColor);
            Image = image;
            Angle = angle;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            if (Angle != 0)
            {
                float baseX = drawRectangle.Left + drawRectangle.Width / 2;
                float baseY = drawRectangle.Top + drawRectangle.Height / 2;
                var oldTransform = g.Transform;
                g.TranslateTransform(baseX, baseY);
                g.RotateTransform(Angle);
                g.TranslateTransform(-baseX, -baseY);
                ColorFix.DrawImage(g, Image, drawRectangle, Colors.Opacity, Colors.RFix, Colors.GFix, Colors.BFix);
                g.Transform.Dispose();
                g.Transform = oldTransform;
            }
            else
            {
                ColorFix.DrawImage(g, Image, drawRectangle, Colors.Opacity, Colors.RFix, Colors.GFix, Colors.BFix);
            }
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件,未綁定物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawPic(Colors.Copy(), Image, Angle)
            {
                Scale = this.Scale,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled
            };
        }
    }
}
