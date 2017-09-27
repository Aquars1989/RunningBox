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
        /// 重試/繼續/下一關按鈕
        /// </summary>
        private ObjectUI _UICommandAction;

        /// <summary>
        /// 返回按鈕
        /// </summary>
        private ObjectUI _UICommandBack;


        public ObjectUIGameMenu(DirectionType anchor, int x, int y, MoveBase moveObject)
            : base(anchor, x, y, 380, 200, new DrawUIFrame(Color.AliceBlue, Color.DarkSlateBlue, 2, 20), moveObject)
        {
            _UICommandAction = new ObjectUI(200, 130, 150, 50, new DrawUITextFrame(Color.Black, Color.White, Color.White, Color.Black, 2, 10, "重試", Global.CommandFont, GlobalFormat.MiddleCenter));
            _UICommandAction.Layout.Depend.SetObject(this);
            _UICommandAction.Layout.Depend.Anchor = DirectionType.TopLeft;
            _UICommandAction.Click += (s, e) =>
              {
                  OnRetryButtonClick();
              };

            _UICommandBack = new ObjectUI(30, 130, 150, 50, new DrawUITextFrame(Color.Black, Color.White, Color.White, Color.Black, 2, 10, "回選單", Global.CommandFont, GlobalFormat.MiddleCenter));
            _UICommandBack.Layout.Depend.SetObject(this);
            _UICommandBack.Layout.Depend.Anchor = DirectionType.TopLeft;
            _UICommandBack.Click += (s, e) =>
            {
                OnBackButtonClick();
            };

            UIObjects.Add(_UICommandBack);
            UIObjects.Add(_UICommandAction);
        }


        public override void Draw(Graphics g)
        {
            base.Draw(g);


        }
    }
}
