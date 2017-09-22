using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 自訂繪圖物件
    /// </summary>
    public class DrawCustom : DrawBase
    {
        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Color.Empty; }
        }

        /// <summary>
        /// 使用繪圖工具管理物件新增自訂繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        public DrawCustom(DrawColors drawColor)
            : base(drawColor) { }

        /// <summary>
        /// 新增自訂繪圖物件
        /// </summary>
        public DrawCustom() { }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件(不含事件,未綁定物件)
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawCustom(Colors.Copy())
            {
                Scale = this.Scale,
                Angle = this.Angle,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled
            };
        }

        protected override void OnDraw(Graphics g, Rectangle rectangle) { }
    }
}
