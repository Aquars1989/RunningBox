using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 凍結特性,凍結的物件無法移動
    /// </summary>
    class PropertyFreeze : PropertyBase
    {
        /// <summary>
        /// 新增凍結特性,凍結的物件無法移動
        /// </summary>
        /// <param name="durationTime">凍結時間(毫秒)</param>
        public PropertyFreeze(int durationTime)
        {
            Status = PropertyStatus.Enabled;
            DurationTime.Limit = durationTime;
        }

        public override void DoBeforeActionMove()
        {
            Owner.Moves.Clear();
        }

        public override void DoAfterAction() { }
        public override void DoBeforeAction() { }
        public override void DoBeforeActionPlan() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoBeforeEnd(PropertyEndType endType) { }
    }
}
