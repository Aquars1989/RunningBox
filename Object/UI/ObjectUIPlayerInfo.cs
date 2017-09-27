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
    /// 玩家資訊畫面
    /// </summary>
    public class ObjectUIPlayerInfo : ObjectUIPanel
    {
        public event EventHandler Close;

        public void OnClose()
        {
            if (Close != null)
            {
                Close(this, new EventArgs());
            }
        }

        private DrawUITextFrame _DrawPlayerName;

        public ObjectUIPlayerInfo(DirectionType anchor, int x, int y, MoveBase moveObject)
            : base(anchor, x, y, 330, 150, new DrawUIFrame(Color.AliceBlue, Color.CornflowerBlue, 2, 20), moveObject)
        {
            Propertys.Add(new PropertyShadow(0, 6, 0.95F, 1));

            //玩家姓名
            _DrawPlayerName = new DrawUITextFrame(Color.OrangeRed, Color.Orange, Color.FromArgb(230, 255, 245), Color.CornflowerBlue, 1, 10, "口口口口口口口口口口", new Font("標楷體", 16), GlobalFormat.MiddleLeft) { TextPadding = new Padding(10, 10, 0, 5) };
            ObjectUI uiPlayerName = new ObjectUI(30, 80, 270, 40, _DrawPlayerName);

            //重新選取
            DrawUITextFrame drawCommandBuildName = new DrawUITextFrame(Color.CornflowerBlue, Color.Empty, Color.Empty, Color.Empty, 0, 0, "↻", new Font("標楷體", 22, FontStyle.Bold), GlobalFormat.MiddleCenter) { TextPadding = new Padding(0, 10, 0, 0) };
            DrawUITextFrame drawCommandBuildNameHover = new DrawUITextFrame(Color.Orange, Color.Empty, Color.Empty, Color.Empty, 0, 0, "↻", new Font("標楷體", 22, FontStyle.Bold), GlobalFormat.MiddleCenter) { TextPadding = new Padding(0, 10, 0, 0) };
            ObjectUI uiCommandBuildName = new ObjectUI(260, 80, 40, 40, drawCommandBuildName) { DrawObjectHover = drawCommandBuildNameHover };

            DrawUITextFrame drawCommandClose = new DrawUITextFrame(Color.Pink, Color.Empty, Color.Empty, Color.Empty, 0, 0, "X", new Font("微軟正黑體", 16, FontStyle.Bold), GlobalFormat.MiddleCenter);
            DrawUITextFrame drawCommandCloseHover = new DrawUITextFrame(Color.Red, Color.Empty, Color.Empty, Color.Empty, 0, 0, "X", new Font("微軟正黑體", 16, FontStyle.Bold), GlobalFormat.MiddleCenter);
            ObjectUI uiCommandClose = new ObjectUI(290, 15, 30, 30, drawCommandClose) { DrawObjectHover = drawCommandCloseHover };

            uiPlayerName.Layout.Depend.Anchor = DirectionType.TopLeft;
            uiCommandBuildName.Layout.Depend.Anchor = DirectionType.TopLeft;
            uiCommandClose.Layout.Depend.Anchor = DirectionType.TopLeft;

            uiCommandBuildName.Layout.Depend.SetObject(this);
            uiPlayerName.Layout.Depend.SetObject(this);
            uiCommandClose.Layout.Depend.SetObject(this);


            uiCommandBuildName.Click += (s, e) => { _DrawPlayerName.Text = Function.GetRandName(); };
            uiCommandClose.Click += (s, e) => { OnClose(); };

            UIObjects.Add(uiPlayerName);
            UIObjects.Add(uiCommandBuildName);
            UIObjects.Add(uiCommandClose);
        }

        private static Font _TitleFont = new Font("微軟正黑體", 22);
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (Visible)
            {
                g.DrawString("玩家資料", _TitleFont, Brushes.DarkGray, Layout.Rectangle.Left + 21, Layout.Rectangle.Top + 26);
                RectangleF drawRect = new RectangleF(Layout.Rectangle.Left + 20, Layout.Rectangle.Top + 25, 150, 40);
                using (LinearGradientBrush _TitleBrush = new LinearGradientBrush(drawRect, Color.FromArgb(200, 140, 220, 255), Color.RoyalBlue, 110))
                {
                    g.DrawString("玩家資料", _TitleFont, _TitleBrush, drawRect);
                }
            }
        }
    }
}
