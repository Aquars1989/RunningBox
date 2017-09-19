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
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Colors.GetColor("Back"); }
        }

        /// <summary>
        /// 繪製圖片
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// 使用繪圖工具管理物件新增圖片繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="image">繪製圖片</param>
        public DrawImage(DrawColors drawColor, Image image)
            : base(drawColor)
        {
            Image = image;
        }

        /// <summary>
        /// 新增圖片繪圖物件
        /// </summary>
        /// <param name="backColor">繪製顏色(供技能/特效使用)</param>
        /// <param name="image">繪製圖片</param>
        public DrawImage(Color backColor, Image image)
        {
            Colors.SetColor("Back", backColor);
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
            ColorFix.DrawImage(g, Image, drawRectangle, Colors.Opacity, Colors.RFix, Colors.GFix, Colors.BFix);
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件,未綁定物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawImage(Colors.Copy(), Image)
            {
                Scale = this.Scale
            };
        }
    }
}
