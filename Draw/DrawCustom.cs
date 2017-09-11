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
        /// 新增自訂繪圖物件
        /// </summary>
        public DrawCustom() { }

        /// <summary>
        /// 複製繪圖物件(只複製空物件)
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawCustom()
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

        protected override void OnDraw(Graphics g, Rectangle rectangle) { }
        protected override void OnDispose() { }
    }
}
