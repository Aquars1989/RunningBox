﻿using System;
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
        /// <summary>
        /// 發生於特性集合變更
        /// </summary>
        public event ValueChangedEnentHandle<ObjectCollection> UIObjectsChanged;

        /// <summary>
        /// 發生於特性集合變更
        /// </summary>
        protected virtual void OnUIObjectsChanged(ObjectCollection oldValue, ObjectCollection newValue)
        {
            if (oldValue != null)
            {
                oldValue.BindingUnlock();
                oldValue.Binding(Scene);
            }

            if (newValue != null)
            {
                newValue.Binding(this, true);
            }

            if (UIObjectsChanged != null)
            {
                UIObjectsChanged(this, oldValue, newValue);
            }
        }

        private ObjectCollection _UIObjects;
        public ObjectCollection UIObjects
        {
            get { return _UIObjects; }
            private set
            {
                if (_UIObjects == value) return;
                ObjectCollection oldValue = _UIObjects;
                _UIObjects = value;
                OnUIObjectsChanged(oldValue, value);
            }
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
        public ObjectUIPanel(DirectionType anchor, int x, int y, int width, int height, DrawBase drawObject, MoveBase moveObject)
            : base(anchor, x, y, width, height, drawObject, moveObject)
        {
            UIObjects = new ObjectCollection();
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
        public ObjectUIPanel(DirectionType anchor, int x, int y, int width, int height, DrawBase drawObject)
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
            : this(DirectionType.TopLeft, x, y, width, height, drawObject)
        {
            UIObjects = new ObjectCollection();
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
                if (child != null && child.Visible)
                {
                    ObjectUI chileFocus = child.InRectangle(point);
                    if (chileFocus != null)
                    {
                        return chileFocus.Enabled ? chileFocus : result;
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
                Propertys.AllDoBeforeDraw(g);
                DrawObject.Draw(g, Layout.Rectangle);
                UIObjects.AllDrawSelf(g);
                Propertys.AllDoAfterDraw(g);
            }
        }
    }
}
