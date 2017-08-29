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
    public class DrawBrush : IDraw
    {
        private SolidBrush _Brush;
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
                BackBrush();
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
                BackBrush();
            }
        }

        /// <summary>
        /// 縮放比例調整
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// 新增筆刷繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="drawShape">繪製圖形</param>
        public DrawBrush(Color color, ShapeType drawShape)
        {
            Color = color;
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
            Rectangle drawRectangle = rectangle;
            if (Scale != 1)
            {
                int scaleX = (int)(((drawRectangle.Width * Scale) - drawRectangle.Width) / 2);
                int scaleY = (int)(((drawRectangle.Height * Scale) - drawRectangle.Height) / 2);
                drawRectangle = new Rectangle(rectangle.Left - scaleX, rectangle.Top - scaleY, rectangle.Width + scaleX * 2, rectangle.Height + scaleY * 2);
            }

            Brush brush = GetBrush();
            switch (DrawShape)
            {
                case RunningBox.ShapeType.Rectangle:
                    g.FillRectangle(brush, drawRectangle);
                    break;
                case RunningBox.ShapeType.Ellipse:
                    g.FillEllipse(brush, drawRectangle);
                    break;
            }
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public IDraw Copy()
        {
            return new DrawBrush(Color, DrawShape) { Opacity = this.Opacity, RFix = this.RFix, GFix = this.GFix, BFix = this.BFix, Scale = this.Scale };
        }

        /// <summary>
        /// 取得筆刷物件
        /// </summary>
        /// <returns>筆刷物件</returns>
        public SolidBrush GetBrush()
        {
            if (_Brush == null)
            {
                Color brushColor = ColorFix.GetColor(Color, Opacity, RFix, GFix, BFix);
                _Brush = DrawPool.GetBrush(brushColor);
            }
            return _Brush;
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    BackBrush();
                }
                disposedValue = true;
            }
        }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
