
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

        #region===== 屬性 =====
        /// <summary>
        /// 每秒回合數
        /// </summary>
        public float _RoundPerSec { get; private set; }

        /// <summary>
        /// 每回合時間(以毫秒為單位)
        /// </summary>
        public int IntervalOfRound
        {
            get { return _RoundTimer.Interval; }
            set
            {
                _RoundTimer.Interval = value;
                _RoundPerSec = 1000F / value;
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

        public SceneBase()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();

            IntervalOfRound = Global.DefaultIntervalOfRound;
            UIObjects = new ObjectCollection(this);
            _RoundTimer.Tick += RoundTimer_Tick;
        }

        private void RoundTimer_Tick(object sender, EventArgs e)
        {
            UIObjects.ClearAllDead();
            Drawing();
        }

        private void Drawing()
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
            UIObjects.AllDrawSelf(_BufferGraphics);
            _BufferGraphics.ResetTransform();

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

        protected virtual void OnBeforeDraw()
        {

        }

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
}
