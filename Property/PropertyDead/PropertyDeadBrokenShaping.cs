using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件死亡時會造成碎片飛散(精準形狀)
    /// </summary>
    class PropertyDeadBrokenShaping : PropertyBase
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
        /// 擴散角度
        /// </summary>
        public int Radiation { get; set; }

        /// <summary>
        /// 符合指定的死亡方式才會觸發
        /// </summary>
        public ObjectDeadType DeadType { get; set; }

        /// <summary>
        /// 碎片外觀範本繪圖物件(可為null,null為使用所有者外觀)
        /// </summary>
        public DrawBase ScrapDrawObject { get; set; }

        /// <summary>
        /// 新增破碎特性且指定碎片外觀,擁有此特性的物件死亡時會造成碎片飛散
        /// </summary>
        /// <param name="scrapDrawObject">碎片外觀範本繪圖物件</param>
        /// <param name="scrapCount">產生碎片數量</param>
        /// <param name="scrapWidth">碎片寬度</param>
        /// <param name="scrapHeight">碎片高度</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        /// <param name="radiation">擴散角度</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
        public PropertyDeadBrokenShaping(DrawBase scrapDrawObject, int scrapCount, int scrapWidth, int scrapHeight, ObjectDeadType deadType, int radiation, int scrapSpeedMin, int scrapSpeedMax, int scrapLifeMin, int scrapLifeMax) :
            this(scrapCount, scrapWidth, scrapHeight, deadType, radiation, scrapSpeedMin, scrapSpeedMax, scrapLifeMin, scrapLifeMax)
        {
            ScrapDrawObject = scrapDrawObject;
        }

        /// <summary>
        /// 新增破碎特性且指定碎片顏色,擁有此特性的物件死亡時會造成碎片飛散
        /// </summary>
        /// <param name="color">產生碎片顏色</param>
        /// <param name="scrapCount">產生碎片數量</param>
        /// <param name="scrapWidth">碎片寬度</param>
        /// <param name="scrapHeight">碎片高度</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param
        /// <param name="radiation">擴散角度</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
        public PropertyDeadBrokenShaping(Color color, int scrapCount, int scrapWidth, int scrapHeight, ObjectDeadType deadType, int radiation, int scrapSpeedMin, int scrapSpeedMax, int scrapLifeMin, int scrapLifeMax) :
            this(scrapCount, scrapWidth, scrapHeight, deadType, radiation, scrapSpeedMin, scrapSpeedMax, scrapLifeMin, scrapLifeMax)
        {
            ScrapDrawObject = new DrawBrush(color, ShapeType.Ellipse);
        }

        /// <summary>
        /// 新增破碎特性且使用所有者顏色,擁有此特性的物件死亡時會造成碎片飛散
        /// </summary>
        /// <param name="scrapCount">產生碎片數量</param>
        /// <param name="scrapWidth">碎片寬度</param>
        /// <param name="scrapHeight">碎片高度</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param
        /// <param name="radiation">擴散角度</param>
        /// <param name="scrapSpeedMax">碎片移動速度最大值</param>
        /// <param name="scrapSpeedMin">碎片移動速度最小值</param>
        /// <param name="scrapLifeMax">碎片生命週期最大值</param>
        /// <param name="scrapLifeMin">碎片生命週期最小值</param>
        public PropertyDeadBrokenShaping(int scrapCount, int scrapWidth, int scrapHeight, ObjectDeadType deadType, int radiation, int scrapSpeedMin, int scrapSpeedMax, int scrapLifeMin, int scrapLifeMax)
        {
            DeadType = deadType;
            ScrapCount = scrapCount;
            ScrapWidth = scrapWidth;
            ScrapHeight = scrapHeight;
            Radiation = radiation;
            ScrapSpeedMax = scrapSpeedMax;
            ScrapSpeedMin = scrapSpeedMin;
            ScrapLifeMin = scrapLifeMin;
            ScrapLifeMax = scrapLifeMax;
            BreakAfterDead = false;
        }

        public override void DoAfterDead(ObjectBase killer, ObjectDeadType deadType)
        {
            if (Owner.DrawObject == DrawNull.Value || (DeadType & deadType) != deadType) return;

            double angle = Function.GetAngle(0, 0, Owner.MoveObject.MoveX, Owner.MoveObject.MoveY);

            DrawBase drawObject = Owner.DrawObject;
            Rectangle baseRectangle = Owner.Layout.Rectangle;
            Rectangle drawRectangle;
            if (drawObject.Scale > 1)
            {
                drawRectangle = drawObject.GetScaleRectangle(baseRectangle);
                baseRectangle.Location.Offset(-drawRectangle.Location.X, -drawRectangle.Location.Y);
                drawRectangle.Location = new Point(0, 0);
            }
            else
            {
                baseRectangle.Location = new Point(0, 0);
                drawRectangle = baseRectangle;
            }

            List<Point> getPoints = new List<Point>();
            using (Bitmap image = new Bitmap(drawRectangle.Width, drawRectangle.Height))
            using (Graphics g = Graphics.FromImage(image))
            {
                Owner.DrawObject.Draw(g, baseRectangle);
                BitmapData bitData = image.LockBits(drawRectangle, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                IntPtr scan0 = bitData.Scan0;
                unsafe
                {
                    byte* p = (byte*)scan0.ToPointer();
                    for (int y = 0; y < drawRectangle.Height; y++)
                    {
                        for (int x = 0; x < drawRectangle.Width; x++)
                        {
                            if (p[3] > 0)
                            {
                                getPoints.Add(new Point(x, y));
                            }
                            p += 4;
                        }
                    }
                }
                image.UnlockBits(bitData);

                if (getPoints.Count == 0) return;
                for (int i = 0; i < ScrapCount; i++)
                {
                    Point point = getPoints[Global.Rand.Next(getPoints.Count)];
                    PointF putPoint = new PointF(Owner.Layout.CenterX - (drawRectangle.Width / 2) + point.X,
                                                 Owner.Layout.CenterY - (drawRectangle.Width / 2) + point.Y);

                    int speed = Global.Rand.Next(ScrapSpeedMin, Math.Max(ScrapSpeedMin, ScrapSpeedMax));
                    int fadeTime = Global.Rand.Next(ScrapLifeMin, Math.Max(ScrapLifeMin, ScrapLifeMax));
                    double scrapDirection = angle + (Global.Rand.NextDouble() - 0.5) * Radiation;

                    MoveStraight moveObject = new MoveStraight(null, 1, speed, 1, 0, 1);
                    moveObject.Target.SetOffsetByAngle(scrapDirection, 1000);
                    ObjectSmoke newObject;
                    if (ScrapDrawObject == null)
                    {
                        Color drawColor = image.GetPixel(point.X, point.Y);
                        newObject = new ObjectSmoke(putPoint.X, putPoint.Y, ScrapWidth, ScrapHeight, fadeTime, 1, 0, drawColor, moveObject);
                    }
                    else
                    {
                        DrawBase scrapDraw = ScrapDrawObject.Copy();
                        scrapDraw.Angle = Global.Rand.Next(360);
                        newObject = new ObjectSmoke(putPoint.X, putPoint.Y, ScrapWidth, ScrapHeight, fadeTime, 1, 0, scrapDraw, moveObject);
                        newObject.Propertys.Add(new PropertyRotate(-1, Global.Rand.Next(280, 520), false, true));
                    }
                    moveObject.Target.SetObject(newObject);
                    Owner.Container.Add(newObject);
                }
            }
            base.DoAfterDead(killer, deadType);
        }
    }
}
