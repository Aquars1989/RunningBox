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
        /// <param name="target">追蹤目標(必要)</param>
        /// <param name="speed">移動速度,決定每個移動調整值的最大距離</param>
        /// <param name="offsetsLimit">移動調整值列表最大數量</param>
        /// <param name="closeRange">定義鄰近距離,與目標距離小於此值會開始減速</param>
        /// <param name="closeSpeedSlow">進入鄰近範圍後,移動速度降低比例最大值,速度比例=1-(CloseSpeedSlow * (CloseRange-Distance)/CloseRange)</param>
        public MoveStraight(float speed, int offsetsLimit, float closeRange, float closeSpeedSlow, ITarget target)
            : base(target)
        {
            CloseRange = closeRange;
            CloseSpeedSlow = closeSpeedSlow;
        }

        public override void Plan()
        {
            if (Target != null)
            {
                double distance = Function.GetDistance(Owner.Layout.CenterX, Owner.Layout.CenterY, Target.X, Target.Y);
                double direction = Function.GetAngle(Owner.Layout.CenterX, Owner.Layout.CenterY, Target.X, Target.Y);

                float speed = Speed;
                if (distance < CloseRange)
                {
                    double speedFix = 1 - (CloseSpeedSlow * (CloseRange - distance) / CloseRange);
                    speed = (float)(Speed * speedFix);
                }
                AddOffset(Function.GetOffsetPoint(0, 0, direction, speed));
            }
        }
    }
}
