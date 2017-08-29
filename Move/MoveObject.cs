using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    class MoveObject
    {
        /// <summary>
        /// 移動調整值紀錄
        /// </summary>
        public List<PointF> Moves { get; set; }

        /// <summary>
        /// 最大調整值紀錄數量
        /// </summary>
        public int MaxMoves { get; set; }

        /// <summary>
        /// 移動速度,決定每個移動調整值的距離
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// 追尋目標
        /// </summary>
        public ITarget Target { get; set; }

        /// <summary>
        /// 物件在回合內進行的規劃活動
        /// </summary>
        protected virtual void ActionPlan()
        {
            if (Target != null)
            {
                double distance = Function.GetDistance(Layout.CenterX, Layout.CenterY, Target.X, Target.Y);
                double direction = Function.GetAngle(Layout.CenterX, Layout.CenterY, Target.X, Target.Y);

                float speed = Speed;
                if (distance < 50)
                {
                    distance -= 0.1;
                    if (distance < 0) distance = 0;
                    speed = (float)(Speed * distance / 50);
                }

                Moves.Add(GetMovePoint(direction, speed));
            }
        }
    }
}
