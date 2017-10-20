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
    /// 場景資訊畫面
    /// </summary>
    public class ObjectUISceneInfo : ObjectUIPanel
    {
        private static Font _ItemFont = new Font("微軟正黑體", 12, FontStyle.Bold);
        private static Font _ItemFont2 = new Font("微軟正黑體", 10, FontStyle.Bold);
        private static Font TitleFont = new Font("標楷體", 22, FontStyle.Bold);
        private static Font TitleFont2 = new Font("標楷體", 14, FontStyle.Bold);
        private static Font InfoFont = new Font("微軟正黑體", 11, FontStyle.Bold);
        private static Brush _BrushTextBack = new SolidBrush(Color.FromArgb(200, 255, 255, 200));

        /// <summary>
        /// 發生於場景被選取時
        /// </summary>
        public event SceneInfoEnentHandle SceneChoice;

        /// <summary>
        /// 發生於場景被選取時
        /// </summary>
        /// <param name="sceneInfo">場景資訊</param>
        /// <param name="level">關卡等級</param>
        protected virtual void OnSceneChoice(ISceneInfo sceneInfo, int level)
        {
            if (SceneChoice != null)
            {
                SceneChoice(this, sceneInfo, level);
            }
        }

        private ISceneInfo _BindingScene;
        /// <summary>
        /// 綁定場景資訊
        /// </summary>
        public ISceneInfo BindingScene
        {
            get { return _BindingScene; }
            set
            {
                _BindingScene = value;
                UIObjects.Clear();

                if (_BindingScene == null) return;

                int itemWidth = (Layout.Width - 20) / 6;
                int itemHeight = 25;
                int left = 15;
                int top = Layout.Height - itemHeight - 5;
                bool nextChallenge = true;
                for (int i = 0; i < _BindingScene.MaxLevel; i++)
                {
                    int idx = i + 1;
                    DrawUIFrame drawObject = new DrawUIFrame(Color.Empty, Color.Empty, 1, 8) { Scale = 0.8F };
                    ObjectUI newObject = new ObjectUI(left, top, itemWidth, itemHeight, drawObject);
                    newObject.Layout.Depend.SetObject(this);
                    newObject.Layout.Depend.Anchor = DirectionType.TopLeft;
                    newObject.Propertys.Add(new PropertyShadow(3, 3) { Opacity = 0.2F, ScaleX = 0.95F, ScaleY = 0.95F });
                    UIObjects.Add(newObject);
                    left += itemWidth;



                    bool complete = _BindingScene.GetComplete(idx);
                    bool levelLock = !complete && !nextChallenge;
                    if (levelLock)
                    {
                        drawObject.Colors.SetColor("Border", Color.FromArgb(140, 140, 140));
                        drawObject.Colors.SetColor("Back", Color.FromArgb(50, 200, 200, 200));

                        drawObject.AfterDraw += (x, g, r) =>
                        {
                            g.DrawString(idx.ToString(), _ItemFont, Brushes.DarkGray, r, GlobalFormat.MiddleCenter);
                        };
                    }
                    else
                    {
                        drawObject.Colors.SetColor("Border", Color.FromArgb(100, 140, 255));
                        if (complete)
                        {
                            drawObject.Colors.SetColor("Back", Color.FromArgb(175, 180, 255, 220));
                        }
                        else
                        {
                            drawObject.Colors.SetColor("Back", Color.FromArgb(175, 255, 255, 180));
                        }

                        newObject.GetFocus += (x, e) =>
                        {
                            (x as ObjectUI).DrawObject.Colors.SetColor("Border", Color.Red);
                            Level = idx;
                        };

                        newObject.LostFocus += (x, e) =>
                        {
                            (x as ObjectUI).DrawObject.Colors.SetColor("Border", Color.FromArgb(100, 140, 255));
                            Level = 0;
                        };

                        newObject.Click += (x, e) =>
                        {
                            OnSceneChoice(BindingScene, idx);
                        };

                        drawObject.AfterDraw += (x, g, r) =>
                        {

                            g.DrawString(idx.ToString(), _ItemFont, Brushes.SteelBlue, r, GlobalFormat.MiddleCenter);
                            if (complete)
                            {
                                g.FillEllipse(Brushes.Orange, r.Left + r.Width - 10, r.Top + 4, 5, 5);
                            }
                        };

                        if (!complete)
                        {
                            nextChallenge = false;
                        }
                    }
                }
            }
        }

        private int _Level;
        /// <summary>
        /// 關卡等級
        /// </summary>
        public int Level
        {
            get { return _Level; }
            set
            {
                if (_Level == value) return;
                _Level = value;

            }
        }

        /// <summary>
        /// 新增場景資訊畫面
        /// </summary>
        /// <param name="anchor">訂位位置</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectUISceneInfo(DirectionType anchor, int x, int y, MoveBase moveObject)
            : base(anchor, x, y, 220, 150, new DrawUIFrame(Color.AliceBlue, Color.DarkSlateBlue, 2, 20), moveObject)
        {
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);

            int left = Layout.Rectangle.Left;
            int top = Layout.Rectangle.Top;
            int width = Layout.Rectangle.Width;
            int height = Layout.Rectangle.Height;

            Rectangle titleBackRect = new Rectangle(left + 1, top + 1, width - 120, 50);
            Rectangle titleBack2Rect = new Rectangle(left + width - 80 + 1, top + 1, 80, 50);
            Rectangle titleRect = new Rectangle(left, top, width - 120, 50);
            Rectangle title2Rect = new Rectangle(left + width - 80, top, 80, 50);
            Rectangle info1Rect = new Rectangle(left + 30, top + 50, width - 60, 20);
            Rectangle info2Rect = new Rectangle(left + 30, top + 70, width - 60, 20);
            Rectangle info3Rect = new Rectangle(left + 30, top + 90, width - 60, 20);

            g.DrawString(_BindingScene.SceneName, TitleFont, Brushes.LemonChiffon, titleBackRect, GlobalFormat.BottomCenter);
            g.DrawString(_BindingScene.SceneName, TitleFont, Brushes.DarkSlateBlue, titleRect, GlobalFormat.BottomCenter);
            if (Level > 0)
            {
                g.DrawString("等級" + Level.ToString(), TitleFont2, Brushes.LemonChiffon, titleBack2Rect, GlobalFormat.BottomLeft);
                g.DrawString("等級" + Level.ToString(), TitleFont2, Brushes.DarkSlateBlue, title2Rect, GlobalFormat.BottomLeft);
                g.DrawString(string.Format("挑戰次數：{0:N0}次", _BindingScene.GetCountOfChallenge(Level)), InfoFont, Brushes.SteelBlue, info1Rect, GlobalFormat.BottomLeft);
                g.DrawString(string.Format("存活時間：{0:N0}秒", (int)(_BindingScene.GetHighPlayingTime(Level) / 1000F)), InfoFont, Brushes.SteelBlue, info2Rect, GlobalFormat.BottomLeft);
                g.DrawString(string.Format("最高分數：{0:N0}分", _BindingScene.GetHighScore(Level)), InfoFont, Brushes.SteelBlue, info3Rect, GlobalFormat.BottomLeft);
            }
            else
            {
                g.DrawString(string.Format("挑戰次數：{0:N0}次", _BindingScene.CountOfChallenge), InfoFont, Brushes.SteelBlue, info1Rect, GlobalFormat.BottomLeft);
                g.DrawString(string.Format("時間總計：{0:N0}秒", (int)(_BindingScene.HighPlayingTime / 1000F)), InfoFont, Brushes.SteelBlue, info2Rect, GlobalFormat.BottomLeft);
                g.DrawString(string.Format("分數總計：{0:N0}分", _BindingScene.HighScore), InfoFont, Brushes.SteelBlue, info3Rect, GlobalFormat.BottomLeft);
            }
        }
    }
}
