using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class ObjectPlayer : ObjectBase
    {
        public int Energy { get; set; }
        public int EnergyMax { get; set; }
        public int EnergyGetPerAction { get; set; }

        private Pen _Pen;
        private Color _Color;
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color == value) return;

                if (_Pen != null)
                {
                    DrawPool.BackPen(_Color);
                }

                _Color = value;
                _Pen = DrawPool.GetPen(_Color);
            }
        }

        public ObjectPlayer(float x, float y, int maxMoves, int size, float speed, Color color)
        {
            Status = ObjectStatus.Alive;
            MaxMoves = maxMoves;
            X = x;
            Y = y;
            Size = size;
            Speed = speed;
            Moves = new List<PointF>();
            Color = color;

            EnergyMax = Energy = 1000;
            EnergyGetPerAction = 5;
        }

        ~ObjectPlayer()
        {
            DrawPool.BackPen(_Color);
        }

        public override void Action()
        {
            Energy += EnergyGetPerAction;
            if (Energy > EnergyMax)
            {
                Energy = EnergyMax;
            }

            if (Moves.Count >= MaxMoves)
            {
                Moves.RemoveAt(0);
            }

            Point trackPoint = Scene.TrackPoint;
            Rectangle rectScene = Scene.GameRectangle;
            double direction = Function.PointRotation(X, Y, trackPoint.X, trackPoint.Y);
            float move = ((Math.Abs(trackPoint.X - X) * 2 + Math.Abs(trackPoint.Y - Y) * 2) / 120) + 0.4F;
            float moveX = (float)Math.Cos(direction / 180 * Math.PI) * move;
            float moveY = (float)Math.Sin(direction / 180 * Math.PI) * move;

            if (X < rectScene.Left)
            {
                moveX = Math.Abs(moveX) * 2;
            }
            else if (X > rectScene.Left + rectScene.Width)
            {
                moveX = -Math.Abs(moveX) * 2;
            }

            if (Y < rectScene.Top)
            {
                moveY = Math.Abs(moveY) * 2;
            }
            else if (Y > rectScene.Top + rectScene.Height)
            {
                moveY = -Math.Abs(moveY) * 2;
            }


            Moves.Add(new PointF(moveX, moveY));
            float moveTotalX = 0;
            float moveTotalY = 0;
            foreach (PointF pt in Moves)
            {
                moveTotalX += pt.X;
                moveTotalY += pt.Y;
            }

            X += moveTotalX * Scene.WorldSpeed;
            Y += moveTotalY * Scene.WorldSpeed;
        }

        public override void Kill()
        {
            if (Status != ObjectStatus.Alive) return;

            float moveTotalX = 0;
            float moveTotalY = 0;
            foreach (PointF pt in Moves)
            {
                moveTotalX += pt.X;
                moveTotalY += pt.Y;
            }

            double direction = Function.PointRotation(0, 0, moveTotalX, moveTotalY);
            for (int i = 0; i < 15; i++)
            {
                int speed = Global.Rand.Next(300, 900);
                int life = Global.Rand.Next(25, 40);
                int size = Global.Rand.Next(1, 4) / 2;
                double scrapDirection = direction + (Global.Rand.NextDouble() - 0.5) * 20;
                Scene.GameObjects.Add(new ObjectScrap(X, Y, 1, speed, life, scrapDirection, Color));
            }
            Scene.EffectObjects.Add(new EffectShark(20, 10));
            base.Kill();
        }

        public override void DrawSelf(Graphics g)
        {
            _Pen.Width = 2;
            g.DrawEllipse(_Pen, Rectangle);
        }
    }
}
