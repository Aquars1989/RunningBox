
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
    public class SceneBase : UserControl
    {
        private Timer _RoundTimer = new Timer();
        /// <summary>
        /// 回合計時器
        /// </summary>
        protected Timer RoundTimer { get { return _RoundTimer; } }

        /// <summary>
        /// 場景物件本身的Graphics物件
        /// </summary>
        protected Graphics ThisGraphics { get; set; }

        /// <summary>
        /// 緩衝畫布的Graphics物件
        /// </summary>
        protected Graphics BufferGraphics { get; set; }

        /// <summary>
        /// 緩衝畫布
        /// </summary>
        protected Bitmap BufferImage { get; set; }

        #region ===== 事件 =====
        /// <summary>
        /// 場景速度減慢值變更
        /// </summary>
        public event EventHandler SceneSlowChanged;

        /// <summary>
        /// 回合時間變更
        /// </summary>
        public event EventHandler IntervalOfRoundChanged;

        /// <summary>
        /// 回合開始前執行
        /// </summary>
        public event EventHandler BeforeRound;

        /// <summary>
        /// 回合結束後執行
        /// </summary>
        public event EventHandler AfterRound;

        /// <summary>
        /// 繪製畫面前執行,在此設定繪製參數
        /// </summary>
        public event PaintEventHandler BeforeDraw;

        /// <summary>
        /// 繪製地板
        /// </summary>
        public event PaintEventHandler DrawFloor;

        /// <summary>
        /// 繪製UI前執行
        /// </summary>
        public event PaintEventHandler BeforeDrawUI;

        /// <summary>
        /// 繪製UI後執行
        /// </summary>
        public event PaintEventHandler AfterDrawUI;

        /// <summary>
        /// 繪製結束前執行,在此重置畫布設定
        /// </summary>
        public event PaintEventHandler AfterDrawReset;

        /// <summary>
        /// 繪製畫面後執行
        /// </summary>
        public event PaintEventHandler AfterDraw;
        #endregion

        #region===== 屬性 =====
        /// <summary>
        /// 場景每秒回合數(計入場景速度)
        /// </summary>
        public float SceneRoundPerSec { get; private set; }

        /// <summary>
        /// 原始每秒回合數(不計場景速度)
        /// </summary>
        public float RoundPerSec { get; private set; }

        /// <summary>
        /// 場景每秒回合時間(以毫秒為單位,計入場景速度)
        /// </summary>
        public int SceneIntervalOfRound { get; private set; }

        /// <summary>
        /// 原始每回合時間(以毫秒為單位,不計場景速度)
        /// </summary>
        public virtual int IntervalOfRound
        {
            get { return RoundTimer.Interval; }
            set
            {
                RoundTimer.Interval = value;
                OnIntervalOfRoundChanged();
            }
        }

        private float _SceneSlow;
        /// <summary>
        /// 場景速度減慢值 speed=(speed/SceneSlow)
        /// </summary>
        public float SceneSlow
        {
            get { return _SceneSlow; }
            set
            {
                _SceneSlow = value;
                OnSceneSlowChanged();
            }
        }

        /// <summary>
        /// 場景追蹤點
        /// </summary>
        public Point TrackPoint { get; set; }

        /// <summary>
        /// 主要區域
        /// </summary>
        public Rectangle MainRectangle { get; set; }

        /// <summary>
        /// UI物件集合
        /// </summary>
        public ObjectCollection UIObjects { get; private set; }

        /// <summary>
        /// 場景物件集合
        /// </summary>
        public ObjectCollection GameObjects { get; private set; }

        /// <summary>
        /// 場景特效集合
        /// </summary>
        public EffectCollection EffectObjects { get; private set; }
        #endregion

        /// <summary>
        /// 新增基本場景物件
        /// </summary>
        public SceneBase()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
            SceneSlow = 1;
            IntervalOfRound = Global.DefaultIntervalOfRound;
            UIObjects = new ObjectCollection(this);
            EffectObjects = new EffectCollection(this);
            GameObjects = new ObjectCollection(this);
            RoundTimer.Tick += RoundTimer_Tick;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RoundTimer.Enabled = false;
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SceneBase
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Name = "SceneBase";
            this.Load += new System.EventHandler(this.SceneBase_Load);
            this.SizeChanged += new System.EventHandler(this.SceneBase_SizeChanged);
            this.ResumeLayout(false);

        }

        private void SceneBase_Load(object sender, EventArgs e)
        {
            MainRectangle = ClientRectangle;
            BufferImage = new Bitmap(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            BufferGraphics = Graphics.FromImage(BufferImage);
            BufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            BufferGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            ThisGraphics = CreateGraphics();
            RoundTimer.Enabled = true;
        }

        private void RoundTimer_Tick(object sender, EventArgs e)
        {
            Round();
        }

        /// <summary>
        /// 回合動作
        /// </summary>
        protected virtual void Round()
        {
            OnBeforeRound();
            EffectObjects.AllDoBeforeRound();
            GameObjects.AllAction();
            EffectObjects.AllDoAfterRound();
            OnAfterRound();

            GameObjects.ClearAllDead();
            UIObjects.ClearAllDead();
            EffectObjects.ClearAllDisabled();
            Drawing();
        }

        /// <summary>
        /// 繪製畫面
        /// </summary>
        protected void Drawing()
        {
            BufferGraphics.Clear(Color.White);
            OnBeforeDraw(BufferGraphics);
            EffectObjects.AllDoBeforeDraw(BufferGraphics);
            EffectObjects.AllDoBeforeDrawFloor(BufferGraphics);
            OnDrawFloor(BufferGraphics);
            EffectObjects.AllDoBeforeDrawObject(BufferGraphics);
            GameObjects.AllDrawSelf(BufferGraphics);
            OnBeforeDrawUI(BufferGraphics);
            EffectObjects.AllDoBeforeDrawUI(BufferGraphics);
            UIObjects.AllDrawSelf(BufferGraphics);
            OnAfterDrawUI(BufferGraphics);
            EffectObjects.AllDoAfterDraw(BufferGraphics);
            OnAfterDrawReset(BufferGraphics);
            OnAfterDraw(BufferGraphics);
            ThisGraphics.DrawImageUnscaled(BufferImage, 0, 0);
        }

        /// <summary>
        /// 回合時間變更
        /// </summary>
        protected virtual void OnIntervalOfRoundChanged()
        {
            RoundPerSec = 1000F / IntervalOfRound;
            SceneRoundPerSec = 1000F / IntervalOfRound * SceneSlow;
            SceneIntervalOfRound = (int)(IntervalOfRound / SceneSlow);

            if (IntervalOfRoundChanged != null)
            {
                IntervalOfRoundChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 場景速度減慢值變更
        /// </summary>
        protected virtual void OnSceneSlowChanged()
        {
            SceneRoundPerSec = 1000F / IntervalOfRound * SceneSlow;
            SceneIntervalOfRound = (int)(IntervalOfRound / SceneSlow);
            if (SceneSlowChanged != null)
            {
                SceneSlowChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 回合開始前執行,此方法應永遠在回合的開端
        /// </summary>
        protected virtual void OnBeforeRound()
        {
            if (BeforeRound != null)
            {
                BeforeRound(this, new EventArgs());
            }
        }

        /// <summary>
        /// 回合結束執行,此方法應永遠在回合的結尾
        /// </summary>
        protected virtual void OnAfterRound()
        {
            if (BeforeRound != null)
            {
                AfterRound(this, new EventArgs());
            }
        }

        /// <summary>
        /// 繪製畫面前執行,此方法應永遠在繪製流程的開端,在此設定繪製參數
        /// </summary>
        protected virtual void OnBeforeDraw(Graphics g)
        {
            if (BeforeDraw != null)
            {
                BeforeDraw(this, new PaintEventArgs(g, this.ClientRectangle));
            }
        }

        /// <summary>
        /// 繪製地板
        /// </summary>
        protected virtual void OnDrawFloor(Graphics g)
        {
            if (DrawFloor != null)
            {
                DrawFloor(this, new PaintEventArgs(g, this.ClientRectangle));
            }
        }

        /// <summary>
        /// 繪製UI前執行
        /// </summary>
        protected virtual void OnBeforeDrawUI(Graphics g)
        {
            if (BeforeDrawUI != null)
            {
                BeforeDrawUI(this, new PaintEventArgs(g, this.ClientRectangle));
            }
        }

        /// <summary>
        /// 繪製UI後執行
        /// </summary>
        protected virtual void OnAfterDrawUI(Graphics g)
        {
            if (AfterDrawUI != null)
            {
                AfterDrawUI(this, new PaintEventArgs(g, this.ClientRectangle));
            }
        }

        /// <summary>
        /// 繪製結束前執行,在OnAfterDraw前發生,在此重置畫布設定
        /// </summary>
        protected virtual void OnAfterDrawReset(Graphics g)
        {
            g.ResetTransform();
            if (AfterDrawReset != null)
            {
                AfterDrawReset(this, new PaintEventArgs(g, this.ClientRectangle));
            }
        }

        /// <summary>
        /// 繪製畫面後執行,此方法應永遠在繪製流程的結尾(畫布重置後)
        /// </summary>
        protected virtual void OnAfterDraw(Graphics g)
        {
            if (AfterDraw != null)
            {
                AfterDraw(this, new PaintEventArgs(g, this.ClientRectangle));
            }
        }

        /// <summary>
        /// 滑鼠移動時更新追蹤點
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            TrackPoint = e.Location;
            base.OnMouseMove(e);
        }

        /// <summary>
        /// 秒轉換為毫秒
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public int Sec(float sec)
        {
            return (int)(sec * 1000);
        }

        /// <summary>
        /// 回合數轉換為毫秒
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public int Round(float round)
        {
            return (int)(round * IntervalOfRound);
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

        private void SceneBase_SizeChanged(object sender, EventArgs e)
        {
            if (BufferImage != null) BufferImage.Dispose();
            if (BufferGraphics != null) BufferGraphics.Dispose();
            if (ThisGraphics != null) ThisGraphics.Dispose();

            MainRectangle = ClientRectangle;
            BufferImage = new Bitmap(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            BufferGraphics = Graphics.FromImage(BufferImage);
            BufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            BufferGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            ThisGraphics = CreateGraphics();
        }
    }
}
