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
    public class DrawNone : DrawBase
    {
        /// <summary>
        /// 代表無效的繪圖物件
        /// </summary>
        public DrawNone() { }

        public override void Draw(Graphics g, Rectangle rectangle) { }

        public override DrawBase Copy()
        {
            return new DrawNone();
        }

        protected override void OnDispose() { }
    }
}
