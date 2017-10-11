using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 玩家資訊物件
    /// </summary>
    public class ObjectUIPlayerInfo : ObjectUIPanel
    {
        private static Font _NameFont = new Font("標楷體", 16);
        private static Font _NameFont2 = new Font("標楷體", 15, FontStyle.Bold);
        private static Font _InfoFont = new Font("微軟正黑體", 10);
        private static SolidBrush _BrushTextBack = new SolidBrush(Color.FromArgb(200, 255, 255, 200));

        /// <summary>
        /// 新增場景資訊畫面
        /// </summary>
        /// <param name="anchor">訂位位置</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectUIPlayerInfo(DirectionType anchor, int x, int y, MoveBase moveObject)
            : base(anchor, x, y, 250, 80, new DrawUIFrame(Color.FromArgb(50, 220, 255, 255), Color.FromArgb(160, 240, 160, 150), 1, 0), moveObject)
        {
        }

        public override void Draw(Graphics g)
        {
            int left = Layout.Rectangle.Left;
            int top = Layout.Rectangle.Top;
            int width = Layout.Rectangle.Width;
            int height = Layout.Rectangle.Height;
            Rectangle halfRect = new Rectangle(left, top, width, 60);
            Rectangle nameRect = new Rectangle(left + 5, top + 18, width - 10, 24);
            Rectangle nameBackRect = new Rectangle(left + 5, top + 5, width - 10, 50);
            Rectangle scroeRect = new Rectangle(left + 10, top + 50, width - 10, 25);

            using (LinearGradientBrush brush = new LinearGradientBrush(halfRect, Color.FromArgb(100, 255, 255, 180), Color.FromArgb(0, 0, 0, 0), 90))
            {
                g.FillRectangle(brush, nameBackRect);
            }
            base.Draw(g);


            using (LinearGradientBrush brush = new LinearGradientBrush(nameRect, Color.Maroon, Color.DarkSlateBlue, 90))
            using (LinearGradientBrush brush2 = new LinearGradientBrush(nameRect, Color.FromArgb(50, 200, 200, 200), Color.Empty, 270))
            {
                nameRect.Offset(0, 4);
                g.DrawString(GlobalPlayer.PlayerName, _NameFont2, brush2, nameRect, GlobalFormat.MiddleCenter);
                nameRect.Offset(0, -4);
                g.DrawString(GlobalPlayer.PlayerName, _NameFont, brush, nameRect, GlobalFormat.MiddleCenter);
            }
            g.DrawString(string.Format("總分數：{0:N0}    完成度：{1} / {2}", GlobalScenes.Scenes.HighScore, GlobalScenes.Scenes.CountOfComplete, GlobalScenes.Scenes.CountOfLevel), _InfoFont, Brushes.Maroon, scroeRect, GlobalFormat.MiddleLeft);
        }
    }
}
