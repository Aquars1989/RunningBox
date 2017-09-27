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
        public event EventHandler _BackButtonClick;

        /// <summary>
        /// 發生於繼續按鈕被按下
        /// </summary>
        public event EventHandler _ResumeButtonClick;

        /// <summary>
        /// 發生於重試按鈕被按下
        /// </summary>
        public event EventHandler _RetryButtonClick;

        /// <summary>
        /// 發生於下一關按鈕被按下
        /// </summary>
        public event EventHandler _NextButtonClick;

        /// <summary>
        /// 重試/繼續/下一關按鈕
        /// </summary>
        private ObjectUI _UICommand2;

        /// <summary>
        /// 返回按鈕
        /// </summary>
        private ObjectUI _UICommandBack;

      
        public ObjectUIGameMenu(DirectionType anchor, int x, int y, MoveBase moveObject)
            : base(anchor, x, y, 220, 150, new DrawUIFrame(Color.AliceBlue, Color.DarkSlateBlue, 2, 20), moveObject)
        {
            _UICommand2 = new ObjectUI(0, 0, 150, 50, new DrawUITextFrame(Color.Black, Color.White, Color.White, Color.Black, 2, 10, "重試", Global.CommandFont, GlobalFormat.MiddleCenter));
            _UICommandBack = new ObjectUI(0, 0, 150, 50, new DrawUITextFrame(Color.Black, Color.White, Color.White, Color.Black, 2, 10, "回選單", Global.CommandFont, GlobalFormat.MiddleCenter));

            UIObjects.Add(_UICommandBack);
            UIObjects.Add(_UICommand2);
        }

      
        public override void Draw(Graphics g)
        {
            base.Draw(g);

          
        }
    }
}
