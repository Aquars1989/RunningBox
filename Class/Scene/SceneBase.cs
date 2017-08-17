
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
        private static Font _FPSFont = new Font("Arial", 12);
        private Stopwatch _FPSWatch = new Stopwatch();
        private int _FPSTick = 0;
        private string _FPSText = "";

        /// <summary>
        /// 回合計時器
        /// </summary>
        private Timer _RoundTimer = new Timer();

        /// <summary>
        /// 場景物件本身的Graphics物件
        /// </summary>
        private Graphics _ThisGraphics;

        /// <summary>
        /// 緩衝畫布的Graphics物件
        /// </summary>
        private Graphics _BufferGraphics;

        /// <summary>
        /// 緩衝畫布
        /// </summary>
        private Bitmap _BufferImage;

        #region ===== 事件 =====
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
        /// 每秒回合數
        /// </summary>
        public float _RoundPerSec { get; private set; }

        /// <summary>
        /// 每回合時間(以毫秒為單位)
        /// </summary>
        public virtual int IntervalOfRound
        {
            get { return _RoundTimer.Interval; }
            set
            {
                _RoundTimer.Interval = value;
                OnIntervalOfRoundChanged();
            }
        }

        /// <summary>
        /// 場景追蹤點
        /// </summary>
        public Point TrackPoint { get; set; }

        /// <summary>
        /// UI物件集合
        /// </summary>
        public ObjectCollection UIObjects { get; private set; }
        #endregion

        /// <summary>
        /// 新增基本場景物件
        /// </summary>
        public SceneBase()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();

            IntervalOfRound = Global.DefaultIntervalOfRound;
            UIObjects = new ObjectCollection(this);
            _RoundTimer.Tick += RoundTimer_Tick;
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
            this.ResumeLayout(false);

        }

        private void SceneBase_Load(object sender, EventArgs e)
        {
            _BufferImage = new Bitmap(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            _BufferGraphics = Graphics.FromImage(_BufferImage);
            _BufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _BufferGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            _ThisGraphics = CreateGraphics();
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
            UIObjects.ClearAllDead();
            OnBeforeRound();
            OnAfterRound();
            Drawing();
        }

        /// <summary>
        /// 繪製畫面
        /// </summary>
        protected virtual void Drawing()
        {
            _FPSTick--;
            bool refreshFPS = false;
            if (_FPSTick <= 0)
            {
                _FPSTick = 10;
                _FPSWatch.Restart();
                refreshFPS = true;
            }

            _BufferGraphics.Clear(Color.White);
            OnBeforeDraw(_BufferGraphics);
            OnBeforeDrawUI(_BufferGraphics);
            UIObjects.AllDrawSelf(_BufferGraphics);
            OnAfterDrawUI(_BufferGraphics);
            OnAfterDrawReset(_BufferGraphics);
            OnAfterDraw(_BufferGraphics);

            if (Global.DebugMode)
            {
                _BufferGraphics.DrawString(_FPSText, _FPSFont, Brushes.Red, Width - 50, 5);
            }
            _ThisGraphics.DrawImageUnscaled(_BufferImage, 0, 0);

            if (refreshFPS)
            {
                _FPSWatch.Stop();
                _FPSText = (TimeSpan.TicksPerSecond / _FPSWatch.Elapsed.Ticks).ToString();
            }
        }

        /// <summary>
        /// 回合時間變更
        /// </summary>
        protected virtual void OnIntervalOfRoundChanged()
        {
            _RoundPerSec = 1000F / IntervalOfRound;
            if (IntervalOfRoundChanged != null)
            {
                IntervalOfRoundChanged(this, new EventArgs());
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
        /// 秒轉換為Round數量
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public int SecToRounds(float sec)
        {
            return (int)(sec * _RoundPerSec);
        }
    }
}
