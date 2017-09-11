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
    public class DrawImage : DrawBase
    {
        /// <summary>
        /// 繪製圖片
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// 新增圖片繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色(供技能/特效使用)</param>
        /// <param name="drawShape">繪製圖片</param>
        public DrawImage(Color color, Image image)
        {
            Color = color;
            Image = image;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            ColorFix.DrawImage(g, Image, drawRectangle, Opacity, RFix, GFix, BFix);
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawImage(Color, Image)
            {
                Scene = this.Scene,
                Owner = this.Owner,
                Opacity = this.Opacity,
                RFix = this.RFix,
                GFix = this.GFix,
                BFix = this.BFix,
                Scale = this.Scale
            };
        }

        protected override void OnDispose() { }
    }
}
