using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 基本玩家物件範本,不包含任何技能,特性
    /// </summary>
    public class ObjectPlayer : ObjectActive
    {
        /// <summary>
        /// 建立一個基本玩家物件範本
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="maxMoves">最大調整值紀錄數量</param>
        /// <param name="size">物件大小</param>
        /// <param name="speed">基本速度</param>
        /// <param name="leage">物件所屬陣營,供技能或特性判定</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="target">追蹤目標</param>
        public ObjectPlayer(float x, float y, int maxMoves, int size, float speed, League leage, IDraw drawObject, ITarget target)
        {
            LifeLimit = -1;
            Status = ObjectStatus.Alive;
            MaxMoves = maxMoves;
            X = x;
            Y = y;
            Size = size;
            Speed = speed;
            EnergyMax = Energy = 1000;
            EnergyGetPerSec = 5;
            League = leage;
            DrawObject = drawObject;
            Target = target;
        }

        protected override void ActionPlan()
        {
            Point trackPoint = Scene.TrackPoint;
            Rectangle rectScene = Scene.MainRectangle;
            double direction = Function.GetAngle(X, Y, trackPoint.X, trackPoint.Y);
            float speed = (Math.Abs(trackPoint.X - X) + Math.Abs(trackPoint.Y - Y)) + 10;
            //float speed = (float)Function.GetDistance(trackPoint.X, trackPoint.Y, X, Y) * 10 + 5;
            if (speed > Speed) speed = Speed;

            PointF move = GetMovePoint(direction, speed);
            float moveX = move.X;
            float moveY = move.Y;

            if (X < rectScene.Left)
            {
                moveX = Math.Abs(moveX) * 2 + 2;
            }
            else if (X > rectScene.Left + rectScene.Width)
            {
                moveX = -Math.Abs(moveX) * 2 - 2;
            }

            if (Y < rectScene.Top)
            {
                moveY = Math.Abs(moveY) * 2 + 2;
            }
            else if (Y > rectScene.Top + rectScene.Height)
            {
                moveY = -Math.Abs(moveY) * 2 - 2;
            }

            Moves.Add(new PointF(moveX, moveY));
        }

        public override void Kill(ObjectActive killer, ObjectDeadType deadType)
        {
            if (Status != ObjectStatus.Alive) return;
            Scene.EffectObjects.Add(new EffectShark(Scene.Sec(2), 10) { CanBreak = false });
            base.Kill(killer, deadType);
        }
    }
}
