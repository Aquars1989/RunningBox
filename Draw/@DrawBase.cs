using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 基礎繪圖物件物件
    /// </summary>
    public abstract class DrawBase : IDisposable
    {
        #region ===== 事件 =====
        /// <summary>
        /// 發生於繪製前
        /// </summary>
        public event DrawObjectEnentHandle BeforeDraw;

        /// <summary>
        /// 發生於繪製後
        /// </summary>
        public event DrawObjectEnentHandle AfterDraw;

        /// <summary>
        /// 發生於場景物件改變時
        /// </summary>
        public event EventHandler SceneChanged;

        /// <summary>
        /// 發生於所有者改變時
        /// </summary>
        public event EventHandler OwnerChanged;

        /// <summary>
        /// 發生於顏色改變時
        /// </summary>
        public event EventHandler ColorChanged;

        /// <summary>
        /// 發生於顏色調整改變時
        /// </summary>
        public event EventHandler ColorFixChanged;

        /// <summary>
        /// 發生於繪製大小改變時
        /// </summary>
        public event EventHandler ScaleChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於場景物件改變時
        /// </summary>
        protected virtual void OnSceneChanged()
        {
            if (SceneChanged != null)
            {
                SceneChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於所有者改變時
        /// </summary>
        protected virtual void OnOwnerChanged()
        {
            if (OwnerChanged != null)
            {
                OwnerChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於顏色改變時
        /// </summary>
        protected virtual void OnColorChanged()
        {
            if (ColorChanged != null)
            {
                ColorChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於顏色調整改變時
        /// </summary>
        protected virtual void OnColorFixChanged()
        {
            if (ColorFixChanged != null)
            {
                ColorFixChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於繪製大小改變時
        /// </summary>
        protected virtual void OnScaleChanged()
        {
            if (ScaleChanged != null)
            {
                ScaleChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於繪製前
        /// </summary>
        protected virtual void OnBeforeDraw(Graphics g, Rectangle rectangle)
        {
            if (BeforeDraw != null)
            {
                BeforeDraw(this, g, rectangle);
            }
        }

        /// <summary>
        /// 發生於繪製後
        /// </summary>
        protected virtual void OnAfterDraw(Graphics g, Rectangle rectangle)
        {
            if (AfterDraw != null)
            {
                AfterDraw(this, g, rectangle);
            }
        }
        #endregion

        #region ===== 屬性 =====
        private SceneBase _Scene;
        /// <summary>
        /// 所在的場景物件
        /// </summary>
        public SceneBase Scene
        {
            get { return _Scene; }
            set
            {
                if (_Scene == value) return;
                _Scene = value;
                OnSceneChanged();
            }
        }

        private ObjectBase _Owner;
        /// <summary>
        /// 依附的活動物件
        /// </summary>
        public ObjectBase Owner
        {
            get { return _Owner; }
            set
            {
                if (_Owner == value) return;
                _Owner = value;
                OnOwnerChanged();
            }
        }

        private Color _Color;
        /// <summary>
        /// 主要繪製顏色
        /// </summary>
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color == value) return;
                _Color = value;
                OnColorChanged();
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
        /// 紅色值調整(-1~1)
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
        /// 綠色值調整(-1~1)
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
        /// 藍色值調整(-1~1)
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

        private float _Scale = 1;
        /// <summary>
        /// 縮放比例調整
        /// </summary>
        public float Scale
        {
            get { return _Scale; }
            set
            {
                if (value < 0) value = 0;
                if (_Scale == value) return;
                _Scale = value;
                OnScaleChanged();
            }
        }
        #endregion

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public void Draw(Graphics g, Rectangle rectangle)
        {
            OnBeforeDraw(g, rectangle);
            OnDraw(g, rectangle);
            OnAfterDraw(g, rectangle);
        }

        #region ===== 方法 =====
        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public abstract DrawBase Copy();

        /// <summary>
        /// 取得縮放的繪製區域
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public Rectangle GetScaleRectangle(Rectangle rectangle)
        {
            if (Scale == 1) return rectangle;

            int scaleX = (int)(((rectangle.Width * Scale) - rectangle.Width) / 2);
            int scaleY = (int)(((rectangle.Height * Scale) - rectangle.Height) / 2);
            return new Rectangle(rectangle.Left - scaleX, rectangle.Top - scaleY, rectangle.Width + scaleX * 2, rectangle.Height + scaleY * 2);
        }

        /// <summary>
        /// 取得畫筆物件
        /// </summary>
        /// <returns>畫筆物件</returns>
        public void GetPen(ref Pen pen, Color color, float opacity, float rfix, float gfix, float bfix)
        {
            if (pen == null)
            {
                Color penColor = ColorFix.GetColor(color, Opacity, RFix, GFix, BFix);
                pen = DrawPool.GetPen(penColor);
            }
        }

        /// <summary>
        /// 取得筆刷物件
        /// </summary>
        /// <returns>筆刷物件</returns>
        public void GetBrush(ref SolidBrush brush, Color color, float opacity, float rfix, float gfix, float bfix)
        {
            if (brush == null)
            {
                Color brushColor = ColorFix.GetColor(color, opacity, rfix, gfix, bfix);
                brush = DrawPool.GetBrush(brushColor);
            }
        }

        /// <summary>
        /// 返還畫筆物件
        /// </summary>
        public void BackPen(ref Pen pen)
        {
            if (pen != null)
            {
                DrawPool.BackPen(pen);
                pen = null;
            }
        }

        /// <summary>
        /// 返還筆刷物件
        /// </summary>
        public void BackBrush(ref SolidBrush brush)
        {
            if (brush != null)
            {
                DrawPool.BackBrush(brush);
                brush = null;
            }
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected abstract void OnDraw(Graphics g, Rectangle rectangle);

        /// <summary>
        /// 釋放時進行動作
        /// </summary>
        protected abstract void OnDispose();
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDispose();
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
