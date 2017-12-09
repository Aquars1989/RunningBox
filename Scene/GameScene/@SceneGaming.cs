
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
        private static Font _InfoFont = new Font("微軟正黑體", 10);
        private static Font _LastTimeFont = new Font("微軟正黑體", 38, FontStyle.Bold);
        private static Cursor _StartCursor = new Cursor(new Bitmap(1, 1).GetHicon());

        #region ===== 事件 =====
        /// <summary>
        /// 每波時間變更
        /// </summary>
        public event ValueChangedEnentHandle<int> IntervalOfWaveChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於每波時間變更
        /// </summary>
        protected virtual void OnIntervalOfWaveChanged(int oldValue, int newValue)
        {
            SceneIntervalOfWave = (int)(IntervalOfWave / SceneSlow * SceneTimeFix);
            RoundPerWave = IntervalOfWave / IntervalOfRound;
            if (IntervalOfWaveChanged != null)
            {
                IntervalOfWaveChanged(this, oldValue, newValue);
            }
        }

        protected override void OnSceneTimeFixChanged(float oldValue, float newValue)
        {
            SceneIntervalOfWave = (int)(IntervalOfWave / SceneSlow * SceneTimeFix);
            base.OnSceneTimeFixChanged(oldValue, newValue);
        }

        /// <summary>
        /// 發生於回合時間變更
        /// </summary>
        protected override void OnIntervalOfRoundChanged(int oldValue, int newValue)
        {
            RoundPerWave = IntervalOfWave / IntervalOfRound;
            base.OnIntervalOfRoundChanged(oldValue, newValue);
        }

        /// <summary>
        /// 發生於場景速度減慢值變更
        /// </summary>
        protected override void OnSceneSlowChanged(float oldValue, float newValue)
        {
            SceneIntervalOfWave = (int)(IntervalOfWave / SceneSlow * SceneTimeFix);
            base.OnSceneSlowChanged(oldValue, newValue);
        }

        /// <summary>
        /// 發生於物件死亡時
        /// </summary>
        protected virtual void OnObjectDead(ObjectBase sender, ObjectBase killer, ObjectDeadType deadType)
        {
            if (sender.Equals(PlayerObject))
            {
                SetEnd(false);
            }
        }

        /// <summary>
        /// 發生於位置重新配置時
        /// </summary>
        protected override void OnReLayout()
        {
            base.OnReLayout();

            Padding padding = Global.DefaultMainRectanglePadding;
            MainRectangle = new Rectangle(padding.Left, padding.Top, Width - padding.Horizontal, Height - padding.Vertical);
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

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!ShowMenu)
            {
                if (IsStart)
                {
                    switch (e.Button)
                    {
                        case System.Windows.Forms.MouseButtons.Left:
                            ReleasePlayerSkill1();
                            break;
                        case System.Windows.Forms.MouseButtons.Right:
                            ReleasePlayerSkill2();
                            break;
                    }
                }
                else
                {
                    SetStart(e.X, e.Y);
                }
            }
            base.OnMouseUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && PlayerObject != null)
            {
                if (ShowMenu)
                {
                    ShowMenu = false;
                    IsStart = true;
                }
                else
                {
                    if (MenuCooldownCounter.IsFull)
                    {
                        ShowMenu = true;
                        MenuCooldownCounter.Value = 0;
                    }
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnDrawFloor(Graphics g)
        {
            if (MainRectangle != null)
            {
                g.DrawRectangle(_PenRectGaming, MainRectangle);
            }

            if (IsStart)
            {
                int lastTime = (PlayingInfo.PlayingTime.Limit - PlayingInfo.PlayingTime.Value) / Sec(1) + 1;
                if (lastTime < 10)
                {
                    g.DrawString(lastTime.ToString(), _LastTimeFont, BrushLastTime, MainRectangle, GlobalFormat.MiddleCenter);
                }
            }

            base.OnDrawFloor(g);
        }

        protected override void OnAfterDrawUI(Graphics g)
        {
            if (PlayingInfo != null)
            {
                g.DrawString(string.Format("波數：{0}/{1}", WaveNo.Value, WaveNo.Limit), _InfoFont, Brushes.Black, 390, 10);
                g.DrawString(string.Format("存活時間：{0:N1} 秒", PlayingInfo.PlayingTime.Value / (float)Sec(1F)), _InfoFont, Brushes.Black, 390, 30);
                g.DrawString(string.Format("分數：{0:N0}", PlayingInfo.Score), _InfoFont, Brushes.RoyalBlue, 390, 50);
            }

            if (!IsStart && !ShowMenu)
            {
                g.DrawString("請點擊任意區域開始", Font, Brushes.OrangeRed, MainRectangle, GlobalFormat.MiddleCenter);
            }
            base.OnAfterDrawUI(g);
        }

        /// <summary>
        /// 回合後執行動作
        /// </summary>
        protected override void OnAfterRound()
        {
            if (IsStart && !IsEnding)
            {
                PlayingInfo.PlayingTime.Value += SceneIntervalOfRound;
                if (PlayingInfo.PlayingTime.IsFull)
                {
                    SetEnd(true);
                }
            }

            base.OnAfterRound();
        }
        #endregion

        #region ===== 繪製物件 =====
        private DrawUISkillFrame _DrawSkill1 = new DrawUISkillFrame(Color.White, Color.Black, 2, 10, SkillKeyType.MouseButtonLeft);

        private DrawUISkillFrame _DrawSkill2 = new DrawUISkillFrame(Color.White, Color.Black, 2, 10, SkillKeyType.MouseButtonRight);
        #endregion

        #region ===== UI物件 =====
        /// <summary>
        /// 遊戲選單
        /// </summary>
        private ObjectUIGameMenu _UIMenu;

        /// <summary>
        /// 名稱顯示
        /// </summary>
        private ObjectUI _UIPlayName;

        /// <summary>
        /// 能量條物件
        /// </summary>
        private ObjectUI _UIEnergyBar;

        /// <summary>
        /// 技能1顯示物件
        /// </summary>
        private ObjectUI _UISkillIcon1;

        /// <summary>
        /// 技能2顯示物件
        /// </summary>
        private ObjectUI _UISkillIcon2;
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
                if (_DrawSkill1.DrawObjectInside != DrawNull.Value)
                {
                    _DrawSkill1.DrawObjectInside.Dispose();
                }

                _DrawSkill1.DrawObjectInside = Skill1 == null ? DrawNull.Value : Skill1.GetDrawObject(Color.DarkSlateGray) as DrawBase;
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
                if (_DrawSkill2.DrawObjectInside != DrawNull.Value)
                {
                    _DrawSkill2.DrawObjectInside.Dispose();
                }

                _DrawSkill2.DrawObjectInside = Skill2 == null ? DrawNull.Value : Skill2.GetDrawObject(Color.DarkSlateGray) as DrawBase;
            }
        }
        #endregion

        #region ===== 屬性 =====
        private CounterObject _MenuCooldownCounter;
        /// <summary>
        /// 選單冷卻計時器
        /// </summary>
        public CounterObject MenuCooldownCounter
        {
            get { return _MenuCooldownCounter; }
            set { _MenuCooldownCounter = value; }
        }

        private CounterObject _WaveCounter = new CounterObject(0);
        /// <summary>
        /// 波數計時器
        /// </summary>
        public CounterObject WaveCounter
        {
            get { return _WaveCounter; }
            set { _WaveCounter = value; }
        }

        /// <summary>
        /// 場景關卡設定檔
        /// </summary>
        public ISceneInfo SceneInfo { get; set; }

        /// <summary>
        /// 關卡等級
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 每波回合數
        /// </summary>
        public float RoundPerWave { get; private set; }

        /// <summary>
        /// 每波時間(以毫秒為單位,計入場景速度)
        /// </summary>
        public int SceneIntervalOfWave { get; private set; }

        /// <summary>
        /// 每波時間(以毫秒為單位)
        /// </summary>
        public int IntervalOfWave
        {
            get { return WaveCounter.Limit; }
            set
            {
                if (WaveCounter.Limit == value) return;
                int oldValue = WaveCounter.Limit;
                WaveCounter.Limit = value;
                OnIntervalOfWaveChanged(oldValue, value);
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
                if (value)
                {
                    IsStart = false;
                    LockScene(Color.FromArgb(100, 0, 0, 0), 6);

                    int mode = 0;
                    if (PlayingInfo.PlayingTime.IsFull)
                    {
                        mode = Level >= SceneInfo.MaxLevel ? 4 : 3;
                    }
                    else
                    {
                        mode = PlayerObject == null ? 2 : 1;
                    }

                    _UIMenu = new ObjectUIGameMenu(DirectionType.Center, Width / 2, Height / 2, MoveNull.Value, _PlayingInfo, mode);
                    _UIMenu.ButtonClick += (x, e) =>
                    {
                        switch (e)
                        {
                            case GameMenuCommandType.NextLevel:
                                ShowMenu = false;
                                GlobalScenes.ChoiceLevel++;
                                Level++;
                                Reset();
                                break;
                            case GameMenuCommandType.Retry:
                                ShowMenu = false;
                                Reset();
                                break;
                            case GameMenuCommandType.Resume:
                                ShowMenu = false;
                                IsStart = true;
                                break;
                            case GameMenuCommandType.Back:
                                OnGoScene(new SceneSkill());
                                break;
                        }
                    };
                    UIObjects.Add(_UIMenu);
                }
                else
                {
                    UnlockScene(12);
                    if (_UIMenu != null)
                    {
                        _UIMenu.Kill(null, ObjectDeadType.Clear);
                    }
                }
            }
        }

        private ScenePlayingInfo _PlayingInfo;
        /// <summary>
        /// 場景挑戰記錄
        /// </summary>
        public ScenePlayingInfo PlayingInfo
        {
            get { return _PlayingInfo; }
            private set
            {
                _PlayingInfo = value;
            }
        }

        /// <summary>
        /// 波數
        /// </summary>
        public CounterObject WaveNo { get; set; }

        private bool _IsStart;
        /// <summary>
        /// 是否開始遊戲
        /// </summary>
        protected bool IsStart
        {
            get { return _IsStart; }
            set
            {
                if (_IsStart == value) return;
                _IsStart = value;
                if (value)
                {
                    Cursor = _StartCursor;
                    DefaultCursor = _StartCursor;
                }
                else
                {
                    Cursor = Cursors.Default;
                    DefaultCursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// 遊戲是否已結束
        /// </summary>
        protected bool IsEnding { get; set; }

        private CounterObject _EndDelay;
        /// <summary>
        /// 遊戲結束延遲回合計時器
        /// </summary>
        protected CounterObject EndDelay
        {
            get { return _EndDelay; }
            set { _EndDelay = value; }
        }

        private Brush _BrushLastTime = new SolidBrush(Color.LightGreen);
        /// <summary>
        /// 剩餘時間繪製筆刷
        /// </summary>
        public Brush BrushLastTime
        {
            get { return _BrushLastTime; }
            set
            {
                if (_BrushLastTime == value) return;
                if (_BrushLastTime != null)
                {
                    _BrushLastTime.Dispose();
                }
                _BrushLastTime = value;
            }
        }

        private Pen _PenRectGaming = new Pen(Color.LightGreen, 2);
        /// <summary>
        /// 遊戲區域繪製畫筆
        /// </summary>
        public Pen PenRectGaming
        {
            get { return _PenRectGaming; }
            set
            {
                if (_PenRectGaming == value) return;
                if (_PenRectGaming != null)
                {
                    _PenRectGaming.Dispose();
                }
                _PenRectGaming = value;
            }
        }
        #endregion

        public SceneGaming()
        {
            SceneTimeFix = 1.4F;
            MenuCooldownCounter = new CounterObject(Sec(2));
            EndDelay = new CounterObject(Global.DefaultEndDelayLimit);
            IntervalOfWave = Global.DefaultIntervalOfWave;
            WaveNo = new CounterObject(0);
            Waves = new List<WaveLine>();
            WaveEvents = new Dictionary<string, WaveEventHandle>();


            _UIPlayName = new ObjectUI(75, 20, 150, 25, new DrawUIText(Color.Black, Color.WhiteSmoke, Color.Empty, Color.Empty, 0, 0, GlobalPlayer.PlayerName, new Font("微軟正黑體", 11), GlobalFormat.MiddleLeft));
            _UIEnergyBar = new ObjectUI(80, 45, 150, 15, new DrawUICounterBar(GlobalColors.EnergyBar, Color.Black, Color.AliceBlue, 2, false));
            _UISkillIcon1 = new ObjectUI(250, 10, 50, 50, _DrawSkill1);
            _UISkillIcon2 = new ObjectUI(320, 10, 50, 50, _DrawSkill2);

            Skill1 = GlobalScenes.ChoiceSkill1;
            Skill2 = GlobalScenes.ChoiceSkill2;

            UIObjects.Add(_UISkillIcon1);
            UIObjects.Add(_UISkillIcon2);
            UIObjects.Add(_UIPlayName);
            UIObjects.Add(_UIEnergyBar);
            GameObjects.ObjectDead += OnObjectDead;
            ShowMenu = false;
        }

        protected override void OnLoadComplete()
        {
            Reset();
            base.OnLoadComplete();
        }

        /// <summary>
        /// 回合事件
        /// </summary>
        protected override void Round()
        {
            if (Global.Freeze) return;

            UIObjects.ClearAllDead();

            OnBeforeRound();

            if (IsStart)
            {
                EffectObjects.AllDoBeforeRound();
                GameObjects.AllAction();
                EffectObjects.AllDoAfterRound();
            }
            OnAfterRound();

            if (IsStart)
            {
                EffectObjects.AllSettlement();
            }
            
            UIObjects.AllAction();

            GameObjects.ClearAllDead();
            UIObjects.ClearAllDead();
            EffectObjects.ClearAllDisabled();

            if (IsStart)
            {
                // 結束時停止波數增加但不立即停止遊戲
                if (IsEnding)
                {
                    if (EndDelay.IsFull)
                    {
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
                    if (WaveCounter.IsFull)
                    {
                        WaveCounter.Value = 0;
                        WaveNo.Value++;
                        GoWave(WaveNo.Value);
                    }
                    WaveCounter.Value += SceneIntervalOfRound;
                }

                if (!MenuCooldownCounter.IsFull)
                {
                    MenuCooldownCounter.Value += IntervalOfRound;
                }
            }

            Drawing();
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
            (_UIEnergyBar.DrawObject as DrawUICounterBar).BindingCounter = PlayerObject.Energy;


            IsStart = true;
            DoAfterStart();
        }

        /// <summary>
        /// 重設場景
        /// </summary>
        public void Reset()
        {
            IsStart = false;
            IsEnding = false;
            PlayingInfo = new ScenePlayingInfo(SceneInfo.SceneID, Level, SceneInfo.GetPlayingTimeLimit(Level));
            SceneSlow = 1F;
            GameObjects.Clear();
            EffectObjects.Clear();
            Waves.Clear();
            SetWave();

            int maxWave = 0;
            foreach (WaveLine wave in Waves)
            {
                maxWave = Math.Max(maxWave, wave.Length);
            }
            WaveNo.Limit = maxWave;
            WaveNo.Value = 0;
            WaveCounter.Value = 0;
            MenuCooldownCounter.Value = MenuCooldownCounter.Limit;

            Padding padding = Global.DefaultMainRectanglePadding;
            MainRectangle = new Rectangle(padding.Left, padding.Top, Width - padding.Horizontal, Height - padding.Vertical);
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
        /// 結束遊戲
        /// </summary>
        /// <param name="complete">是否完成</param>
        private void SetEnd(bool complete)
        {
            EffectObjects.AllBreak();
            PlayerObject = null;
            if (complete)
            {
                IsStart = false;
                ShowMenu = true;
                PlayingInfo.Complete = true;
            }
            else
            {
                IsEnding = true;
                EndDelay.Value = 0;
            }

            GlobalScenes.ChoiceScene.Settlement(PlayingInfo);
            DoAfterEnd();
        }

        /// <summary>
        /// 使用玩家物件技能1
        /// </summary>
        public virtual void UsePlayerSkill1()
        {
            if (Skill1 != null)
            {
                Skill1.Cast(this);
            }
        }

        /// <summary>
        /// 使用玩家物件技能2
        /// </summary>
        public virtual void UsePlayerSkill2()
        {
            if (Skill2 != null)
            {
                Skill2.Cast(this);
            }
        }

        /// <summary>
        /// 放開玩家物件技能1
        /// </summary>
        public virtual void ReleasePlayerSkill1()
        {
            if (Skill1 != null)
            {
                Skill1.Release(this);
            }
        }

        /// <summary>
        /// 放開玩家物件技能2
        /// </summary>
        public virtual void ReleasePlayerSkill2()
        {
            if (Skill2 != null)
            {
                Skill2.Release(this);
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
        /// 增加玩家分數
        /// </summary>
        /// <param name="from">來源</param>
        /// <param name="score">分數</param>
        public void AddScoreToPlayer(string from, int score)
        {
            PlayingInfo.AddScore(from, score);
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

            /// <summary>
            /// 此事件波數長度
            /// </summary>
            public int Length { get { return _Value.Length; } }

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
