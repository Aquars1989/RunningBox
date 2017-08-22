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
    public class LayoutCentral : LayoutBase
    {
        private bool SizeChange = false;
        private bool LocationChange = false;


        /// <summary>
        /// 取得物件中心點X座標
        /// </summary>
        public override float CenterX
        {
            get { return X; }
        }

        /// <summary>
        /// 取得物件中心點Y座標
        /// </summary>
        public override float CenterY
        {
            get { return Y; }
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
            if (SizeChange)
            {
                int width = (int)(Width * Scale + 0.5F);
                int height = (int)(Height * Scale + 0.5F);
                width = Math.Min(width, 1);
                height = Math.Min(height, 1);

                _Rectangle.Size = new Size(width, height);
                _Rectangle.Location = new Point((int)(X - (width / 2F) + 0.5F), (int)(Y - (height / 2F) + 0.5F));

                SizeChange = false;
                LocationChange = false;
            }

            if (LocationChange)
            {
                int width = _Rectangle.Width;
                int height = _Rectangle.Height;
                _Rectangle.Location = new Point((int)(X - (width / 2F) + 0.5F), (int)(Y - (height / 2F) + 0.5F));
                LocationChange = false;
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
