using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件每回合速度會增加
    /// </summary>
    class PropertySpeeded : PropertyBase
    {
        /// <summary>
        /// 每回合速度提升
        /// </summary>
        public float SpeededPerRound { get; set; }

        /// <summary>
        /// 新增加速特性,擁有此特性的物件每回合速度會增加
        /// </summary>
        /// <param name="durationRound">持續回合數</param>
        /// <param name="speededPerRound">每回合移動速度增加</param>
        public PropertySpeeded(int durationRound, float speededPerRound)
        {
            Status = PropertyStatus.Enabled;
            DurationRoundMax = durationRound;
            SpeededPerRound = speededPerRound;
        }

        public override void DoAfterAction()
        {
            Owner.Speed += SpeededPerRound;
        }

        public override void DoBeforeEnd(PropertyEndType endType)
        {
            Owner.Speed -= DurationRound * SpeededPerRound;
        }

        public override void DoBeforeActionMove() { }
        public override void DoBeforeAction() { }
        public override void DoBeforeActionPlan() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
    }
}
