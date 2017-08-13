using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 畫筆繪圖物件
    /// </summary>
    public class DrawIconSlow : IDraw
    {
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
        /// 動畫進度
        /// </summary>
        public int Animation { get; set; }

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
        /// <param name="width">畫筆寬度</param>
        public DrawIconSlow(Color color, float opacity = 1)
        {
            Color = color;
            Opacity = opacity;
            Animation = 0;
        }

        private GraphicsPath _TempPath;
        private Rectangle _TempRect;

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public void Draw(Graphics g, Rectangle rectangle)
        {
            if (_TempPath == null || rectangle != _TempRect)
            {
                if (_TempPath != null)
                {
                    _TempPath.Dispose();
                }
                _TempPath = new GraphicsPath();
                _TempRect = rectangle;

                _TempPath.AddRectangle(rectangle);
                int width = rectangle.Width;
                int height = rectangle.Height;
                int paddingX = (int)(width * 0.1F);
                int paddingY = (int)(height * 0.1F);
                Rectangle clockRect = new Rectangle(rectangle.Left + paddingX, rectangle.Top + paddingY, width - paddingX * 2, height - paddingY * 2);
                _TempPath.AddEllipse(clockRect);
            }

            Pen pen = GetPen();
            pen.Width = 2;

            if (Animation > 1440)
            {
                Animation %= 1440;
            }
            float h = Animation / 60F;
            int m = Animation % 60;

            g.DrawPath(pen, _TempPath);

            PointF point1 = new PointF(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
            float directionH = (h * 15) - 180;
            float lengthH = (rectangle.Width + rectangle.Height) / 4 * 0.5F;
            float moveHX = (float)Math.Cos(directionH / 180 * Math.PI) * lengthH;
            float moveHY = (float)Math.Sin(directionH / 180 * Math.PI) * lengthH;
            PointF pointH = new PointF(point1.X + moveHX, point1.Y + moveHY);
            g.DrawLine(pen, point1, pointH);

            float directionM = (m * 6) - 180;
            float lengthM = (rectangle.Width + rectangle.Height) / 4 * 0.7F;
            float moveMX = (float)Math.Cos(directionM / 180 * Math.PI) * lengthM;
            float moveMY = (float)Math.Sin(directionM / 180 * Math.PI) * lengthM;
            PointF pointM = new PointF(point1.X + moveMX, point1.Y + moveMY);
            g.DrawLine(pen, point1, pointM);
            Animation += 1;
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public IDraw Copy()
        {
            return new DrawIconSlow(Color, Opacity);
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

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    BackPen();
                    if (_TempPath != null)
                    {
                        _TempPath.Dispose();
                    }
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
