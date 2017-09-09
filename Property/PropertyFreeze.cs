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
            : base(TargetNull.Value)
        {
            DurationTime.Limit = durationTime;
        }

        public override void DoBeforeActionMove()
        {
            Owner.MoveObject.ClearOffset();
        }
    }
}
