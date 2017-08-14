using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件每回合會在原位置產生四散的碎片
    /// </summary>
    class PropertySpeeded : PropertyBase
    {
        /// <summary>
        /// 每回合速度提升
        /// </summary>
        public float SpeededPerRound { get; set; }

        /// <summary>
        /// 新增碎裂特性，擁有此特性的物件每回合會在原位置產生四散的碎片
        /// </summary>
        /// <param name="durationRound">持續回合數</param>
        /// <param name="scrapCount">每回合產生碎片數量</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
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
