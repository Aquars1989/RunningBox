using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 依附於物件上的UI
    /// </summary>
    class PropertyUI : PropertyBase
    {
        /// <summary>
        /// 碰撞強度
        /// </summary>
        public int CollisionPower { get; set; }

        /// <summary>
        /// UI的大小
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// 繪製物件
        /// </summary>
        public DrawBase DrawObject { get; set; }

        /// <summary>
        /// 新增依附於物件上的UI特性
        /// </summary>
        /// <param name="duration">持續回合數,小於0為永久</param>
        /// <param name="size">UI尺寸</param>
        /// <param name="drawObject">繪製物件</param>
        public PropertyUI(int duration, Size size, DrawBase drawObject)
        {
            Size = size;
            DrawObject = drawObject;
            DurationTime.Limit = duration;
        }

        public override void DoAfterDraw(Graphics g)
        {
            int drawX = (int)(Owner.Layout.LeftTopX + (Owner.Layout.RectWidth - Size.Width) / 2 + 0.5F);
            int drawY = (int)(Owner.Layout.LeftTopY + Owner.Layout.RectHeight + 0.5F) + Owner.UIOffSetY + 10;
            Rectangle drawRectangle = new Rectangle(new Point(drawX, drawY), Size);
            DrawObject.Draw(g, drawRectangle);
            Owner.UIOffSetY += Size.Height + 5;
        }
    }
}
