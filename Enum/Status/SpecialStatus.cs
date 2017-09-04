using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 特疏狀態(由特性提供,附加於物件上)
    /// </summary>
    [Flags]
    public enum SpecialStatus
    {
        /// <summary>
        /// 無狀態
        /// </summary>
        None = 0
    }
}
