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
        private static Font _InfoFont3 = new Font("微軟正黑體", 11, FontStyle.Bold);
        private static Font _UpdateFont = new Font(Global.CommandFont.FontFamily, 11);
        private static int _DetailHeight = 20;

        /// <summary>
        /// 發生於返回按鈕被按下
        /// </summary>
        public event GameMenuCommandEnentHandle ButtonClick;

        /// <summary>
        /// 發生於返回按鈕被按下
        /// </summary>
        public void OnBackButtonClick()
        {
            if (ButtonClick != null)
            {
                ButtonClick(this, GameMenuCommandType.Back);
            }
        }

        /// <summary>
        /// 發生於重試/繼續/下一關按鈕被按下
        /// </summary>
        public void OnActionButtonClick(GameMenuCommandType command)
        {
            if (ButtonClick != null)
            {
                ButtonClick(this, command);
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
        /// 上傳按鈕繪製物件
        /// </summary>
        private DrawUIText _DrawCommandUpdate;

        /// <summary>
        /// 重試/繼續/下一關按鈕焦點繪製物件
        /// </summary>
        private DrawUIText _DrawCommandActionHover;

        /// <summary>
        /// 返回按鈕焦點繪製物件
        /// </summary>
        private DrawUIText _DrawCommandBackHover;

        /// <summary>
        /// 上傳按鈕焦點繪製物件
        /// </summary>
        private DrawUIText _DrawCommandUpdateHover;

        /// 重試/繼續/下一關按鈕
        /// </summary>
        private ObjectUI _UICommandAction;

        /// <summary>
        /// 返回按鈕
        /// </summary>
        private ObjectUI _UICommandBack;

        /// <summary>
        /// 重試/繼續/下一關按鈕回傳值
        /// </summary>
        private GameMenuCommandType _ActionCommand;

        /// <summary>
        /// 上傳分數
        /// </summary>
        private ObjectUI _UICommandUpdate;

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
                        _ActionCommand = GameMenuCommandType.Resume;
                        //_UICommandUpdate.Visible = false;
                        _DrawCommandUpdate.Text = "上傳分數";
                        break;
                    case 2:
                        _DrawCommandAction.Text = "重試";
                        _DrawCommandActionHover.Text = "重試";
                        _ActionCommand = GameMenuCommandType.Retry;

                        _UICommandUpdate.Visible = false;
                        break;
                    case 3:
                        _DrawCommandAction.Text = "下一關";
                        _DrawCommandActionHover.Text = "下一關";
                        _ActionCommand = GameMenuCommandType.NextLevel;

                        _DrawCommandUpdate.Text = "上傳分數";
                        _DrawCommandUpdateHover.Text = "上傳分數";
                        _UICommandUpdate.Visible = Global.Online;
                        break;
                    case 4:
                        _DrawCommandAction.Text = "重試";
                        _DrawCommandActionHover.Text = "重試";
                        _ActionCommand = GameMenuCommandType.Retry;

                        _DrawCommandUpdate.Text = "上傳分數";
                        _DrawCommandUpdateHover.Text = "上傳分數";
                        _UICommandUpdate.Visible = Global.Online;
                        break;
                }
            }
        }

        public ObjectUIGameMenu(DirectionType anchor, int x, int y, MoveBase moveObject, ScenePlayingInfo playingInfo, int mode)
            : base(anchor, x, y, 380, 240 + playingInfo.ScoreDetail.Count * _DetailHeight, new DrawUIFrame(Color.Empty, Color.DarkSlateBlue, 2, 20), moveObject)
        {
            _DrawCommandAction = new DrawUIText(Color.Black, Color.White, Color.FromArgb(150, 255, 255, 255), Color.Black, 2, 10, "", Global.CommandFont, GlobalFormat.MiddleCenter);
            _DrawCommandActionHover = new DrawUIText(Color.Black, Color.White, Color.FromArgb(200, 255, 255, 220), Color.Black, 2, 10, "", Global.CommandFont, GlobalFormat.MiddleCenter);
            _UICommandAction = new ObjectUI(210, Layout.Height - 75, 150, 50, _DrawCommandAction);
            _UICommandAction.DrawObjectHover = _DrawCommandActionHover;
            _UICommandAction.Layout.Depend.SetObject(this);
            _UICommandAction.Layout.Depend.Anchor = DirectionType.TopLeft;
            _UICommandAction.Propertys.Add(new PropertyShadow(-4, 4) { RFix = 0, GFix = 0, BFix = 0 });
            _UICommandAction.Click += (s, e) =>
              {
                  OnActionButtonClick(_ActionCommand);
              };

            _DrawCommandBack = new DrawUIText(Color.Black, Color.White, Color.FromArgb(150, 255, 255, 255), Color.Black, 2, 10, "回選單", Global.CommandFont, GlobalFormat.MiddleCenter);
            _DrawCommandBackHover = new DrawUIText(Color.Black, Color.White, Color.FromArgb(200, 255, 255, 220), Color.Black, 2, 10, "回選單", Global.CommandFont, GlobalFormat.MiddleCenter);
            _UICommandBack = new ObjectUI(20, Layout.Height - 75, 150, 50, _DrawCommandBack);
            _UICommandBack.DrawObjectHover = _DrawCommandBackHover;
            _UICommandBack.Layout.Depend.SetObject(this);
            _UICommandBack.Layout.Depend.Anchor = DirectionType.TopLeft;
            _UICommandBack.Propertys.Add(new PropertyShadow(4, 4) { RFix = 0, GFix = 0, BFix = 0 });
            _UICommandBack.Click += (s, e) =>
            {
                OnBackButtonClick();
            };

            _DrawCommandUpdate = new DrawUIText(Color.Black, Color.White, Color.FromArgb(150, 225, 255, 225), Color.Black, 2, 10, "上傳分數", _UpdateFont, GlobalFormat.MiddleCenter);
            _DrawCommandUpdateHover = new DrawUIText(Color.Black, Color.White, Color.FromArgb(200, 255, 235, 220), Color.Black, 2, 10, "上傳分數", _UpdateFont, GlobalFormat.MiddleCenter);
            _UICommandUpdate = new ObjectUI(270, 10, 90, 25, _DrawCommandUpdate);
            _UICommandUpdate.DrawObjectHover = _DrawCommandUpdateHover;
            _UICommandUpdate.Layout.Depend.SetObject(this);
            _UICommandUpdate.Layout.Depend.Anchor = DirectionType.TopLeft;
            _UICommandUpdate.Propertys.Add(new PropertyShadow(4, 4) { RFix = 0, GFix = 0, BFix = 0 });
            _UICommandUpdate.Click += (s, e) =>
            {
                if (Global.Online)
                {

                    Global.SQL.AddParameter("@SceneID", PlayingInfo.SceneID);
                    Global.SQL.AddParameter("@Level", PlayingInfo.Level);
                    Global.SQL.AddParameter("@PlayerName", GlobalPlayer.PlayerName);
                    Global.SQL.AddParameter("@Score", PlayingInfo.Score);
                    if (Global.SQL.Run(@"set time_zone = '+8:00';
                                         insert into Score (SceneID,Level,PlayerName,Score,UpdateTime) 
                                         values (@SceneID,@Level,@PlayerName,@Score,now())"))
                    {
                        _DrawCommandUpdate.Text = "上傳成功";
                        _DrawCommandUpdateHover.Text = "上傳成功";
                        _UICommandUpdate.Enabled = false;
                    }
                    else
                    {
                        _DrawCommandUpdate.Text = "上傳失敗";
                        _DrawCommandUpdateHover.Text = "上傳失敗";
                    }
                }
                else
                {
                    _DrawCommandUpdate.Text = "不可用";
                    _DrawCommandUpdateHover.Text = "不可用";
                    _UICommandUpdate.Enabled = false;
                }
            };

            UIObjects.Add(_UICommandBack);
            UIObjects.Add(_UICommandAction);
            UIObjects.Add(_UICommandUpdate);

            PlayingInfo = playingInfo;
            Mode = mode;
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
                Rectangle titleRect = new Rectangle(left + 2, top + 50, width - 4, 60);
                string title = "";
                using (LinearGradientBrush brushShadow = new LinearGradientBrush(titleRect, Color.Maroon, Color.LightGoldenrodYellow, 25))
                using (Brush brushBack = new SolidBrush(Color.FromArgb(100, 230, 230, 230)))
                {
                    g.FillRectangle(brushBack, titleRect);
                    switch (Mode)
                    {
                        case 1:
                            title = "暫停遊戲";
                            break;
                        case 2:
                            title = "挑戰失敗";
                            break;
                        case 3:
                            title = "挑戰成功";
                            break;
                        case 4:
                            title = "完成全部關卡";
                            break;
                    }

                    g.DrawString(title, _TitleFont, brushShadow, titleRect, GlobalFormat.MiddleCenter);
                    titleRect.Offset(1, 1);
                    g.DrawString(title, _TitleFont, Brushes.OliveDrab, titleRect, GlobalFormat.MiddleCenter);
                }



                Rectangle info1Rect = new Rectangle(left + 20, top + 15, 100, 40);
                Rectangle info2Rect = new Rectangle(left + 30, top + 115, 150, 30);
                Rectangle info3Rect = new Rectangle(left + 180, top + 115, width - 200, 30);

                string info1 = string.Format("等級{0}", PlayingInfo.Level);
                string info2 = string.Format("存活時間：{0:N0}秒", (int)(PlayingInfo.PlayingTime.Value / Scene.Sec(1)), PlayingInfo.Score);
                string info3 = string.Format("分數：{1:N0}分", (int)(PlayingInfo.PlayingTime.Value / Scene.Sec(1)), PlayingInfo.Score);
                g.DrawString(info1, _InfoFont1, Brushes.Gray, info1Rect, GlobalFormat.MiddleLeft);
                g.DrawString(info2, _InfoFont2, Brushes.LightGray, info2Rect, GlobalFormat.MiddleLeft);
                g.DrawString(info3, _InfoFont2, Brushes.LightGray, info3Rect, GlobalFormat.MiddleLeft);
                info1Rect.Offset(1, 1);
                info2Rect.Offset(1, 1);
                info3Rect.Offset(1, 1);
                g.DrawString(info1, _InfoFont1, Brushes.RoyalBlue, info1Rect, GlobalFormat.MiddleLeft);
                g.DrawString(info2, _InfoFont2, Brushes.RoyalBlue, info2Rect, GlobalFormat.MiddleLeft);
                g.DrawString(info3, _InfoFont2, Brushes.RoyalBlue, info3Rect, GlobalFormat.MiddleLeft);

                int bx = info3Rect.Left + 10;
                int ex = info3Rect.Left + 20;
                int detailLeft = left + 200;
                int detailTop = top + 150;
                int detailWidth = width - 200;
                int lastTop = 0;
                foreach (var detail in PlayingInfo.ScoreDetail)
                {
                    int lineTop = detailTop + _DetailHeight / 2;
                    g.DrawLine(Pens.OliveDrab, bx, lineTop, ex, lineTop);
                    Rectangle infoDetail = new Rectangle(detailLeft, detailTop, detailWidth, _DetailHeight);
                    //g.DrawString(string.Format("{0} + {1:N0}", detail.Key, detail.Value), _InfoFont3, Brushes.LightGray, infoDetail, GlobalFormat.MiddleLeft);
                    //infoDetail.Offset(1, 1);
                    g.DrawString(string.Format("{0} + {1:N0}", detail.Key, detail.Value), _InfoFont3, Brushes.OliveDrab, infoDetail, GlobalFormat.MiddleLeft);
                    detailTop += _DetailHeight;
                    lastTop = lineTop;
                }

                if (lastTop > 0)
                {
                    g.DrawLine(Pens.DarkSeaGreen, bx, top + 145, bx, lastTop);
                }
            }
        }
    }
}
