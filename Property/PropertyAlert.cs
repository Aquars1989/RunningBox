using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 附加警示特性,在物件旁邊附上警示文字
    /// </summary>
    public class PropertyAlert : PropertyBase
    {
        private static Font _DrawFont = new Font("微軟正黑體", 12, FontStyle.Bold);

        public string Text { get; set; }

        /// <summary>
        /// 新增附加警示特性,在物件旁邊附上警示文字
        /// </summary>
        /// <param name="durationTime">持續時間(毫秒)</param>
        /// <param name="text">警示文字</param>
        public PropertyAlert(int durationTime, string text = "!")
        {
            DurationTime.Limit = durationTime;
            Text = text;
        }

        public override void DoAfterDraw(Graphics g)
        {
            int drawX = Owner.Layout.Rectangle.Left + Owner.Layout.Rectangle.Width;
            int drawY = Owner.Layout.Rectangle.Top;
            g.DrawString(Text, _DrawFont, Brushes.Gray, drawX + 1, drawY + 1);
            g.DrawString(Text, _DrawFont, Brushes.Red, drawX, drawY);
            base.DoAfterDraw(g);
        }
    }
}
