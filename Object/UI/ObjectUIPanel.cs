using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 基礎容器介面
    /// </summary>
    public class ObjectUIPanel : ObjectUI
    {
        public ObjectCollection UIObjects { get; private set; }

        protected override void OnSceneChanged()
        {
            if (Scene != null)
            {
                UIObjects.Scene = Scene;
            }
            base.OnSceneChanged();
        }

        /// <summary>
        /// 使用指定的定位點和移動物件建立介面物件
        /// </summary>
        /// <param name="x">物件左上位置X</param>
        /// <param name="y">物件左上位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectUIPanel(ContentAlignment anchor, int x, int y, int width, int height, DrawBase drawObject, MoveBase moveObject)
            : base(anchor, x, y, width, height, drawObject, moveObject)
        {
            UIObjects = new ObjectCollection(SceneNull.Value);
        }

        /// <summary>
        /// 使用指定的定位點建立不可移動介面物件
        /// </summary>
        /// <param name="anchor">定位點位置X</param>
        /// <param name="x">物件左上位置X</param>
        /// <param name="y">物件左上位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="drawObject">繪製物件</param>
        public ObjectUIPanel(ContentAlignment anchor, int x, int y, int width, int height, DrawBase drawObject)
            : base(anchor, x, y, width, height, drawObject)
        {
            UIObjects = new ObjectCollection(SceneNull.Value);
        }

        /// <summary>
        /// 使用預設定位點(左上)建立不可移動介面物件
        /// </summary>
        /// <param name="x">物件左上位置X</param>
        /// <param name="y">物件左上位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectUIPanel(int x, int y, int width, int height, DrawBase drawObject)
            : this(ContentAlignment.TopLeft, x, y, width, height, drawObject)
        {
            UIObjects = new ObjectCollection(SceneNull.Value);
        }

        /// <summary>
        /// 檢查座標是否在此物件內
        /// </summary>
        /// <param name="point">檢查座標</param>
        public override ObjectUI InRectangle(Point point)
        {
            ObjectUI result = base.InRectangle(point);
            if (result == null) return null;

            for (int i = UIObjects.Count - 1; i >= 0; i--)
            {
                ObjectUI child = UIObjects[i] as ObjectUI;
                if (child != null)
                {
                    ObjectUI chileFocus = child.InRectangle(point);
                    if (chileFocus != null)
                    {
                        return chileFocus;
                    }
                }
            }
            return result;
        }

        public override void Action()
        {
            UIObjects.AllAction();
            UIObjects.ClearAllDead();
            base.Action();
        }

        public override void Draw(Graphics g)
        {
            if (Visible)
            {
                DrawObject.Draw(g, Layout.Rectangle);
                UIObjects.AllDrawSelf(g);
            }
        }
    }
}
