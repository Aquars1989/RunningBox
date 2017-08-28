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
        /// <param name="x">物件中心位置X</param>
        /// <param name="y">物件中心位置Y</param>
        /// <param name="maxMoves">最大調整值紀錄數量</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="speed">基本速度</param>
        /// <param name="leage">物件所屬陣營,供技能或特性判定</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="target">追蹤目標</param>
        public ObjectPlayer(float x, float y, int maxMoves, int width, int height, float speed, League leage, IDraw drawObject, ITarget target)
        {
            Layout.Anchor = ContentAlignment.MiddleCenter;
            Layout.X = x;
            Layout.Y = y;
            Layout.Width = width;
            Layout.Height = height;

            Status = ObjectStatus.Alive;
            MaxMoves = maxMoves;
            Speed = speed;
            League = leage;
            DrawObject = drawObject;
            Target = target;
        }

        protected override void ActionPlan()
        {
            Point trackPoint = Scene.TrackPoint;
            Rectangle rectScene = Scene.MainRectangle;
            double direction = Function.GetAngle(Layout.CenterX, Layout.CenterY, trackPoint.X, trackPoint.Y);
            float speed = (Math.Abs(trackPoint.X - Layout.CenterX) + Math.Abs(trackPoint.Y - Layout.CenterY)) + 10;
            //float speed = (float)Function.GetDistance(trackPoint.X, trackPoint.Y, X, Y) * 10 + 5;
            if (speed > Speed) speed = Speed;

            PointF move = GetMovePoint(direction, speed);
            float moveX = move.X;
            float moveY = move.Y;

            if (Layout.CenterX < rectScene.Left)
            {
                moveX = Math.Abs(moveX) * 2 + 2;
            }
            else if (Layout.CenterX > rectScene.Left + rectScene.Width)
            {
                moveX = -Math.Abs(moveX) * 2 - 2;
            }

            if (Layout.CenterY < rectScene.Top)
            {
                moveY = Math.Abs(moveY) * 2 + 2;
            }
            else if (Layout.CenterY > rectScene.Top + rectScene.Height)
            {
                moveY = -Math.Abs(moveY) * 2 - 2;
            }

            Moves.Add(new PointF(moveX, moveY));
        }

        public override void Kill(ObjectActive killer, ObjectDeadType deadType)
        {
            if (Status != ObjectStatus.Alive) return;
            Scene.EffectObjects.Add(new EffectShark(Scene.Sec(0.6F), 10) { CanBreak = false });
            base.Kill(killer, deadType);
        }
    }
}
