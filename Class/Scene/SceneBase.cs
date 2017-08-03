
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
    public class SceneBase : UserControl
    {
        public float WorldSpeed { get; set; }
        public Point TrackPoint { get; set; }

        public ObjectPlayer PlayerObject { get; set; }
        public ObservableCollection<ObjectBase> GameObjects { get; private set; }
        public List<IEffect> EffectObjects { get; private set; }
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

        private Timer _TimerOfAction = new Timer() { Enabled = true, Interval = 25 };
        public SceneBase()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            WorldSpeed = 1;

            GameObjects = new ObservableCollection<ObjectBase>();
            GameObjects.CollectionChanged += (x, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    (e.NewItems[0] as ObjectBase).Scene = this;
                    (e.NewItems[0] as ObjectBase).Killed += OnObjectDead;
                }
            };
            EffectObjects = new List<IEffect>();

            _TimerOfAction.Tick += TimerOfAction_Tick;
        }

        private void TimerOfAction_Tick(object sender, EventArgs e)
        {
            if (!IsStart) return;

            foreach (IEffect effect in EffectObjects)
            {
                effect.DoBeforeAction();
            }

            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Action();
            }

            foreach (IEffect effect in EffectObjects)
            {
                effect.DoAfterAction();
            }

            //Clear Dead Object
            List<ObjectBase> deadObjects = new List<ObjectBase>();
            for (int i = 0; i < GameObjects.Count; i++)
            {
                if (GameObjects[i].Status == ObjectStatus.Dead)
                {
                    deadObjects.Add(GameObjects[i]);
                }
            }

            foreach (ObjectBase deadObject in deadObjects)
            {
                GameObjects.Remove(deadObject);
            }

            //Clear Dead Effect
            List<IEffect> deadEffects = new List<IEffect>();
            for (int i = 0; i < EffectObjects.Count; i++)
            {
                if (EffectObjects[i].Status == EffectStatus.Dead)
                {
                    deadEffects.Add(EffectObjects[i]);
                }
            }

            foreach (IEffect deadEffect in deadEffects)
            {
                EffectObjects.Remove(deadEffect);
            }

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
            _watchFPS.Restart();
            _BackGraphics.Clear(Color.White);

            foreach (IEffect effect in EffectObjects)
            {
                effect.DoBeforeDraw(_BackGraphics);
            }

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

            foreach (IEffect effect in EffectObjects)
            {
                effect.DoBeforeDrawObject(_BackGraphics);
            }

            foreach (ObjectBase activeObj in GameObjects)
            {
                activeObj.DrawSelf(_BackGraphics);
            }

            foreach (IEffect effect in EffectObjects)
            {
                effect.DoAfterDraw(_BackGraphics);
            }

            _BackGraphics.ResetTransform();
            _BackGraphics.DrawString(_txtFPS, _fontFPS, Brushes.Red, Width - 50, 5);
            _ThisGraphics.DrawImageUnscaled(_BackImage, 0, 0);

            _watchFPS.Stop();
            _tickFPS--;
            if (_tickFPS <= 0)
            {
                _tickFPS = 10;
                _txtFPS = ((int)(1 / (_watchFPS.ElapsedTicks / (float)TimeSpan.TicksPerSecond))).ToString();
            }
        }


        public void SetStart(int potX, int potY)
        {
            Level = 0;
            Score = 0;

            GameObjects.Clear();
            PlayerObject = new ObjectPlayer(potX, potY, 8, 3, 0, Color.Black);
            GameObjects.Add(PlayerObject);
            IsStart = true;
            GameRectangle = new Rectangle(50, 50, Width - 100, Height - 100);

            if (_BackImage != null) _BackImage.Dispose();
            if (_BackGraphics != null) _BackGraphics.Dispose();
            if (_ThisGraphics != null) _ThisGraphics.Dispose();

            _BackImage = new Bitmap(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            _BackGraphics = Graphics.FromImage(_BackImage);
            _ThisGraphics = CreateGraphics();

            _BackGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Cursor.Hide();
        }

        public virtual void OnObjectDead(object sender, EventArgs e)
        {
            if (sender is ObjectPlayer)
            {
                SetEnd();
            }
        }

        /// <summary>
        /// 結束遊戲
        /// </summary>
        private void SetEnd()
        {
            PlayerObject = null;
            IsEnding = true;
            EndDelay = 30;
            Cursor.Show();
        }

        /// <summary>
        /// 秒轉換為Action
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public int SecToAction(float sec)
        {
            return (int)(sec * 1000 / _TimerOfAction.Interval);
        }
    }
}
