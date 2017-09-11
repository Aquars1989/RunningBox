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
        /// 碎片外觀範本繪圖物件
        /// </summary>
        public DrawBase ScrapDrawObject { get; set; }

        /// <summary>
        /// 新增碎裂特性且指定碎片外觀，擁有此特性的物件每回合會在原位置產生四散的碎片
        /// </summary>
        /// <param name="scrapDrawObject">碎片外觀範本繪圖物件</param>
        /// <param name="duration">持續回合數,小於0為永久</param>
        /// <param name="scrapCount">每回合產生碎片數量</param>
        /// <param name="scrapWidth">碎片寬度</param>
        /// <param name="scrapHeight">碎片高度</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
        public PropertyScraping(DrawBase scrapDrawObject, int duration, int scrapCount, int scrapWidth, int scrapHeight, int scrapSpeedMin, int scrapSpeedMax, int scrapLifeMin, int scrapLifeMax)
            : this(duration, scrapCount, scrapWidth, scrapHeight, scrapSpeedMin, scrapSpeedMax, scrapLifeMin, scrapLifeMax)
        {
            ScrapDrawObject = scrapDrawObject;
        }

        /// <summary>
        /// 新增碎裂特性且指定碎片顏色，擁有此特性的物件每回合會在原位置產生四散的碎片
        /// </summary>
        /// <param name="color">產生碎片顏色</param>
        /// <param name="duration">持續回合數,小於0為永久</param>
        /// <param name="scrapCount">每回合產生碎片數量</param>
        /// <param name="scrapWidth">碎片寬度</param>
        /// <param name="scrapHeight">碎片高度</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
        public PropertyScraping(Color color, int duration, int scrapCount, int scrapWidth, int scrapHeight, int scrapSpeedMin, int scrapSpeedMax, int scrapLifeMin, int scrapLifeMax)
            : this(duration, scrapCount, scrapWidth, scrapHeight, scrapSpeedMin, scrapSpeedMax, scrapLifeMin, scrapLifeMax)
        {
            ScrapDrawObject = new DrawBrush(color, ShapeType.Ellipse);
        }

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
            : base(TargetNull.Value)
        {
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
            if (ScrapDrawObject == null && Owner.DrawObject == DrawNull.Value) return;

            for (int i = 0; i < ScrapCount; i++)
            {
                int speed = Global.Rand.Next(ScrapSpeedMin, Math.Max(ScrapSpeedMin, ScrapSpeedMax) + 1);
                int life = Global.Rand.Next(ScrapLifeMin, Math.Max(ScrapLifeMin, ScrapLifeMax) + 1);
                double scrapDirection = Global.Rand.NextDouble() * 360;

                TargetOffset moveTarget = new TargetOffset(TargetNull.Value, scrapDirection, 1000);
                MoveStraight moveObject = new MoveStraight(moveTarget, 1, speed, 1, 0, 1);
                ObjectScrap newObject;
                if (ScrapDrawObject == null)
                {
                    newObject = new ObjectScrap(Owner.Layout.CenterX, Owner.Layout.CenterY, ScrapWidth, ScrapHeight, life, Owner.DrawObject.Color, moveObject);
                }
                else
                {
                    DrawBase scrapDraw = ScrapDrawObject.Copy();
                    DrawPolygon scrapDrawPolygon = scrapDraw as DrawPolygon;

                    //多邊型時旋轉方向
                    if (scrapDrawPolygon != null)
                    {
                        scrapDrawPolygon.Angle = Global.Rand.Next(360);
                        scrapDrawPolygon.RotatingPerSec = Global.Rand.Next(280, 520);
                    }
                    newObject = new ObjectScrap(Owner.Layout.CenterX, Owner.Layout.CenterY, ScrapWidth, ScrapHeight, life, scrapDraw, moveObject);
                }
                moveTarget.Target = new TargetObject(newObject);
                Owner.Container.Add(newObject);
            }
        }

        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType)
        {
            //死亡不中斷
        }
    }
}
