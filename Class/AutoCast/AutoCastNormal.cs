using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 一般自動施放,目標為當前目標
    /// </summary>
    public class AutoCastNormal : AutoCastBase
    {
        /// <summary>
        /// 建立一般自動施放,目標為當前目標
        /// </summary>
        /// <param name="probability">施放機率</param>
        public AutoCastNormal(float probability)
        {
            Probability = probability;

        }

        protected override bool Cast(SkillBase skill)
        {
            skill.Use(skill.Owner.Target);
            return true;
        }
    }
}
