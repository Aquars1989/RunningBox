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
        /// 縮小周期計數(毫秒)
        /// </summary>
        public int ShrinkTicks { get; set; }

        /// <summary>
        /// 縮小周期最大值(毫秒),小於0為永久
        /// </summary>
        public int ShrinkLimit { get; set; }

        /// <summary>
        /// 新增虛擬物件,會逐漸縮小直到消失
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="size">物件大小</param>
        /// <param name="shrinkRound">縮小周期(毫秒),小於0為永久</param>
        /// <param name="drawObject">繪製物件</param>
        public ObjectSmoke(float x, float y, int size, int shrinkRound, IDraw drawObject)
        {
            Status = ObjectStatus.Alive;
            X = x;
            Y = y;
            //Size = size;
            ShrinkLimit = shrinkRound;
            DrawObject = drawObject;
        }

        public override void Action()
        {
            if (ShrinkLimit >= 0 && ShrinkTicks >= ShrinkLimit)
            {
                Size--;
                if (Size == 0)
                {
                    Kill(null, ObjectDeadType.LifeEnd);
                }
                else
                {
                    ShrinkTicks = 0;
                }
            }
            ShrinkTicks += Scene.SceneIntervalOfRound;
        }
    }
}
