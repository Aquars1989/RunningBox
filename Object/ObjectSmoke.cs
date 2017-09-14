using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 虛擬物件,會逐漸縮小直到消失
    /// </summary>
    public class ObjectSmoke : ObjectBase
    {
        /// <summary>
        /// 縮小時間計時器(毫秒)
        /// </summary>
        public CounterObject Life { get; private set; }

        /// <summary>
        /// 新增虛擬物件,會逐漸縮小直到消失
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="life">縮小時間(毫秒),小於0為永久</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectSmoke(float x, float y, int width, int height, int life, DrawBase drawObject, MoveBase moveObject) :
            base(drawObject, moveObject)
        {
            Layout.CollisonShape = ShapeType.Ellipse;
            Layout.Anchor = ContentAlignment.MiddleCenter;
            Layout.X = x;
            Layout.Y = y;
            Layout.Width = width;
            Layout.Height = height;
            Life = new CounterObject(life);
        }

        /// <summary>
        /// 使用指定的配置新增虛擬物件,會逐漸縮小直到消失
        /// </summary>
        /// <param name="layout">配置資訊</param>
        /// <param name="life">縮小時間(毫秒),小於0為永久</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectSmoke(LayoutSet layout, int life, DrawBase drawObject, MoveBase moveObject)
            : this(layout.CenterX, layout.CenterY, layout.RectWidth, layout.RectHeight, life, drawObject, moveObject) { }

        public override void Action()
        {
            if (Life.IsFull)
            {
                Kill(null, ObjectDeadType.LifeEnd);
            }
            else
            {
                Layout.Scale = 1F - Life.GetRatio();
                Life.Value += Scene.SceneIntervalOfRound;
                base.Action();
            }
        }
    }
}
