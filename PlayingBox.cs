using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace RunningBox
{
    public partial class PlayingBox2 : SceneBase
    {
        private const int _DefaultGroupCount = 6;
        private const int _DefaultMineCount = 4;

        private const int _EnergyMax = 1000;
        private const int _EnergyGetPerAction = 5;
        private const int _EnergyForSprint = 350;
        private const int _EnergyForSlow = 350;
        private const int _EnergyForSlowPerAction = 15;

        private const int _EndMaxActions = 30;

        private const int _DarkMaxActions = 100;
        private const int _DarkFadeInActions = 20;
        private const int _DarkFadeOutActions = 20;

        private const int _LimitedMaxActions = 100;
        private const int _LimitedFadeActions = 20;


        private int _Energy = 0;
        private bool _SlowON = false;

        private Timer _TimerOfAction = new Timer() { Enabled = true, Interval = 25 };
        private Timer _TimerOfBuild = new Timer() { Enabled = true, Interval = 1000 };
        private Random _Rand = new Random();

        private Pen _PenRectGaming = new Pen(Color.LightGreen, 2);

        private bool _IsStart = false;
        private bool _IsEnd = false;
        private int _EndTick = 0;
        private int _DarkAni = 0;

        private int _LevelOfDark = 22;
        private int _LevelOfGroup = 15;
        private int _LevelOfInterceptor = 12;
        private int _LeveLOfMine = 8;
        private int _LevelOfCatcherFaster = 3;
        private int _LevelOfLimited = 28;


        private int _GroupCount = 0;
        private int _MineCount = 0;
        private int _MineExpand = 8;

        private Rectangle _RectOfEngery = new Rectangle(50, 10, 100, 10);

        private Graphics _ControlGraphics;
        private Graphics _BackGraphics;
        private Bitmap _BackImage;

        private int _TickOfLimited = 0;
        private Padding _LimitedPerAction = new Padding();

        private int _FPSTic = 10;
        public PlayingBox2()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            _TimerOfAction.Tick += TimerOfAction_Tick;
            _TimerOfBuild.Tick += TimerOfBuild_Tick;
        }

        private void TimerOfBuild_Tick(object sender, EventArgs e)
        {
            if (!_IsStart) return;

            Level++;

            float lifeLevel = (100F + Level) / 100F;
            float speedLevel = (70F + Level) / 100F;

            //if (_LeveL % _LevelOfDark == 0 && _DarkAni == 0)
            //{
            //    _DarkAni = 1;
            //}

            //if (_LeveL % _LevelOfLimited == 0 && _TickOfLimited == 0)
            //{
            //    _TickOfLimited = 1;
            //    double scaleX = _Rand.NextDouble();
            //    double scaleY = 1 - scaleX;
            //    int limitX = (int)(scaleX * _RectOfGaming.Width / 100);
            //    int limitY = (int)(scaleY * _RectOfGaming.Height / 100);

            //    int limitLeft = _Rand.Next(0, limitX);
            //    int limitTop = _Rand.Next(0, limitY);
            //    int limitRight = limitX - limitLeft;
            //    int limitDown = limitY - limitTop;

            //    _LimitedPerAction = new Padding(limitLeft, limitTop, limitRight, limitDown);
            //}

            //if (_LeveL % _LeveLOfMine == 0)
            //{
            //    for (int i = 0; i < _MineCount; i++)
            //    {
            //        int size = _Rand.Next(8, 10);
            //        int movesCount = _Rand.Next(10, 15);
            //        float speed = _Rand.Next(850, 1000) - (size * 50) * speedLevel;
            //        float speedPerMove = speed / movesCount;
            //        int life = SecToAction(_LeveLOfMine * 0.5F);// +_Rand.Next(0, 5);
            //        int X = 0, Y = 0, lockPotX = 0, lockPotY = 0;
            //        switch (_Rand.Next(1, 4))
            //        {
            //            case 1:
            //                X = -_Rand.Next(20, 60);
            //                Y = _Rand.Next(0, Height);
            //                lockPotX = _Rand.Next(_RectOfGaming.Left + 20, _RectOfGaming.Left + _RectOfGaming.Width / 2);
            //                lockPotY = _Rand.Next(_RectOfGaming.Top + 20, _RectOfGaming.Top + _RectOfGaming.Height - 20);
            //                break;
            //            case 2:
            //                X = Width + _Rand.Next(20, 60);
            //                Y = _Rand.Next(0, Height);
            //                lockPotX = _Rand.Next(_RectOfGaming.Left + _RectOfGaming.Width / 2, _RectOfGaming.Left + _RectOfGaming.Width - 20);
            //                lockPotY = _Rand.Next(_RectOfGaming.Top + 20, _RectOfGaming.Top + _RectOfGaming.Height - 20);
            //                break;
            //            case 3:
            //                X = _Rand.Next(0, Width);
            //                Y = -_Rand.Next(20, 60);
            //                lockPotX = _Rand.Next(_RectOfGaming.Left + 20, _RectOfGaming.Left + _RectOfGaming.Width - 20);
            //                lockPotY = _Rand.Next(_RectOfGaming.Top + 20, _RectOfGaming.Top + _RectOfGaming.Height / 2);
            //                break;
            //            case 4:
            //                X = _Rand.Next(0, Width);
            //                Y = Height + _Rand.Next(20, 60);
            //                lockPotX = _Rand.Next(_RectOfGaming.Left + 20, _RectOfGaming.Left + _RectOfGaming.Width - 20);
            //                lockPotY = _Rand.Next(_RectOfGaming.Top + _RectOfGaming.Height / 2, _RectOfGaming.Height - 20);
            //                break;
            //        }
            //        _GameObjects.Add(new GameObject(ActiveType.Mine, ShapeType.Image, X, Y, movesCount, size, speedPerMove, Color.Firebrick, life)
            //        {
            //            Image = RunningBox.Properties.Resources.Mine,
            //            LockPoint = new PointF(lockPotX, lockPotY)
            //        });
            //    }
            //    _MineCount++;
            //}

            if (Level % _LevelOfGroup == 0)
            {
                for (int i = 0; i < _GroupCount; i++)
                {
                    int size = _Rand.Next(3, 6);
                    int movesCount = _Rand.Next(15, 30);
                    float speed = _Rand.Next(700, 850) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToAction(6F * lifeLevel) + _Rand.Next(0, 5);
                    int X = 0, Y = 0;
                    switch (_Rand.Next(1, 4))
                    {
                        case 1:
                            X = -_Rand.Next(20, 60);
                            Y = _Rand.Next(0, Height);
                            break;
                        case 2:
                            X = Width + _Rand.Next(20, 60);
                            Y = _Rand.Next(0, Height);
                            break;
                        case 3:
                            X = _Rand.Next(0, Width);
                            Y = -_Rand.Next(20, 60);
                            break;
                        case 4:
                            X = _Rand.Next(0, Width);
                            Y = Height + _Rand.Next(20, 60);
                            break;
                    }
                    GameObjects.Add(new ObjectCatcher(X, Y, movesCount, size, speedPerMove, life, Color.Red, PlayerObject));
                }
                _GroupCount++;
            }
            else
            {
                int X = 0, Y = 0;
                switch (_Rand.Next(1, 4))
                {
                    case 1:
                        X = -_Rand.Next(20, 60);
                        Y = _Rand.Next(0, Height);
                        break;
                    case 2:
                        X = Width + _Rand.Next(20, 60);
                        Y = _Rand.Next(0, Height);
                        break;
                    case 3:
                        X = _Rand.Next(0, Width);
                        Y = -_Rand.Next(20, 60);
                        break;
                    case 4:
                        X = _Rand.Next(0, Width);
                        Y = Height + _Rand.Next(20, 60);
                        break;
                }

                if (Level % _LevelOfInterceptor == 0)
                {
                    int size = _Rand.Next(6, 8);
                    int movesCount = _Rand.Next(6, 10);
                    float speed = _Rand.Next(250, 300) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToAction(7F * lifeLevel);
                    GameObjects.Add(new ObjectCatcherInterceptor(X, Y, movesCount, size, speedPerMove, life, Color.Fuchsia));
                }
                else if (Level % _LevelOfCatcherFaster == 0)
                {
                    int size = _Rand.Next(3, 4);
                    int movesCount = _Rand.Next(8, 15);
                    float speed = _Rand.Next(800, 900) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToAction(4.5F * lifeLevel);
                    GameObjects.Add(new ObjectCatcherFaster(X, Y, movesCount, size, speedPerMove, life, Color.Blue, PlayerObject));
                }
                else
                {
                    int size = _Rand.Next(3, 6);
                    int movesCount = _Rand.Next(15, 30);
                    float speed = _Rand.Next(700, 850) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToAction(3.5F * lifeLevel);

                    GameObjects.Add(new ObjectCatcher(X, Y, movesCount, size, speedPerMove, life, Color.Red, PlayerObject));
                }
            }
        }

        private void TimerOfAction_Tick(object sender, EventArgs e)
        {
            if (!_IsStart) return;

            if (!_IsEnd)
            {
                //Score
                Score += Level;

                //Slow
                if (_SlowON)
                {
                    _Energy -= _EnergyForSlowPerAction;
                    if (_Energy <= 0)
                    {
                        _Energy = 0;
                        _SlowON = false;
                    }
                }
                else
                {
                    if (_Energy < _EnergyMax) _Energy += _EnergyGetPerAction;
                    if (_Energy > _EnergyMax)
                    {
                        _Energy = _EnergyMax;
                    }
                }
            }
            else
            {
                if (_EndTick >= _EndMaxActions)
                {
                    _IsStart = false;
                    _IsEnd = false;
                }
                else _EndTick++;
            }

            float worldSpeed = _SlowON ? 0.3F : 1;

            //Dark
            if (_DarkAni > 0)
            {
                if (_DarkAni >= _DarkMaxActions)
                {
                    _DarkAni = 0;
                }
                else _DarkAni++;
            }

            //Limited
            if (_TickOfLimited > 0)
            {
                if (_TickOfLimited >= _LimitedMaxActions)
                {
                    _TickOfLimited = 0;
                }
                else
                {
                    if (_TickOfLimited < _LimitedFadeActions)
                    {
                        GameRectangle = new Rectangle(GameRectangle.X + _LimitedPerAction.Left, GameRectangle.Y + _LimitedPerAction.Top,
                                                      GameRectangle.Width - _LimitedPerAction.Right, GameRectangle.Height - _LimitedPerAction.Bottom);
                    }
                    else if (_TickOfLimited > _LimitedMaxActions - _LimitedFadeActions)
                    {
                        GameRectangle = new Rectangle(GameRectangle.X - _LimitedPerAction.Left, GameRectangle.Y - _LimitedPerAction.Top,
                                                  GameRectangle.Width + _LimitedPerAction.Right, GameRectangle.Height + _LimitedPerAction.Bottom);
                    }

                    _TickOfLimited++;
                }
            }

            List<ObjectBase> deadObjects = new List<ObjectBase>();
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Action();
            }

            for (int i = 0; i < GameObjects.Count; i++)
            {
                ObjectBase gameObject = GameObjects[i];
                if (gameObject.Status == ObjectStatus.Dead)
                {
                    deadObjects.Add(gameObject);
                }
            }

            foreach (ObjectBase deadObject in deadObjects)
            {
                GameObjects.Remove(deadObject);
            }

            Drawing();
        }

        private void RunningBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (_IsStart)
            {
                switch (e.Button)
                {
                    case System.Windows.Forms.MouseButtons.Left:
                        break;
                    case System.Windows.Forms.MouseButtons.Right:
                        if (_Energy > _EnergyForSlow)
                        {
                            _SlowON = true;
                        }
                        break;
                }
            }
            else
            {
                SetStart(e.X, e.Y);
            }
        }

        private void RunningBox_MouseMove(object sender, MouseEventArgs e)
        {
            TrackPoint = e.Location;
        }

        private void RunningBox_MouseEnter(object sender, EventArgs e)
        {
            if (_IsStart)
            {
                Cursor.Hide();
            }
        }

        private void RunningBox_MouseLeave(object sender, EventArgs e)
        {
            if (_IsStart)
            {
                Cursor.Show();
            }
        }

        public void SetStart(int potX, int potY)
        {
            Level = 0;
            Score = 0;

            GameObjects.Clear();
            PlayerObject = new ObjectPlayer(potX, potY, 8, 3, 0, Color.Black);
            GameObjects.Add(PlayerObject);
            _GroupCount = _DefaultGroupCount;
            _MineCount = _DefaultMineCount;
            _Energy = _EnergyMax;
            _DarkAni = 0;
            _TickOfLimited = 0;
            _SlowON = false;
            _IsStart = true;
            GameRectangle = new Rectangle(50, 50, Width - 100, Height - 100);

            if (_BackImage != null) _BackImage.Dispose();
            if (_BackGraphics != null) _BackGraphics.Dispose();
            if (_ControlGraphics != null) _ControlGraphics.Dispose();

            _BackImage = new Bitmap(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            _BackGraphics = Graphics.FromImage(_BackImage);
            _ControlGraphics = CreateGraphics();

            _BackGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Cursor.Hide();
        }



        private void SetEnd(ObjectBase killerObj)
        {
            PlayerObject = null;
            _IsEnd = true;
            _EndTick = 0;
            _DarkAni = 0;
            Cursor.Show();
        }

        public int SecToAction(float sec)
        {
            return (int)(sec * 1000 / _TimerOfAction.Interval);
        }


        Stopwatch sw = new Stopwatch();
        private void Drawing()
        {
            sw.Restart();
            _BackGraphics.Clear(Color.White);
            _BackGraphics.ResetTransform();
            if (_IsEnd && _EndTick < 10)
            {
                int shakeX = _Rand.Next(-10, 10);
                int shakeY = _Rand.Next(-10, 10);
                _BackGraphics.TranslateTransform(shakeX, shakeY, System.Drawing.Drawing2D.MatrixOrder.Append);
            }

            if (GameRectangle != null)
            {
                _BackGraphics.DrawRectangle(_PenRectGaming, GameRectangle);
            }
            if (_DarkAni > 0)
            {
                int alpha = 255;
                if (_DarkAni < _DarkFadeInActions)
                {
                    alpha = (int)(_DarkAni / _DarkFadeInActions * 255F);
                }
                else
                {
                    int fadOutAction = _DarkMaxActions - _DarkFadeOutActions;
                    if (_DarkAni > fadOutAction)
                    {
                        alpha = (int)((_DarkAni - fadOutAction) / _DarkFadeOutActions * 255F);
                    }
                }

                using (SolidBrush sb = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    _BackGraphics.FillRectangle(sb, 0, 0, Width, Height);
                }
            }

            _BackGraphics.FillRectangle(_Energy < _EnergyForSlow ? Brushes.Red : Brushes.Black, _RectOfEngery.X + 2, _RectOfEngery.Y + 2, (_RectOfEngery.Width - 4) * _Energy / _EnergyMax, _RectOfEngery.Height - 4);
            _BackGraphics.DrawRectangle(Pens.Black, _RectOfEngery);
            _BackGraphics.DrawString(Score.ToString(), Font, Brushes.Black, _RectOfEngery.X + _RectOfEngery.Width + 10, _RectOfEngery.Y);

            foreach (ObjectBase activeObj in GameObjects)
            {
                activeObj.DrawSelf(_BackGraphics);
            }


            _ControlGraphics.DrawImageUnscaled(_BackImage, 0, 0);

            sw.Stop();
            _FPSTic--;
            if (_FPSTic <= 0)
            {
                _FPSTic = 10;
                FindForm().Text = (1000 / sw.Elapsed.Milliseconds).ToString();
            }
        }


    }
}
