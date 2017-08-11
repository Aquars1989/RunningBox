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
    public abstract class ObjectBase
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
