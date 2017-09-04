using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 特性狀態
    /// </summary>
    public enum PropertyStatus
    {
        /// <summary>
        /// 失效，等待回收
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// 生效
        /// </summary>
        Enabled = 1,

        /// <summary>
        /// 暫時失效
        /// </summary>
        Pause = 2
    }
}
