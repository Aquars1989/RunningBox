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
        /// <param name="target">追蹤目標</param>
        /// <param name="weight">阻力,最終移動速度會受到此值影響(finalSpeed = speeed/Weight)</param>
        /// <param name="speed">移動速度,決定每個移動調整值的最大距離</param>
        /// <param name="offsetsLimit">移動調整值列表最大數量</param>
        public MovePlayer(ITargetability target, float weight, float speed, int offsetsLimit)
            : base(target, weight, speed, offsetsLimit) { }

        public override void Plan()
        {
            float ownerX, ownerY;
            if ((Anchor & DirectionType.Left) == DirectionType.Left)
            {
                ownerX = Owner.Layout.LeftTopX;
            }
            else if ((Anchor & DirectionType.Right) == DirectionType.Right)
            {
                ownerX = Owner.Layout.RightBottomX;
            }
            else
            {
                ownerX = Owner.Layout.CenterX;
            }

            if ((Anchor & DirectionType.Top) == DirectionType.Top)
            {
                ownerY = Owner.Layout.LeftTopY;
            }
            else if ((Anchor & DirectionType.Bottom) == DirectionType.Bottom)
            {
                ownerY = Owner.Layout.RightBottomY;
            }
            else
            {
                ownerY = Owner.Layout.CenterY;
            }

            Rectangle rectScene = Scene.MainRectangle;
            double direction = Function.GetAngle(ownerX, ownerY, Target.X, Target.Y);
            float speed = (Math.Abs(Target.X - ownerX) + Math.Abs(Target.Y - ownerY)) + 10;
            if (speed > Speed) speed = Speed;

            PointF move = Function.GetOffsetPoint(0, 0, direction, speed);
            float moveX = move.X;
            float moveY = move.Y;

            if (ownerX < rectScene.Left)
            {
                moveX = Math.Abs(moveX) * 2 + 2;
            }
            else if (ownerX > rectScene.Left + rectScene.Width)
            {
                moveX = -Math.Abs(moveX) * 2 - 2;
            }

            if (ownerY < rectScene.Top)
            {
                moveY = Math.Abs(moveY) * 2 + 2;
            }
            else if (ownerY > rectScene.Top + rectScene.Height)
            {
                moveY = -Math.Abs(moveY) * 2 - 2;
            }

            AddOffset(new PointF(moveX, moveY));
        }

        /// <summary>
        /// 移動所有者
        /// </summary>
        public virtual void Move()
        {
            float moveX = MoveX / Scene.SceneRoundPerSec / Resistance / Scene.SceneTimeFix;
            float moveY = MoveY / Scene.SceneRoundPerSec / Resistance / Scene.SceneTimeFix;
            if (MoveX != 0 || MoveY != 0)
            {
                ObjectActive ownerActive = Owner as ObjectActive;
                if (ownerActive != null && (ownerActive.Propertys.Affix & SpecialStatus.Movesplit) == SpecialStatus.Movesplit)
                {
                    //移動距離大時分成多次移動,供碰撞用
                    int partCount = (int)(Math.Max(Math.Abs(moveX / Owner.Layout.Width), Math.Abs(moveY / Owner.Layout.Height))) + 1;
                    float partX = moveX / partCount;
                    float partY = moveY / partCount;
                    for (int i = 0; i < partCount; i++)
                    {
                        Owner.Layout.X += partX;
                        Owner.Layout.Y += partY;
                        OnMoving();
                    }
                }
                else
                {
                    Owner.Layout.X += moveX;
                    Owner.Layout.Y += moveY;
                    OnMoving();
                }
            }
            OnAfterMove();
        }
    }
}
