using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 自動施放基底
    /// </summary>
    public abstract class AutoCastBase
    {
        /// <summary>
        /// 施放機率 0-100
        /// </summary>
        public float Probability { get; set; }

        /// <summary>
        /// 是否啟用自動施放技能
        /// </summary>
        public bool Enabeld { get; set; }

        public AutoCastBase()
        {
            Enabeld = true;
        }

        /// <summary>
        /// 檢查是否要施放技能
        /// </summary>
        /// <param name="skill">技能</param>
        /// <returns>是否施放技能</returns>
        public bool Check(SkillBase skill)
        {
            if (Enabeld && skill.Status == SkillStatus.Disabled && skill.Owner.Energy >= skill.CostEnergy)
            {
                float n = (float)(Global.Rand.NextDouble() * 100);
                if (n < Probability)
                {
                    return Cast(skill);
                }
            }
            return false;
        }

        /// <summary>
        /// 符合條件時要如何施放技能
        /// </summary>
        /// <param name="skill">技能</param>
        /// <returns>是否施放技能</returns>
        protected abstract bool Cast(SkillBase skill);
    }
}
