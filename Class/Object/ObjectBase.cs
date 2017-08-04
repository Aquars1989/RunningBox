using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public enum ObjectStatus
    {
        Alive = 0,
        Dying = 1,
        Dead = 2,
    }

    /// <summary>
    /// 處理物件死亡事件
    /// </summary>
    /// <param name="sender">死亡物件</param>
    /// <param name="killer">殺手物件</param>
    public delegate void ObjectDeadEventHandle(ObjectBase sender, ObjectBase killer);


    public abstract class ObjectBase
    {
        /// <summary>
        /// 發生於物件死亡
        /// </summary>
        public event ObjectDeadEventHandle Dead;

        public SceneBase Scene { get; set; }
        public ObjectStatus Status { get; set; }
        public List<PointF> Moves { get; set; }
        public int MaxMoves { get; set; }
        public float Speed { get; set; }

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
        /// 物件的實體範圍
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
        public virtual void Kill(ObjectBase killer)
        {
            Status = ObjectStatus.Dead;
            OnDead(this, killer);
        }

        protected void OnDead(ObjectBase sender, ObjectBase killer)
        {
            if (Dead != null)
            {
                Dead(sender, killer);
            }

        }

        /// <summary>
        /// 物件在1回合內進行的活動
        /// </summary>
        public abstract void Action();

        /// <summary>
        /// 繪製物件本身
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public abstract void DrawSelf(Graphics g);
    }
}
