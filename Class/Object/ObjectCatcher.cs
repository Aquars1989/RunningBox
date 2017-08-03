using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class ObjectCatcher : ObjectBase
    {
        public ObjectBase Target { get; set; }
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

        public ObjectCatcher(float x, float y, int maxMoves, int size, float speed, int life, Color color, ObjectBase target)
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

        public override void Action()
        {
            LifeTick--;
            switch (Status)
            {
                case ObjectStatus.Alive:
                    ObjectBase playObject = Scene.PlayerObject;
                    if (playObject != null && playObject.Rectangle.IntersectsWith(Rectangle))
                    {
                        playObject.Kill();
                    }

                    if (Target != null)
                    {
                        if (Moves.Count >= MaxMoves)
                        {
                            Moves.RemoveAt(0);
                        }

                        double direction = Function.PointRotation(X, Y, Target.X, Target.Y);
                        float moveX = (float)Math.Cos(direction / 180 * Math.PI) * (Speed / 100F);
                        float moveY = (float)Math.Sin(direction / 180 * Math.PI) * (Speed / 100F);

                        if (SprintTime > 0)
                        {
                            Scene.GameObjects.Add(new ObjectSmoke(X, Y, Size, _SmokeColor, 3));
                            SprintTime--;
                            moveX *= 5;
                            moveY *= 5;
                        }

                        if (SprintCooldown <= 0)
                        {
                            double randNum = Global.Rand.NextDouble() * 100;
                            if (randNum < 0.5)
                            {
                                SprintTime = MaxMoves;
                                SprintCooldown = MaxMoves * 2;
                            }
                            else if (randNum < 3)
                            {
                                SprintCooldown = MaxMoves / 2;
                                moveX *= 15;
                                moveY *= 15;
                            }
                        }
                        else
                        {
                            SprintCooldown--;
                        }

                        Moves.Add(new PointF((float)moveX, (float)moveY));
                    }

                    float moveTotalX = 0;
                    float moveTotalY = 0;
                    foreach (PointF pt in Moves)
                    {
                        moveTotalX += pt.X;
                        moveTotalY += pt.Y;
                    }

                    X += moveTotalX * Scene.WorldSpeed;
                    Y += moveTotalY * Scene.WorldSpeed;

                    if (playObject != null && playObject.Rectangle.IntersectsWith(Rectangle))
                    {
                        playObject.Kill();
                    }

                    if (LifeTick <= 0)
                    {
                        Status = ObjectStatus.Dying;
                        LifeTick = 10;
                        LifeTickMax = 10;
                    }
                    break;
                case ObjectStatus.Dying:
                    if (LifeTick <= 0)
                    {
                        Kill();
                    }
                    break;
            }
        }

        public override void DrawSelf(Graphics g)
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
