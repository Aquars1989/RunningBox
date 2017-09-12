﻿using System;
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
        /// 新增冒煙特性，擁有此特性的物件每回合會在原位置留下smoke物件
        /// </summary>
        /// <param name="duration">持續時間(毫秒),小於0為永久</param>
        /// <param name="shrinkTime">smoke縮小時間(毫秒),小於0為永久</param>
        public PropertySmoking(int duration, int shrinkTime)
            : base(TargetNull.Value)
        {
            DurationTime.Limit = duration;
            ShrinkLimit = shrinkTime;
        }

        public override void DoAfterAction()
        {
            if (Owner.DrawObject == null) return;

            DrawBase drawSmoke = Owner.DrawObject.Copy();
            drawSmoke.Colors.Opacity *= 0.2F;
            Owner.Container.Add(new ObjectSmoke(Owner.Layout, ShrinkLimit, drawSmoke, MoveNull.Value));
        }

        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType)
        {
            //死亡不中斷
        }
    }
}
