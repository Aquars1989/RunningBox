
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 活動物件狀態
    /// </summary>
    public enum ObjectStatus
    {
        /// <summary>
        /// 存活
        /// </summary>
        Alive = 0,

        /// <summary>
        /// 死亡,等待回收
        /// </summary>
        Dead = 1
    }
}
