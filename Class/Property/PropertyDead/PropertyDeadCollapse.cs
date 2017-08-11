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
        /// 縮小計時器
        /// </summary>
        public int ShrinkRound { get; set; }

        /// <summary>
        /// 縮小計時器最大值
        /// </summary>
        public int ShrinkRoundMax { get; set; }

        /// <summary>
        /// 符合指定的死亡方式才會觸發
        /// </summary>
        public ObjectDeadType DeadType { get; set; }

        /// <summary>
        /// 新增崩塌特性,擁有此特性的物件死亡時會逐漸縮小並碎裂
        /// </summary>
        /// <param name="scrapCount">每回合產生碎片數量</param>
        /// <param name="shrinkRound">每次縮小的週期</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        public PropertyDeadCollapse(int scrapCount, int shrinkRound, ObjectDeadType deadType)
        {
            Status = PropertyStatus.Enabled;
            DeadType = deadType;
            ScrapCount = scrapCount;
            ShrinkRoundMax = shrinkRound;
        }


        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType)
        {
            if (Owner.DrawObject == null || (DeadType & deadType) != deadType) return;

            Owner.Status = ObjectStatus.Dying;
            Owner.Propertys.Add(new PropertyScraping(0, ScrapCount));
        }

        public override void DoAfterAction()
        {
            if (Owner.Status == ObjectStatus.Dying)
            {
                if (ShrinkRound == ShrinkRoundMax)
                {
                    ShrinkRound = 0;
                    if (Owner.Size > 1)
                    {
                        Owner.Size--;
                    }
                    else
                    {
                        Owner.Status = ObjectStatus.Dead;
                    }
                }
                ShrinkRound++;
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
