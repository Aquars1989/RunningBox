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
    public class PropertyAlert : PropertyBase
    {


        /// <summary>
        /// 新增附加狀態特性,單純增加附加狀態無特殊效果的特性
        /// </summary>
        /// <param name="durationTime">持續時間(毫秒)</param>
        public PropertyAlert(int durationTime)
        {
            DurationTime.Limit = durationTime;
        }

        public override void DoAfterDraw(Graphics g)
        {
            g.DrawString("!",);
            base.DoAfterDraw(g);
        }
    }
}
