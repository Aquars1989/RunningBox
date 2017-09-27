using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 方向列舉
    /// </summary>
    [Flags]
    public enum DirectionType
    {
        /// <summary>
        /// 中心
        /// </summary>
        Center = 0,

        /// <summary>
        /// 左
        /// </summary>
        Left = 1,

        /// <summary>
        /// 右
        /// </summary>
        Right = 2,

        /// <summary>
        /// 上
        /// </summary>
        Top = 4,

        /// <summary>
        /// 下
        /// </summary>
        Bottom = 8,

        /// <summary>
        /// 左上
        /// </summary>
        TopLeft = 5,

        /// <summary>
        /// 右上
        /// </summary>
        TopRight = 6,

        /// <summary>
        /// 左下
        /// </summary>
        BottomLeft =9,
        
        /// <summary>
        /// 右下
        /// </summary>
        BottomRight = 10
    }
}
