﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 多邊型繪圖物件
    /// </summary>
    public class DrawPolygon : DrawBase
    {
        private SolidBrush _Brush;
        private Pen _PenBorder;

        /// <summary>
        /// 框線粗細
        /// </summary>
        public int BorderWidth { get; set; }

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
        /// 多邊形邊數,2為直線
        /// </summary>
        public int NumberOfSides { get; set; }

        /// <summary>
        /// 旋轉角度
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// 每秒旋轉角度
        /// </summary>
        public float RotatingPerSec { get; set; }

        /// <summary>
        /// 新增畫筆繪圖物件
        /// </summary>
        /// <param name="color">框架顏色</param>
        /// <param name="borderColor">填滿顏色</param>
        /// <param name="numberOfSides">多邊形邊數,2為直線</param>
        /// <param name="borderWidth">框線粗細</param>
        /// <param name="angle">旋轉角度</param>
        /// <param name="rotatingPerSec">每秒旋轉角度</param>
        public DrawPolygon(Color color, Color borderColor, int numberOfSides, int borderWidth, float angle, float rotatingPerSec)
        {
            Color = color;
            BorderColor = borderColor;
            BorderWidth = borderWidth;
            NumberOfSides = numberOfSides;
            Angle = angle;
            RotatingPerSec = rotatingPerSec;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public override void Draw(Graphics g, Rectangle rectangle)
        {
            if (BorderWidth < 1 || NumberOfSides < 2) return;

            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            GetPen(ref _PenBorder, BorderColor, Opacity, RFix, GFix, BFix);
            _PenBorder.Width = BorderWidth;

            int helfWidth = drawRectangle.Width / 2;
            int helfHeight = drawRectangle.Width / 2;
            int midX = drawRectangle.Left + helfWidth;
            int midY = drawRectangle.Top + helfHeight;

            Point[] pots = new Point[NumberOfSides];
            float partAngle = 360F / NumberOfSides;
            for (int i = 0; i < NumberOfSides; i++)
            {
                float angle = 180 - i * partAngle + Angle;
                int x = (int)(Math.Sin(angle / 180F * Math.PI) * helfWidth);
                int y = (int)(Math.Cos(angle / 180F * Math.PI) * helfHeight);
                pots[i] = new Point(midX + x, midY + y);
            }

            if (NumberOfSides > 2)
            {
                GetBrush(ref _Brush, Color, Opacity, RFix, GFix, BFix);
                g.FillPolygon(_Brush, pots);
                g.DrawPolygon(_PenBorder, pots);
            }
            else
            {
                g.DrawLine(_PenBorder, pots[0], pots[1]);
            }

            if (Scene != null)
            {
                Angle += RotatingPerSec / Scene.SceneRoundPerSec;
            }
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawPolygon(Color, BorderColor, NumberOfSides, BorderWidth, Angle, RotatingPerSec)
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

        protected override void OnColorFixChanged()
        {
            BackBrush(ref _Brush);
            BackPen(ref _PenBorder);
            base.OnColorFixChanged();
        }

        protected override void OnDispose()
        {
            BackBrush(ref _Brush);
            BackPen(ref _PenBorder);
        }
    }
}