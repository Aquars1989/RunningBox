using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 特性生效延遲器
    /// </summary>
    class PropertyDelay : PropertyBase
    {
        /// <summary>
        /// 要延遲生效的特性
        /// </summary>
        public PropertyBase Property { get; set; }

        /// <summary>
        /// 新增特性生效延遲器
        /// </summary>
        /// <param name="duration">持續時間(毫秒),小於0為永久</param>
        /// <param name="property">要延遲生效的特性</param>
        public PropertyDelay(int duration, PropertyBase property)
        {
            DurationTime.Limit = duration;
            Property = property;
        }

        public override void DoBeforeEnd(PropertyEndType endType)
        {
            if (endType == PropertyEndType.Finish && Property != null)
            {
                Owner.Propertys.Add(Property);
            }
            base.DoBeforeEnd(endType);
        }
    }
}
