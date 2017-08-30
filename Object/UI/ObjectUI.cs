using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 基礎介面物件
    /// </summary>
    public class ObjectUI : ObjectBase
    {
        public bool Enabled { get; set; }

        public override void Action() { }

        /// <summary>
        /// 使用指定的定位點建立介面物件
        /// </summary>
        /// <param name="anchor">定位點位置X</param>
        /// <param name="x">物件左上位置X</param>
        /// <param name="y">物件左上位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="drawObject">繪製物件</param>
        public ObjectUI(ContentAlignment anchor, int x, int y, int width, int height, DrawBase drawObject)
        {
            Layout.CollisonShape = ShapeType.Rectangle;
            Layout.Anchor = anchor;
            Layout.X = x;
            Layout.Y = y;
            Layout.Width = width;
            Layout.Height = height;
            DrawObject = drawObject;
        }

        /// <summary>
        /// 使用預設定位點(左上)建立介面物件
        /// </summary>
        /// <param name="x">物件左上位置X</param>
        /// <param name="y">物件左上位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="drawObject">繪製物件</param>
        public ObjectUI(int x, int y, int width, int height, DrawBase drawObject) :
            this(ContentAlignment.TopLeft, x, y, width, height, drawObject) { }
    }
}
