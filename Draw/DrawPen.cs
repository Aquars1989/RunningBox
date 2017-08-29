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
        public ShapeType DrawShape { get; set; }

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
            }
        }

        /// <summary>
        /// 縮放比例調整
        /// </summary>
        public float Scale { get; set; }

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
            _Opacity = 1;
            Scale = 1;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public void Draw(Graphics g, Rectangle rectangle)
        {
            if (Width < 1) return;

            Rectangle drawRectangle = rectangle;
            if (Scale != 1)
            {
                int scaleX = (int)(((drawRectangle.Width * Scale) - drawRectangle.Width) / 2);
                int scaleY = (int)(((drawRectangle.Height * Scale) - drawRectangle.Height) / 2);
                drawRectangle = new Rectangle(rectangle.Left - scaleX, rectangle.Top - scaleY, rectangle.Width + scaleX * 2, rectangle.Height + scaleY * 2);
            }

            Pen pen = GetPen();
            pen.Width = Width;
            switch (DrawShape)
            {
                case RunningBox.ShapeType.Rectangle:
                    g.DrawRectangle(pen, drawRectangle);
                    break;
                case RunningBox.ShapeType.Ellipse:
                    g.DrawEllipse(pen, drawRectangle);
                    break;
            }
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public IDraw Copy()
        {
            return new DrawPen(Color, DrawShape, Width) { Opacity = this.Opacity, RFix = this.RFix, GFix = this.GFix, BFix = this.BFix, Scale = this.Scale };
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

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    BackPen();
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
