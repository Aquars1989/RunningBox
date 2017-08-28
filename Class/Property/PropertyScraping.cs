using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件每回合會在原位置產生四散的碎片
    /// </summary>
    class PropertyScraping : PropertyBase
    {
        /// <summary>
        /// 每回合產生碎片數量
        /// </summary>
        public int ScrapCount { get; set; }

        /// <summary>
        /// 碎片寬度
        /// </summary>
        public int ScrapWidth { get; set; }

        /// <summary>
        /// 碎片高度
        /// </summary>
        public int ScrapHeight { get; set; }

        /// <summary>
        /// 碎片移動速度最大值
        /// </summary>
        public int ScrapSpeedMax { get; set; }

        /// <summary>
        /// 碎片移動速度最小值
        /// </summary>
        public int ScrapSpeedMin { get; set; }

        /// <summary>
        /// 碎片生命週期最大值
        /// </summary>
        public int ScrapLifeMax { get; set; }

        /// <summary>
        /// 碎片生命週期最小值
        /// </summary>
        public int ScrapLifeMin { get; set; }

        /// <summary>
        /// 新增碎裂特性，擁有此特性的物件每回合會在原位置產生四散的碎片
        /// </summary>
        /// <param name="duration">持續回合數,小於0為永久</param>
        /// <param name="scrapCount">每回合產生碎片數量</param>
        /// <param name="scrapWidth">碎片寬度</param>
        /// <param name="scrapHeight">碎片高度</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
        public PropertyScraping(int duration, int scrapCount, int scrapWidth, int scrapHeight, int scrapSpeedMin, int scrapSpeedMax, int scrapLifeMin, int scrapLifeMax)
        {
            Status = PropertyStatus.Enabled;
            DurationTime.Limit = duration;
            ScrapCount = scrapCount;
            ScrapWidth = scrapWidth;
            ScrapHeight = scrapHeight;
            ScrapSpeedMax = scrapSpeedMax;
            ScrapSpeedMin = scrapSpeedMin;
            ScrapLifeMax = scrapLifeMax;
            ScrapLifeMin = scrapLifeMin;
        }

        public override void DoAfterAction()
        {
            if (Owner.DrawObject == null) return;

            for (int i = 0; i < ScrapCount; i++)
            {
                int speed = Global.Rand.Next(ScrapSpeedMin, Math.Max(ScrapSpeedMin, ScrapSpeedMax) + 1);
                int life = Global.Rand.Next(ScrapLifeMin, Math.Max(ScrapLifeMin, ScrapLifeMax) + 1);
                double scrapDirection = Global.Rand.NextDouble() * 360 - 180;
                Owner.Container.Add(new ObjectScrap(Owner.Layout.CenterX, Owner.Layout.CenterY, ScrapWidth, ScrapHeight, speed, life, scrapDirection, Owner.DrawObject.Color));
            }
        }

        public override void DoBeforeActionMove() { }
        public override void DoBeforeAction() { }
        public override void DoBeforeActionPlan() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoBeforeEnd(PropertyEndType endType) { }
        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType) { }
    }
}
