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
        /// 參考的繪圖物件
        /// </summary>
        private DrawBase _BaseDrawObject;

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
        /// <param name="offsetX">陰影位置X軸偏移</param>
        /// <param name="offsetY">陰影位置Y軸偏移</param>
        /// <param name="scaleX">陰影寬度縮放</param>
        /// <param name="scaleY">陰影高度縮放</param>
        /// <param name="opacity">陰影不透明度</param>
        public PropertyShadow(int offsetX, int offsetY, float scaleX = 1, float scaleY = 1, float opacity = 0.2F)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            ScaleX = scaleX;
            ScaleY = scaleY;
            Opacity = opacity;
            BreakAfterDead = false;
        }

        public override void DoBeforeDraw(Graphics g)
        {
            GetDrawObject();
            int drawWidth = Owner.Layout.Rectangle.Width;
            int drawHeight = Owner.Layout.Rectangle.Height;
            float drawX = Owner.Layout.LeftTopX + OffsetX;
            float drawY = Owner.Layout.LeftTopY + OffsetY;
            if (ScaleX != 1)
            {
                drawWidth = (int)(drawWidth * ScaleX + 0.5F);
                drawX += (Owner.Layout.RectWidth - drawWidth) / 2;
            }

            if (ScaleY != 1)
            {
                drawHeight = (int)(drawHeight * ScaleY + 0.5F);
                drawY += (Owner.Layout.RectHeight - drawHeight) / 2;
            }

            Rectangle drawRect = new Rectangle((int)drawX, (int)drawY, drawWidth, drawHeight);
            _DrawObject.Colors.Opacity = Opacity;
            _DrawObject.Scene = Owner.Scene;
            _DrawObject.Draw(g, drawRect);

            base.DoBeforeDraw(g);
        }

        private void GetDrawObject()
        {
            if (_BaseDrawObject != Owner.DrawObject && _DrawObject != null)
            {
                if (_DrawObject != DrawNull.Value)
                {
                    _DrawObject.Dispose();
                }
                _DrawObject = null;
            }

            if (_DrawObject == null)
            {
                _BaseDrawObject = Owner.DrawObject;
                _DrawObject = _BaseDrawObject.Copy();
                _DrawObject.Colors.RFix = -1;
                _DrawObject.Colors.GFix = -1;
                _DrawObject.Colors.BFix = -1;
            }
        }
    }
}
