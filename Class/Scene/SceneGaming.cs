
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
        /// 能量條物件
        /// </summary>
        private ObjectUI EnergyBar = new ObjectUI(80, 20, 150, 15, new DrawUiEnergyBar(Color.FromArgb(255, 200, 0)));

        /// <summary>
        /// 技能1顯示物件
        /// </summary>
        private ObjectUI SkillIcon1 = new ObjectUI(320, 10, 50, 50, null);

        /// <summary>
        /// 技能2顯示物件
        /// </summary>
        private ObjectUI SkillIcon2 = new ObjectUI(400, 10, 50, 50, null);
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

        #region ===== 屬性 =====
        /// <summary>
        /// 每波回合數
        /// </summary>
        public float RoundPerWave { get; private set; }

        /// <summary>
        /// 波數技時器
        /// </summary>
        private int _WaveTicks;

        /// <summary>
        /// 每波時間(以毫秒為單位,計入場景速度)
        /// </summary>
        public int SceneIntervalOfWave { get; private set; }

        private int _IntervalOfWave;
        /// <summary>
        /// 每波時間(以毫秒為單位)
        /// </summary>
        public int IntervalOfWave
        {
            get { return _IntervalOfWave; }
            set
            {
                _IntervalOfWave = value;
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

        /// <summary>
        /// 場景物件集合
        /// </summary>
        public ObjectCollection GameObjects { get; private set; }

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
        public int EndDelayTicks { get; set; }

        /// <summary>
        /// 遊戲結束延遲回合最大值
        /// </summary>
        public int EndDelayLimit { get; set; }

        private Pen _PenRectGaming = new Pen(Color.LightGreen, 2);
        public Pen PenRectGaming
        {
            get { return _PenRectGaming; }
            set { _PenRectGaming = value; }
        }
        #endregion

        public SceneGaming()
        {
            SceneSlow = 1;
            EndDelayLimit = Global.DefaultEndDelayLimit;
            IntervalOfWave = Global.DefaultIntervalOfWave;
            Waves = new List<WaveLine>();
            WaveEvents = new Dictionary<string, WaveEventHandle>();
            GameObjects = new ObjectCollection(this);

            UIObjects.Add(SkillIcon1);
            UIObjects.Add(SkillIcon2);
            UIObjects.Add(EnergyBar);
            GameObjects.ObjectDead += OnObjectDead;
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
            if (IsStart)
            {
                OnBeforeRound();
                EffectObjects.AllDoBeforeRound();
                GameObjects.AllAction();
                EffectObjects.AllDoAfterRound();
                OnAfterRound();

                GameObjects.ClearAllDead();
                UIObjects.ClearAllDead();
                EffectObjects.ClearAllDisabled();

                //結束時停止波數增加但不立即停止遊戲
                if (IsEnding)
                {
                    if (EndDelayTicks >= EndDelayLimit)
                    {
                        IsEnding = false;
                        IsStart = false;
                    }
                    EndDelayTicks += SceneIntervalOfRound;
                }
                else
                {
                    if (_WaveTicks >= IntervalOfWave)
                    {
                        _WaveTicks = 0;
                        Level++;
                        GoWave(Level);
                    }
                    _WaveTicks += SceneIntervalOfRound;
                }
            }
            Drawing();

            //顯示FPS
            if (Global.DebugMode)
            {
                BufferGraphics.DrawString(_FPSText, _FPSFont, Brushes.Red, Width - 50, 5);
            }
            ThisGraphics.DrawImageUnscaled(BufferImage, 0, 0);

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
        /// 繪製畫面
        /// </summary>
        protected override void Drawing()
        {
            BufferGraphics.Clear(Color.White);
            OnBeforeDraw(BufferGraphics);
            EffectObjects.AllDoBeforeDraw(BufferGraphics);

            //繪製場地邊界
            EffectObjects.AllDoBeforeDrawBack(BufferGraphics);
            if (MainRectangle != null)
            {
                BufferGraphics.DrawRectangle(_PenRectGaming, MainRectangle);
            }

            //繪製物件
            EffectObjects.AllDoBeforeDrawObject(BufferGraphics);
            GameObjects.AllDrawSelf(BufferGraphics);

            //繪製UI
            OnBeforeDrawUI(BufferGraphics);
            EffectObjects.AllDoBeforeDrawUI(BufferGraphics);

            BufferGraphics.DrawString(string.Format("Lv:{0}    Score:{1}", Level, Score), Font, Brushes.Black, 85, 50);
            UIObjects.AllDrawSelf(BufferGraphics);
            OnAfterDrawUI(BufferGraphics);

            EffectObjects.AllDoAfterDraw(BufferGraphics);
            OnAfterDrawReset(BufferGraphics);
            OnAfterDraw(BufferGraphics);
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
            SceneSlow = 1;

            GameObjects.Clear();
            EffectObjects.Clear();
            Waves.Clear();
            SetWave();

            PlayerObject = CreatePlayerObject(potX, potY);
            GameObjects.Add(PlayerObject);
            Skill1 = PlayerObject.Skills.Count > 0 ? PlayerObject.Skills[0] : null;
            Skill2 = PlayerObject.Skills.Count > 1 ? PlayerObject.Skills[1] : null;
            (EnergyBar.DrawObject as DrawUiEnergyBar).BindingObject = PlayerObject;
            MainRectangle = new Rectangle(80, 80, Width - 160, Height - 160);

            Cursor.Hide();
            IsStart = true;
            DoAfterStart();
        }

        /// <summary>
        /// 回合後執行動作
        /// </summary>
        protected override void OnAfterRound()
        {
            if (!IsEnding)
            {
                Score += Level;
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

        protected override void OnSceneSlowChanged()
        {

            base.OnSceneSlowChanged();
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

        /// <summary>
        /// 結束遊戲
        /// </summary>
        private void SetEnd()
        {
            EffectObjects.AllBreak();
            PlayerObject = null;
            IsEnding = true;
            EndDelayTicks = 0;
            Cursor.Show();
            DoAfterEnd();
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
                        int n = c - '0';
                        if (n >= 0 && n <= 9)
                        {
                            result = n;
                        }
                        WaveNO = waveNO + 1;
                    }
                }
                return result;
            }
        }
    }
}
