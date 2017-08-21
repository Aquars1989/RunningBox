using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件死亡時會逐漸縮小並碎裂
    /// </summary>
    class PropertyDeadCollapse : PropertyBase
    {
        /// <summary>
        /// 每回合產生碎片數量
        /// </summary>
        public int ScrapCount { get; set; }

        /// <summary>
        /// 縮小週期計數(毫秒)
        /// </summary>
        public int ShrinkTicks { get; set; }

        /// <summary>
        /// 縮小週期最大值(毫秒),小於0為永久
        /// </summary>
        public int ShrinkLimit { get; set; }

        /// <summary>
        /// 符合指定的死亡方式才會觸發
        /// </summary>
        public ObjectDeadType DeadType { get; set; }

        /// <summary>
        /// 新增崩塌特性,擁有此特性的物件死亡時會逐漸縮小並碎裂
        /// </summary>
        /// <param name="scrapCount">每回合產生碎片數量</param>
        /// <param name="shrinkRound">縮小週期(毫秒),小於0為永久</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        public PropertyDeadCollapse(int scrapCount, int shrinkRound, ObjectDeadType deadType)
        {
            Status = PropertyStatus.Enabled;
            DeadType = deadType;
            ScrapCount = scrapCount;
            ShrinkLimit = shrinkRound;
        }


        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType)
        {
            if (Owner.DrawObject == null || (DeadType & deadType) != deadType) return;

            Owner.Status = ObjectStatus.Dying;
            Owner.Propertys.Add(new PropertyScraping(0, ScrapCount, 200, 300, Owner.Scene.Sec(0.15F), Owner.Scene.Sec(0.25F)));
        }

        public override void DoAfterAction()
        {
            if (Owner.Status == ObjectStatus.Dying)
            {
                if (ShrinkTicks == ShrinkLimit)
                {
                    ShrinkTicks = 0;
                    if (Owner.Size > 1)
                    {
                        Owner.Size--;
                    }
                    else
                    {
                        Owner.Status = ObjectStatus.Dead;
                    }
                }
                ShrinkTicks++;
            }
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
