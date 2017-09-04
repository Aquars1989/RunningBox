using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 基礎活動物件
    /// </summary>
    public abstract class ObjectBase : IDisposable
    {
        #region ===== 事件 =====
        /// <summary>
        /// 發生於物件死亡
        /// </summary>
        public event ObjectDeadEventHandle Dead;

        /// <summary>
        /// 發生於物件狀態變更
        /// </summary>
        public event EventHandler StatusChanged;

        /// <summary>
        /// 發生於繪圖物件變更
        /// </summary>
        public event EventHandler DrawObjectChanged;

        /// <summary>
        /// 發生於移動物件變更
        /// </summary>
        public event EventHandler MoveObjectChanged;

        /// <summary>
        /// 發生於歸屬群組變更
        /// </summary>
        public event EventHandler ContainerChanged;

        /// <summary>
        /// 發生於歸屬場景變更
        /// </summary>
        public event EventHandler SceneChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於物件狀態變更
        /// </summary>
        public void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於繪圖物件變更
        /// </summary>
        public void OnDrawObjectChanged()
        {
            if (DrawObject != null)
            {
                DrawObject.Scene = Scene;
            }

            if (DrawObjectChanged != null)
            {
                DrawObjectChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於移動物件變更
        /// </summary>
        public void OnMoveObjectChanged()
        {
            if (MoveObject != null)
            {
                MoveObject.Owner = this;
            }

            if (MoveObjectChanged != null)
            {
                MoveObjectChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於歸屬群組變更
        /// </summary>
        public void OnContainerChanged()
        {
            if (ContainerChanged != null)
            {
                ContainerChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於歸屬場景變更
        /// </summary>
        public void OnSceneChanged()
        {
            if (DrawObject != null)
            {
                DrawObject.Scene = Scene;
            }

            if (SceneChanged != null)
            {
                SceneChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生在物件死亡時
        /// </summary>
        /// <param name="sender">死亡物件</param>
        /// <param name="killer">殺手物件</param>
        protected void OnDead(ObjectBase sender, ObjectBase killer, ObjectDeadType deadType)
        {
            if (Dead != null)
            {
                Dead(sender, killer, deadType);
            }
        }
        #endregion

        #region ===== 屬性 =====
        private SceneBase _Scene;
        /// <summary>
        /// 物件歸屬場景(必要,上層設定)
        /// </summary>
        public SceneBase Scene
        {
            get { return _Scene; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_Scene == value) return;
                _Scene = value;
                OnSceneChanged();
            }

        }

        private ObjectCollection _Container;
        /// <summary>
        /// 物件歸屬集合(必要,上層設定)
        /// </summary>
        public ObjectCollection Container
        {
            get { return _Container; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_Container == value) return;
                _Container = value;
                OnContainerChanged();
            }
        }

        private DrawBase _DrawObject;
        /// <summary>
        /// 繪製物件(必要)
        /// </summary>
        public DrawBase DrawObject
        {
            get { return _DrawObject; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_DrawObject == value) return;
                _DrawObject = value;
                OnDrawObjectChanged();
            }
        }

        private MoveBase _MoveObject;
        /// <summary>
        /// 移動物件(必要)
        /// </summary>
        public MoveBase MoveObject
        {
            get { return _MoveObject; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_MoveObject == value) return;
                _MoveObject = value;
                OnMoveObjectChanged();
            }
        }

        private Layout _Layout;
        /// <summary>
        /// 物件配置方式(必要)
        /// </summary>
        public Layout Layout
        {
            get { return _Layout; }
            private set
            {
                if (value == null) throw new ArgumentNullException();
                _Layout = value;
            }
        }

        private ObjectStatus _Status;
        /// <summary>
        /// 物件狀態
        /// </summary>
        public ObjectStatus Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value) return;
                _Status = value;
                OnStatusChanged();
            }
        }
        #endregion

        /// <summary>
        /// 使用繪製物件和移動物件建立基本活動物件
        /// </summary>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectBase(DrawBase drawObject, MoveBase moveObject)
        {
            Layout = new Layout();
            Status = ObjectStatus.Alive;
            DrawObject = drawObject;
            MoveObject = moveObject;
        }

        #region ===== 方法 =====
        /// <summary>
        /// 殺死此物件
        /// </summary>
        /// <param name="killer">殺手物件</param>
        public virtual void Kill(ObjectActive killer, ObjectDeadType deadType)
        {
            if (Status == ObjectStatus.Alive)
            {
                Status = ObjectStatus.Dead;
                OnDead(this, killer, deadType);
            }
        }

        /// <summary>
        /// 物件在1回合內進行的活動
        /// </summary>
        public virtual void Action()
        {
            MoveObject.Plan();
            MoveObject.Move();
        }

        /// <summary>
        /// 繪製物件
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public virtual void Draw(Graphics g)
        {
            DrawObject.Draw(g, Layout.Rectangle);
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
                    if (DrawObject != null)
                    {
                        DrawObject.Dispose();
                    }
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
    /// 處理物件死亡事件
    /// </summary>
    /// <param name="sender">死亡物件</param>
    /// <param name="killer">殺手物件</param>
    public delegate void ObjectDeadEventHandle(ObjectBase sender, ObjectBase killer, ObjectDeadType deadType);
}
