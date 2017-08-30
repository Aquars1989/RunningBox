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
        /// <summary>
        /// 發生於物件死亡
        /// </summary>
        public event ObjectDeadEventHandle Dead;

        /// <summary>
        /// 物件歸屬場景
        /// </summary>
        public SceneBase Scene
        {
            get { return Container == null ? null : Container.Scene; }
        }

        private ObjectCollection _Container;
        /// <summary>
        /// 物件歸屬集合
        /// </summary>
        public ObjectCollection Container
        {
            get { return _Container; }
            set
            {
                _Container = value;
                if (_DrawObject != null)
                {
                    _DrawObject.Scene = Scene;
                }
            }
        }

        /// <summary>
        /// 物件狀態
        /// </summary>
        public ObjectStatus Status { get; set; }

        private DrawBase _DrawObject;
        /// <summary>
        /// 繪製物件
        /// </summary>
        public DrawBase DrawObject
        {
            get { return _DrawObject; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                _DrawObject = value;
                _DrawObject.Owner = this;
                _DrawObject.Scene = Scene;
            }
        }

        /// <summary>
        /// 物件配置方式
        /// </summary>
        public Layout Layout { get; private set; }

        public ObjectBase()
        {
            Layout = new Layout();
        }

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

        /// <summary>
        /// 物件在1回合內進行的活動
        /// </summary>
        public abstract void Action();

        /// <summary>
        /// 繪製物件
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public virtual void Draw(Graphics g)
        {
            if (DrawObject == null) return;
            DrawObject.Draw(g, Layout.Rectangle);
        }

        /// <summary>
        /// 取得位移值(不計入場景速度)
        /// </summary>
        /// <param name="angle">位移角度</param>
        /// <param name="speed">速度值</param>
        /// <returns>位移點</returns>
        public PointF GetMovePoint(double angle, float speed)
        {
            float moveX = (float)(Math.Cos(angle / 180 * Math.PI) * speed / Scene.RoundPerSec);
            float moveY = (float)(Math.Sin(angle / 180 * Math.PI) * speed / Scene.RoundPerSec);
            return new PointF(moveX, moveY);
        }

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
    /// 活動物件狀態
    /// </summary>
    public enum ObjectStatus
    {
        Alive = 0,
        Dying = 1,
        Dead = 2,
    }

    /// <summary>
    /// 物件死亡類型
    /// </summary>
    [Flags]
    public enum ObjectDeadType
    {
        All = 255,
        Clear = 1,
        LifeEnd = 2,
        Collision = 4
    }

    /// <summary>
    /// 處理物件死亡事件
    /// </summary>
    /// <param name="sender">死亡物件</param>
    /// <param name="killer">殺手物件</param>
    public delegate void ObjectDeadEventHandle(ObjectBase sender, ObjectBase killer, ObjectDeadType deadType);
}
