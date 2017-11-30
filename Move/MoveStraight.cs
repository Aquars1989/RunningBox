using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 直行移動物件,靠近目標後會依距離減緩速度
    /// </summary>
    class MoveStraight : MoveBase
    {
        /// <summary>
        /// 定義鄰近距離,與目標距離小於此值會開始減速
        /// </summary>
        public float CloseRange { get; set; }

        /// <summary>
        /// 進入鄰近範圍後,移動速度降低比例最大值,速度比例=1-(CloseSpeedSlow * (CloseRange-Distance)/CloseRange)
        /// </summary>
        public float CloseSpeedSlow { get; set; }

        /// <summary>
        /// 建立直行移動物件,靠近目標後會依距離減緩速度
        /// </summary>
        /// <param name="target">追蹤目標</param>
        /// <param name="weight">阻力,最終移動速度會受到此值影響(finalSpeed = speeed/Weight)</param>
        /// <param name="speed">移動速度,決定每個移動調整值的最大距離</param>
        /// <param name="offsetsLimit">移動調整值列表最大數量</param>
        /// <param name="closeRange">定義鄰近距離,與目標距離小於此值會開始減速</param>
        /// <param name="closeSpeedSlow">進入鄰近範圍後,移動速度降低比例最大值,速度比例=1-(CloseSpeedSlow * (CloseRange-Distance)/CloseRange)</param>
        public MoveStraight(ITargetability target, float weight, float speed, int offsetsLimit, float closeRange, float closeSpeedSlow)
            : base(target, weight, speed, offsetsLimit)
        {
            CloseRange = closeRange;
            CloseSpeedSlow = closeSpeedSlow;
        }

        public override void Plan()
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

                double distance = Function.GetDistance(ownerX, ownerY, Target.X, Target.Y);
                double direction = Function.GetAngle(ownerX, ownerY, Target.X, Target.Y) + AngleOffset;

                float speed = SpeedPerOffsets;
                if (distance < CloseRange)
                {
                    double speedFix = 1 - (CloseSpeedSlow * (CloseRange - distance) / CloseRange);
                    speed = (float)(speed * Math.Max(speedFix, 0));
                }
                AddOffset(Function.GetOffsetPoint(0, 0, direction, speed));
            }
        }
    }
}
