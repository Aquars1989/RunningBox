
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 定義目標物件
    /// </summary>
    public interface ITarget
    {
        // <summary>
        /// 目標的X座標
        /// </summary>
        float X { get; }

        /// <summary>
        /// 目標的Y座標
        /// </summary>
        float Y { get; }

        /// <summary>
        /// 取得目標點
        /// </summary>
        /// <returns></returns>
        PointF GetPoint();
    }
}
