
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 基本場景物件
    /// </summary>
    public class SceneBase : UserControl
    {
        /// <summary>
        /// 世界速度
        /// </summary>
        public float WorldSpeed { get; set; }

        /// <summary>
        /// 場景追蹤點
        /// </summary>
        public Point TrackPoint { get; set; }

        /// <summary>
        /// 玩家物件
        /// </summary>
        public ObjectPlayer PlayerObject { get; set; }

        /// <summary>
        /// 場景物件集合
        /// </summary>
        public ObjectCollection GameObjects { get; private set; }

        /// <summary>
        /// 場景特效集合
        /// </summary>
        public EffectCollection EffectObjects { get; private set; }

        /// <summary>
        /// 遊戲區域
        /// </summary>
        public Rectangle GameRectangle { get; set; }

        /// <summary>
        /// 分數
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 難度等級
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 是否開始遊戲
        /// </summary>
        public bool IsStart { get; set; }

        /// <summary>
        /// 遊戲是否已結束
        /// </summary>
        public bool IsEnding { get; set; }

        /// <summary>
        /// 遊戲結束延遲回合計數
        /// </summary>
        public int EndDelayRound { get; set; }

        /// <summary>
        /// 遊戲結束延遲回合最大值
        /// </summary>
        public int EndDelayRoundMax { get; set; }

        /// <summary>
        /// 本物件的Graphics物件
        /// </summary>
        private Graphics _ThisGraphics;

        /// <summary>
        /// 緩衝畫布的Graphics物件
        /// </summary>
        private Graphics _SceneGraphics;

        /// <summary>
        /// 緩衝畫布
        /// </summary>
        private Bitmap _SceneImage;

        /// <summary>
        /// 能量UI繪製區域
        /// </summary>
        private Rectangle _RectOfEngery = new Rectangle(80, 30, 100, 10);
        public Rectangle RectOfEngery
        {
            get { return _RectOfEngery; }
            set { _RectOfEngery = value; }
        }

        private Pen _PenRectGaming = new Pen(Color.LightGreen, 2);
        public Pen PenRectGaming
        {
            get { return _PenRectGaming; }
            set { _PenRectGaming = value; }
        }

        private Timer _TimerOfRound = new Timer() { Enabled = true, Interval = 25 };
        /// <summary>
        /// 回合計時器器
        /// </summary>
        private Timer TimerOfRound
        {
            get { return _TimerOfRound; }
        }

        public SceneBase()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            WorldSpeed = 1;
            EndDelayRoundMax = 30;
            GameObjects = new ObjectCollection(this);
            EffectObjects = new EffectCollection(this);

            GameObjects.ObjectDead += OnObjectDead;
            _TimerOfRound.Tick += TimerOfRound_Tick;
        }

        /// <summary>
        /// 回合計時器運作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerOfRound_Tick(object sender, EventArgs e)
        {
            if (!IsStart) return;

            EffectObjects.AllDoBeforeRound();
            GameObjects.AllAction();
            EffectObjects.AllDoAfterRound();

            GameObjects.ClearAllDead();
            EffectObjects.ClearAllDisabled();

            if (IsEnding)
            {
                if (EndDelayRound >= EndDelayRoundMax)
                {
                    IsEnding = false;
                    IsStart = false;
                }
                EndDelayRound++;
            }
            Drawing();
            DoAfterRound();
        }

        Stopwatch _watchFPS = new Stopwatch();
        int _tickFPS = 0;
        string _txtFPS = "";
        Font _fontFPS = new Font("Arial", 12);
        /// <summary>
        /// 繪製畫面
        /// </summary>
        private void Drawing()
        {
            _tickFPS--;
            bool refreshFPS = false;
            if (_tickFPS <= 0)
            {
                _tickFPS = 10;
                _watchFPS.Restart();
                refreshFPS = true;
            }


            _SceneGraphics.Clear(Color.White);

            EffectObjects.AllDoBeforeDraw(_SceneGraphics);
            EffectObjects.AllDoBeforeDrawUI(_SceneGraphics);

            if (GameRectangle != null)
            {
                _SceneGraphics.DrawRectangle(_PenRectGaming, GameRectangle);
            }

            if (PlayerObject != null)
            {
                float ratio = (float)PlayerObject.Energy / PlayerObject.EnergyMax;
                Brush brush = ratio < 0.3 ? Brushes.Red : Brushes.Black;
                _SceneGraphics.FillRectangle(brush, _RectOfEngery.X + 2, _RectOfEngery.Y + 2, (_RectOfEngery.Width - 4) * ratio, _RectOfEngery.Height - 4);
            }
            _SceneGraphics.DrawRectangle(Pens.Black, _RectOfEngery);
            _SceneGraphics.DrawString(Score.ToString(), Font, Brushes.Black, _RectOfEngery.X + _RectOfEngery.Width + 10, _RectOfEngery.Y);

            EffectObjects.AllDoBeforeDrawObject(_SceneGraphics);
            GameObjects.AllDrawSelf(_SceneGraphics);
            EffectObjects.AllDoAfterDraw(_SceneGraphics);

            _SceneGraphics.ResetTransform();

            _SceneGraphics.DrawString(_txtFPS, _fontFPS, Brushes.Red, Width - 50, 5);
            _ThisGraphics.DrawImageUnscaled(_SceneImage, 0, 0);

            if (refreshFPS)
            {
                _watchFPS.Stop();
                _txtFPS = (TimeSpan.TicksPerSecond / _watchFPS.Elapsed.Ticks).ToString();
            }
        }

        /// <summary>
        /// 開始遊戲
        /// </summary>
        /// <param name="potX">玩家起始點X</param>
        /// <param name="potY">玩家起始點Y</param>
        public void SetStart(int potX, int potY)
        {
            Level = 0;
            Score = 0;

            GameObjects.Clear();
            EffectObjects.Clear();

            ObjectActive PlayerObject = CreatePlayerObject(potX, potY);
            GameObjects.Add(PlayerObject);
            GameRectangle = new Rectangle(80, 80, Width - 160, Height - 160);

            if (_SceneImage != null) _SceneImage.Dispose();
            if (_SceneGraphics != null) _SceneGraphics.Dispose();
            if (_ThisGraphics != null) _ThisGraphics.Dispose();

            _SceneImage = new Bitmap(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            _SceneGraphics = Graphics.FromImage(_SceneImage);
            _ThisGraphics = CreateGraphics();
            _SceneGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Cursor.Hide();
            IsStart = true;
            DoAfterStart();
        }

        /// <summary>
        /// 回合後執行動作
        /// </summary>
        public virtual void DoAfterRound()
        {
            if (!IsEnding)
            {
                Score += Level;
            }
        }

        /// <summary>
        /// 開始後執行動作
        /// </summary>
        public virtual void DoAfterStart()
        {
        }

        /// <summary>
        /// 結束後執行動作
        /// </summary>
        public virtual void DoAfterEnd()
        {
        }

        /// <summary>
        /// 建立玩家物件
        /// </summary>
        public virtual ObjectActive CreatePlayerObject(int potX, int potY)
        {
            PlayerObject = new ObjectPlayer(potX, potY, 8, 3, 100, new DrawPen(Color.Black, DrawShape.Ellipse, 2), new TargetTrackPoint(this));
            // PlayerObject = new ObjectPlayer(potX, potY, 8, 20, 100, new DrawIconSprint(Color.Black), new TargetTrackPoint(this));
            PlayerObject.Skills.Add(new SkillSprint(200, 20, 15, true));
            PlayerObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
            PlayerObject.Propertys.Add(new PropertyCollision(1, null));
            return PlayerObject;
        }

        /// <summary>
        /// 物件死亡時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnObjectDead(ObjectBase sender, ObjectBase killer, ObjectDeadType deadType)
        {
            if (sender.Equals(PlayerObject))
            {
                SetEnd();
            }
        }

        /// <summary>
        /// 結束遊戲
        /// </summary>
        private void SetEnd()
        {
            EffectObjects.AllBreak();
            PlayerObject = null;
            IsEnding = true;
            EndDelayRound = 0;
            Cursor.Show();
            DoAfterEnd();
        }

        /// <summary>
        /// 秒轉換為Rounds
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public int SecToRounds(float sec)
        {
            return (int)(sec * 1000 / _TimerOfRound.Interval);
        }

        /// <summary>
        /// 取得隨機的物件進入點
        /// </summary>
        /// <returns>物件進入點</returns>
        public Point GetEnterPoint()
        {
            return GetEnterPoint((EnumDirection)Global.Rand.Next(4));
        }

        /// <summary>
        /// 取得特定方向的物件進入點
        /// </summary>
        /// <param name="enterSide">進入方向</param>
        /// <returns>物件進入點</returns>
        public Point GetEnterPoint(EnumDirection enterSide)
        {
            int x = 0, y = 0;
            switch (enterSide)
            {
                case EnumDirection.Left:
                    x = -Global.Rand.Next(20, 60);
                    y = Global.Rand.Next(0, Height);
                    break;
                case EnumDirection.Right:
                    x = Width + Global.Rand.Next(20, 60);
                    y = Global.Rand.Next(0, Height);
                    break;
                case EnumDirection.Top:
                    x = Global.Rand.Next(0, Width);
                    y = -Global.Rand.Next(20, 60);
                    break;
                case EnumDirection.Bottom:
                    x = Global.Rand.Next(0, Width);
                    y = Height + Global.Rand.Next(20, 60);
                    break;
            }
            return new Point(x, y);
        }

        /// <summary>
        /// 使用玩家物件技能
        /// </summary>
        /// <param name="index">技能索引</param>
        public virtual void UsePlayerSkill(int index)
        {
            if (PlayerObject == null || index >= PlayerObject.Skills.Count) return;

            PlayerObject.Skills[index].Use(new TargetTrackPoint(this));
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SceneBase
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Name = "SceneBase";
            this.ResumeLayout(false);

        }
    }

    /// <summary>
    /// 方向列舉
    /// </summary>
    public enum EnumDirection
    {
        Left = 0,
        Right = 1,
        Top = 2,
        Bottom = 3
    }
}
