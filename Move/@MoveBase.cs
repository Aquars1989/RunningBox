using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public abstract class MoveBase
    {
        /// <summary>
        /// 所有者
        /// </summary>
        public ObjectBase Owner { get; set; }

        /// <summary>
        /// 移動調整值紀錄
        /// </summary>
        public List<PointF> Moves { get; set; }

        /// <summary>
        /// 最大調整值紀錄數量
        /// </summary>
        public int MaxMoves { get; set; }

        /// <summary>
        /// 移動速度,決定每個移動調整值的最大距離
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// 追尋目標
        /// </summary>
        public ITarget Target { get; set; }

        /// <summary>
        /// 規劃移動調整值
        /// </summary>
        protected abstract void Plan();

        /// <summary>
        /// 移動所有者
        /// </summary>
        protected virtual void Move()
        {
            if (Moves.Count > MaxMoves)
            {
                Moves.RemoveRange(0, Moves.Count - MaxMoves);
            }

            float moveTotalX = 0;
            float moveTotalY = 0;
            foreach (PointF pt in Moves)
            {
                moveTotalX += pt.X;
                moveTotalY += pt.Y;
            }

            Owner.Layout.X += moveTotalX / Owner.Scene.SceneSlow;
            Owner.Layout.Y += moveTotalY / Owner.Scene.SceneSlow;
        }
    }
}
