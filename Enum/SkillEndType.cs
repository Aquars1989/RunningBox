
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能結束類型
    /// </summary>
    public enum SkillEndType
    {
        /// <summary>
        /// 持續時間結束
        /// </summary>
        Finish = 0,

        /// <summary>
        /// 施放前被中斷
        /// </summary>
        CastBreak = 1,

        /// <summary>
        /// 引導時被中斷
        /// </summary>
        ChanneledBreak = 2
    }
}
