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
                BackBrush();
            }
        }

        /// <summary>
        /// 新增筆刷繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="opacity">透明度0-1</param>
        /// <param name="drawShape">繪製圖形</param>
        public DrawBrush(Color color, DrawShape drawShape, float opacity = 1)
        {
            Color = color;
            Opacity = opacity;
            DrawShape = drawShape;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public void Draw(Graphics g, Rectangle rectangle)
        {
            Brush brush = GetBrush();
            switch (DrawShape)
            {
                case RunningBox.DrawShape.Rectangle:
                    g.FillRectangle(brush, rectangle);
                    break;
                case RunningBox.DrawShape.Ellipse:
                    g.FillEllipse(brush, rectangle);
                    break;
            }
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public IDraw Copy()
        {
            return new DrawBrush(Color, DrawShape, Opacity);
        }

        /// <summary>
        /// 取得筆刷物件
        /// </summary>
        /// <returns>筆刷物件</returns>
        public SolidBrush GetBrush()
        {
            if (_Brush == null)
            {
                Color brushColor = Color.FromArgb((int)(Color.A * Opacity), Color.R, Color.G, Color.B);
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
