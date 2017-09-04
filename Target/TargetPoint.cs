using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 代表座標點的目標物件
    /// </summary>
    public class TargetPoint : ITarget
    {
        /// <summary>
        /// 目標X座標
        /// </summary>
        public float X
        {
            get { return Target.X; }
        }

        /// <summary>
        /// 目標Y座標
        /// </summary>
        public float Y
        {
            get { return Target.Y; }
        }

        /// <summary>
        /// 目標點
        /// </summary>
        public PointF Target { get; set; }

        /// <summary>
        /// 新增代表座標點的目標物件
        /// </summary>
        /// <param name="x">目標點X座標</param>
        /// <param name="y">目標點Y座標</param>
        public TargetPoint(int x, int y)
        {
            Target = new PointF(x, y);
        }

        /// <summary>
        /// 新增代表座標點的目標物件
        /// </summary>
        /// <param name="point">座標點</param>
        public TargetPoint(PointF point)
        {
            Target = point;
        }

        /// <summary>
        /// 取得目標點
        /// </summary>
        /// <returns>目標點</returns>
        public PointF GetPoint()
        {
            return Target;
        }
    }
}
