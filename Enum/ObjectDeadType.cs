
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 物件死亡類型
    /// </summary>
    [Flags]
    public enum ObjectDeadType
    {
        /// <summary>
        /// 所有方式
        /// </summary>
        All = 255,

        /// <summary>
        /// 被清除
        /// </summary>
        Clear = 1,

        /// <summary>
        /// 生命時限到期
        /// </summary>
        LifeEnd = 2,

        /// <summary>
        /// 被碰撞
        /// </summary>
        Collision = 4
    }
}
