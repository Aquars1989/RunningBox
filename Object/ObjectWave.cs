using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 虛擬物件,會逐漸擴散然後慢慢消失
    /// </summary>
    public class ObjectWave : ObjectBase
    {
        /// <summary>
        /// 擴散時間計時器(毫秒),小於0為永久
        /// </summary>
        public CounterObject DiffusionTime { get; private set; }

        /// <summary>
        /// 消失時間計時器(毫秒)
        /// </summary>
        public CounterObject FadeTime { get; private set; }

        /// <summary>
        /// 原始寬度
        /// </summary>
        public int BaseWidth { get; set; }

        /// <summary>
        /// 原始高度
        /// </summary>
        public int BaseHeight { get; set; }

        /// <summary>
        /// 原始不透明度
        /// </summary>
        public float BaseOpacity { get; set; }

        /// <summary>
        /// 擴散寬度
        /// </summary>
        public int DiffusionWidth { get; set; }

        /// <summary>
        /// 擴散高度
        /// </summary>
        public int DiffusionHeight { get; set; }

        /// <summary>
        /// 擴散後不透明度
        /// </summary>
        public float DiffusionOpacity { get; set; }

        /// <summary>
        /// 開始淡出時不透明度
        /// </summary>
        public float FadeOpacity { get; set; }

        /// <summary>
        /// 新增虛擬物件,會逐漸縮小直到消失
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="baseWidth">原始寬度</param>
        /// <param name="baseHeight">原始高度</param>
        /// <param name="diffusionWidth">擴散寬度</param>
        /// <param name="diffusionHeight">擴散高度</param>
        /// <param name="diffusionTime">擴散時間(毫秒)</param>
        /// <param name="fadeTime">消失時間(毫秒)</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectWave(float x, float y, int baseWidth, int baseHeight, int diffusionWidth, int diffusionHeight, int diffusionTime, int fadeTime, DrawBase drawObject, MoveBase moveObject) :
            base(drawObject, moveObject)
        {
            Layout.CollisonShape = ShapeType.Ellipse;
            Layout.Anchor = ContentAlignment.MiddleCenter;
            Layout.X = x;
            Layout.Y = y;
            Layout.Width = baseWidth;
            Layout.Height = baseHeight;
            BaseWidth = baseWidth;
            BaseHeight = baseHeight;
            BaseOpacity = 1;
            DiffusionWidth = diffusionWidth;
            DiffusionHeight = diffusionHeight;
            DiffusionOpacity = 1;
            FadeOpacity = 1;
            DiffusionTime = new CounterObject(diffusionTime);
            FadeTime = new CounterObject(fadeTime);
        }


        /// <summary>
        /// 使用指定的配置新增虛擬物件,會逐漸縮小直到消失
        /// </summary>
        /// <param name="layout">配置資訊</param>
        /// <param name="diffusionWidth">擴散寬度</param>
        /// <param name="diffusionHeight">擴散高度</param>
        /// <param name="diffusionTime">擴散時間(毫秒)</param>
        /// <param name="fadeTime">消失時間(毫秒)</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectWave(LayoutSet layout, int diffusionWidth, int diffusionHeight, int diffusionTime, int fadeTime, DrawBase drawObject, MoveBase moveObject)
            : this(layout.CenterX, layout.CenterY, layout.RectWidth, layout.RectHeight, diffusionWidth, diffusionHeight, diffusionTime, fadeTime, drawObject, moveObject) { }

        public override void Action()
        {
            if (DiffusionTime.IsFull)
            {
                if (FadeTime.IsFull)
                {
                    Kill(null, ObjectDeadType.LifeEnd);
                }
                else
                {
                    DrawObject.Colors.Opacity = FadeOpacity * (1 - FadeTime.GetRatio());
                    FadeTime.Value += Scene.SceneIntervalOfRound;
                    base.Action();

                }
            }
            else
            {
                Layout.Width = BaseWidth + (int)((DiffusionWidth - BaseWidth) * DiffusionTime.GetRatio());
                Layout.Height = BaseHeight + (int)((DiffusionWidth - BaseWidth) * DiffusionTime.GetRatio());
                DrawObject.Colors.Opacity = BaseOpacity + ((DiffusionOpacity - BaseOpacity) * DiffusionTime.GetRatio());
                DiffusionTime.Value += Scene.SceneIntervalOfRound;
                base.Action();
            }
        }
    }
}
