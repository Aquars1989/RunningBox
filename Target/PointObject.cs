using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 封裝座標點的物件(目標用)
    /// </summary>
    public class PointObject : ITargetability
    {
        /// <summary>
        /// 座標點
        /// </summary>
        public PointF Point { get; set; }

        /// <summary>
        /// 新增封裝座標點的物件(目標用)
        /// </summary>
        /// <param name="point"></param>
        public PointObject(PointF point)
        {
            Point = point;
        }

        /// <summary>
        /// 取得目標點X座標
        /// </summary>
        /// <param name="anchor">定位點,此處無用</param>
        /// <returns>目標點X座標</returns>
        public float GetTargetX(DirectionType anchor)
        {
            return Point.X;
        }

        /// <summary>
        /// 取得目標點Y座標
        /// </summary>
        /// <param name="anchor">定位點,此處無用</param>
        /// <returns>目標點Y座標</returns>
        public float GetTargetY(DirectionType anchor)
        {
            return Point.Y;
        }

        /// <summary>
        /// 取得目標點
        /// </summary>
        /// <param name="anchor">定位點,此處無用</param>
        /// <returns>目標點</returns>
        public PointF GetTargetPoint(DirectionType anchor)
        {
            return Point;
        }
    }
}
