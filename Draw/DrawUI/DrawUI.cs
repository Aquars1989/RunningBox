using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// UI繪製基本物件
    /// </summary>
    public abstract class DrawUI : IDraw
    {
        private Pen _Pen;
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
                BackPenAndBrush();
            }
        }

        /// <summary>
        /// 不透明度,此處無用
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// 紅色值調整,此處無用
        /// </summary>
        public float RFix { get; set; }

        /// <summary>
        /// 綠色值調整,此處無用
        /// </summary>
        public float GFix { get; set; }

        /// <summary>
        /// 藍色值調整,此處無用
        /// </summary>
        public float BFix { get; set; }

        /// <summary>
        /// 縮放比例調整,此處無用
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public abstract void Draw(Graphics g, Rectangle rectangle);

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public abstract IDraw Copy();

        /// <summary>
        /// 取得畫筆物件
        /// </summary>
        /// <returns>畫筆物件</returns>
        public Pen GetPen()
        {
            if (_Pen == null)
            {
                _Pen = DrawPool.GetPen(Color);
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
                _Brush = DrawPool.GetBrush(Color);
            }
            return _Brush;
        }

        /// <summary>
        /// 返還畫筆物件
        /// </summary>
        public void BackPenAndBrush()
        {
            if (_Pen != null)
            {
                DrawPool.BackPen(_Pen);
                _Pen = null;
            }

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
                    BackPenAndBrush();
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
