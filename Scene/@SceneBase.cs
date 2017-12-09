
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
    public class SceneBase : Control, ITargetability, IDisposable
    {
        private static Font _FPSFont = new Font("Arial", 12);
        private Stopwatch _FPSWatch = new Stopwatch();
        private int _FPSTick = 0;
        private string _FPSText = "";
        private Stack<ObjectUI> _LockCover = new Stack<ObjectUI>();

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

        /// <summary>
        /// 繪圖物件清除計次器
        /// </summary>
        protected CounterObject DrawClearCount { get; set; }

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
        public event ValueChangedEnentHandle<Point> TrackPointChanged;

        /// <summary>
        /// 發生於目前獲得焦點的UI變更
        /// </summary>
        public event ValueChangedEnentHandle<ObjectUI> FocusObjectUIChanged;

        /// <summary>
        /// 發生於場景計時器速度修正變更
        /// </summary>
        public event ValueChangedEnentHandle<float> SceneTimeFixChanged;

        /// <summary>
        /// 發生於場景速度減慢值變更
        /// </summary>
        public event ValueChangedEnentHandle<float> SceneSlowChanged;

        /// <summary>
        /// 發生於回合時間變更
        /// </summary>
        public event ValueChangedEnentHandle<int> IntervalOfRoundChanged;

        /// <summary>
        /// 發生於UI物件集合變更
        /// </summary>
        public event ValueChangedEnentHandle<ObjectCollection> UIObjectsChanged;

        /// <summary>
        /// 發生於活動物件集合變更
        /// </summary>
        public event ValueChangedEnentHandle<ObjectCollection> GameObjectsChanged;

        /// <summary>
        /// 發生於特效物件集合變更
        /// </summary>
        public event ValueChangedEnentHandle<EffectCollection> EffectObjectsChanged;

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
            //BufferGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            //BufferGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
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
            this.Dispose();
        }

        /// <summary>
        /// 發生於場景追蹤點變更
        /// </summary>
        protected virtual void OnTrackPointChanged(Point oldValue, Point newValue)
        {
            SearchFocusObjectUI();

            if (TrackPointChanged != null)
            {
                TrackPointChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於目前獲得焦點的UI變更
        /// </summary>
        protected virtual void OnFocusObjectUIChanged(ObjectUI oldValue, ObjectUI newValue)
        {
            if (oldValue != null)
            {
                oldValue.OnLostFocus();
            }

            if (newValue != null)
            {
                newValue.OnGetFocus();
                if (newValue.HasClickEnevt)
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

            if (FocusObjectUIChanged != null)
            {
                FocusObjectUIChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於場景計時器速度修正變更
        /// </summary>
        protected virtual void OnSceneTimeFixChanged(float oldValue, float newValue)
        {
            SceneIntervalOfRound = (int)(IntervalOfRound / SceneSlow * SceneTimeFix);

            if (SceneTimeFixChanged != null)
            {
                SceneTimeFixChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於回合時間變更
        /// </summary>
        protected virtual void OnIntervalOfRoundChanged(int oldValue, int newValue)
        {
            RoundPerSec = 1000F / IntervalOfRound;
            SceneRoundPerSec = 1000F / IntervalOfRound * SceneSlow;
            SceneIntervalOfRound = (int)(IntervalOfRound / SceneSlow * SceneTimeFix);

            if (IntervalOfRoundChanged != null)
            {
                IntervalOfRoundChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於場景速度減慢值變更
        /// </summary>
        protected virtual void OnSceneSlowChanged(float oldValue, float newValue)
        {
            SceneRoundPerSec = (float)1000 / IntervalOfRound * SceneSlow;
            SceneIntervalOfRound = (int)(IntervalOfRound / SceneSlow * SceneTimeFix);
            if (SceneSlowChanged != null)
            {
                SceneSlowChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於UI物件集合變更
        /// </summary>
        protected virtual void OnUIObjectsChanged(ObjectCollection oldValue, ObjectCollection newValue)
        {
            if (oldValue != null)
            {
                oldValue.BindingUnlock();
            }

            if (newValue != null)
            {
                newValue.Binding(this, true);
            }

            if (UIObjectsChanged != null)
            {
                UIObjectsChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於活動物件集合變更
        /// </summary>
        protected virtual void OnGameObjectsChanged(ObjectCollection oldValue, ObjectCollection newValue)
        {
            if (oldValue != null)
            {
                oldValue.BindingUnlock();
            }

            if (newValue != null)
            {
                newValue.Binding(this, true);
            }

            if (GameObjectsChanged != null)
            {
                GameObjectsChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於活動物件集合變更
        /// </summary>
        protected virtual void OnEffectObjectsChanged(EffectCollection oldValue, EffectCollection newValue)
        {
            if (oldValue != null)
            {
                oldValue.BindingUnlock();
            }

            if (newValue != null)
            {
                newValue.Binding(this, true);
            }

            if (EffectObjectsChanged != null)
            {
                EffectObjectsChanged(this, oldValue, newValue);
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
            // 顯示FPS
            if (Global.DebugMode)
            {
                g.DrawString(string.Format("Object:{0}\nDraw:{1}\nFPS:{2}", GameObjects.Count, DrawPool.BrushCount + DrawPool.PenCount, _FPSText), _FPSFont, Brushes.Red, Width - 80, 5);
            }

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
        private float _SceneTimeFix = 1;
        /// <summary>
        /// 場景計時器速度修正
        /// </summary>
        public float SceneTimeFix
        {
            get { return _SceneTimeFix; }
            set
            {
                if (_SceneTimeFix == value) return;
                float oldValue = _SceneTimeFix;
                _SceneTimeFix = value;
                OnSceneTimeFixChanged(oldValue, value);
            }
        }

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
                ObjectUI oldValue = _FocusObjectUI;
                _FocusObjectUI = value;
                OnFocusObjectUIChanged(oldValue, value);
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
        /// 場景每回合時間(以毫秒為單位,計入場景速度)
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
                if (RoundTimer.Interval == value) return;
                int oldValue = RoundTimer.Interval;
                RoundTimer.Interval = value;
                OnIntervalOfRoundChanged(oldValue, value);
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
                if (_SceneSlow == value) return;
                float oldValue = _SceneSlow;
                _SceneSlow = value;
                OnSceneSlowChanged(oldValue, value);
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
                Point oldValue = _TrackPoint;
                _TrackPoint = value;
                OnTrackPointChanged(oldValue, value);
            }
        }

        /// <summary>
        /// 主要區域
        /// </summary>
        public Rectangle MainRectangle { get; set; }

        private ObjectCollection _UIObjects;
        /// <summary>
        /// UI物件集合
        /// </summary>
        public ObjectCollection UIObjects
        {
            get { return _UIObjects; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_UIObjects == value) return;

                ObjectCollection oldValue = _UIObjects;
                _UIObjects = value;
                OnUIObjectsChanged(oldValue, value);
            }
        }

        private ObjectCollection _GameObjects;
        /// <summary>
        /// 場景物件集合
        /// </summary>
        public ObjectCollection GameObjects
        {
            get { return _GameObjects; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_GameObjects == value) return;

                ObjectCollection oldValue = _GameObjects;
                _GameObjects = value;
                OnGameObjectsChanged(oldValue, value);
            }
        }

        private EffectCollection _EffectObjects;
        /// <summary>
        /// 場景特效集合
        /// </summary>
        public EffectCollection EffectObjects
        {
            get { return _EffectObjects; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_EffectObjects == value) return;

                EffectCollection oldValue = _EffectObjects;
                _EffectObjects = value;
                OnEffectObjectsChanged(oldValue, value);
            }
        }
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
            DrawClearCount = new CounterObject(20); // 計次
            RoundTimer.Tick += RoundTimer_Tick;
        }

        private Timer _LoadTimer;
        protected override void OnParentChanged(EventArgs e)
        {
            if (Parent != null && !IsLoadComplete && _LoadTimer == null)
            {
                _LoadTimer = new Timer() { Interval = 1 };
                _LoadTimer.Tick += (x, te) =>
                {
                    Timer s = x as Timer;
                    s.Enabled = false;
                    OnLoadComplete();
                    s.Dispose();
                };
                _LoadTimer.Enabled = true;
            }
            base.OnParentChanged(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SceneBase
            // 
            this.SizeChanged += new System.EventHandler(this.SceneBase_SizeChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SceneBase_MouseDown);
            this.ResumeLayout(false);

        }

        private void SceneBase_SizeChanged(object sender, EventArgs e)
        {
            if (!Visible) return;
            Form form = FindForm();
            if (IsDisposed || form == null || form.WindowState == FormWindowState.Minimized || !IsLoadComplete) return;
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
            // 計算FPS
            _FPSTick--;
            bool refreshFPS = false;
            if (_FPSTick <= 0)
            {
                _FPSTick = 10;
                _FPSWatch.Restart();
                refreshFPS = true;
            }

            Round();

            // 計算FPS
            if (refreshFPS)
            {
                _FPSWatch.Stop();
                _FPSText = _FPSWatch.Elapsed.Ticks > 0 ? (TimeSpan.TicksPerSecond / _FPSWatch.Elapsed.Ticks).ToString() : "--";
            }

            // 清除繪圖物件
            if (DrawClearCount.IsFull)
            {
                DrawPool.ReleaseUseless();
                DrawClearCount.Value = 0;
            }
            DrawClearCount.Value++;
        }

        /// <summary>
        /// 回合動作
        /// </summary>
        protected virtual void Round()
        {
            if (Global.Freeze) return;

            OnBeforeRound();
            EffectObjects.AllDoBeforeRound();
            GameObjects.AllAction();
            EffectObjects.AllDoAfterRound();
            OnAfterRound();

            UIObjects.AllAction();
            EffectObjects.AllSettlement();

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
        /// 場景定義的秒數轉換為毫秒數
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
            return GetEnterPoint(Global.Rand.Next(4));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directIdx">物件進入點(1:左 2:上 3:右 4:下 5:左...)</param>
        /// <returns></returns>
        public Point GetEnterPoint(int directIdx)
        {
            switch (directIdx)
            {
                case 0:
                    return GetEnterPoint(DirectionType.Left);
                case 1:
                    return GetEnterPoint(DirectionType.Top);
                case 2:
                    return GetEnterPoint(DirectionType.Right);
                case 3:
                    return GetEnterPoint(DirectionType.Bottom);
                default:
                    return GetEnterPoint(directIdx % 4);
            }
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
        /// 鎖定場景
        /// </summary>
        /// <param name="coverColor">遮罩顏色</param>
        /// <param name="fadeSpeed">遮罩出現速度</param>
        public void LockScene(Color coverColor, float fadeSpeed)
        {
            DrawBrush drawCover = new DrawBrush(coverColor, ShapeType.Rectangle);
            ObjectUI newCover = new ObjectUI(0, 0, Width, Height, drawCover);

            if (fadeSpeed > 0)
            {
                drawCover.Colors.Opacity = 0;
                newCover.Propertys.Add(new PropertyOpacityFix(1, fadeSpeed, true));
            }
            _UIObjects.Add(newCover);
            _LockCover.Push(newCover);
            SearchFocusObjectUI();
        }

        /// <summary>
        /// 取消鎖定場景
        /// </summary>
        /// <param name="fadeSpeed">遮罩消失速度</param>
        public void UnlockScene(float fadeSpeed)
        {
            if (_LockCover.Count == 0) return;

            ObjectUI cover = _LockCover.Pop();
            if (fadeSpeed > 0)
            {
                PropertyOpacityFix fadeout = new PropertyOpacityFix(0, fadeSpeed, true);
                fadeout.End += (x, e) =>
                {
                    cover.Kill(null, ObjectDeadType.Clear);
                };
                cover.Propertys.Add(fadeout);
            }
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
                if (item != null && item.Visible)
                {
                    ObjectUI hoverUI = item.InRectangle(TrackPoint);
                    if (hoverUI != null)
                    {
                        fidUI = hoverUI.Enabled ? hoverUI : null;
                        break;
                    }
                }
            }
            FocusObjectUI = fidUI;
        }

        #region ===== 實作ITargetability =====
        /// <summary>
        /// 取得場景追蹤點X座標
        /// </summary>
        /// <param name="anchor">定位位置(此處無用)</param>
        /// <returns>場景追蹤點X座標</returns>
        public float GetTargetX(DirectionType anchor)
        {
            return TrackPoint.X;
        }

        /// <summary>
        /// 取得場景追蹤點Y座標
        /// </summary>
        /// <param name="anchor">定位位置(此處無用)</param>
        /// <returns>場景追蹤點Y座標</returns>
        public float GetTargetY(DirectionType anchor)
        {
            return TrackPoint.Y;
        }

        /// <summary>
        /// 取得場景追蹤點
        /// </summary>
        /// <param name="anchor">定位位置(此處無用)</param>
        /// <returns>場景追蹤點</returns>
        public PointF GetTargetPoint(DirectionType anchor)
        {
            return TrackPoint;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EffectObjects.Clear();
                    GameObjects.Clear();
                    UIObjects.Clear();
                    RoundTimer.Enabled = false;
                    RoundTimer.Dispose();
                }
                disposedValue = true;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
