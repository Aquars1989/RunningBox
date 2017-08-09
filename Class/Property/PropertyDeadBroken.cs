using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件死亡時會造成碎片飛散
    /// </summary>
    class PropertyDeadBroken : PropertyBase
    {
        /// <summary>
        /// 產生碎片數量
        /// </summary>
        public int ScrapCount { get; set; }

        /// <summary>
        /// 新增破碎特性,擁有此特性的物件死亡時會造成碎片飛散
        /// </summary>
        /// <param name="durationRound">產生碎片數量</param>
        public PropertyDeadBroken(int scrapCount)
        {
            Status = PropertyStatus.Enabled;
            ScrapCount = scrapCount;
        }


        public override void DoAfterDead(ObjectActive killer)
        {
            if (Owner.DrawObject == null) return;
 
            float moveTotalX = 0;
            float moveTotalY = 0;
            foreach (PointF pt in Owner.Moves)
            {
                moveTotalX += pt.X;
                moveTotalY += pt.Y;
            }

            double direction = Function.PointRotation(0, 0, moveTotalX, moveTotalY);
            for (int i = 0; i < ScrapCount; i++)
            {
                int speed = Global.Rand.Next(300, 900);
                int life = Global.Rand.Next(25, 40);
                int size = Global.Rand.Next(1, 4) / 2;
                double scrapDirection = direction + (Global.Rand.NextDouble() - 0.5) * 20;
                Owner.ParentCollection.Add(new ObjectScrap(Owner.X, Owner.Y, 1, speed, life, scrapDirection,Owner.DrawObject.Color));
            }
        }

        public override void DoBeforeActionMove() { }
        public override void DoBeforeAction() { }
        public override void DoBeforeActionPlan() { }
        public override void DoAfterAction() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoBeforeEnd(PropertyEndType endType) { }
    }
}
