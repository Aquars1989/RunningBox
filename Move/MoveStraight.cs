using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
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

        protected override void Plan()
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
                Moves.Add(Function.GetOffsetPoint(0, 0, direction, speed));
            }
        }
    }
}
