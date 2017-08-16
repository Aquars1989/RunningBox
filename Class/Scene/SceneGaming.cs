
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 基本場景物件
    /// </summary>
    public abstract class SceneGaming : SceneBase
    {
        #region ===== 委派 =====
        /// <summary>
        /// 處理關卡事件
        /// </summary>
        /// <param name="value">事件參數值</param>
        /// <returns>回傳是否引發事件</returns>
        public delegate void WaveEventHandle(int value);
        #endregion

        #region ===== UI物件 =====
        /// <summary>
        /// 技能1顯示物件
        /// </summary>
        private ObjectUIIcon SkillIcon1 = new ObjectUIIcon(null) { X = 320, Y = 35, Size = 25 };

        /// <summary>
        /// 技能2顯示物件
        /// </summary>
        private ObjectUIIcon SkillIcon2 = new ObjectUIIcon(null) { X = 400, Y = 35, Size = 25 };
        #endregion

        #region ===== 技能物件 =====
        private SkillBase _Skill1;
        /// <summary>
        /// 玩家技能1
        /// </summary>
        public SkillBase Skill1
        {
            get { return _Skill1; }
            set
            {
                _Skill1 = value;
                if (SkillIcon1.DrawObject != null)
                {
                    SkillIcon1.DrawObject.Dispose();
                }

                SkillIcon1.DrawObject = Skill1 == null ? null : Skill1.GetDrawObject(Color.Black, EnumSkillButton.MouseButtonLeft);
            }
        }

        private SkillBase _Skill2;
        /// <summary>
        /// 玩家技能2
        /// </summary>
        public SkillBase Skill2
        {
            get { return _Skill2; }
            set
            {
                _Skill2 = value;
                if (SkillIcon2.DrawObject != null)
                {
                    SkillIcon2.DrawObject.Dispose();
                }

                SkillIcon2.DrawObject = Skill2 == null ? null : Skill2.GetDrawObject(Color.Black, EnumSkillButton.MouseButtonRight);
            }
        }
        #endregion

        /// <summary>
        /// 關卡波數設定值
        /// </summary>
        public List<WaveLine> Waves { get; private set; }

        /// <summary>
        /// 關卡事件設定
        /// </summary>
        public Dictionary<string, WaveEventHandle> WaveEvents { get; private set; }

        /// <summary>
        /// 世界速度減慢值 speed=(old speed/WorldSpeedSlow)
        /// </summary>
        public float WorldSpeedSlow { get; set; }

        /// <summary>
        /// 玩家物件
        /// </summary>
        public ObjectActive PlayerObject { get; set; }

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

        private Timer _TimerOfWave = new Timer() { Enabled = true, Interval = 1000 };
        /// <summary>
        /// 關卡計時器
        /// </summary>
        public Timer TimerOfWave
        {
            get { return _TimerOfWave; }
        }

        public SceneGaming()
        {
            WorldSpeedSlow = 1;
            EndDelayRoundMax = 30;
            Waves = new List<WaveLine>();
            WaveEvents = new Dictionary<string, WaveEventHandle>();
            GameObjects = new ObjectCollection(this);
            EffectObjects = new EffectCollection(this);

            UIObjects.Add(SkillIcon1);
            UIObjects.Add(SkillIcon2);

            GameObjects.ObjectDead += OnObjectDead;
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
            UIObjects.ClearAllDead();
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

        /// <summary>
        /// 關卡計時器運作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerOfWave_Tick(object sender, EventArgs e)
        {
            if (!IsStart) return;

            bool end = true;
            int waveNo = Level - 1;
            foreach (WaveLine waveLine in Waves)
            {
                if (waveLine.IsEnd) continue;
                WaveEventHandle waveEvent;
                if (WaveEvents.TryGetValue(waveLine.WaveID, out waveEvent))
                {
                    end = false;
                    int value = waveLine.GetValue(waveNo);
                    if (value >= 0)
                    {
                        waveEvent(value);
                    }
                }
            }

            Level++;
            DoAfterWave();
        }

        private Stopwatch _watchFPS = new Stopwatch();
        private int _tickFPS = 0;
        private string _txtFPS = "";
        private Font _fontFPS = new Font("Arial", 12);

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
            EffectObjects.AllDoBeforeDrawBack(_SceneGraphics);

            if (GameRectangle != null)
            {
                _SceneGraphics.DrawRectangle(_PenRectGaming, GameRectangle);
            }

            _SceneGraphics.FillRectangle(Brushes.AliceBlue, _RectOfEngery);
            if (PlayerObject != null)
            {
                float ratio = (float)PlayerObject.Energy / PlayerObject.EnergyMax;
                Brush brush = ratio < 0.3 ? Brushes.Red : Brushes.Black;
                _SceneGraphics.FillRectangle(brush, _RectOfEngery.X + 2, _RectOfEngery.Y + 2, (_RectOfEngery.Width - 4) * ratio, _RectOfEngery.Height - 4);
            }

            _SceneGraphics.DrawRectangle(Pens.Black, _RectOfEngery);
            _SceneGraphics.DrawString(string.Format("Lv:{0}    Score:{1}", Level, Score), Font, Brushes.Black, _RectOfEngery.X + _RectOfEngery.Width + 10, _RectOfEngery.Y);

            EffectObjects.AllDoBeforeDrawObject(_SceneGraphics);
            GameObjects.AllDrawSelf(_SceneGraphics);
            EffectObjects.AllDoAfterDraw(_SceneGraphics);

            EffectObjects.AllDoBeforeDrawUI(_SceneGraphics);
            UIObjects.AllDrawSelf(_SceneGraphics);

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
            WorldSpeedSlow = 1;

            GameObjects.Clear();
            EffectObjects.Clear();
            Waves.Clear();
            SetWave();

            PlayerObject = CreatePlayerObject(potX, potY);
            GameObjects.Add(PlayerObject);
            Skill1 = PlayerObject.Skills.Count > 0 ? PlayerObject.Skills[0] : null;
            Skill2 = PlayerObject.Skills.Count > 1 ? PlayerObject.Skills[1] : null;

            GameRectangle = new Rectangle(80, 80, Width - 160, Height - 160);

            if (_SceneImage != null) _SceneImage.Dispose();
            if (_SceneGraphics != null) _SceneGraphics.Dispose();
            if (_ThisGraphics != null) _ThisGraphics.Dispose();

            _SceneImage = new Bitmap(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            _SceneGraphics = Graphics.FromImage(_SceneImage);
            _SceneGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _SceneGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            _ThisGraphics = CreateGraphics();
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
        /// 關卡設置
        /// </summary>
        public abstract void SetWave();

        /// <summary>
        /// 每波結束後執行動作
        /// </summary>
        public abstract void DoAfterWave();

        /// <summary>
        /// 開始後執行動作
        /// </summary>
        public abstract void DoAfterStart();

        /// <summary>
        /// 結束後執行動作
        /// </summary>
        public abstract void DoAfterEnd();

        /// <summary>
        /// 建立玩家物件
        /// </summary>
        public abstract ObjectActive CreatePlayerObject(int potX, int potY);

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
            return (int)(sec * 1000 / _RoundTimer.Interval);
        }

        /// <summary>
        /// 波數時間轉換為Rounds
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public int WaveToRounds(float sec)
        {
            return (int)(sec * _TimerOfWave.Interval / _RoundTimer.Interval);
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
        /// 使用玩家物件技能1
        /// </summary>
        public virtual void UsePlayerSkill1()
        {
            if (Skill1 != null)
            {
                Skill1.Use(new TargetTrackPoint(this));
            }
        }

        /// <summary>
        /// 使用玩家物件技能2
        /// </summary>
        public virtual void UsePlayerSkill2()
        {
            if (Skill2 != null)
            {
                Skill2.Use(new TargetTrackPoint(this));
            }
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

        /// <summary>
        /// 新增特定種類事件每波設定值
        /// </summary>
        public class WaveLine
        {
            /// <summary>
            /// 此事件是否結束
            /// </summary>
            public bool IsEnd { get; private set; }

            private string _Value;
            /// <summary>
            /// 每波設定值,有效字元為0-9和+號('+++ + '='3   1 '),其餘視為-1
            /// </summary>
            public string Value
            {
                get { return _Value; }
                set
                {
                    _Value = value;
                    IsEnd = _WaveNO >= _Value.Length;
                }
            }

            private string _WaveID;
            /// <summary>
            /// 關卡事件ID
            /// </summary>
            public string WaveID
            {
                get { return _WaveID; }
                set { _WaveID = value.Trim(); }
            }

            private int _WaveNO;
            /// <summary>
            /// 目前在第幾波
            /// </summary>
            public int WaveNO
            {
                get { return _WaveNO; }
                set
                {
                    _WaveNO = value;
                    IsEnd = _WaveNO >= _Value.Length;
                }
            }

            /// <summary>
            /// 新增特定種類事件每波設定值
            /// </summary>
            /// <param name="waveID">關卡事件ID</param>
            /// <param name="value">每波設定值,有效字元為0-9和+號('+++ + '='3   1 '),其餘視為-1</param>
            public WaveLine(string waveID, string value)
            {
                WaveID = waveID;
                _Value = value;
                _WaveNO = 0;
                IsEnd = _Value.Length == 0;
            }

            /// <summary>
            /// <para>如果輸入波數大於目前波數 取得輸入波數值並調整目前波數=輸入波數值+1</para>
            /// <para>如為+號,回傳有幾個連續+號並調整目前波數=+號尾端</para>
            /// </summary>
            /// <param name="no"></param>
            /// <returns></returns>
            public int GetValue(int no)
            {
                if (IsEnd) return -1;

                int result = -1;
                if (no >= WaveNO)
                {
                    WaveNO = no + 1;

                    char c = Value[no];
                    if (c == '+')
                    {
                        result = 1;
                        int i = WaveNO;
                        for (; i < Value.Length && Value[i] == '+'; i++)
                        {
                            result++;
                        }
                        WaveNO = i;
                    }
                    else
                    {
                        int n = Value[no] - '0';
                        if (n >= 0 && n <= 9)
                        {
                            result = n;
                        }
                    }
                }
                return result;
            }
        }
    }
}
