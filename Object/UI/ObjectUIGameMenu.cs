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
    /// 遊戲選單畫面
    /// </summary>
    public class ObjectUIGameMenu : ObjectUIPanel
    {
        private static Font _TitleFont = new Font("微軟正黑體", 30);
        private static Font _InfoFont1 = new Font("新細明體", 22, FontStyle.Bold);
        private static Font _InfoFont2 = new Font("微軟正黑體", 12, FontStyle.Bold);

        /// <summary>
        /// 發生於返回按鈕被按下
        /// </summary>
        public event EventHandler BackButtonClick;

        /// <summary>
        /// 發生於繼續按鈕被按下
        /// </summary>
        public event EventHandler ResumeButtonClick;

        /// <summary>
        /// 發生於重試按鈕被按下
        /// </summary>
        public event EventHandler RetryButtonClick;

        /// <summary>
        /// 發生於下一關按鈕被按下
        /// </summary>
        public event EventHandler NextButtonClick;

        /// <summary>
        /// 發生於返回按鈕被按下
        /// </summary>
        public void OnBackButtonClick()
        {
            if (BackButtonClick != null)
            {
                BackButtonClick(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於繼續按鈕被按下
        /// </summary>
        public void OnResumeButtonClick()
        {
            if (ResumeButtonClick != null)
            {
                ResumeButtonClick(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於重試按鈕被按下
        /// </summary>
        public void OnRetryButtonClick()
        {
            if (RetryButtonClick != null)
            {
                RetryButtonClick(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於下一關按鈕被按下
        /// </summary>
        public void OnNextButtonClick()
        {
            if (NextButtonClick != null)
            {
                NextButtonClick(this, new EventArgs());
            }
        }

        /// <summary>
        /// 重試/繼續/下一關按鈕繪製物件
        /// </summary>
        private DrawUIText _DrawCommandAction;

        /// <summary>
        /// 返回按鈕繪製物件
        /// </summary>
        private DrawUIText _DrawCommandBack;

        /// <summary>
        /// 重試/繼續/下一關按鈕焦點繪製物件
        /// </summary>
        private DrawUIText _DrawCommandActionHover;

        /// <summary>
        /// 返回按鈕焦點繪製物件
        /// </summary>
        private DrawUIText _DrawCommandBackHover;

        /// <summary>
        /// 重試/繼續/下一關按鈕
        /// </summary>
        private ObjectUI _UICommandAction;

        /// <summary>
        /// 返回按鈕
        /// </summary>
        private ObjectUI _UICommandBack;

        /// <summary>
        /// 場景挑戰資訊
        /// </summary>
        public ScenePlayingInfo PlayingInfo;

        private int _Mode;
        /// <summary>
        /// 模式 1:暫停 2:失敗 3:完成
        /// </summary>
        public int Mode
        {
            get { return _Mode; }
            set
            {
                _Mode = value;
                switch (Mode)
                {
                    case 1:
                        _DrawCommandAction.Text = "繼續";
                        _DrawCommandActionHover.Text = "繼續";
                        break;
                    case 2:
                        _DrawCommandAction.Text = "重試";
                        _DrawCommandActionHover.Text = "重試";
                        break;
                    case 3:
                        _DrawCommandAction.Text = "下一關";
                        _DrawCommandActionHover.Text = "下一關";
                        break;
                    case 4:
                        _DrawCommandAction.Text = "重試";
                        _DrawCommandActionHover.Text = "重試";
                        break;
                }
            }
        }

        public ObjectUIGameMenu(DirectionType anchor, int x, int y, MoveBase moveObject)
            : base(anchor, x, y, 380, 250, new DrawUIFrame(Color.Empty, Color.DarkSlateBlue, 2, 20), moveObject)
        {
            _DrawCommandAction = new DrawUIText(Color.Black, Color.White, Color.FromArgb(150, 255, 255, 255), Color.Black, 2, 10, "", Global.CommandFont, GlobalFormat.MiddleCenter);
            _DrawCommandActionHover = new DrawUIText(Color.Black, Color.White, Color.FromArgb(200, 255, 255, 220), Color.Black, 2, 10, "", Global.CommandFont, GlobalFormat.MiddleCenter);
            _UICommandAction = new ObjectUI(210, 175, 150, 50, _DrawCommandAction);
            _UICommandAction.DrawObjectHover = _DrawCommandActionHover;
            _UICommandAction.Layout.Depend.SetObject(this);
            _UICommandAction.Layout.Depend.Anchor = DirectionType.TopLeft;
            _UICommandAction.Propertys.Add(new PropertyShadow(-4, 4) { RFix = 0, GFix = 0, BFix = 0 });
            _UICommandAction.Click += (s, e) =>
              {
                  switch (Mode)
                  {
                      case 1:
                          OnResumeButtonClick();
                          break;
                      case 2:
                          OnRetryButtonClick();
                          break;
                      case 3:
                          OnNextButtonClick();
                          break;
                      case 4:
                          OnRetryButtonClick();
                          break;
                  }
              };

            _DrawCommandBack = new DrawUIText(Color.Black, Color.White, Color.FromArgb(150, 255, 255, 255), Color.Black, 2, 10, "回選單", Global.CommandFont, GlobalFormat.MiddleCenter);
            _DrawCommandBackHover = new DrawUIText(Color.Black, Color.White, Color.FromArgb(200, 255, 255, 220), Color.Black, 2, 10, "回選單", Global.CommandFont, GlobalFormat.MiddleCenter);
            _UICommandBack = new ObjectUI(20, 175, 150, 50, _DrawCommandBack);
            _UICommandBack.DrawObjectHover = _DrawCommandBackHover;
            _UICommandBack.Layout.Depend.SetObject(this);
            _UICommandBack.Layout.Depend.Anchor = DirectionType.TopLeft;
            _UICommandBack.Propertys.Add(new PropertyShadow(4, 4) { RFix = 0, GFix = 0, BFix = 0 });
            _UICommandBack.Click += (s, e) =>
            {
                OnBackButtonClick();
            };

            UIObjects.Add(_UICommandBack);
            UIObjects.Add(_UICommandAction);
        }

        public override void Draw(Graphics g)
        {
            int left = Layout.Rectangle.Left;
            int top = Layout.Rectangle.Top;
            int width = Layout.Rectangle.Width;
            int height = Layout.Rectangle.Height;

            if (Visible)
            {
                using (LinearGradientBrush brushBack = new LinearGradientBrush(new Rectangle(left, top - 20, width, height + 20), Color.LightYellow, Color.AliceBlue, 90))
                {
                    g.FillPath(brushBack, Function.GetRadiusFrame(Layout.Rectangle, (DrawObject as DrawUIFrame).Readius));
                }
            }

            base.Draw(g);

            if (Visible)
            {
                Rectangle titleRect = new Rectangle(left + 2, top + 95, width - 4, 60);
                Rectangle titleRect2 = new Rectangle(left + 2 + 1, top + 1 + 95, width - 4, 60);

                using (LinearGradientBrush brushShadow = new LinearGradientBrush(titleRect, Color.Maroon, Color.LightGoldenrodYellow, 25))
                using (SolidBrush brushBack = new SolidBrush(Color.FromArgb(100, 230, 230, 230)))
                {
                    g.FillRectangle(brushBack, titleRect);
                    //g.DrawRectangle(Pens.Gray, titleRect);
                    switch (Mode)
                    {
                        case 1:
                            g.DrawString("暫停遊戲", _TitleFont, Brushes.OliveDrab, titleRect2, GlobalFormat.MiddleCenter);
                            g.DrawString("暫停遊戲", _TitleFont, brushShadow, titleRect, GlobalFormat.MiddleCenter);
                            break;
                        case 2:
                            g.DrawString("挑戰失敗", _TitleFont, Brushes.OliveDrab, titleRect2, GlobalFormat.MiddleCenter);
                            g.DrawString("挑戰失敗", _TitleFont, brushShadow, titleRect, GlobalFormat.MiddleCenter);
                            break;
                        case 3:
                            g.DrawString("挑戰成功", _TitleFont, Brushes.OliveDrab, titleRect2, GlobalFormat.MiddleCenter);
                            g.DrawString("挑戰成功", _TitleFont, brushShadow, titleRect, GlobalFormat.MiddleCenter);
                            break;
                        case 4:
                            g.DrawString("完成全部關卡!!", _TitleFont, Brushes.OliveDrab, titleRect2, GlobalFormat.MiddleCenter);
                            g.DrawString("完成全部關卡!!", _TitleFont, brushShadow, titleRect, GlobalFormat.MiddleCenter);
                            break;
                    }
                }

                Rectangle info1Rect = new Rectangle(left + 20, top + 15, 100, 40);
                Rectangle info2Rect = new Rectangle(left + 50, top + 45, width - 130, 40);
                Rectangle shadow1Rect = new Rectangle(info1Rect.Left + 1, info1Rect.Top + 1, info1Rect.Width, info1Rect.Height);
                Rectangle shadow2Rect = new Rectangle(info2Rect.Left + 1, info2Rect.Top + 1, info2Rect.Width, info2Rect.Height);
                Rectangle brushRect = new Rectangle(left, top + 100, width, 40);

                string info1 = string.Format("等級{0}", PlayingInfo.Level);
                string info2 = string.Format("存活時間：{0:N0}秒　分數：{1:N0}分", (int)(PlayingInfo.PlayingTime.Value / Scene.Sec(1)), PlayingInfo.Score);
                g.DrawString(info1, _InfoFont1, Brushes.Gray, shadow1Rect, GlobalFormat.BottomLeft);
                g.DrawString(info2, _InfoFont2, Brushes.LightGray, shadow2Rect, GlobalFormat.BottomLeft);
                g.DrawString(info1, _InfoFont1, Brushes.RoyalBlue, info1Rect, GlobalFormat.BottomLeft);
                g.DrawString(info2, _InfoFont2, Brushes.RoyalBlue, info2Rect, GlobalFormat.BottomLeft);
            }
        }
    }
}
