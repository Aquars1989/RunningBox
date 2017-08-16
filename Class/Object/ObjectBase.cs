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
        public SceneGaming Scene
        {
            get { return ParentCollection == null ? null : ParentCollection.Scene; }
        }

        /// <summary>
        /// 物件歸屬集合
        /// </summary>
        public ObjectCollection ParentCollection { get; set; }

        /// <summary>
        /// 物件狀態
        /// </summary>
        public ObjectStatus Status { get; set; }

        /// <summary>
        /// 繪製物件
        /// </summary>
        public IDraw DrawObject { get; set; }

        private bool BuildRect = false;
        private float _X;
        /// <summary>
        /// 物件位置X
        /// </summary>
        public float X
        {
            get { return _X; }
            set
            {
                _X = value;
                BuildRect = false;
            }
        }

        private float _Y;
        /// <summary>
        /// 物件位置Y
        /// </summary>
        public float Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
                BuildRect = false;
            }
        }

        private int _Size;
        /// <summary>
        /// 物件大小
        /// </summary>
        public int Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
                BuildRect = false;
            }
        }

        private Rectangle _Rectangle;
        /// <summary>
        /// 物件的碰撞位置
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                if (!BuildRect)
                {
                    _Rectangle = new Rectangle((int)X - Size, (int)Y - Size, Size * 2, Size * 2);
                    BuildRect = true;
                }
                return _Rectangle;
            }
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
            DrawObject.Draw(g, Rectangle);
        }

        /// <summary>
        /// 取得位移值
        /// </summary>
        /// <param name="angle">位移角度</param>
        /// <param name="speed">速度值</param>
        /// <returns>位移點</returns>
        public PointF GetMovePoint(double angle, float speed)
        {
            double ratioSecToRound = 1000 / Scene.TimerOfRound.Interval;
            float moveX = (float)(Math.Cos(angle / 180 * Math.PI) * speed / ratioSecToRound);
            float moveY = (float)(Math.Sin(angle / 180 * Math.PI) * speed / ratioSecToRound);
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
