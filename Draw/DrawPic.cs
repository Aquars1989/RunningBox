using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 圖片繪圖物件
    /// </summary>
    public class DrawImage : IDraw
    {
        /// <summary>
        /// 繪製顏色(供技能/特效使用)
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// 繪製圖片
        /// </summary>
        public Image Image { get; set; }

        private float _Opacity;
        /// <summary>
        /// 不透明度(0~1)
        /// </summary>
        public float Opacity
        {
            get { return _Opacity; }
            set
            {
                if (_Opacity == value) return;
                _Opacity = value;
            }
        }

        private float _RFix;
        /// <summary>
        /// 紅色值調整(-1~1)
        /// </summary>
        public float RFix
        {
            get { return _RFix; }
            set
            {
                if (_RFix == value) return;
                _RFix = value;
            }
        }

        private float _GFix;
        /// <summary>
        /// 綠色值調整(-1~1)
        /// </summary>
        public float GFix
        {
            get { return _GFix; }
            set
            {
                if (_GFix == value) return;
                _GFix = value;
            }
        }

        private float _BFix;
        /// <summary>
        /// 藍色值調整(-1~1))
        /// </summary>
        public float BFix
        {
            get { return _BFix; }
            set
            {
                if (_BFix == value) return;
                _BFix = value;
            }
        }

        /// <summary>
        /// 縮放比例調整
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// 新增圖片繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色(供技能/特效使用)</param>
        /// <param name="drawShape">繪製圖片</param>
        public DrawImage(Color color, Image image)
        {
            Color = color;
            Image = image;
            _Opacity = 1;
            Scale = 1;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public void Draw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = rectangle;
            if (Scale != 1)
            {
                int scaleX = (int)(((drawRectangle.Width * Scale) - drawRectangle.Width) / 2);
                int scaleY = (int)(((drawRectangle.Height * Scale) - drawRectangle.Height) / 2);
                drawRectangle = new Rectangle(rectangle.Left - scaleX, rectangle.Top - scaleY, rectangle.Width + scaleX * 2, rectangle.Height + scaleY * 2);
            }

            ColorFix.DrawImage(g, Image, drawRectangle, Opacity, RFix, GFix, BFix);
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public IDraw Copy()
        {
            return new DrawImage(Color, Image) { Opacity = this.Opacity, RFix = this.RFix, GFix = this.GFix, BFix = this.BFix, Scale = this.Scale };
        }

        public void Dispose() { }
    }
}
