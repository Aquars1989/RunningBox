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
    public class ObjectUINameSetting : ObjectUIPanel
    {
        public event EventHandler Close;

        public void OnClose()
        {
            if (Close != null)
            {
                Close(this, new EventArgs());
            }
        }

        private DrawUITextScroll _DrawPlayerName;
        private ObjectUI _CommandOK;
        private ObjectUI _CommandClose;
        public ObjectUINameSetting(DirectionType anchor, int x, int y, MoveBase moveObject)
            : base(anchor, x, y, 310, 125, new DrawUIFrame(Color.AliceBlue, Color.CornflowerBlue, 2, 20), moveObject)
        {
            Propertys.Add(new PropertyShadow(0, 6, 0.95F, 1));

            // 玩家姓名
            _DrawPlayerName = new DrawUITextScroll(Color.Maroon, Color.LightGray, Color.FromArgb(230, 255, 245), Color.CornflowerBlue, 1, 10, GlobalPlayer.PlayerName, new Font("細明體", 12), GlobalFormat.MiddleLeft, 120) { TextPadding = new Padding(10, 10, 0, 5) };
            ObjectUI uiPlayerName = new ObjectUI(20, 60, 270, 40, _DrawPlayerName);

            // 重新選取
            DrawUIText drawCommandBuildName = new DrawUIText(Color.CornflowerBlue, Color.Empty, Color.Empty, Color.Empty, 0, 0, "↻", new Font("標楷體", 22, FontStyle.Bold), GlobalFormat.MiddleCenter) { TextPadding = new Padding(0, 10, 0, 0) };
            DrawUIText drawCommandBuildNameHover = new DrawUIText(Color.Orange, Color.Empty, Color.Empty, Color.Empty, 0, 0, "↻", new Font("標楷體", 22, FontStyle.Bold), GlobalFormat.MiddleCenter) { TextPadding = new Padding(0, 10, 0, 0) };
            ObjectUI uiCommandBuildName = new ObjectUI(250, 60, 40, 40, drawCommandBuildName) { DrawObjectHover = drawCommandBuildNameHover };

            DrawUIText drawCommandOK = new DrawUIText(Color.FromArgb(100, 220, 100), Color.Empty, Color.Empty, Color.Empty, 0, 0, "✔", new Font("微軟正黑體", 16, FontStyle.Bold), GlobalFormat.MiddleCenter);
            DrawUIText drawCommandOKHover = new DrawUIText(Color.FromArgb(0, 180, 0), Color.Empty, Color.Empty, Color.Empty, 0, 0, "✔", new Font("微軟正黑體", 16, FontStyle.Bold), GlobalFormat.MiddleCenter);
            _CommandOK = new ObjectUI(230, 15, 30, 30, drawCommandOK) { Visible = false, DrawObjectHover = drawCommandOKHover };
            _CommandOK.Propertys.Add(new PropertyShadow(2, 2) { RFix = -0.5F, GFix = -0.5F, BFix = -0.5F, Opacity = 0.2F });

            DrawUIText drawCommandClose = new DrawUIText(Color.FromArgb(255, 150, 150), Color.Empty, Color.Empty, Color.Empty, 0, 0, "✘", new Font("微軟正黑體", 16, FontStyle.Bold), GlobalFormat.MiddleCenter);
            DrawUIText drawCommandCloseHover = new DrawUIText(Color.FromArgb(255, 70, 70), Color.Empty, Color.Empty, Color.Empty, 0, 0, "✘", new Font("微軟正黑體", 16, FontStyle.Bold), GlobalFormat.MiddleCenter);
            _CommandClose = new ObjectUI(260, 15, 30, 30, drawCommandClose) { Visible = !string.IsNullOrWhiteSpace(GlobalPlayer.PlayerName), DrawObjectHover = drawCommandCloseHover };
            _CommandClose.Propertys.Add(new PropertyShadow(2, 2) { RFix = -0.5F, GFix = -0.5F, BFix = -0.5F, Opacity = 0.2F });

            uiPlayerName.Layout.Depend.Anchor = DirectionType.TopLeft;
            uiCommandBuildName.Layout.Depend.Anchor = DirectionType.TopLeft;
            _CommandOK.Layout.Depend.Anchor = DirectionType.TopLeft;
            _CommandClose.Layout.Depend.Anchor = DirectionType.TopLeft;

            uiCommandBuildName.Layout.Depend.SetObject(this);
            uiPlayerName.Layout.Depend.SetObject(this);
            _CommandOK.Layout.Depend.SetObject(this);
            _CommandClose.Layout.Depend.SetObject(this);

            uiCommandBuildName.Click += (s, e) =>
            {
                _DrawPlayerName.Text = Function.GetRandName();
                _CommandOK.Visible = true;
            };

            _CommandOK.Click += (s, e) =>
            {
                GlobalPlayer.PlayerName = _DrawPlayerName.Text;
                GlobalPlayer.WriteRegistry();
                OnClose();
            };

            _CommandClose.Click += (s, e) => { OnClose(); };

            UIObjects.Add(uiPlayerName);
            UIObjects.Add(uiCommandBuildName);
            UIObjects.Add(_CommandOK);
            UIObjects.Add(_CommandClose);
        }

        private static Font _TitleFont = new Font("微軟正黑體", 16);
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (Visible)
            {
                RectangleF drawRect = new RectangleF(Layout.Rectangle.Left + 20, Layout.Rectangle.Top + 20, 200, 30);
                if (string.IsNullOrWhiteSpace(GlobalPlayer.PlayerName))
                {
                    g.DrawString("請選擇您的暱稱", _TitleFont, Brushes.DarkGray, drawRect);
                    drawRect.Offset(-1, -1);
                    using (LinearGradientBrush _TitleBrush = new LinearGradientBrush(drawRect, Color.FromArgb(200, 140, 220, 255), Color.RoyalBlue, 110))
                    {
                        g.DrawString("請選擇您的暱稱", _TitleFont, _TitleBrush, drawRect);
                    }
                }
                else
                {
                    g.DrawString("暱稱", _TitleFont, Brushes.DarkGray, drawRect);
                    drawRect.Offset(-1, -1);
                    using (LinearGradientBrush _TitleBrush = new LinearGradientBrush(drawRect, Color.FromArgb(200, 140, 220, 255), Color.RoyalBlue, 110))
                    {
                        g.DrawString("暱稱", _TitleFont, _TitleBrush, drawRect);
                    }
                }
            }
        }
    }
}
