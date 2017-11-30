using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 蛙行移動物件,依規律時間移動
    /// </summary>
    class MoveFrog : MoveBase
    {
        private CounterObject _MoveTime;

        /// <summary>
        /// 建立蛙行移動物件,依規律時間移動
        /// </summary>
        /// <param name="target">追蹤目標</param>
        /// <param name="weight">阻力,最終移動速度會受到此值影響(finalSpeed = speeed/Weight)</param>
        /// <param name="speed">移動速度,決定每個移動調整值的最大距離</param>
        /// <param name="offsetsLimit">移動調整值列表最大數量</param>
        public MoveFrog(ITargetability target, float weight, float speed, int offsetsLimit, int moveTime)
            : base(target, weight, speed, offsetsLimit)
        {
            _MoveTime = new CounterObject(moveTime);
        }

        public override void Plan()
        {
            if (_MoveTime.IsFull)
            {
                if (Target.Object != null)
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

                    double direction = Function.GetAngle(ownerX, ownerY, Target.X, Target.Y) + AngleOffset;
                    float speed = (float)(SpeedPerOffsets * (Scene.RoundPerSec * _MoveTime.Limit / Scene.Sec(1) / OffsetsLimit));
                    for (int i = 0; i < OffsetsLimit; i++)
                    {
                        AddOffset(Function.GetOffsetPoint(0, 0, direction, speed));
                    }
                }
                _MoveTime.Value = 0;
            }
            else
            {
                AddOffset(new PointF(0, 0));
            }
            _MoveTime.Value += Scene.SceneIntervalOfRound;
        }
    }
}
