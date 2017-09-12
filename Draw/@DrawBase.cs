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
        public event DrawColorsEnentHandle ColorChanged;

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
        protected virtual void OnColorChanged(string colorId)
        {
            if (ColorChanged != null)
            {
                ColorChanged(this, colorId);
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
        /// <summary>
        /// 繪製顏色管理物件
        /// </summary>
        public DrawColors Colors { get; private set; }

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

        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public abstract Color MainColor { get; }

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
        /// 使用指定管理物件建立繪圖物件
        /// </summary>
        /// <param name="drawColor"></param>
        public DrawBase(DrawColors drawColor)
        {
            Colors = drawColor;
            drawColor.ColorFixChanged += (x, e) => { OnColorFixChanged(); };
            drawColor.ColorChanged += (x, e) => { OnColorChanged(e); };
        }

        /// <summary>
        /// 建立繪圖物件
        /// </summary>
        public DrawBase()
            : this(new DrawColors()) { }

        #region ===== 方法 =====
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

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件
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
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected abstract void OnDraw(Graphics g, Rectangle rectangle);

        /// <summary>
        /// 試放物件時執行動作
        /// </summary>
        protected virtual void OnDispose() 
        {
            Colors.Dispose();
        }
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
