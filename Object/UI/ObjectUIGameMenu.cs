﻿using System;
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
        private DrawUITextFrame _DrawCommandAction;

        /// <summary>
        /// 返回按鈕繪製物件
        /// </summary>
        private DrawUITextFrame _DrawCommandBack;

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
                        break;
                    case 2:
                        _DrawCommandAction.Text = "重試";
                        break;
                    case 3:
                        _DrawCommandAction.Text = "下一關";
                        break;
                }
            }
        }

        public ObjectUIGameMenu(DirectionType anchor, int x, int y, MoveBase moveObject)
            : base(anchor, x, y, 380, 240, new DrawUIFrame(Color.AliceBlue, Color.DarkSlateBlue, 2, 20), moveObject)
        {
            _DrawCommandAction = new DrawUITextFrame(Color.Black, Color.White, Color.White, Color.Black, 2, 10, "", Global.CommandFont, GlobalFormat.MiddleCenter);
            _UICommandAction = new ObjectUI(200, 170, 150, 50, _DrawCommandAction);
            _UICommandAction.Layout.Depend.SetObject(this);
            _UICommandAction.Layout.Depend.Anchor = DirectionType.TopLeft;
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
                  }
              };

            _DrawCommandBack = new DrawUITextFrame(Color.Black, Color.White, Color.White, Color.Black, 2, 10, "回選單", Global.CommandFont, GlobalFormat.MiddleCenter);
            _UICommandBack = new ObjectUI(30, 170, 150, 50, _DrawCommandBack);
            _UICommandBack.Layout.Depend.SetObject(this);
            _UICommandBack.Layout.Depend.Anchor = DirectionType.TopLeft;
            _UICommandBack.Click += (s, e) =>
            {
                OnBackButtonClick();
            };

            UIObjects.Add(_UICommandBack);
            UIObjects.Add(_UICommandAction);
        }

        private static Font _TitleFont = new Font("標楷體", 28, FontStyle.Bold);
        private static Font _InfoFont1 = new Font("微軟正黑體", 18, FontStyle.Bold);
        private static Font _InfoFont2 = new Font("微軟正黑體", 12, FontStyle.Bold);
        public override void Draw(Graphics g)
        {
            base.Draw(g);

            if (Visible)
            {
                int left = Layout.Rectangle.Left;
                int top = Layout.Rectangle.Top;
                int width = Layout.Rectangle.Width;
                int height = Layout.Rectangle.Height;

                Rectangle titleRect = new Rectangle(left, top, width, 90);
                Rectangle titleRect2 = new Rectangle(left + 1, top + 1, width, 90);
                using (LinearGradientBrush brushInfo = new LinearGradientBrush(titleRect, Color.Maroon, Color.FromArgb(150, 255, 255, 200), 35))
                {
                    switch (Mode)
                    {
                        case 1:
                            g.DrawString("暫停遊戲", _TitleFont, Brushes.OliveDrab, titleRect2, GlobalFormat.BottomCenter);
                            g.DrawString("暫停遊戲", _TitleFont, brushInfo, titleRect, GlobalFormat.BottomCenter);
                            break;
                        case 2:
                            g.DrawString("挑戰失敗", _TitleFont, Brushes.OliveDrab, titleRect2, GlobalFormat.BottomCenter);
                            g.DrawString("挑戰失敗", _TitleFont, brushInfo, titleRect, GlobalFormat.BottomCenter);
                            break;
                        case 3:
                            g.DrawString("挑戰成功", _TitleFont, Brushes.OliveDrab, titleRect2, GlobalFormat.BottomCenter);
                            g.DrawString("挑戰成功", _TitleFont, brushInfo, titleRect, GlobalFormat.BottomCenter);
                            break;
                    }
                }

                Rectangle info1Rect = new Rectangle(left + 30, top + 100, 80, 40);
                Rectangle info2Rect = new Rectangle(left + 110, top + 100, width - 100, 40);
                Rectangle shadow1Rect = new Rectangle(info1Rect.Left + 1, info1Rect.Top + 1, info1Rect.Width, info1Rect.Height);
                Rectangle shadow2Rect = new Rectangle(info2Rect.Left + 1, info2Rect.Top + 1, info2Rect.Width, info2Rect.Height);
                Rectangle brushRect = new Rectangle(left, top + 100, width, 40);
                using (LinearGradientBrush brushInfo = new LinearGradientBrush(brushRect, Color.Maroon, Color.FromArgb(200, 255, 255, 200), 90F))
                {
                    g.DrawString("等級" + PlayingInfo.Level.ToString(), _InfoFont1, Brushes.RoyalBlue, shadow1Rect, GlobalFormat.BottomLeft);
                    g.DrawString(string.Format("存活時間：{0:N0}秒　分數：{0:N0}分", (int)(PlayingInfo.PlayingTime.Value / 1000), PlayingInfo.Score), _InfoFont2, Brushes.RoyalBlue, shadow2Rect, GlobalFormat.BottomLeft);
                    g.DrawString("等級" + PlayingInfo.Level.ToString(), _InfoFont1, brushInfo, info1Rect, GlobalFormat.BottomLeft);
                    g.DrawString(string.Format("存活時間：{0:N0}秒　分數：{0:N0}分", (int)(PlayingInfo.PlayingTime.Value / 1000), PlayingInfo.Score), _InfoFont2, brushInfo, info2Rect, GlobalFormat.BottomLeft);
                }
            }
        }
    }
}