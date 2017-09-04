
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能狀態
    /// </summary>
    public enum SkillStatus
    {
        /// <summary>
        /// 失效
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// 生效
        /// </summary>
        Enabled = 1,

        /// <summary>
        /// 引導中
        /// </summary>
        Channeled = 2,

        /// <summary>
        /// 冷卻中
        /// </summary>
        Cooldown = 3
    }
}
