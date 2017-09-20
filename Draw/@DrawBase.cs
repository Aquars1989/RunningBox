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
        /// 發生於所屬物件變更時(所屬物件可為繪圖物件>所有人>場景)
        /// </summary>
        public event EventHandler BindingChanged;

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
        /// 發生於所屬物件變更時(所屬物件可為繪圖物件>所有人>場景)
        /// </summary>
        protected virtual void OnBindingChanged()
        {
            if (BindingChanged != null)
            {
                BindingChanged(this, new EventArgs());
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
        /// 旋轉角度
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// 是否鎖定綁定功能
        /// </summary>
        public bool BindingLock { get; private set; }

        /// <summary>
        /// 繪製顏色管理物件
        /// </summary>
        public DrawColors Colors { get; private set; }

        private DrawBase _BindDraw;
        /// <summary>
        /// 取得綁定的繪製物件(繪圖物件>所有人>場景)
        /// </summary>
        public DrawBase BindDraw
        {
            get { return _BindDraw; }
            private set
            {
                if (_BindDraw == value) return;
                _BindDraw = value;
            }
        }

        private ObjectBase _Owner;
        /// <summary>
        /// 取得歸屬的活動物件(繪圖物件>所有人>場景)
        /// </summary>
        public ObjectBase Owner
        {
            get { return BindDraw == null ? _Owner : BindDraw.Owner; }
            private set
            {
                if (_Owner == value) return;
                _Owner = value;
            }
        }

        private SceneBase _Scene;
        /// <summary>
        /// 取得歸屬的場景物件(繪圖物件>所有人>場景)
        /// </summary>
        public SceneBase Scene
        {
            get { return BindDraw == null ? Owner == null ? _Scene : Owner.Scene : BindDraw.Scene; }
            private set
            {
                if (_Scene == value) return;
                _Scene = value;
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
        /// 綁定繪製物件到另一個繪製物件(繪圖物件>物件>場景)
        /// </summary>
        /// <param name="drawBase">繪圖物件</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public virtual void Binding(DrawBase drawBase, bool bindingLock = false)
        {
            if (_BindDraw == drawBase) return;
            if (BindingLock) throw new Exception("繪製物件已被鎖定無法綁定");

            BindDraw = drawBase;
            Owner = null;
            Scene = null;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定繪製物件到場景(繪圖物件>物件>場景)
        /// </summary>
        /// <param name="scene">場景</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public virtual void Binding(SceneBase scene, bool bindingLock = false)
        {
            if (_Scene == scene) return;
            if (BindingLock) throw new Exception("繪製物件已被鎖定無法綁定");

            BindDraw = null;
            Owner = null;
            Scene = scene;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定繪製物件到所有人物件(繪圖物件>物件>場景,由所有者綁定,除此之外勿使用此函數)
        /// </summary>
        /// <param name="owner">所有者</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public virtual void Binding(ObjectBase owner, bool bindingLock = false)
        {
            if (_Owner == owner) return;
            if (BindingLock) throw new Exception("繪製物件已被鎖定無法綁定");
            if (owner != null && owner.DrawObject != this) throw new Exception("所有者的繪製物件不符");
            BindDraw = null;
            Owner = owner;
            Scene = null;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 清除綁定
        /// </summary>
        public virtual void ClearBinding()
        {
            if (BindingLock) throw new Exception("繪製物件已被鎖定無法解除綁定");
            BindDraw = null;
            Owner = null;
            Scene = null;
            OnBindingChanged();
        }

        /// <summary>
        /// 解除綁定鎖定
        /// </summary>
        public void BindingUnlock()
        {
            BindingLock = false;
        }

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
        /// 複製繪圖物件及內部的繪圖工具管理物件,未綁定物件
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
        /// 釋放物件時執行動作
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
