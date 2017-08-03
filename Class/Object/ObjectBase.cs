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

    public abstract class ObjectBase
    {
        public event EventHandler Killed;

        public SceneBase Scene { get; set; }
        public ObjectStatus Status { get; set; }
        public List<PointF> Moves { get; set; }
        public int MaxMoves { get; set; }
        public float Speed { get; set; }

        private bool BuildRect = false;
        private float _X;
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

        public virtual void Kill()
        {
            Status = ObjectStatus.Dead;
            if (Killed != null)
            {
                Killed(this, new EventArgs());
            }
        }

        public abstract void Action();
        public abstract void DrawSelf(Graphics g);
    }
}
