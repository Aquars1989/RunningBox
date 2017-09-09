using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 附加狀態特性,單純增加附加狀態無特殊效果的特性
    /// </summary>
    public class PropertyAffix : PropertyBase
    {
        /// <summary>
        /// 新增附加狀態特性,單純增加附加狀態無特殊效果的特性
        /// </summary>
        /// <param name="affix">附加狀態</param>
        /// <param name="durationTime">持續時間(毫秒)</param>
        public PropertyAffix(SpecialStatus affix, int durationTime)
            : base(TargetNull.Value)
        {
            Affix = affix;
            DurationTime.Limit = durationTime;
        }
    }
}
