using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 繪製工具池
    /// </summary>
    class DrawPool
    {
        private static Dictionary<Color, DrawPoolBrush> _BrushPool = new Dictionary<Color, DrawPoolBrush>();
        private static Dictionary<Color, DrawPoolPen> _PenPool = new Dictionary<Color, DrawPoolPen>();

        /// <summary>
        /// 使用的筆刷數量
        /// </summary>
        public static int BrushCount
        {
            get { return _BrushPool.Count; }
        }

        /// <summary>
        /// 使用的畫筆數量
        /// </summary>
        public static int PenCount
        {
            get { return _PenPool.Count; }
        }

        /// <summary>
        /// 取得特定顏色的筆刷
        /// </summary>
        /// <param name="color">筆刷顏色</param>
        /// <returns>筆刷</returns>
        public static SolidBrush GetBrush(Color color)
        {
            DrawPoolBrush drawPoolBrush;
            if (_BrushPool.TryGetValue(color, out drawPoolBrush))
            {
                drawPoolBrush.UseCount++;
                return drawPoolBrush.Brush;
            }
            else
            {
                drawPoolBrush = new DrawPoolBrush(new SolidBrush(color), 1);
                _BrushPool.Add(color, drawPoolBrush);
                return drawPoolBrush.Brush;
            }
        }

        /// <summary>
        /// 取得特定顏色的畫筆
        /// </summary>
        /// <param name="color">畫筆顏色</param>
        /// <returns>畫筆</returns>
        public static Pen GetPen(Color color)
        {
            DrawPoolPen drawPoolPen;
            if (_PenPool.TryGetValue(color, out drawPoolPen))
            {
                drawPoolPen.UseCount++;
                return drawPoolPen.Pen;
            }
            else
            {
                drawPoolPen = new DrawPoolPen(new Pen(color), 1);
                _PenPool.Add(color, drawPoolPen);
                return drawPoolPen.Pen;
            }
        }

        /// <summary>
        /// 返還筆刷
        /// </summary>
        /// <param name="brush">筆刷</param>
        public static void BackBrush(SolidBrush brush)
        {
            DrawPoolBrush drawPoolBrush;
            if (_BrushPool.TryGetValue(brush.Color, out drawPoolBrush))
            {
                drawPoolBrush.UseCount--;
                if (drawPoolBrush.UseCount == 0)
                {
                    _BrushPool.Remove(brush.Color);
                    drawPoolBrush.Dispose();
                }
            }
        }

        /// <summary>
        /// 返還畫筆
        /// </summary>
        /// <param name="pen">畫筆</param>
        public static void BackPen(Pen pen)
        {
            DrawPoolPen drawPoolPen;
            if (_PenPool.TryGetValue(pen.Color, out drawPoolPen))
            {
                drawPoolPen.UseCount--;
                if (drawPoolPen.UseCount == 0)
                {
                    _PenPool.Remove(pen.Color);
                    drawPoolPen.Dispose();
                }
            }
        }

        /// <summary>
        /// 工具池筆刷物件
        /// </summary>
        private class DrawPoolBrush : IDisposable
        {
            /// <summary>
            /// 筆刷
            /// </summary>
            public SolidBrush Brush { get; private set; }

            /// <summary>
            /// 正在使用的數量計數
            /// </summary>
            public int UseCount { get; set; }

            /// <summary>
            /// 新增工具池筆刷物件
            /// </summary>
            /// <param name="brush">筆刷</param>
            /// <param name="useCount">使用的數量計數</param>
            public DrawPoolBrush(SolidBrush brush, int useCount)
            {
                Brush = brush;
                UseCount = useCount;
            }

            #region IDisposable Support
            private bool disposedValue = false; // 偵測多餘的呼叫
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        Brush.Dispose();
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

        /// <summary>
        /// 工具池畫筆物件
        /// </summary>
        private class DrawPoolPen : IDisposable
        {
            /// <summary>
            /// 畫筆
            /// </summary>
            public Pen Pen { get; private set; }
            
            /// <summary>
            /// 正在使用的數量計數
            /// </summary>
            public int UseCount { get; set; }

            /// <summary>
            /// 新增工具池畫筆物件
            /// </summary>
            /// <param name="pen">畫筆</param>
            /// <param name="useCount">使用的數量計數</param>
            public DrawPoolPen(Pen pen, int useCount)
            {
                Pen = pen;
                UseCount = useCount;
            }

            #region IDisposable Support
            private bool disposedValue = false; // 偵測多餘的呼叫
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        Pen.Dispose();
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
}
