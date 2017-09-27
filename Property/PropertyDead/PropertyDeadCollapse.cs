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
        private PropertyScraping _PropertyScraping; //碎裂特性
        private bool _Enabled = false;

        /// <summary>
        /// 碎片寬度
        /// </summary>
        public int ScrapWidth { get; set; }

        /// <summary>
        /// 碎片高度
        /// </summary>
        public int ScrapHeight { get; set; }

        /// <summary>
        /// 產生碎片週期(毫秒)
        /// </summary>
        public int BuildTime { get; set; }

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
        /// 縮小時間計時器(毫秒)
        /// </summary>
        public CounterObject ShrinkTime { get; private set; }

        /// <summary>
        /// 符合指定的死亡方式才會觸發
        /// </summary>
        public ObjectDeadType DeadType { get; set; }

        /// <summary>
        /// 碎片外觀範本繪圖物件
        /// </summary>
        public DrawBase ScrapDrawObject { get; set; }

        /// <summary>
        /// 新增崩塌特性且指定碎片外觀,擁有此特性的物件死亡時會逐漸縮小並碎裂
        /// </summary>
        /// <param name="scrapDrawObject">碎片外觀範本繪圖物件</param>
        /// <param name="buildTime">產生碎片週期(毫秒)</param>
        /// <param name="scrapCount">每回合產生碎片數量</param>
        /// <param name="shrinkTime">縮小時間(毫秒)</param>
        /// <param name="scrapWidth">碎片寬度</param>
        /// <param name="scrapHeight">碎片高度</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
        public PropertyDeadCollapse(DrawBase scrapDrawObject, int scrapCount, int shrinkTime, int buildTime, int scrapWidth, int scrapHeight, ObjectDeadType deadType, int scrapSpeedMin, int scrapSpeedMax, int scrapLifeMin, int scrapLifeMax)
            : this(scrapCount, shrinkTime, buildTime, scrapWidth, scrapHeight, deadType, scrapSpeedMin, scrapSpeedMax, scrapLifeMin, scrapLifeMax)
        {
            ScrapDrawObject = scrapDrawObject;
        }

        /// <summary>
        /// 新增崩塌特性且指定碎片顏色,擁有此特性的物件死亡時會逐漸縮小並碎裂
        /// </summary>
        /// <param name="color">產生碎片顏色</param>
        /// <param name="buildTime">產生碎片週期(毫秒)</param>
        /// <param name="scrapCount">每回合產生碎片數量</param>
        /// <param name="shrinkTime">縮小時間(毫秒)</param>
        /// <param name="scrapWidth">碎片寬度</param>
        /// <param name="scrapHeight">碎片高度</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
        public PropertyDeadCollapse(Color color, int scrapCount, int shrinkTime, int buildTime, int scrapWidth, int scrapHeight, ObjectDeadType deadType, int scrapSpeedMin, int scrapSpeedMax, int scrapLifeMin, int scrapLifeMax)
            : this(scrapCount, shrinkTime, buildTime, scrapWidth, scrapHeight, deadType, scrapSpeedMin, scrapSpeedMax, scrapLifeMin, scrapLifeMax)
        {
            ScrapDrawObject = new DrawBrush(color, ShapeType.Ellipse);
        }

        /// <summary>
        /// 新增崩塌特性且使用所有者顏色,擁有此特性的物件死亡時會逐漸縮小並碎裂
        /// </summary>
        /// <param name="buildTime">產生碎片週期(毫秒)</param>
        /// <param name="scrapCount">每回合產生碎片數量</param>
        /// <param name="shrinkTime">縮小時間(毫秒)</param>
        /// <param name="scrapWidth">碎片寬度</param>
        /// <param name="scrapHeight">碎片高度</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
        public PropertyDeadCollapse(int scrapCount, int shrinkTime, int buildTime, int scrapWidth, int scrapHeight, ObjectDeadType deadType, int scrapSpeedMin, int scrapSpeedMax, int scrapLifeMin, int scrapLifeMax)
        {
            DeadType = deadType;
            BuildTime = buildTime;
            ScrapCount = scrapCount;
            ScrapWidth = scrapWidth;
            ScrapHeight = scrapHeight;
            ShrinkTime = new CounterObject(shrinkTime);
            ScrapSpeedMax = scrapSpeedMax;
            ScrapSpeedMin = scrapSpeedMin;
            ScrapLifeMax = scrapLifeMax;
            ScrapLifeMin = scrapLifeMin;
            BreakAfterDead = false;
        }


        public override void DoAfterDead(ObjectBase killer, ObjectDeadType deadType)
        {
            if ((DeadType & deadType) != deadType) return;

            if (ScrapDrawObject == null)
            {
                _PropertyScraping = new PropertyScraping((int)(ShrinkTime.Limit * 0.8F), BuildTime, ScrapCount, ScrapWidth, ScrapHeight, ScrapSpeedMin, ScrapSpeedMax, ScrapLifeMin, ScrapLifeMax);
            }
            else
            {
                _PropertyScraping = new PropertyScraping(ScrapDrawObject, (int)(ShrinkTime.Limit * 0.8F), BuildTime, ScrapCount, ScrapWidth, ScrapHeight, ScrapSpeedMin, ScrapSpeedMax, ScrapLifeMin, ScrapLifeMax);
            }
            Owner.Propertys.Add(_PropertyScraping);
            Affix = SpecialStatus.Remain;
            ShrinkTime.Value = 0;
            _Enabled = true;

            base.DoAfterDead(killer, deadType);
        }

        public override void DoAfterAction()
        {
            if (Owner.Status == ObjectStatus.Dead && _Enabled)
            {
                if (ShrinkTime.IsFull)
                {
                    _PropertyScraping.Break();
                    Affix = SpecialStatus.None;
                    _Enabled = false;
                }
                else
                {
                    Owner.Layout.Scale = 1F - ShrinkTime.GetRatio();
                    ShrinkTime.Value += Scene.SceneIntervalOfRound;
                }
            }

            base.DoAfterAction();
        }
    }
}
