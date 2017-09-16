using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件每回合會在原位置留下smoke物件
    /// </summary>
    class PropertySmoking : PropertyBase
    {
        /// <summary>
        /// 縮小時間(毫秒),小於0為永久
        /// </summary>
        public int ShrinkLimit { get; set; }

        /// <summary>
        /// 產生的smoke物件的不透明度
        /// </summary>
        public float SmokeOpacity { get; set; }

        /// <summary>
        /// 新增冒煙特性，擁有此特性的物件每回合會在原位置留下smoke物件
        /// </summary>
        /// <param name="duration">持續時間(毫秒),小於0為永久</param>
        /// <param name="shrinkTime">smoke縮小時間(毫秒),小於0為永久</param>
        /// <param name="smokeOpacity">smoke物件的不透明度</param>
        public PropertySmoking(int duration, int shrinkTime, float smokeOpacity=0.2F)
        {
            DurationTime.Limit = duration;
            ShrinkLimit = shrinkTime;
            SmokeOpacity = smokeOpacity;
            BreakAfterDead = false;
        }

        public override void DoAfterAction()
        {
            if (Owner.DrawObject == null) return;

            DrawBase drawSmoke = Owner.DrawObject.Copy();
            drawSmoke.Colors.Opacity *= SmokeOpacity;
            Owner.Container.Add(new ObjectSmoke(Owner.Layout, ShrinkLimit, drawSmoke, MoveNull.Value));

            base.DoAfterAction();
        }
    }
}
