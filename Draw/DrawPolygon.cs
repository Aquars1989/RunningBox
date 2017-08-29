using System;
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
    public class DrawPolygon : IDraw
    {
        /// <summary>
        /// 畫筆寬度
        /// </summary>
        public int Width { get; set; }

        private Pen _Pen;
        private Color _Color;
        /// <summary>
        /// 框架顏色
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

        private SolidBrush _Brush;
        private Color _Color2;
        /// <summary>
        /// 填滿顏色
        /// </summary>
        public Color Color2
        {
            get { return _Color2; }
            set
            {
                if (_Color2 == value) return;
                _Color2 = value;
                BackBrush();
            }
        }

        /// <summary>
        /// 多邊形邊數
        /// </summary>
        public int NumberOfSides { get; set; }

        /// <summary>
        /// 旋轉角度
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// 每次旋轉角度
        /// </summary>
        public float Rotating { get; set; }

        private float _Opacity;
        /// <summary>
        /// 不透明度(0~1)
        /// </summary>
        public float Opacity
        {
            get { return _Opacity; }
            set
            {
                if (_Opacity == value) return;
                _Opacity = value;
                BackPen();
                BackBrush();
            }
        }

        private float _RFix;
        /// <summary>
        /// 紅色值調整(-1~1)
        /// </summary>
        public float RFix
        {
            get { return _RFix; }
            set
            {
                if (_RFix == value) return;
                _RFix = value;
                BackPen();
                BackBrush();
            }
        }

        private float _GFix;
        /// <summary>
        /// 綠色值調整(-1~1)
        /// </summary>
        public float GFix
        {
            get { return _GFix; }
            set
            {
                if (_GFix == value) return;
                _GFix = value;
                BackPen();
                BackBrush();
            }
        }

        private float _BFix;
        /// <summary>
        /// 藍色值調整(-1~1)
        /// </summary>
        public float BFix
        {
            get { return _BFix; }
            set
            {
                if (_BFix == value) return;
                _BFix = value;
                BackPen();
                BackBrush();
            }
        }

        /// <summary>
        /// 縮放比例調整
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// 新增畫筆繪圖物件
        /// </summary>
        /// <param name="color">框架顏色</param>
        /// <param name="color">填滿顏色</param>
        /// <param name="numberOfSides">多邊形邊數</param>
        /// <param name="width">畫筆寬度</param>
        /// <param name="angle">旋轉角度</param>
        /// <param name="rotating">每次旋轉角度</param>
        public DrawPolygon(Color color, Color color2, int numberOfSides, int width, float angle, float rotating)
        {
            Color = color;
            Color2 = color2;
            Width = width;
            NumberOfSides = numberOfSides;
            _Opacity = 1;
            Scale = 1;
            Angle = angle;
            Rotating = rotating;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public void Draw(Graphics g, Rectangle rectangle)
        {
            if (Width < 1 || NumberOfSides < 3) return;

            Rectangle drawRectangle = rectangle;
            if (Scale != 1)
            {
                int scaleX = (int)(((drawRectangle.Width * Scale) - drawRectangle.Width) / 2);
                int scaleY = (int)(((drawRectangle.Height * Scale) - drawRectangle.Height) / 2);
                drawRectangle = new Rectangle(rectangle.Left - scaleX, rectangle.Top - scaleY, rectangle.Width + scaleX * 2, rectangle.Height + scaleY * 2);
            }

            Pen pen = GetPen();
            Brush brush = GetBrush();
            pen.Width = Width;

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
            g.FillPolygon(brush, pots);
            g.DrawPolygon(pen, pots);
            Angle += Rotating;
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public IDraw Copy()
        {
            return new DrawPolygon(Color, Color2, NumberOfSides, Width, Angle, Rotating) { Opacity = this.Opacity, RFix = this.RFix, GFix = this.GFix, BFix = this.BFix, Scale = this.Scale };
        }

        /// <summary>
        /// 取得畫筆物件
        /// </summary>
        /// <returns>畫筆物件</returns>
        public Pen GetPen()
        {
            if (_Pen == null)
            {
                Color penColor = ColorFix.GetColor(Color, Opacity, RFix, GFix, BFix);
                _Pen = DrawPool.GetPen(penColor);
            }
            return _Pen;
        }

        /// <summary>
        /// 取得筆刷物件
        /// </summary>
        /// <returns>筆刷物件</returns>
        public SolidBrush GetBrush()
        {
            if (_Brush == null)
            {
                Color brushColor = ColorFix.GetColor(Color2, Opacity, RFix, GFix, BFix);
                _Brush = DrawPool.GetBrush(brushColor);
            }
            return _Brush;
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

        /// <summary>
        /// 返還筆刷物件
        /// </summary>
        public void BackBrush()
        {
            if (_Brush != null)
            {
                DrawPool.BackBrush(_Brush);
                _Brush = null;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    BackPen();
                    BackBrush();
                }
                disposedValue = true;
            }
        }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
        }
        #endregion
    }
}
