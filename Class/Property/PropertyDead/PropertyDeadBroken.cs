﻿using System;
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
        /// 碎片寬度
        /// </summary>
        public int ScrapWidth { get; set; }

        /// <summary>
        /// 碎片高度
        /// </summary>
        public int ScrapHeight { get; set; }

        /// <summary>
        /// 產生碎片數量
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
        /// 符合指定的死亡方式才會觸發
        /// </summary>
        public ObjectDeadType DeadType { get; set; }

        /// <summary>
        /// 新增破碎特性,擁有此特性的物件死亡時會造成碎片飛散
        /// </summary>
        /// <param name="scrapCount">產生碎片數量</param>
        /// <param name="scrapWidth">碎片寬度</param>
        /// <param name="scrapHeight">碎片高度</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
        public PropertyDeadBroken(int scrapCount, int scrapWidth, int scrapHeight, ObjectDeadType deadType, int scrapSpeedMax, int scrapSpeedMin, int scrapLifeMax, int scrapLifeMin)
        {
            Status = PropertyStatus.Enabled;
            DeadType = deadType;
            ScrapCount = scrapCount;
            ScrapWidth = scrapWidth;
            ScrapHeight = scrapHeight;
            ScrapSpeedMax = scrapSpeedMax;
            ScrapSpeedMin = scrapSpeedMin;
            ScrapLifeMax = scrapLifeMax;
            ScrapLifeMin = scrapLifeMin;
        }


        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType)
        {
            if (Owner.DrawObject == null || (DeadType & deadType) != deadType) return;

            float moveTotalX = 0;
            float moveTotalY = 0;
            foreach (PointF pt in Owner.Moves)
            {
                moveTotalX += pt.X;
                moveTotalY += pt.Y;
            }

            double angle = Function.GetAngle(0, 0, moveTotalX, moveTotalY);
            for (int i = 0; i < ScrapCount; i++)
            {
                int speed = Global.Rand.Next(ScrapSpeedMin, Math.Max(ScrapSpeedMin, ScrapSpeedMax) + 1);
                int life = Global.Rand.Next(ScrapLifeMin, Math.Max(ScrapLifeMin, ScrapLifeMax) + 1);
                double scrapDirection = angle + (Global.Rand.NextDouble() - 0.5) * 20;
                Owner.Container.Add(new ObjectScrap(Owner.Layout.CenterX, Owner.Layout.CenterY, ScrapWidth, ScrapHeight, speed, life, scrapDirection, Owner.DrawObject.Color));
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
