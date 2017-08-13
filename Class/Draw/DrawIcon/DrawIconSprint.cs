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
    public class DrawIconSprint : IDraw
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
                BackBrush();
            }
        }

        /// <summary>
        /// 新增畫筆繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="width">畫筆寬度</param>
        public DrawIconSprint(Color color, float opacity = 1)
        {
            Color = color;
            Opacity = opacity;
            Animation = 0;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public void Draw(Graphics g, Rectangle rectangle)
        {
           // using (Pen pen = new Pen(Color.Black, 2))
            {
                if (Animation > 32)
                {
                    Animation %= 32;
                }

                int ani = Animation / 2 % 4;

                g.DrawRectangle(Pens.Black, rectangle);

                float drawX = rectangle.Left + (rectangle.Width * 0.1F), drawY = rectangle.Top + (rectangle.Height * 0.1F);
                float size = rectangle.Width * 0.3F;
                g.FillEllipse(Brushes.Black, drawX, drawY, size, size);


                do
                {
                    size -= ani * rectangle.Width * 0.3F / 16F;
                    drawX += ani * rectangle.Width * 0.7F / 16F;
                    drawY += ani * rectangle.Width * 0.7F / 16F;
                    if (size > 0)
                    {
                        g.FillEllipse(Brushes.Black, drawX, drawY, size, size);
                    }
                    ani = 4;
                } while (size > 0);
                Animation++;
            }




        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public IDraw Copy()
        {
            return new DrawIconSprint(Color, Opacity);
        }

        /// <summary>
        /// 取得畫筆物件
        /// </summary>
        /// <returns>畫筆物件</returns>
        public Brush GetBrush()
        {
            if (_Brush == null)
            {
                Color brushColor = Color.FromArgb((int)(Color.A * Opacity), Color.R, Color.G, Color.B);
                _Brush = DrawPool.GetBrush(brushColor);
            }
            return _Brush;
        }

        /// <summary>
        /// 返還畫筆物件
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
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
        }
        #endregion
    }
}
