
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
    public enum EnumDirection
    {
        Left = 0,
        Right = 1,
        Top = 2,
        Bottom = 3
    }

    public class SceneBase : UserControl
    {
        public float WorldSpeed { get; set; }
        public Point TrackPoint { get; set; }

        public ObjectPlayer PlayerObject { get; set; }
        public ObjectCollection GameObjects { get; private set; }
        public EffectCollection EffectObjects { get; private set; }
        public Rectangle GameRectangle { get; set; }

        public int Score { get; set; }
        public int Level { get; set; }

        public bool IsStart { get; set; }
        public bool IsEnding { get; set; }

        public int EndDelay { get; set; }

        private Graphics _ThisGraphics;
        private Graphics _BackGraphics;
        private Bitmap _BackImage;

        private Rectangle _RectOfEngery = new Rectangle(50, 10, 100, 10);

        private Pen _PenRectGaming = new Pen(Color.LightGreen, 2);

        private Timer _TimerOfRound = new Timer() { Enabled = true, Interval = 25 };
        public SceneBase()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            WorldSpeed = 1;

            GameObjects = new ObjectCollection(this);
            EffectObjects = new EffectCollection(this);

            GameObjects.ObjectDead += OnObjectDead;
            _TimerOfRound.Tick += TimerOfRound_Tick;
        }

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
                EndDelay--;
                if (EndDelay <= 0)
                {
                    IsEnding = false;
                    IsStart = false;
                }
            }
            Drawing();
        }

        Stopwatch _watchFPS = new Stopwatch();
        int _tickFPS = 0;
        string _txtFPS = "";
        Font _fontFPS = new Font("Arial", 12);
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


            _BackGraphics.Clear(Color.White);

            EffectObjects.AllDoBeforeDraw(_BackGraphics);
            EffectObjects.AllDoBeforeDrawUI(_BackGraphics);

            if (GameRectangle != null)
            {
                _BackGraphics.DrawRectangle(_PenRectGaming, GameRectangle);
            }

            if (PlayerObject != null)
            {
                float ratio = (float)PlayerObject.Energy / PlayerObject.EnergyMax;
                Brush brush = ratio < 0.3 ? Brushes.Red : Brushes.Black;
                _BackGraphics.FillRectangle(brush, _RectOfEngery.X + 2, _RectOfEngery.Y + 2, (_RectOfEngery.Width - 4) * ratio, _RectOfEngery.Height - 4);
            }
            _BackGraphics.DrawRectangle(Pens.Black, _RectOfEngery);
            _BackGraphics.DrawString(Score.ToString(), Font, Brushes.Black, _RectOfEngery.X + _RectOfEngery.Width + 10, _RectOfEngery.Y);

            EffectObjects.AllDoBeforeDrawObject(_BackGraphics);
            GameObjects.AllDrawSelf(_BackGraphics);
            EffectObjects.AllDoAfterDraw(_BackGraphics);

            _BackGraphics.ResetTransform();

            _BackGraphics.DrawString(_txtFPS, _fontFPS, Brushes.Red, Width - 50, 5);
            _ThisGraphics.DrawImageUnscaled(_BackImage, 0, 0);

            if (refreshFPS)
            {
                _watchFPS.Stop();
                _txtFPS = (TimeSpan.TicksPerSecond / _watchFPS.Elapsed.Ticks).ToString();
            }
        }

        public void SetStart(int potX, int potY)
        {
            Level = 0;
            Score = 0;

            GameObjects.Clear();
            EffectObjects.Clear();
            PlayerObject = new ObjectPlayer(potX, potY, 8, 3, 0, Color.Black)
            {
                DrawObject = new DrawPen(Color.Black, DrawShape.Ellipse, 2),
                Target = new TargetTrackPoint(this)
            };
            PlayerObject.Skills.Add(new SkillSprint(200, 100, 10, true));
            PlayerObject.Propertys.Add(new PropertyDeadBroken(15));
            PlayerObject.Propertys.Add(new PropertyCollision(1, null));
            GameObjects.Add(PlayerObject);
            GameRectangle = new Rectangle(50, 50, Width - 100, Height - 100);

            if (_BackImage != null) _BackImage.Dispose();
            if (_BackGraphics != null) _BackGraphics.Dispose();
            if (_ThisGraphics != null) _ThisGraphics.Dispose();

            _BackImage = new Bitmap(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            _BackGraphics = Graphics.FromImage(_BackImage);
            _ThisGraphics = CreateGraphics();
            _BackGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Cursor.Hide();
            IsStart = true;
        }

        /// <summary>
        /// 物件死亡時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnObjectDead(ObjectBase sender, ObjectBase killer)
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
            EndDelay = 30;
            Cursor.Show();
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

        public Point GetEnterPoint()
        {
            return GetEnterPoint((EnumDirection)Global.Rand.Next(4));
        }

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

        public virtual void UsePlayerSkill(int index)
        {
            if (PlayerObject == null) return;

            PlayerObject.Skills[index].Use(new TargetTrackPoint(this));
        }
    }
}
