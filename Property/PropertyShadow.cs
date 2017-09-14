using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 陰影特性,物件下方產生陰影
    /// </summary>
    class PropertyShadow : PropertyBase
    {
        /// <summary>
        /// 繪製物件
        /// </summary>
        private DrawBase _DrawObject;

        /// <summary>
        /// 陰影位置X軸偏移
        /// </summary>
        public int OffsetX { get; set; }

        /// <summary>
        /// 陰影位置Y軸偏移
        /// </summary>
        public int OffsetY { get; set; }

        /// <summary>
        /// 陰影寬度縮放
        /// </summary>
        public float ScaleX { get; private set; }

        /// <summary>
        /// 陰影高度縮放
        /// </summary>
        public float ScaleY { get; private set; }

        /// <summary>
        /// 陰影不透明度
        /// </summary>
        public float Opacity { get; private set; }

        /// <summary>
        /// 新增陰影特性,物件下方產生陰影
        /// </summary>
        public PropertyShadow() { }

        protected override void OnOwnerChanged(object oldValue, object newValue)
        {
            if (oldValue != null)
            {
                (oldValue as ObjectBase).DrawObjectChanged -= Owner_DrawObjectChanged;
            }

            if (_DrawObject != null)
            {
                _DrawObject.Dispose();
            }

            BackDrawObject();
            Owner.DrawObjectChanged += Owner_DrawObjectChanged;
            base.OnOwnerChanged(oldValue, newValue);
        }

        //todo
        private void Owner_DrawObjectChanged(object sender, EventArgs e)
        {
            BackDrawObject();
        }

        public override void DoBeforeDraw(Graphics g)
        {
            GetDrawObject();
            int drawWidth = ScaleX == 1 ? Owner.Layout.Rectangle.Width : (int)(Owner.Layout.Rectangle.Width * ScaleX + 0.5F);
            int drawHeight = ScaleY == 1 ? Owner.Layout.Rectangle.Height : (int)(Owner.Layout.Rectangle.Height * ScaleY + 0.5F);
            int drawX = (int)(Owner.Layout.CenterX - drawWidth / 2F + OffsetX);
            int drawY = (int)(Owner.Layout.CenterY - drawWidth / 2F + OffsetY);
            Rectangle drawRect = new Rectangle(drawX, drawY, drawWidth, drawHeight);
            _DrawObject.Colors.Opacity = Opacity;
            _DrawObject.Scene = Owner.Scene;
            _DrawObject.Draw(g, drawRect);
            base.DoBeforeDraw(g);
        }

        private void GetDrawObject()
        {
            if (_DrawObject == null)
            {
                _DrawObject = Owner.DrawObject.Copy();
                _DrawObject.Colors.RFix = -1;
                _DrawObject.Colors.GFix = -1;
                _DrawObject.Colors.BFix = -1;
            }
        }

        private void BackDrawObject()
        {
            if (_DrawObject != null)
            {
                if (_DrawObject != DrawNull.Value)
                {
                    _DrawObject.Dispose();
                }
                _DrawObject = null;
            }
        }
    }
}
