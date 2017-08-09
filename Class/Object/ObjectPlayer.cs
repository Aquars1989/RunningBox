using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class ObjectPlayer : ObjectActive
    {
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
            EnergyGetPerRound = 5;
        }

        ~ObjectPlayer()
        {
            DrawPool.BackPen(_Color);
        }

        protected override void ActionPlan()
        {
            Energy += EnergyGetPerRound;
            if (Energy > EnergyMax)
            {
                Energy = EnergyMax;
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
        }

        public override void Kill(ObjectActive killer)
        {
            if (Status != ObjectStatus.Alive) return;
            Scene.EffectObjects.Add(new EffectShark(20, 10) { CanBreak = false });
            base.Kill(killer);
        }
    }
}
