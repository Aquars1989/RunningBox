using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class ObjectCatcher : ObjectActive
    {
        public int LifeTick { get; set; }
        public int LifeTickMax { get; set; }

        private SolidBrush _Brush;
        private Color _SmokeColor;
        private Color _Color;
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color == value) return;

                if (_Brush != null)
                {
                    DrawPool.BackBrush(_Color);
                }

                _Color = value;
                _SmokeColor = Color.FromArgb(_Color.A / 8, _Color.R, _Color.G, _Color.G);
                _Brush = DrawPool.GetBrush(_Color);
            }
        }

        public int SprintTime { get; set; }
        public int SprintCooldown { get; set; }

        public ObjectCatcher(float x, float y, int maxMoves, int size, float speed, int life, Color color, ITarget target)
        {
            Status = ObjectStatus.Alive;
            MaxMoves = maxMoves;
            X = x;
            Y = y;
            Size = size;
            Speed = speed;
            LifeTickMax = life;
            LifeTick = life;
            Moves = new List<PointF>();
            Color = color;
            Target = target;
        }

        ~ObjectCatcher()
        {
            DrawPool.BackBrush(_Color);
        }

        protected override void ActionMove()
        {
            base.ActionMove();
            LifeTick--;
            if (LifeTick <= 0)
            {
                Status = ObjectStatus.Dead;
            }
        }

        protected override void DrawSelf(Graphics g)
        {
            switch (Status)
            {
                case ObjectStatus.Alive:
                    g.FillEllipse(_Brush, Rectangle);
                    break;
                case ObjectStatus.Dying:
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb((int)(255F / LifeTickMax * LifeTick), Color.R, Color.G, Color.B)))
                    {
                        g.FillEllipse(brush, Rectangle);
                    }
                    break;
            }
        }
    }
}
