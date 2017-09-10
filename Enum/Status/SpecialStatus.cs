using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 特殊狀態(由特性提供,附加於物件上)
    /// </summary>
    [Flags]
    public enum SpecialStatus
    {
        /// <summary>
        /// 無狀態
        /// </summary>
        None = 0,

        /// <summary>
        /// 死亡時不清除
        /// </summary>
        Remain = 1,

        /// <summary>
        /// 可碰撞的
        /// </summary>
        Collision = 2,

        /// <summary>
        /// 需拆分移動步驟
        /// </summary>
        Movesplit = 4,

        /// <summary>
        /// 幽靈,暫停碰撞
        /// </summary>
        Ghost = 8,

        /// <summary>
        /// 潛行,不會被鎖定
        /// </summary>
        Sneak = 16
    }
}
