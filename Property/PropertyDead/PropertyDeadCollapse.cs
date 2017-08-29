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
        /// 縮小時間計時器(毫秒)
        /// </summary>
        public CounterObject ShrinkTime { get; private set; }

        /// <summary>
        /// 符合指定的死亡方式才會觸發
        /// </summary>
        public ObjectDeadType DeadType { get; set; }

        /// <summary>
        /// 新增崩塌特性,擁有此特性的物件死亡時會逐漸縮小並碎裂
        /// </summary>
        /// <param name="scrapCount">每回合產生碎片數量</param>
        /// <param name="shrinkTime">縮小時間(毫秒)</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        public PropertyDeadCollapse(int scrapCount, int shrinkTime, ObjectDeadType deadType)
        {
            Status = PropertyStatus.Enabled;
            DeadType = deadType;
            ScrapCount = scrapCount;
            ShrinkTime = new CounterObject(shrinkTime);
        }


        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType)
        {
            if (Owner.DrawObject == null || (DeadType & deadType) != deadType) return;

            Owner.Status = ObjectStatus.Dying;
            Owner.Propertys.Add(new PropertyScraping((int)(ShrinkTime.Limit * 0.7F + 0.5F), ScrapCount, 2, 2, 50, 100, Owner.Scene.Sec(0.15F), Owner.Scene.Sec(0.25F)));
        }

        public override void DoAfterAction()
        {
            if (Owner.Status == ObjectStatus.Dying)
            {
                if (ShrinkTime.IsFull)
                {
                    Owner.Status = ObjectStatus.Dead;
                }
                else
                {
                    Owner.Layout.Scale = 1F - ShrinkTime.GetRatio();
                    ShrinkTime.Value += Owner.Scene.SceneIntervalOfRound;
                }
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
