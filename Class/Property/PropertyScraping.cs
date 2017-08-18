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
        /// <param name="durationRound">持續回合數</param>
        /// <param name="scrapCount">每回合產生碎片數量</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
        public PropertyScraping(int durationRound, int scrapCount, int scrapSpeedMax = 200, int scrapSpeedMin = 100, int scrapLifeMax = 15, int scrapLifeMin = 10)
        {
            Status = PropertyStatus.Enabled;
            DurationRoundMax = durationRound;
            ScrapCount = scrapCount;
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
                Owner.Container.Add(new ObjectScrap(Owner.X, Owner.Y, 1, speed, life, scrapDirection, Owner.DrawObject.Color));
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
