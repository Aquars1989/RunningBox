using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 特效狀態
    /// </summary>
    public enum EffectStatus
    {
        /// <summary>
        /// 失效，等待回收
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// 生效中
        /// </summary>
        Enabling = 1,

        /// <summary>
        /// 生效
        /// </summary>
        Enabled = 2,

        /// <summary>
        /// 失效中
        /// </summary>
        Disabling = 3
    }
}
