using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 使用中心點及尺寸建立實體位置
    /// </summary>
    class LayoutAnchor : LayoutBase
    {
        private bool SizeChange = false;
        private bool LocationChange = false;

        private float _CenterX;
        /// <summary>
        /// 取得物件中心點X座標
        /// </summary>
        public override float CenterX
        {
            get
            {
                GetRectangle();
                return _CenterX;
            }
        }

        private float _CenterY;
        /// <summary>
        /// 取得物件中心點Y座標
        /// </summary>
        public override float CenterY
        {
            get
            {
                GetRectangle();
                return _CenterY;
            }
        }

        private Rectangle _Rectangle;
        /// <summary>
        /// 取得物件實體位置
        /// </summary>
        public override Rectangle Rectangle
        {
            get { return GetRectangle(); }
        }

        /// <summary>
        /// 設置Rectangle
        /// </summary>
        private Rectangle GetRectangle()
        {
            bool resetCenter = false;
            if (SizeChange)
            {
                int width = (int)(Width * Scale + 0.5F);
                int height = (int)(Height * Scale + 0.5F);
                width = Math.Min(width, 1);
                height = Math.Min(height, 1);

                _Rectangle.Size = new Size(width, height);
                SizeChange = false;
                resetCenter = true;
            }

            if (LocationChange)
            {
                _Rectangle.Location = new Point((int)(X + 0.5F), (int)(Y + 0.5F));
                LocationChange = false;
                resetCenter = true;
            }

            if (resetCenter)
            {
                _CenterX = _Rectangle.Left + _Rectangle.Width / 2;
                _CenterY = _Rectangle.Top + _Rectangle.Height / 2;
            }
            return _Rectangle;
        }

        /// <summary>
        /// 發生於尺寸變化時
        /// </summary>
        protected override void OnSizeChanged()
        {
            SizeChange = true;
        }

        /// <summary>
        /// 發生於定位點變化時
        /// </summary>
        protected override void OnLocationChanged()
        {
            LocationChange = true;
        }
    }
}
