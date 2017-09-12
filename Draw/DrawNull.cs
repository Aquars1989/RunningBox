using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 代表無效的繪圖物件
    /// </summary>
    public class DrawNull : DrawBase
    {
        public static DrawNull Value = new DrawNull();

        public override Color MainColor
        {
            get { return Color.Empty; }
        }

        /// <summary>
        /// 代表無效的繪圖物件
        /// </summary>
        private DrawNull() { }

        protected override void OnDraw(Graphics g, Rectangle rectangle) { }

        public override DrawBase Copy()
        {
            return Value;
        }
    }
}
