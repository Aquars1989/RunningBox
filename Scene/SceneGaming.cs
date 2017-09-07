
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
        private static Cursor _StartCursor = new Cursor(new Bitmap(1, 1).GetHicon());
        private static Font _FPSFont = new Font("Arial", 12);
        private Stopwatch _FPSWatch = new Stopwatch();
        private int _FPSTick = 0;
        private string _FPSText = "";

        #region ===== 委派 =====
        /// <summary>
        /// 處理關卡事件
        /// </summary>
        /// <param name="value">事件參數值</param>
        /// <returns>回傳是否引發事件</returns>
        public delegate void WaveEventHandle(int value);
        #endregion

        #region ===== 事件 =====
        /// <summary>
        /// 每波時間變更
        /// </summary>
        public event EventHandler IntervalOfWaveChanged;
        #endregion

        #region ===== UI物件 =====
        /// <summary>
        /// 
        /// </summary>
        private ObjectUI _DarkCover = new ObjectUI(0, 0, 150, 15, new DrawBrush(Color.FromArgb(100, 0, 0, 0), ShapeType.Rectangle));

        /// <summary>
        /// 
        /// </summary>
        private ObjectUI _CommandRetry = new ObjectUI(0, 0, 150, 50, new DrawUIString(Color.Black, Color.White, Color.Black, 2, "重試", Global.CommandFont, Global.CommandFormat));

        /// <summary>
        /// 
        /// </summary>
        private ObjectUI _CommandBack = new ObjectUI(0, 0, 150, 50, new DrawUIString(Color.Black, Color.White, Color.Black, 2, "返回", Global.CommandFont, Global.CommandFormat));

        /// <summary>
        /// 能量條物件
        /// </summary>
        private ObjectUI _EnergyBar = new ObjectUI(80, 20, 150, 15, new DrawUICounterBar(Colors.EnergyBar, Color.Black, Color.AliceBlue, 2, false));

        /// <summary>
        /// 技能1顯示物件
        /// </summary>
        private ObjectUI _SkillIcon1 = new ObjectUI(320, 10, 50, 50, new DrawUISkillFrame(Color.Black, SkillKeyType.MouseButtonLeft));

        /// <summary>
        /// 技能2顯示物件
        /// </summary>
        private ObjectUI _SkillIcon2 = new ObjectUI(400, 10, 50, 50, new DrawUISkillFrame(Color.Black, SkillKeyType.MouseButtonRight));
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
                if (_SkillIcon1.DrawObject != null)
                {
                    _SkillIcon1.DrawObject.Dispose();
                }

                (_SkillIcon1.DrawObject as DrawUISkillFrame).IconDrawObject = Skill1 == null ? DrawNull.Value : Skill1.GetDrawObject(Color.DarkSlateGray) as DrawBase;
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
                if (_SkillIcon2.DrawObject != null)
                {
                    _SkillIcon2.DrawObject.Dispose();
                }

                (_SkillIcon2.DrawObject as DrawUISkillFrame).IconDrawObject = Skill2 == null ? DrawNull.Value : Skill2.GetDrawObject(Color.DarkSlateGray) as DrawBase;
            }
        }
        #endregion

        #region ===== 屬性 =====
        /// <summary>
        /// 每波回合數
        /// </summary>
        public float RoundPerWave { get; private set; }

        /// <summary>
        /// 每波時間(以毫秒為單位,計入場景速度)
        /// </summary>
        public int SceneIntervalOfWave { get; private set; }

        private CounterObject _WaveCounter = new CounterObject(0);
        /// <summary>
        /// 每波時間(以毫秒為單位)
        /// </summary>
        public int IntervalOfWave
        {
            get { return _WaveCounter.Limit; }
            set
            {
                _WaveCounter.Limit = value;
                OnIntervalOfWaveChanged();
            }
        }

        /// <summary>
        /// 關卡波數設定值
        /// </summary>
        public List<WaveLine> Waves { get; private set; }

        /// <summary>
        /// 關卡事件設定
        /// </summary>
        public Dictionary<string, WaveEventHandle> WaveEvents { get; private set; }

        /// <summary>
        /// 玩家物件
        /// </summary>
        public ObjectActive PlayerObject { get; set; }

        private bool _ShowMenu;
        /// <summary>
        /// 是否顯示選單
        /// </summary>
        public bool ShowMenu
        {
            get { return _ShowMenu; }
            set
            {
                _ShowMenu = value;
                _DarkCover.Visible = value;
                _CommandBack.Visible = value;
                _CommandRetry.Visible = value;
            }
        }

        /// <summary>
        /// 分數
        /// </summary>
        public CounterObject Score { get; private set; }

        /// <summary>
        /// 波數
        /// </summary>
        public CounterObject WaveNo { get; set; }

        /// <summary>
        /// 是否開始遊戲
        /// </summary>
        protected bool IsStart { get; set; }

        /// <summary>
        /// 遊戲是否已結束
        /// </summary>
        protected bool IsEnding { get; set; }

        /// <summary>
        /// 遊戲結束延遲回合計時器
        /// </summary>
        protected CounterObject EndDelay { get; private set; }


        private Pen _PenRectGaming = new Pen(Color.LightGreen, 2);
        public Pen PenRectGaming
        {
            get { return _PenRectGaming; }
            set { _PenRectGaming = value; }
        }
        #endregion

        public SceneGaming()
        {
            EndDelay = new CounterObject(Global.DefaultEndDelayLimit);
            IntervalOfWave = Global.DefaultIntervalOfWave;
            Score = new CounterObject(Global.DefaultScoreMax);
            WaveNo = new CounterObject(Global.DefaultWaveMax);
            Waves = new List<WaveLine>();
            WaveEvents = new Dictionary<string, WaveEventHandle>();

            _CommandRetry.Click += (x, e) =>
            {
                ShowMenu = false;
                SetStart(e.X, e.Y);
            };
            _CommandBack.Click += (x, e) => { OnGoScene(new SceneSkill()); };
            UIObjects.Add(_SkillIcon1);
            UIObjects.Add(_SkillIcon2);
            UIObjects.Add(_EnergyBar);
            UIObjects.Add(_DarkCover);
            UIObjects.Add(_CommandBack);
            UIObjects.Add(_CommandRetry);
            GameObjects.ObjectDead += OnObjectDead;
            ShowMenu = false;
        }

        /// <summary>
        /// 回合事件
        /// </summary>
        protected override void Round()
        {
            //計算FPS
            _FPSTick--;
            bool refreshFPS = false;
            if (_FPSTick <= 0)
            {
                _FPSTick = 10;
                _FPSWatch.Restart();
                refreshFPS = true;
            }

            UIObjects.ClearAllDead();

            OnBeforeRound();
            EffectObjects.AllDoBeforeRound();
            if (IsStart)
            {
                GameObjects.AllAction();
            }
            EffectObjects.AllDoAfterRound();
            OnAfterRound();

            GameObjects.ClearAllDead();
            UIObjects.ClearAllDead();
            EffectObjects.ClearAllDisabled();

            if (IsStart)
            {
                //結束時停止波數增加但不立即停止遊戲
                if (IsEnding)
                {
                    if (EndDelay.IsFull)
                    {
                        Cursor = Cursors.Default;
                        DefaultCursor = Cursors.Default;
                        //Cursor.Show();
                        ShowMenu = true;
                        IsEnding = false;
                        IsStart = false;
                    }
                    else
                    {
                        EndDelay.Value += IntervalOfRound;
                    }
                }
                else if (!WaveNo.IsFull)
                {
                    if (_WaveCounter.IsFull)
                    {
                        _WaveCounter.Value = 0;
                        WaveNo.Value++;
                        GoWave(WaveNo.Value);
                    }
                    _WaveCounter.Value += SceneIntervalOfRound;
                }
            }

            Drawing();

            //顯示FPS
            if (Global.DebugMode)
            {
                ThisGraphics.DrawString(_FPSText, _FPSFont, Brushes.Red, Width - 50, 5);
            }

            //計算FPS
            if (refreshFPS)
            {
                _FPSWatch.Stop();
                _FPSText = (TimeSpan.TicksPerSecond / _FPSWatch.Elapsed.Ticks).ToString();
            }
        }

        /// <summary>
        /// 前往指定波數
        /// </summary>
        /// <param name="waveNo">波數號碼</param>
        /// <returns>是否已達波數尾端</returns>
        protected virtual bool GoWave(int waveNo)
        {
            bool result = true;
            foreach (WaveLine waveLine in Waves)
            {
                if (waveLine.IsEnd) continue;
                WaveEventHandle waveEvent;
                if (WaveEvents.TryGetValue(waveLine.WaveID, out waveEvent))
                {
                    result = false;
                    int value = waveLine.GetValue(waveNo);
                    if (value >= 0)
                    {
                        waveEvent(value);
                    }
                }
            }
            DoAfterWave();
            return result;
        }

        /// <summary>
        /// 開始遊戲
        /// </summary>
        /// <param name="potX">玩家起始點X</param>
        /// <param name="potY">玩家起始點Y</param>
        public void SetStart(int potX, int potY)
        {
            WaveNo.Value = 0;
            Score.Value = 0;
            SceneSlow = 1;

            GameObjects.Clear();
            EffectObjects.Clear();
            Waves.Clear();
            SetWave();

            PlayerObject = CreatePlayerObject(potX, potY);
            if (Skill1 != null)
            {
                Skill1.Reset();
                PlayerObject.Skills.Add(Skill1);
            }

            if (Skill2 != null)
            {
                Skill2.Reset();
                PlayerObject.Skills.Add(Skill2);
            }

            GameObjects.Add(PlayerObject);
            (_EnergyBar.DrawObject as DrawUICounterBar).BindingCounter = PlayerObject.Energy;

            Padding padding = Global.DefaultMainRectanglePadding;
            MainRectangle = new Rectangle(padding.Left, padding.Top, Width - padding.Horizontal, Height - padding.Vertical);

            Cursor = _StartCursor;
            DefaultCursor = _StartCursor;
            //Cursor.Hide();
            IsStart = true;
            DoAfterStart();
        }

        protected override void OnDrawFloor(Graphics g)
        {
            if (MainRectangle != null)
            {
                BufferGraphics.DrawRectangle(_PenRectGaming, MainRectangle);
            }
            base.OnDrawFloor(g);
        }

        protected override void OnAfterDrawUI(Graphics g)
        {
            BufferGraphics.DrawString(string.Format("波數:{0:N0}    存活時間:{1:N2} 秒", WaveNo.Value, Score.Value / 1000F), Font, Brushes.Black, 85, 50);
            base.OnAfterDrawUI(g);
        }

        /// <summary>
        /// 回合後執行動作
        /// </summary>
        protected override void OnAfterRound()
        {
            if (IsStart && !IsEnding)
            {
                Score.Value += IntervalOfRound;
                if (Score.IsFull)
                {
                    SetEnd();
                }
            }

            base.OnAfterRound();
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
        /// 每波時間變更
        /// </summary>
        protected virtual void OnIntervalOfWaveChanged()
        {
            SceneIntervalOfWave = (int)(IntervalOfWave / SceneSlow);
            RoundPerWave = IntervalOfWave / IntervalOfRound;
            if (IntervalOfWaveChanged != null)
            {
                IntervalOfWaveChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 回合時間變更
        /// </summary>
        protected override void OnIntervalOfRoundChanged()
        {
            SceneIntervalOfWave = (int)(IntervalOfWave / SceneSlow);
            RoundPerWave = IntervalOfWave / IntervalOfRound;
            base.OnIntervalOfRoundChanged();
        }

        /// <summary>
        /// 物件死亡時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnObjectDead(ObjectBase sender, ObjectBase killer, ObjectDeadType deadType)
        {
            if (sender.Equals(PlayerObject))
            {
                SetEnd();
            }
        }

        protected override void OnReLayout()
        {
            base.OnReLayout();

            Padding padding = Global.DefaultMainRectanglePadding;
            MainRectangle = new Rectangle(padding.Left, padding.Top, Width - padding.Horizontal, Height - padding.Vertical);

            _DarkCover.Layout.Width = Width;
            _DarkCover.Layout.Height = Height;
            _CommandRetry.Layout.Y = (Height - _CommandRetry.Layout.Height) / 2;
            _CommandBack.Layout.Y = (Height - _CommandBack.Layout.Height) / 2;
            _CommandRetry.Layout.X = Width / 2 + (int)(Width * 0.05F);
            _CommandBack.Layout.X = Width / 2 - (int)(Width * 0.05F) - _CommandBack.Layout.Width;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!ShowMenu)
            {
                if (IsStart)
                {
                    switch (e.Button)
                    {
                        case System.Windows.Forms.MouseButtons.Left:
                            UsePlayerSkill1();
                            break;
                        case System.Windows.Forms.MouseButtons.Right:
                            UsePlayerSkill2();
                            break;
                    }
                }
                else
                {
                    SetStart(e.X, e.Y);
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            TrackPoint = e.Location;
            base.OnMouseMove(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && PlayerObject != null)
            {
                PlayerObject.Kill(null, ObjectDeadType.Clear);
                EndDelay.Value = EndDelay.Limit;
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// 結束遊戲
        /// </summary>
        private void SetEnd()
        {
            EffectObjects.AllBreak();
            PlayerObject = null;
            IsEnding = true;
            EndDelay.Value = 0;
            DoAfterEnd();
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

        /// <summary>
        /// 波數時間轉換為毫秒
        /// </summary>
        /// <param name="wave">波數</param>
        /// <returns>毫秒</returns>
        public int Wave(float wave)
        {
            return (int)(wave * IntervalOfWave + 0.5F);
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
            /// 目前在第幾波(從1開始)
            /// </summary>
            public int WaveNO
            {
                get { return _WaveNO; }
                set
                {
                    _WaveNO = value;
                    IsEnd = _WaveNO > _Value.Length;
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
            /// <param name="waveNO">波數編號(從1開始)</param>
            /// <returns></returns>
            public int GetValue(int waveNO)
            {
                if (IsEnd) return -1;

                int result = -1;
                if (waveNO >= WaveNO)
                {
                    int no = waveNO - 1;
                    char c = Value[no];
                    if (c == '+')
                    {
                        result = 1;
                        for (; no < Value.Length && Value[no] == '+'; ++no)
                        {
                            result++;
                        }
                        WaveNO = no + 1;
                    }
                    else
                    {
                        if (c >= '0' && c <= '9')
                        {
                            result = c - '0';
                        }
                        else if (c >= 'A' && c <= 'Z')
                        {
                            result = c - 'A' + 10;
                        }
                        WaveNO = waveNO + 1;
                    }
                }
                return result;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SceneGaming
            // 
            this.Name = "SceneGaming";
            this.ResumeLayout(false);

        }
    }
}
