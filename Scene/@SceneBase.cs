
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
    public class SceneBase : Control
    {
        public delegate void GoSceneEventHandle(object sender, SceneBase scene);

        protected new Cursor DefaultCursor { get; set; }

        /// <summary>
        /// 是否載入完成
        /// </summary>
        public bool IsLoadComplete { get; private set; }

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
        /// 載入完成
        /// </summary>
        public event EventHandler LoadComplete;

        /// <summary>
        /// 前往其他場景
        /// </summary>
        public event GoSceneEventHandle GoScene;

        /// <summary>
        /// 發生於場景追蹤點變更
        /// </summary>
        public event EventHandler TrackPointChanged;

        /// <summary>
        /// 發生於目前獲得焦點的UI變更
        /// </summary>
        public event EventHandler FocusObjectUIChanged;

        /// <summary>
        /// 發生於場景速度減慢值變更
        /// </summary>
        public event EventHandler SceneSlowChanged;

        /// <summary>
        /// 發生於回合時間變更
        /// </summary>
        public event EventHandler IntervalOfRoundChanged;

        /// <summary>
        /// 發生於回合開始前
        /// </summary>
        public event EventHandler BeforeRound;

        /// <summary>
        /// 發生於回合結束後
        /// </summary>
        public event EventHandler AfterRound;

        /// <summary>
        /// 發生於繪製畫面前,在此設定繪製參數
        /// </summary>
        public event PaintEventHandler BeforeDraw;

        /// <summary>
        /// 發生於繪製地板時
        /// </summary>
        public event PaintEventHandler DrawFloor;

        /// <summary>
        /// 發生於繪製UI前
        /// </summary>
        public event PaintEventHandler BeforeDrawUI;

        /// <summary>
        /// 發生於繪製UI後
        /// </summary>
        public event PaintEventHandler AfterDrawUI;

        /// <summary>
        /// 發生於繪製結束前,在此重置畫布設定
        /// </summary>
        public event PaintEventHandler AfterDrawReset;

        /// <summary>
        /// 發生於繪製畫面後
        /// </summary>
        public event PaintEventHandler AfterDraw;
        #endregion

        #region ===== 引發事件 =====
        protected virtual void OnLoadComplete()
        {
            if (IsDisposed || IsLoadComplete || DesignMode) return;

            IsLoadComplete = true;
            OnReLayout();
            RoundTimer.Enabled = true;

            if (LoadComplete != null)
            {
                LoadComplete(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於版面重新配置
        /// </summary>
        protected virtual void OnReLayout()
        {
            if (!IsLoadComplete) return;
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

        /// <summary>
        /// 發生於目前獲得焦點的UI變更
        /// </summary>
        protected virtual void OnGoScene(SceneBase scene)
        {
            scene.Dock = DockStyle.Fill;
            Parent.Controls.Add(scene);
            Parent.Controls.Remove(this);

            if (GoScene != null)
            {
                GoScene(this, scene);
            }
        }

        /// <summary>
        /// 發生於場景追蹤點變更
        /// </summary>
        protected virtual void OnTrackPointChanged()
        {
            SearchFocusObjectUI();

            if (TrackPointChanged != null)
            {
                TrackPointChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於目前獲得焦點的UI變更
        /// </summary>
        protected virtual void OnFocusObjectUIChanged()
        {
            if (FocusObjectUIChanged != null)
            {
                FocusObjectUIChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於回合時間變更
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
        /// 發生於場景速度減慢值變更
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
        /// 發生於回合開始前,此事件應永遠在回合的開端
        /// </summary>
        protected virtual void OnBeforeRound()
        {
            if (BeforeRound != null)
            {
                BeforeRound(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於回合結束,此事件應永遠在回合的結尾
        /// </summary>
        protected virtual void OnAfterRound()
        {
            if (BeforeRound != null)
            {
                AfterRound(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於繪製畫面前,此事件應永遠在繪製流程的開端,在此設定繪製參數
        /// </summary>
        protected virtual void OnBeforeDraw(Graphics g)
        {
            if (BeforeDraw != null)
            {
                BeforeDraw(this, new PaintEventArgs(g, this.ClientRectangle));
            }
        }

        /// <summary>
        /// 發生於繪製地板時
        /// </summary>
        protected virtual void OnDrawFloor(Graphics g)
        {
            if (DrawFloor != null)
            {
                DrawFloor(this, new PaintEventArgs(g, this.ClientRectangle));
            }
        }

        /// <summary>
        /// 發生於繪製UI前
        /// </summary>
        protected virtual void OnBeforeDrawUI(Graphics g)
        {
            if (BeforeDrawUI != null)
            {
                BeforeDrawUI(this, new PaintEventArgs(g, this.ClientRectangle));
            }
        }

        /// <summary>
        /// 發生於繪製UI後
        /// </summary>
        protected virtual void OnAfterDrawUI(Graphics g)
        {
            if (AfterDrawUI != null)
            {
                AfterDrawUI(this, new PaintEventArgs(g, this.ClientRectangle));
            }
        }

        /// <summary>
        /// 發生於繪製結束前,在OnAfterDraw前發生,在此重置畫布設定
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
        /// 發生於繪製畫面後,此事件應永遠在繪製流程的結尾(畫布重置後)
        /// </summary>
        protected virtual void OnAfterDraw(Graphics g)
        {
            if (AfterDraw != null)
            {
                AfterDraw(this, new PaintEventArgs(g, this.ClientRectangle));
            }
        }
        #endregion

        #region===== 屬性 =====
        private ObjectUI _FocusObjectUI;
        /// <summary>
        /// 目前獲得焦點的UI
        /// </summary>
        public ObjectUI FocusObjectUI
        {
            get { return _FocusObjectUI; }
            private set
            {
                if (_FocusObjectUI == value) return;
                if (_FocusObjectUI != null)
                {
                    _FocusObjectUI.OnLostFocus();
                }

                _FocusObjectUI = value;

                if (_FocusObjectUI != null)
                {
                    _FocusObjectUI.OnGetFocus();
                    if (_FocusObjectUI.HasClickEnevt)
                    {
                        Cursor = Cursors.Hand;
                    }
                    else
                    {
                        Cursor = DefaultCursor;
                    }
                }
                else
                {
                    Cursor = DefaultCursor;
                }
                OnFocusObjectUIChanged();
            }
        }

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

        private Point _TrackPoint;
        /// <summary>
        /// 場景追蹤點
        /// </summary>
        public Point TrackPoint
        {
            get { return _TrackPoint; }
            set
            {
                if (_TrackPoint == value) return;
                _TrackPoint = value;
                OnTrackPointChanged();
            }
        }

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
            DefaultCursor = Cursors.Default;
            IntervalOfRound = Global.DefaultIntervalOfRound;
            UIObjects = new ObjectCollection(this);
            EffectObjects = new EffectCollection(this);
            GameObjects = new ObjectCollection(this);
            RoundTimer.Tick += RoundTimer_Tick;

            Timer loadTimer = new Timer() { Interval = 1 };
            loadTimer.Tick += (x, te) =>
            {
                Timer s = x as Timer;
                s.Enabled = false;
                OnLoadComplete();
                s.Dispose();
            };
            loadTimer.Enabled = true;
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
            this.SizeChanged += new System.EventHandler(this.SceneBase_SizeChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SceneBase_MouseDown);
            this.ResumeLayout(false);

        }

        private void SceneBase_SizeChanged(object sender, EventArgs e)
        {
            if (!Visible) return;
            Form form = FindForm();
            if (IsDisposed || form == null || form.WindowState == FormWindowState.Minimized) return;
            OnReLayout();
        }

        private void SceneBase_MouseDown(object sender, MouseEventArgs e)
        {
            if (FocusObjectUI != null)
            {
                FocusObjectUI.OnClick(e);
            }
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

            UIObjects.AllAction();
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
            return (int)(sec * 1000 + 0.5F);
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
            return GetEnterPoint((DirectionType)Global.Rand.Next(4));
        }

        /// <summary>
        /// 取得特定方向的物件進入點
        /// </summary>
        /// <param name="enterSide">進入方向</param>
        /// <returns>物件進入點</returns>
        public Point GetEnterPoint(DirectionType enterSide)
        {
            int x = 0, y = 0;
            switch (enterSide)
            {
                case DirectionType.Left:
                    x = -Global.Rand.Next(20, 60);
                    y = Global.Rand.Next(0, Height);
                    break;
                case DirectionType.Right:
                    x = Width + Global.Rand.Next(20, 60);
                    y = Global.Rand.Next(0, Height);
                    break;
                case DirectionType.Top:
                    x = Global.Rand.Next(0, Width);
                    y = -Global.Rand.Next(20, 60);
                    break;
                case DirectionType.Bottom:
                    x = Global.Rand.Next(0, Width);
                    y = Height + Global.Rand.Next(20, 60);
                    break;
            }
            return new Point(x, y);
        }

        /// <summary>
        /// 重新搜尋目前獲得焦點的UI
        /// </summary>
        public void SearchFocusObjectUI()
        {
            ObjectUI fidUI = null;
            for (int i = UIObjects.Count - 1; i >= 0; i--) // 由後往前找
            {
                ObjectUI item = UIObjects[i] as ObjectUI;
                if (item != null && item.Visible && item.InRectangle(TrackPoint))
                {
                    fidUI = item.Enabled ? item : null;
                    break;
                }
            }
            FocusObjectUI = fidUI;
        }
    }
}
