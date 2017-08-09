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
        /// 淡出計時器
        /// </summary>
        public int FadeTick { get; set; }

        /// <summary>
        /// 淡出計時器最大值
        /// </summary>
        public int FadeTickMax { get; set; }

        /// <summary>
        /// 新增虛擬物件,會逐漸縮小直到消失
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="size">物件大小</param>
        /// <param name="fadeTick">每次縮小的週期</param>
        /// <param name="drawObject">繪製物件</param>
        public ObjectSmoke(float x, float y, int size, int fadeTick, IDraw drawObject)
        {
            Status = ObjectStatus.Dying;
            X = x;
            Y = y;
            Size = size;
            FadeTickMax = fadeTick;
            DrawObject = drawObject;
        }

        public override void Action()
        {
            if (FadeTick >= FadeTickMax)
            {
                Size--;
                if (Size == 0)
                {
                    Kill(null);
                }
                else
                {
                    FadeTick = 0;
                }
            }
            FadeTick++;
        }
    }
}
