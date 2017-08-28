﻿using System;
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
        public CounterObject ShrinkTime { get; private set; }

        /// <summary>
        /// 新增虛擬物件,會逐漸縮小直到消失
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="shrinkTime">縮小時間(毫秒),小於0為永久</param>
        /// <param name="drawObject">繪製物件</param>
        public ObjectSmoke(float x, float y, int width, int height, int shrinkTime, IDraw drawObject)
        {
            Layout.Anchor = ContentAlignment.MiddleCenter;
            Layout.X = x;
            Layout.Y = y;
            Layout.Width = width;
            Layout.Height = height;

            Status = ObjectStatus.Alive;
            ShrinkTime = new CounterObject(shrinkTime);
            DrawObject = drawObject;
        }

        /// <summary>
        /// 使用指定的配置新增虛擬物件,會逐漸縮小直到消失
        /// </summary>
        /// <param name="layout">配置資訊</param>
        /// <param name="shrinkTime">縮小時間(毫秒),小於0為永久</param>
        /// <param name="drawObject">繪製物件</param>
        public ObjectSmoke(Layout layout, int shrinkTime, IDraw drawObject)
        {
            Layout.Anchor = ContentAlignment.MiddleCenter;
            Layout.X = layout.CenterX;
            Layout.Y = layout.CenterY;
            Layout.Width = layout.RectWidth;
            Layout.Height = layout.RectHeight;

            Status = ObjectStatus.Alive;
            ShrinkTime = new CounterObject(shrinkTime);
            DrawObject = drawObject;
        }

        public override void Action()
        {
            if (ShrinkTime.IsFull)
            {
                Kill(null, ObjectDeadType.LifeEnd);
            }
            else
            {
                Layout.Scale = 1F - ShrinkTime.GetRatio();
                ShrinkTime.Value += Scene.SceneIntervalOfRound;
            }
        }
    }
}
