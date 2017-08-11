using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class ObjectPlayerNormal : ObjectActive
    {
        /// <summary>
        /// 建立一個正常玩家物件,
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="maxMoves">最大調整值紀錄數量</param>
        /// <param name="size">物件大小</param>
        /// <param name="speed">基本速度</param>
        public ObjectPlayerNormal(float x, float y, int maxMoves, int size, float speed)
        {
            LifeRoundMax = -1;
            Status = ObjectStatus.Alive;
            MaxMoves = maxMoves;
            X = x;
            Y = y;
            Size = size;
            Speed = speed;
            EnergyMax = Energy = 1000;
            EnergyGetPerRound = 5;
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
            float move = ((Math.Abs(trackPoint.X - X) * 2 + Math.Abs(trackPoint.Y - Y) * 2) / 120) + Speed;
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

        public override void Kill(ObjectActive killer, ObjectDeadType deadType)
        {
            if (Status != ObjectStatus.Alive) return;
            Scene.EffectObjects.Add(new EffectShark(20, 10) { CanBreak = false });
            base.Kill(killer, deadType);
        }
    }
}
