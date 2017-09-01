using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 玩家專屬移動物件
    /// </summary>
    public class MovePlayer : MoveBase
    {
        /// <summary>
        /// 建立玩家專屬移動物件
        /// </summary>
        /// <param name="target">追蹤目標(必要)</param>
        /// <param name="speed">移動速度,決定每個移動調整值的最大距離</param>
        /// <param name="offsetsLimit">移動調整值列表最大數量</param>
        public MovePlayer(float speed, int offsetsLimit, ITarget target)
            : base(target)
        {
            Speed = speed;
            OffsetsLimit = offsetsLimit;
        }

        public override void Plan()
        {
            PointF trackPoint = Target.GetPoint();
            Rectangle rectScene = Owner.Scene.MainRectangle;
            double direction = Function.GetAngle(Owner.Layout.CenterX, Owner.Layout.CenterY, trackPoint.X, trackPoint.Y);
            float speed = (Math.Abs(trackPoint.X - Owner.Layout.CenterX) + Math.Abs(trackPoint.Y - Owner.Layout.CenterY)) + 10;
            if (speed > Speed) speed = Speed;

            PointF move = Function.GetOffsetPoint(0, 0, direction, speed);
            float moveX = move.X;
            float moveY = move.Y;

            if (Owner.Layout.CenterX < rectScene.Left)
            {
                moveX = Math.Abs(moveX) * 2 + 2;
            }
            else if (Owner.Layout.CenterX > rectScene.Left + rectScene.Width)
            {
                moveX = -Math.Abs(moveX) * 2 - 2;
            }

            if (Owner.Layout.CenterY < rectScene.Top)
            {
                moveY = Math.Abs(moveY) * 2 + 2;
            }
            else if (Owner.Layout.CenterY > rectScene.Top + rectScene.Height)
            {
                moveY = -Math.Abs(moveY) * 2 - 2;
            }

            AddOffset(new PointF(moveX, moveY));
        }
    }
}
