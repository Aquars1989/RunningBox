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
    public class DrawPen : IDraw
    {
        /// <summary>
        /// 畫筆寬度
        /// </summary>
        public int Width { get; set; }

        private Pen _Pen;
        private Color _Color;
        /// <summary>
        /// 繪製顏色
        /// </summary>
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color == value) return;
                _Color = value;
                BackPen();
            }
        }


        /// <summary>
        /// 繪製圖形
        /// </summary>
        public DrawShape DrawShape { get; set; }

        private float _Opacity;
        /// <summary>
        /// 不透明度0-1
        /// </summary>
        public float Opacity
        {
            get { return _Opacity; }
            set
            {
                if (_Opacity == value) return;
                _Opacity = value;
                if (_Opacity > 1) _Opacity = 1;
                else if (_Opacity < 0) _Opacity = 0;
                BackPen();
            }
        }

        /// <summary>
        /// 新增畫筆繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="opacity">透明度0-1</param>
        /// <param name="drawShape">繪製圖形</param>
        /// <param name="width">畫筆寬度</param>
        public DrawPen(Color color, DrawShape drawShape, int width, float opacity = 1)
        {
            Color = color;
            Opacity = opacity;
            Width = width;
            DrawShape = drawShape;
        }

        ~DrawPen()
        {
            BackPen();
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public void Draw(Graphics g, Rectangle rectangle)
        {
            if (Width < 1) return;

            Pen pen = GetPen();
            pen.Width = Width;
            switch (DrawShape)
            {
                case RunningBox.DrawShape.Rectangle:
                    g.DrawRectangle(pen, rectangle);
                    break;
                case RunningBox.DrawShape.Ellipse:
                    g.DrawEllipse(pen, rectangle);
                    break;
            }
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public IDraw Copy()
        {
            return new DrawPen(Color, DrawShape, Width, Opacity);
        }

        /// <summary>
        /// 取得畫筆物件
        /// </summary>
        /// <returns>畫筆物件</returns>
        public Pen GetPen()
        {
            if (_Pen == null)
            {
                Color penColor = Color.FromArgb((int)(Color.A * Opacity), Color.R, Color.G, Color.B);
                _Pen = DrawPool.GetPen(penColor);
            }
            return _Pen;
        }

        /// <summary>
        /// 返還畫筆物件
        /// </summary>
        public void BackPen()
        {
            if (_Pen != null)
            {
                DrawPool.BackPen(_Pen);
                _Pen = null;
            }
        }
    }
}
