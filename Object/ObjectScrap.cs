using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 虛擬物件,朝特定方向移動並淡出
    /// </summary>
    public class ObjectScrap : ObjectBase
    {
        /// <summary>
        /// 存活時間計數器(毫秒)
        /// </summary>
        public CounterObject Life { get; private set; }

        /// <summary>
        /// 移動速度
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public double Direction { get; set; }

        /// <summary>
        /// 使用顏色建立圓形虛擬物件,朝特定方向移動並淡出
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="speed">速度</param>
        /// <param name="life">存活時間(毫秒),小於0為永久</param>
        /// <param name="direction">方向</param>
        /// <param name="color">顏色</param>
        public ObjectScrap(float x, float y, int width, int height, float speed, int life, double direction, Color color) :
            this(x, y, width, height, speed, life, direction, new DrawBrush(color, ShapeType.Ellipse)) { }

        /// <summary>
        /// 使用繪製物件建立虛擬物件,朝特定方向移動並淡出
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="speed">速度</param>
        /// <param name="life">存活時間(毫秒),小於0為永久</param>
        /// <param name="direction">方向</param>
        /// <param name="drawObject">繪製物件</param>
        public ObjectScrap(float x, float y, int width, int height, float speed, int life, double direction, DrawBase drawObject)
        {
            Layout.CollisonShape = ShapeType.Ellipse;
            Layout.Anchor = ContentAlignment.MiddleCenter;
            Layout.X = x;
            Layout.Y = y;
            Layout.Width = width;
            Layout.Height = height;

            Status = ObjectStatus.Alive;
            Speed = speed;
            Life = new CounterObject(life);
            Direction = direction;
            DrawObject = drawObject;
        }

        public override void Action()
        {
            if (Life.IsFull)
            {
                Kill(null, ObjectDeadType.LifeEnd);
            }
            else
            {
                PointF move = GetMovePoint(Direction, Speed);
                Layout.X += move.X / Scene.SceneSlow;
                Layout.Y += move.Y / Scene.SceneSlow;
                DrawObject.Opacity = 1F - Life.GetRatio();
                Life.Value += Scene.SceneIntervalOfRound;
            }
        }
    }
}
