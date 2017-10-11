using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 虛擬物件,會逐漸縮小(放大)並淡出
    /// </summary>
    public class ObjectSmoke : ObjectBase
    {
        /// <summary>
        /// 新增虛擬物件,會逐漸縮小(放大)並淡出後消失
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="fadeTime">淡出時間(毫秒),小於0為永久</param>
        /// <param name="finelScale">計時器結束時大小比例</param>
        /// <param name="finelOpacity">計時器結束時透明度</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectSmoke(float x, float y, int width, int height, int fadeTime, float finelScale, float finelOpacity, DrawBase drawObject, MoveBase moveObject) :
            base(drawObject, moveObject)
        {
            Layout.CollisonShape = ShapeType.Ellipse;
            Layout.Anchor = DirectionType.Center;
            Layout.X = x;
            Layout.Y = y;
            Layout.Width = width;
            Layout.Height = height;
            Propertys.Add(new PropertyDeadSmoke(fadeTime, finelScale, finelOpacity, ObjectDeadType.All));
            Kill(null, ObjectDeadType.LifeEnd);
        }

        /// <summary>
        /// 使用指定的配置新增虛擬物件,會逐漸縮小(放大)並淡出後消失
        /// </summary>
        /// <param name="layout">配置資訊</param>
        /// <param name="fadeTime">淡出時間(毫秒),小於0為永久</param>
        /// <param name="finelScale">計時器結束時大小比例</param>
        /// <param name="finelOpacity">計時器結束時透明度</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectSmoke(LayoutSet layout, int fadeTime, float finelScale, float finelOpacity, DrawBase drawObject, MoveBase moveObject)
            : this(layout.CenterX, layout.CenterY, layout.RectWidth, layout.RectHeight, fadeTime, finelScale, finelOpacity, drawObject, moveObject) { }

        /// <summary>
        /// 使用顏色建立圓形虛擬物件,會逐漸縮小(放大)並淡出後消失
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="fadeTime">淡出時間(毫秒),小於0為永久</param>
        /// <param name="fadeTime">計時器結束時大小比例</param>
        /// <param name="fadeTime">計時器結束時透明度</param>
        /// <param name="color">顏色</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectSmoke(float x, float y, int width, int height, int fadeTime, float finelScale, float finelOpacity, Color color, MoveBase moveObject) :
            this(x, y, width, height, fadeTime, finelScale, finelOpacity, new DrawBrush(color, ShapeType.Ellipse), moveObject) { }
    }
}
