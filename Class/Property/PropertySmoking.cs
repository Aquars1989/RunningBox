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
        /// 縮小周期最大值(毫秒),小於0為永久
        /// </summary>
        public int ShrinkLimit { get; set; }

        /// <summary>
        /// 新增冒煙特性，擁有此特性的物件每回合會在原位置留下smoke物件
        /// </summary>
        /// <param name="duration">持續時間(毫秒),小於0為永久</param>
        /// <param name="size">smoke物件尺寸</param>
        /// <param name="shrinkTime">smoke縮小周期最大值(毫秒),小於0為永久</param>
        public PropertySmoking(int duration, int size, int shrinkTime)
        {
            Status = PropertyStatus.Enabled;
            DurationLimit = duration;
            ShrinkLimit = shrinkTime;
        }

        public override void DoAfterAction()
        {
            if (Owner.DrawObject == null) return;

            IDraw drawSmoke = Owner.DrawObject.Copy();
            drawSmoke.Opacity *= 0.2F;
            Owner.Container.Add(new ObjectSmoke(Owner.X, Owner.Y, Owner.Size, ShrinkLimit, drawSmoke));
        }

        public override void DoBeforeActionMove() { }
        public override void DoBeforeAction() { }
        public override void DoBeforeActionPlan() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoBeforeEnd(PropertyEndType endType) { }
    }
}
