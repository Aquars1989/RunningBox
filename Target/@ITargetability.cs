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
    public interface ITargetability
    {
        /// <summary>
        /// 使用特定的定位位置取得目標點X座標
        /// </summary>
        /// <param name="anchor">定位位置</param>
        /// <returns>目標點X座標</returns>
        float GetTargetX(DirectionType anchor);

        /// <summary>
        /// 使用特定的定位位置取得目標點Y座標
        /// </summary>
        /// <param name="anchor">定位位置</param>
        /// <returns>目標點Y座標</returns>
        float GetTargetY(DirectionType anchor);

        /// <summary>
        /// 使用特定的定位位置取得目標點
        /// </summary>
        /// <param name="anchor">定位位置</param>
        /// <returns>目標點</returns>
        PointF GetTargetPoint(DirectionType anchor);
    }
}


