using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class ObjectScrap : ObjectBase
    {
        public int LifeTick { get; set; }
        public int LifeTickMax { get; set; }
        public double Direction { get; set; }

        private Color _Color;
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color == value) return;
                _Color = value;
            }
        }

        public ObjectScrap(float x, float y, int size, float speed, int life, double direction, Color color)
        {
            Status = ObjectStatus.Dying;
            X = x;
            Y = y;
            Size = size;
            Speed = speed;
            LifeTickMax = life;
            LifeTick = life;
            Direction = direction;
            Color = color;
        }

        ~ObjectScrap()
        {
            DrawPool.BackBrush(_Color);
        }

        public override void Action()
        {
            LifeTick--;
            if (LifeTick == 0)
            {
                Kill();
            }
            else
            {
                float moveX = (float)Math.Cos(Direction / 180 * Math.PI) * (Speed / 100F);
                float moveY = (float)Math.Sin(Direction / 180 * Math.PI) * (Speed / 100F);
                X += moveX;
                Y += moveY;
            }
        }

        public override void DrawSelf(Graphics g)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb((int)(255F / LifeTickMax * LifeTick), Color.R, Color.G, Color.B)))
            {
                g.FillEllipse(brush, Rectangle);
            }
        }
    }
}
