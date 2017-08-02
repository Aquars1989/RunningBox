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
    public partial class PlayingBox : UserControl
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

        private bool _PlayerAlive = false;
        private GameObject _PlayerObject = null;
        private List<GameObject> _ActiveObjects = new List<GameObject>();
        private List<VoidObject> _VoidObjects = new List<VoidObject>();
        private PointF _PointOfMouse = new PointF(50, 50);

        private int _Energy = 0;
        private int _Score = 0;
        private bool _SlowON = false;
        private bool _JetON = false;

        private Color _ColorFadeBlue = Color.FromArgb(30, 0, 0, 255);
        private Color _ColorFadeRed = Color.FromArgb(30, 255, 0, 0);
        private Color _ColorFadeBlack = Color.FromArgb(30, 0, 0, 0);
        private Color _ColorFadeFuchsia = Color.FromArgb(30, 255, 0, 255);
        private Color _ColorFadeFirebrick = Color.FromArgb(30, 178, 34, 34);

        private Pen _PenRectGaming = new Pen(Color.LightGreen, 2);
        private Pen _PenPlayer = new Pen(Color.Black, 2);

        private Timer _TimerOfAction = new Timer() { Enabled = true, Interval = 25 };
        private Timer _TimerOfBuild = new Timer() { Enabled = true, Interval = 1000 };
        private Random _Rand = new Random();

        private bool _IsStart = false;
        private bool _IsEnd = false;
        private int _EndTick = 0;
        private int _LeveL = 0;
        private int _DarkAni = 0;

        private int _LevelOfDark = 22;
        private int _LevelOfGroup = 15;
        private int _LevelOfInterceptor = 11;
        private int _LeveLOfMine = 8;
        private int _LevelOfCatcher = 3;
        private int _LevelOfLimited = 28;


        private int _GroupCount = 0;
        private int _MineCount = 0;
        private int _MineExpand = 8;

        private Rectangle _RectOfEngery = new Rectangle(50, 10, 100, 10);
        private Rectangle _RectOfGaming;

        private Graphics _ControlGraphics;
        private Graphics _BackGraphics;
        private Bitmap _BackImage;

        private int _TickOfLimited = 0;
        private Padding _LimitedPerAction = new Padding();

        private int _FPSTic = 10;
        public PlayingBox()
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

            _LeveL++;

            float lifeLevel = (100F + _LeveL) / 100F;
            float speedLevel = (70F + _LeveL) / 100F;

            if (_LeveL % _LevelOfDark == 0 && _DarkAni == 0)
            {
                _DarkAni = 1;
            }

            if (_LeveL % _LevelOfLimited == 0 && _TickOfLimited == 0)
            {
                _TickOfLimited = 1;
                double scaleX = _Rand.NextDouble();
                double scaleY = 1 - scaleX;
                int limitX = (int)(scaleX * _RectOfGaming.Width / 100);
                int limitY = (int)(scaleY * _RectOfGaming.Height / 100);

                int limitLeft = _Rand.Next(0, limitX);
                int limitTop = _Rand.Next(0, limitY);
                int limitRight = limitX - limitLeft;
                int limitDown = limitY - limitTop;

                _LimitedPerAction = new Padding(limitLeft, limitTop, limitRight, limitDown);
            }

            if (_LeveL % _LeveLOfMine == 0)
            {
                for (int i = 0; i < _MineCount; i++)
                {
                    int size = _Rand.Next(8, 10);
                    int movesCount = _Rand.Next(10, 15);
                    float speed = _Rand.Next(850, 1000) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToAction(_LeveLOfMine * 0.5F);// +_Rand.Next(0, 5);
                    int X = 0, Y = 0, lockPotX = 0, lockPotY = 0;
                    switch (_Rand.Next(1, 4))
                    {
                        case 1:
                            X = -_Rand.Next(20, 60);
                            Y = _Rand.Next(0, Height);
                            lockPotX = _Rand.Next(_RectOfGaming.Left + 20, _RectOfGaming.Left + _RectOfGaming.Width / 2);
                            lockPotY = _Rand.Next(_RectOfGaming.Top + 20, _RectOfGaming.Top + _RectOfGaming.Height - 20);
                            break;
                        case 2:
                            X = Width + _Rand.Next(20, 60);
                            Y = _Rand.Next(0, Height);
                            lockPotX = _Rand.Next(_RectOfGaming.Left + _RectOfGaming.Width / 2, _RectOfGaming.Left + _RectOfGaming.Width - 20);
                            lockPotY = _Rand.Next(_RectOfGaming.Top + 20, _RectOfGaming.Top + _RectOfGaming.Height - 20);
                            break;
                        case 3:
                            X = _Rand.Next(0, Width);
                            Y = -_Rand.Next(20, 60);
                            lockPotX = _Rand.Next(_RectOfGaming.Left + 20, _RectOfGaming.Left + _RectOfGaming.Width - 20);
                            lockPotY = _Rand.Next(_RectOfGaming.Top + 20, _RectOfGaming.Top + _RectOfGaming.Height / 2);
                            break;
                        case 4:
                            X = _Rand.Next(0, Width);
                            Y = Height + _Rand.Next(20, 60);
                            lockPotX = _Rand.Next(_RectOfGaming.Left + 20, _RectOfGaming.Left + _RectOfGaming.Width - 20);
                            lockPotY = _Rand.Next(_RectOfGaming.Top + _RectOfGaming.Height / 2, _RectOfGaming.Height - 20);
                            break;
                    }
                    _ActiveObjects.Add(new GameObject(ActiveType.Mine, ShapeType.Image, X, Y, movesCount, size, speedPerMove, Color.Firebrick, life)
                    {
                        Image = RunningBox.Properties.Resources.Mine,
                        LockPoint = new PointF(lockPotX, lockPotY)
                    });
                }
                _MineCount++;
            }

            if (_LeveL % _LevelOfGroup == 0)
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
                    _ActiveObjects.Add(new GameObject(ActiveType.Nomal, ShapeType.Ellipse, X, Y, movesCount, size, speedPerMove, Color.Red, life));
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

                if (_LeveL % _LevelOfInterceptor == 0)
                {
                    int size = _Rand.Next(6, 8);
                    int movesCount = _Rand.Next(10, 15);
                    float speed = _Rand.Next(850, 1000) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToAction(7F * lifeLevel);
                    _ActiveObjects.Add(new GameObject(ActiveType.Smart, ShapeType.Ellipse, X, Y, movesCount, size, speedPerMove, Color.Fuchsia, life));
                }
                else if (_LeveL % _LevelOfCatcher == 0)
                {
                    int size = _Rand.Next(3, 4);
                    int movesCount = _Rand.Next(8, 15);
                    float speed = _Rand.Next(800, 900) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToAction(4.5F * lifeLevel);
                    _ActiveObjects.Add(new GameObject(ActiveType.Fast, ShapeType.Ellipse, X, Y, movesCount, size, speedPerMove, Color.Blue, life));
                }
                else
                {
                    int size = _Rand.Next(3, 6);
                    int movesCount = _Rand.Next(15, 30);
                    float speed = _Rand.Next(700, 850) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToAction(3.5F * lifeLevel);
                    _ActiveObjects.Add(new GameObject(ActiveType.Nomal, ShapeType.Ellipse, X, Y, movesCount, size, speedPerMove, Color.Red, life));
                }
            }
        }

        private void TimerOfAction_Tick(object sender, EventArgs e)
        {
            if (!_IsStart) return;

            if (!_IsEnd)
            {
                //Score
                _Score += _LeveL;

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
                        _RectOfGaming = new Rectangle(_RectOfGaming.X + _LimitedPerAction.Left, _RectOfGaming.Y + _LimitedPerAction.Top,
                                                      _RectOfGaming.Width - _LimitedPerAction.Right, _RectOfGaming.Height - _LimitedPerAction.Bottom);
                    }
                    else if (_TickOfLimited > _LimitedMaxActions - _LimitedFadeActions)
                    {
                        _RectOfGaming = new Rectangle(_RectOfGaming.X - _LimitedPerAction.Left, _RectOfGaming.Y - _LimitedPerAction.Top,
                                                   _RectOfGaming.Width + _LimitedPerAction.Right, _RectOfGaming.Height + _LimitedPerAction.Bottom);
                    }

                    _TickOfLimited++;
                }
            }

            //void
            List<VoidObject> removeVoids = new List<VoidObject>();
            for (int i = 0; i < _VoidObjects.Count; i++)
            {
                VoidObject voidObj = _VoidObjects[i];
                voidObj.FadeTick++;
                if (voidObj.FadeTick >= voidObj.MaxFadeTick)
                {
                    if (voidObj.Size <= 1)
                    {
                        removeVoids.Add(voidObj);
                    }
                    else
                    {
                        voidObj.FadeTick = 0;
                        voidObj.Size--;
                    }
                }
            }

            foreach (VoidObject voidObj in removeVoids)
            {
                _VoidObjects.Remove(voidObj);
            }

            //player
            PointF playerPoint = new PointF(0, 0);
            if (_PlayerAlive)
            {
                if (_PlayerObject.Moves.Count >= _PlayerObject.MaxMoves)
                {
                    _PlayerObject.Moves.Dequeue();
                }

                float playerX = _PlayerObject.X;
                float playerY = _PlayerObject.Y;
                double playerMove = ((Math.Abs(_PointOfMouse.X - playerX) * 2 + Math.Abs(_PointOfMouse.Y - playerY) * 2) / 120) + 0.4F;
                double playerDirection = PointRotation(new PointF(playerX, playerY), _PointOfMouse);
                double playerMoveX = Math.Cos(playerDirection / 180 * Math.PI) * playerMove;
                double playerMoveY = Math.Sin(playerDirection / 180 * Math.PI) * playerMove;

                if (playerX < _RectOfGaming.Left)
                {
                    playerMoveX = Math.Abs(playerMoveX) * 2;
                }
                else if (playerX > _RectOfGaming.Left + _RectOfGaming.Width)
                {
                    playerMoveX = -Math.Abs(playerMoveX) * 2;
                }

                if (playerY < _RectOfGaming.Top)
                {
                    playerMoveY = Math.Abs(playerMoveY) * 2;
                }
                else if (playerY > _RectOfGaming.Top + _RectOfGaming.Height)
                {
                    playerMoveY = -Math.Abs(playerMoveY) * 2;
                }

                if (_PlayerObject.JetTime == 0 && _JetON && _Energy > _EnergyForSprint)
                {
                    _Energy -= _EnergyForSprint;
                    playerMoveX *= 5;
                    playerMoveY *= 5;
                    playerMoveX += Math.Cos(playerDirection / 180 * Math.PI) * 15F;
                    playerMoveY += Math.Sin(playerDirection / 180 * Math.PI) * 15F;
                    _PlayerObject.JetTime = _PlayerObject.MaxMoves;
                }

                if (_PlayerObject.JetTime > 0)
                {
                    _PlayerObject.JetTime--;
                    _VoidObjects.Add(new VoidObject((int)playerX, (int)playerY, _PlayerObject.Size, 3, _ColorFadeBlack));
                }
                _JetON = false;

                _PlayerObject.Moves.Enqueue(new PointF((float)playerMoveX, (float)playerMoveY));
                foreach (PointF pt in _PlayerObject.Moves)
                {
                    playerX += pt.X * worldSpeed;
                    playerY += pt.Y * worldSpeed;
                }

                playerPoint = new PointF(playerX, playerY);
                _PlayerObject.X = (int)playerX;
                _PlayerObject.Y = (int)playerY;
            }

            //active
            List<GameObject> removeActive = new List<GameObject>();
            for (int i = 0; i < _ActiveObjects.Count; i++)
            {
                GameObject activeObj = _ActiveObjects[i];

                //collision
                if (_PlayerAlive && activeObj.ActType != ActiveType.Leave && activeObj.Rect.IntersectsWith(_PlayerObject.Rect))
                {
                    SetEnd(activeObj);
                    return;
                }

                //checkLife
                if (activeObj.ActType == ActiveType.Leave)
                {
                    activeObj.FadeTime -= activeObj.FadeSpeed;
                    if (activeObj.FadeTime <= 0)
                    {
                        removeActive.Add(activeObj);
                    }
                }
                else
                {
                    activeObj.LifeTime--;
                    if (activeObj.LifeTime <= 0)
                    {
                        KillObject(activeObj);
                    }
                }

                //move
                if (activeObj.Moves.Count >= activeObj.MaxMoves)
                {
                    activeObj.Moves.Dequeue();
                }

                float activeX = activeObj.X;
                float activeY = activeObj.Y;
                switch (activeObj.ActType)
                {
                    case ActiveType.Nomal:
                        {
                            if (_PlayerAlive)
                            {
                                double activeDirection = PointRotation(new PointF(activeX, activeY), playerPoint);
                                double activeMoveX = Math.Cos(activeDirection / 180 * Math.PI) * (activeObj.Speed / 100F);
                                double activeMoveY = Math.Sin(activeDirection / 180 * Math.PI) * (activeObj.Speed / 100F);
                                if (activeObj.JetTime > 0)
                                {
                                    if (activeObj.JetType == 2)
                                    {
                                        _VoidObjects.Add(new VoidObject((int)activeX, (int)activeY, activeObj.Size, 3, _ColorFadeRed));
                                    }
                                    activeObj.JetTime--;
                                }
                                else
                                {
                                    double randNum = _Rand.NextDouble() * 100;
                                    if (randNum < 0.5)
                                    {
                                        activeObj.JetTime = activeObj.MaxMoves;
                                        activeObj.JetType = 2;
                                        activeMoveX *= 30;
                                        activeMoveY *= 30;
                                    }
                                    else if (randNum < 3)
                                    {
                                        activeObj.JetTime = activeObj.MaxMoves;
                                        activeObj.JetType = 1;
                                        activeMoveX *= 10;
                                        activeMoveY *= 10;
                                    }
                                }
                                activeObj.Moves.Enqueue(new PointF((float)activeMoveX, (float)activeMoveY));
                            }
                            foreach (PointF pt in activeObj.Moves)
                            {
                                activeX += pt.X * worldSpeed;
                                activeY += pt.Y * worldSpeed;
                            }
                        }
                        break;
                    case ActiveType.Fast:
                        {
                            if (_PlayerAlive)
                            {
                                double activeDirection = PointRotation(new PointF(activeX, activeY), playerPoint);
                                double activeMoveX = Math.Cos(activeDirection / 180 * Math.PI) * (activeObj.Speed / 100F);
                                double activeMoveY = Math.Sin(activeDirection / 180 * Math.PI) * (activeObj.Speed / 100F);
                                if (activeObj.JetTime > 0)
                                {
                                    activeObj.JetTime--;
                                    if (activeObj.JetType == 2)
                                    {
                                        _VoidObjects.Add(new VoidObject((int)activeX, (int)activeY, activeObj.Size, 3, _ColorFadeBlue));
                                    }
                                }
                                else
                                {
                                    double randNum = _Rand.NextDouble() * 100;
                                    if (randNum < 0.8)
                                    {
                                        activeObj.JetTime = activeObj.MaxMoves;
                                        activeObj.JetType = 2;
                                        activeMoveX *= 20;
                                        activeMoveY *= 20;
                                    }
                                    else if (randNum < 3)
                                    {
                                        activeObj.JetTime = activeObj.MaxMoves;
                                        activeObj.JetType = 1;
                                        activeMoveX *= 10;
                                        activeMoveY *= 10;
                                    }
                                }
                                activeObj.Moves.Enqueue(new PointF((float)activeMoveX, (float)activeMoveY));
                            }
                            foreach (PointF pt in activeObj.Moves)
                            {
                                activeX += pt.X * worldSpeed;
                                activeY += pt.Y * worldSpeed;
                            }
                        }
                        break;
                    case ActiveType.Smart:
                        {
                            if (_PlayerAlive)
                            {
                                double activeDirection = PointRotation(new PointF(activeX, activeY), _PointOfMouse);
                                double activeMoveX = Math.Cos(activeDirection / 180 * Math.PI) * (activeObj.Speed / 100F);
                                double activeMoveY = Math.Sin(activeDirection / 180 * Math.PI) * (activeObj.Speed / 100F);
                                if (activeObj.JetTime > 0)
                                {
                                    activeObj.JetTime--;
                                }
                                else
                                {
                                    double randNum = _Rand.NextDouble() * 100;
                                    if (randNum < 3)
                                    {
                                        activeObj.JetTime = activeObj.MaxMoves;
                                        activeObj.JetType = 1;
                                        activeMoveX *= 10;
                                        activeMoveY *= 10;
                                    }
                                }
                                activeObj.Moves.Enqueue(new PointF((float)activeMoveX, (float)activeMoveY));
                            }
                            foreach (PointF pt in activeObj.Moves)
                            {
                                activeX += pt.X * worldSpeed;
                                activeY += pt.Y * worldSpeed;
                            }
                        }
                        break;
                    case ActiveType.Mine:
                    case ActiveType.Leave:
                        {
                            double activeDirection = PointRotation(new PointF(activeX, activeY), activeObj.LockPoint);
                            double activeMoveX = Math.Cos(activeDirection / 180 * Math.PI) * (activeObj.Speed / 100F);
                            double activeMoveY = Math.Sin(activeDirection / 180 * Math.PI) * (activeObj.Speed / 100F);
                            activeObj.Moves.Enqueue(new PointF((float)activeMoveX, (float)activeMoveY));

                            foreach (PointF pt in activeObj.Moves)
                            {
                                activeX += pt.X * worldSpeed;
                                activeY += pt.Y * worldSpeed;
                            }
                        }
                        break;
                }

                activeObj.X = (int)activeX;
                activeObj.Y = (int)activeY;

                //collision
                if (_PlayerAlive && activeObj.ActType != ActiveType.Leave && activeObj.Rect.IntersectsWith(_PlayerObject.Rect))
                {
                    SetEnd(activeObj);
                    return;
                }
            }

            foreach (GameObject activeObj in removeActive)
            {
                _ActiveObjects.Remove(activeObj);
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
                        _JetON = true;
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
            _PointOfMouse = e.Location;
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
            _LeveL = 0;
            _PlayerObject = new GameObject(ActiveType.Player, ShapeType.Ellipse, potX, potY, 8, 3, 0, Color.Black, 0);
            _PlayerAlive = true;
            _VoidObjects.Clear();
            _ActiveObjects.Clear();
            _GroupCount = _DefaultGroupCount;
            _MineCount = _DefaultMineCount;
            _Energy = _EnergyMax;
            _Score = 0;
            _DarkAni = 0;
            _TickOfLimited = 0;
            _SlowON = false;
            _JetON = false;
            _IsStart = true;
            _RectOfGaming = new Rectangle(50, 50, Width - 100, Height - 100);

            if (_BackImage != null) _BackImage.Dispose();
            if (_BackGraphics != null) _BackGraphics.Dispose();
            if (_ControlGraphics != null) _ControlGraphics.Dispose();

            _BackImage = new Bitmap(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            _BackGraphics = Graphics.FromImage(_BackImage);
            _ControlGraphics = CreateGraphics();

            _BackGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Cursor.Hide();
        }

        private void KillObject(GameObject gameObj)
        {
            switch (gameObj.ActType)
            {
                case ActiveType.Mine:
                    gameObj.ShapeType = ShapeType.Ellipse;
                    gameObj.Size *= _MineExpand;
                    if (_PlayerAlive && gameObj.Rect.IntersectsWith(_PlayerObject.Rect))
                    {
                        SetEnd(gameObj);
                        return;
                    }
                    break;
            }
            gameObj.ActType = ActiveType.Leave;
        }

        private void SetEnd(GameObject killerObj)
        {
            _PlayerAlive = false;

            if (killerObj.ActType == ActiveType.Mine)
            {
                KillObject(killerObj);
            }

            int playerX = _PlayerObject.X;
            int playerY = _PlayerObject.Y;
            double playerDirection = PointRotation(new PointF(playerX, playerY), new PointF(_PointOfMouse.X, _PointOfMouse.Y));
            for (int i = 0; i < 15; i++)
            {
                int scrapSpeed = _Rand.Next(300, 900);
                int scrapLife = _Rand.Next(300, 600);
                double scrapDirection = playerDirection + (_Rand.NextDouble() - 0.5) * 20;
                int scrapLockX = playerX + (int)(Math.Cos(scrapDirection / 180 * Math.PI) * 1000);
                int scrapLockY = playerY + (int)(Math.Sin(scrapDirection / 180 * Math.PI) * 1000);
                _ActiveObjects.Add(new GameObject(ActiveType.Leave, ShapeType.Rectangle, playerX, playerY, 1, 1, scrapSpeed, _PlayerObject.Pen.Color, 0) { FadeTime = scrapLife, LockPoint = new PointF(scrapLockX, scrapLockY) });
            }
            _PlayerObject = null;
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

            if (_RectOfGaming != null)
            {
                _BackGraphics.DrawRectangle(_PenRectGaming, _RectOfGaming);
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
            _BackGraphics.DrawString(_Score.ToString(), Font, Brushes.Black, _RectOfEngery.X + _RectOfEngery.Width + 10, _RectOfEngery.Y);

            foreach (VoidObject vo in _VoidObjects)
            {
                _BackGraphics.FillEllipse(vo.Brush, vo.Rect);
            }

            if (_IsStart && _PlayerAlive)
            {
                switch (_PlayerObject.ShapeType)
                {
                    case ShapeType.Rectangle:
                        _BackGraphics.FillRectangle(_PlayerObject.Brush, _PlayerObject.Rect);
                        break;
                    case ShapeType.Ellipse:
                        _BackGraphics.DrawEllipse(_PenPlayer, _PlayerObject.Rect);
                        break;
                    case ShapeType.Image:
                        _BackGraphics.DrawImage(_PlayerObject.Image, _PlayerObject.Rect);
                        break;
                }
            }

            foreach (GameObject activeObj in _ActiveObjects)
            {
                Rectangle rectDraw = Rectangle.Empty;
                Brush brushDraw = null;
                bool newBrush = false;

                switch (activeObj.ActType)
                {
                    case ActiveType.Leave:
                        rectDraw = activeObj.Rect;
                        Color activeColor = ((SolidBrush)activeObj.Brush).Color;
                        int alpha = activeObj.FadeTime;
                        if (alpha > 255) alpha = 255;
                        brushDraw = new SolidBrush(Color.FromArgb(alpha, activeColor.R, activeColor.G, activeColor.B));
                        newBrush = true;
                        break;
                    case ActiveType.Mine:
                        if (activeObj.LifeTime < 50)
                        {
                            int sizeFix = activeObj.LifeTime / 2 % 5;
                            rectDraw = new Rectangle(activeObj.Rect.Left - sizeFix, activeObj.Rect.Top - sizeFix, activeObj.Rect.Width + sizeFix * 2, activeObj.Rect.Height + sizeFix * 2);
                            brushDraw = activeObj.Brush;
                        }
                        else
                        {
                            goto default;
                        }
                        break;
                    default:
                        rectDraw = activeObj.Rect;
                        brushDraw = activeObj.Brush;
                        break;
                }

                switch (activeObj.ShapeType)
                {
                    case ShapeType.Rectangle:
                        _BackGraphics.FillRectangle(brushDraw, rectDraw);
                        break;
                    case ShapeType.Ellipse:
                        _BackGraphics.FillEllipse(brushDraw, rectDraw);
                        break;
                    case ShapeType.Image:
                        _BackGraphics.DrawImage(activeObj.Image, rectDraw);
                        break;
                }

                if (newBrush)
                {
                    brushDraw.Dispose();
                }
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

        public static double PointRotation(PointF PotA, PointF PotB)
        {
            var Dx = PotB.X - PotA.X;
            var Dy = PotB.Y - PotA.Y;
            var DRoation = Math.Atan2(Dy, Dx);
            var WRotation = DRoation / Math.PI * 180;
            return WRotation;
        }

        private enum ActiveType
        {
            Player = 0,
            Nomal = 1,
            Fast = 2,
            Smart = 3,
            Leave = 4,
            Mine = 5
        }

        private enum ShapeType
        {
            Rectangle = 0,
            Ellipse = 1,
            Image = 2
        }

        private class GameObject
        {
            public Brush Brush { get; set; }
            public Pen Pen { get; set; }
            public Queue<PointF> Moves { get; set; }
            public ActiveType ActType { get; set; }
            public ShapeType ShapeType { get; set; }
            public PointF LockPoint { get; set; }
            public Image Image { get; set; }
            public int LifeTime { get; set; }
            public int MaxMoves { get; set; }
            public float Speed { get; set; }
            public int FadeTime { get; set; }
            public int FadeSpeed { get; set; }
            public int JetTime { get; set; }
            public int JetType { get; set; }

            private bool BuildRect = false;
            private int _X;
            public int X
            {
                get { return _X; }
                set
                {
                    _X = value;
                    BuildRect = false;
                }
            }

            private int _Y;
            public int Y
            {
                get { return _Y; }
                set
                {
                    _Y = value;
                    BuildRect = false;
                }
            }

            private int _Size;
            public int Size
            {
                get { return _Size; }
                set
                {
                    _Size = value;
                    BuildRect = false;
                }
            }

            private Rectangle _Rect;
            public Rectangle Rect
            {
                get
                {
                    if (!BuildRect)
                    {
                        _Rect = new Rectangle(X - Size, Y - Size, Size * 2, Size * 2);
                        BuildRect = true;
                    }
                    return _Rect;
                }
            }


            public GameObject(ActiveType actType, ShapeType shapeType, int x, int y, int maxMoves, int size, float speed, Color color, int lifeTime)
            {
                ActType = actType;
                ShapeType = shapeType;
                MaxMoves = maxMoves;
                X = x;
                Y = y;
                LifeTime = lifeTime;
                Size = size;
                Speed = speed;
                Moves = new Queue<PointF>();
                Brush = DrawPool.GetBrush(color);
                Pen = DrawPool.GetPen(color);
                FadeTime = 200;
                FadeSpeed = 20;
            }
        }

        private class VoidObject
        {
            public Brush Brush { get; set; }
            public int FadeTick { get; set; }
            public int MaxFadeTick { get; set; }
            public ShapeType ShapeType { get; set; }

            private bool BuildRect = false;
            private int _X;
            public int X
            {
                get { return _X; }
                set
                {
                    _X = value;
                    BuildRect = false;
                }
            }

            private int _Y;
            public int Y
            {
                get { return _Y; }
                set
                {
                    _Y = value;
                    BuildRect = false;
                }
            }

            private int _Size;
            public int Size
            {
                get { return _Size; }
                set
                {
                    _Size = value;
                    BuildRect = false;
                }
            }

            private Rectangle _Rect;
            public Rectangle Rect
            {
                get
                {
                    if (!BuildRect)
                    {
                        _Rect = new Rectangle(X - Size, Y - Size, Size * 2, Size * 2);
                        BuildRect = true;
                    }
                    return _Rect;
                }
            }


            public VoidObject(int x, int y, int size, int maxFadeTick, Color color)
            {
                X = x;
                Y = y;
                Size = size;
                MaxFadeTick = maxFadeTick;
                Brush = DrawPool.GetBrush(color);
            }
        }
    }
}
