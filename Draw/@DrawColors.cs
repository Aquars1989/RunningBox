using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 管理繪製顏色
    /// </summary>
    public class DrawColors : IDisposable
    {
        private static Pen _PenNull = new Pen(Color.Empty);
        private static SolidBrush _BrushNull = new SolidBrush(Color.Empty);

        private Dictionary<string, DrawColor> _DrawColor = new Dictionary<string, DrawColor>();

        /// <summary>
        /// 發生於顏色改變時
        /// </summary>
        public event DrawColorsEnentHandle ColorChanged;

        /// <summary>
        /// 發生於顏色調整改變時
        /// </summary>
        public event EventHandler ColorFixChanged;

        /// <summary>
        /// 發生於顏色改變時
        /// </summary>
        protected virtual void OnColorChanged(string colorID)
        {
            if (ColorChanged != null)
            {
                ColorChanged(this, colorID);
            }
        }

        /// <summary>
        /// 發生於顏色調整改變時
        /// </summary>
        protected virtual void OnColorFixChanged()
        {
            BackAllPenAndBrush();
            if (ColorFixChanged != null)
            {
                ColorFixChanged(this, new EventArgs());
            }
        }

        private float _Opacity = 1;
        /// <summary>
        /// 不透明度(0~1)
        /// </summary>
        public float Opacity
        {
            get { return _Opacity; }
            set
            {
                if (value > 1) value = 1;
                else if (value < 0) value = 0;

                if (_Opacity == value) return;
                _Opacity = value;
                OnColorFixChanged();
            }
        }

        private float _RFix = 0;
        /// <summary>
        /// 紅色值調整
        /// </summary>
        public float RFix
        {
            get { return _RFix; }
            set
            {
                if (_RFix == value) return;
                _RFix = value;
                OnColorFixChanged();
            }
        }

        private float _GFix = 0;
        /// <summary>
        /// 綠色值調整
        /// </summary>
        public float GFix
        {
            get { return _GFix; }
            set
            {
                if (_GFix == value) return;
                _GFix = value;
                OnColorFixChanged();
            }
        }

        private float _BFix = 0;
        /// <summary>
        /// 藍色值調整
        /// </summary>
        public float BFix
        {
            get { return _BFix; }
            set
            {
                if (_BFix == value) return;
                _BFix = value;
                OnColorFixChanged();
            }
        }

        /// <summary>
        /// 取得指定索引的色彩
        /// </summary>
        /// <param name="colorID">色彩索引</param>
        /// <returns>取得色彩</returns>
        public Color GetColor(string colorID)
        {
            DrawColor result;
            if (_DrawColor.TryGetValue(colorID, out result))
            {
                return result.Color;
            }
            else
            {
                return Color.Empty;
            }
        }

        /// <summary>
        /// 取得指定索引的色彩畫筆
        /// </summary>
        /// <param name="colorID">色彩索引</param>
        /// <returns>取得畫筆</returns>
        public Pen GetPen(string colorID)
        {
            DrawColor drawColor;
            if (_DrawColor.TryGetValue(colorID, out drawColor))
            {
                if (drawColor.Pen == null)
                {
                    Color penColor = ColorFix.GetColor(drawColor.Color, Opacity, RFix, GFix, BFix);
                    drawColor.Pen = DrawPool.GetPen(penColor);
                }
                return drawColor.Pen;
            }
            return _PenNull;
        }

        /// <summary>
        /// 取得指定索引的色彩畫筆並指定線條粗細
        /// </summary>
        /// <param name="colorID">色彩索引</param>
        /// <param name="width">線條粗細</param>
        /// <returns>取得畫筆</returns>
        public Pen GetPen(string colorID, float width)
        {
            Pen result = GetPen(colorID);
            if (result != _PenNull)
            {
                result.Width = width;
            }
            return result;
        }

        /// <summary>
        /// 取得指定索引的色彩筆刷
        /// </summary>
        /// <param name="colorID">色彩索引</param>
        /// <returns>取得筆刷</returns>
        public SolidBrush GetBrush(string colorID)
        {
            DrawColor drawColor;
            if (_DrawColor.TryGetValue(colorID, out drawColor))
            {
                if (drawColor.Brush == null)
                {
                    Color brushColor = ColorFix.GetColor(drawColor.Color, Opacity, RFix, GFix, BFix);
                    drawColor.Brush = DrawPool.GetBrush(brushColor);
                }
                return drawColor.Brush;
            }
            return _BrushNull;
        }

        /// <summary>
        /// 設定指定索引的色彩
        /// </summary>
        /// <param name="colorID">色彩索引</param>
        /// <param name="color">設定色彩</param>
        public void SetColor(string colorID, Color color)
        {
            DrawColor drawColor;
            if (_DrawColor.TryGetValue(colorID, out drawColor))
            {
                Color oldColor = _DrawColor[colorID].Color;
                if (_DrawColor[colorID].Color == color) return;

                _DrawColor[colorID].Color = color;
                _DrawColor[colorID].BackPenAndBrush();
            }
            else
            {
                _DrawColor.Add(colorID, new DrawColor(color));
            }
            OnColorChanged(colorID);
        }

        /// <summary>
        /// 返還所有畫筆和筆刷
        /// </summary>
        public void BackAllPenAndBrush()
        {
            foreach (var drawColor in _DrawColor.Values)
            {
                drawColor.BackPenAndBrush();
            }
        }

        /// <summary>
        /// 複製繪製顏色管理物件
        /// </summary>
        /// <returns></returns>
        public DrawColors Copy()
        {
            DrawColors result = new DrawColors()
            {
                Opacity = this.Opacity,
                RFix = this.RFix,
                GFix = this.GFix,
                BFix = this.BFix
            };

            foreach (var drawColor in _DrawColor)
            {
                result.SetColor(drawColor.Key, drawColor.Value.Color);
            }
            return result;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    BackAllPenAndBrush();
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

        /// <summary>
        /// 記錄顏色和筆刷物件
        /// </summary>
        private class DrawColor
        {
            /// <summary>
            /// 顏色
            /// </summary>
            public Color Color { get; set; }

            /// <summary>
            /// 畫筆物件
            /// </summary>
            public Pen Pen { get; set; }

            /// <summary>
            /// 筆刷物件
            /// </summary>
            public SolidBrush Brush { get; set; }

            /// <summary>
            /// 新增記錄顏色和筆刷物件
            /// </summary>
            /// <param name="color">顏色</param>
            public DrawColor(Color color)
            {
                Color = color;
            }

            /// <summary>
            /// 返還畫筆和筆刷
            /// </summary>
            public void BackPenAndBrush()
            {
                if (Pen != null)
                {
                    DrawPool.BackPen(Pen);
                    Pen = null;
                }

                if (Brush != null)
                {
                    DrawPool.BackBrush(Brush);
                    Brush = null;
                }
            }
        }
    }
}
