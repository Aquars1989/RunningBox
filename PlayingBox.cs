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
        private const int _MaxEnergy = 1000;
        private const int _JetEnergy = 350;
        private const int _LowEnergy = 350;
        private const int _SlowEnergy = 15;
        private const int _EnergyGet = 5;
        private const int _EndTickMax = 30;

        bool _HiderAlive = false;
        private GameObject _HiderObj = null;
        private List<GameObject> _ActiveObjs = new List<GameObject>();
        private List<VoidObject> _VoidObjs = new List<VoidObject>();
        private PointF _MousePot = new PointF(50, 50);

        private int _Energy = 0;
        private int _Score = 0;
        private bool _SlowON = false;
        private bool _JetON = false;

        private SolidBrush _BrushHalfBlue = new SolidBrush(Color.FromArgb(30, 0, 0, 255));
        private SolidBrush _BrushHalfRed = new SolidBrush(Color.FromArgb(30, 255, 0, 0));
        private SolidBrush _BrushHalfBlack = new SolidBrush(Color.FromArgb(30, 0, 0, 0));
        private SolidBrush _BrushHalfFuchsia = new SolidBrush(Color.FromArgb(30, 255, 0, 255));
        private SolidBrush _BrushHalfFirebrick = new SolidBrush(Color.FromArgb(30, 178, 34, 34));

        private Timer _TimerMove = new Timer() { Enabled = true, Interval = 25 };
        private Timer _TimerBuild = new Timer() { Enabled = true, Interval = 1000 };
        private Random _Rand = new Random();

        private bool _Start = false;
        private bool _End = false;
        private int _EndTick = 0;
        private int _LeveL = 0;
        private int _DarkAni = 0;

        private int _DarkLv = 22;
        private int _GroupLv = 15;
        private int _SmartLv = 11;
        private int _MineLv = 8;
        private int _FastLv = 3;
        private int _RerectLv = 28;

        private int _GroupPw = 0;
        private int _MinePw = 0;

        private Rectangle _RectEngery = new Rectangle(50, 10, 100, 10);
        private Rectangle _RectInside;

        private Graphics _ControlGraphics;
        private Graphics _BackGraphics;
        private Bitmap _BackImage;

        private int _RerectTick = 0;
        private Rectangle _RerectValue = new Rectangle();

        private int _FPSTic = 10;
        public PlayingBox()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            _TimerMove.Tick += TimerMove_Tick;
            _TimerBuild.Tick += TimerBuild_Tick;
        }

        private void TimerBuild_Tick(object sender, EventArgs e)
        {
            if (!_Start) return;
            _LeveL++;

            float lifeTimeFx = (100F + _LeveL) / 100F;
            float lifeSpeedFx = (70F + _LeveL) / 100F;

            //darkscreen
            if (_LeveL % _DarkLv == 0)
            {
                _DarkAni = 2;
            }

            if (_LeveL % _RerectLv == 0 && _RerectTick == 0)
            {
                _RerectTick = 1;
                int helfVal = ((_RectInside.Height + _RectInside.Width) / 2) / 100;
                int helfX = _Rand.Next(2, helfVal - 2);
                int helfY = helfVal - helfX;
                int helfX2 = helfX;
                int helfY2 = helfY;


                int helfX1 = _Rand.Next(0, helfX2);
                int helfY1 = _Rand.Next(0, helfY2);
                _RerectValue = new Rectangle(helfX1, helfY1, helfX2, helfY2);
            }

            if (_LeveL % _GroupLv == 0)
            {
                for (int i = 0; i < 6 + _GroupPw; i++)
                {
                    int size = _Rand.Next(3, 6);
                    int maxMove = _Rand.Next(15, 30);
                    float speed = _Rand.Next(700, 850) - (size * 50) * lifeSpeedFx;
                    float speed2 = speed / maxMove;
                    int life = SecToTick(6F * lifeTimeFx) + _Rand.Next(0, 5); ;
                    int x2 = 0, y2 = 0;
                    switch (_Rand.Next(1, 4))
                    {
                        case 1:
                            x2 = -_Rand.Next(20, 60);
                            y2 = _Rand.Next(0, Height);
                            break;
                        case 2:
                            x2 = Width + _Rand.Next(20, 60);
                            y2 = _Rand.Next(0, Height);
                            break;
                        case 3:
                            x2 = _Rand.Next(0, Width);
                            y2 = -_Rand.Next(20, 60);
                            break;
                        case 4:
                            x2 = _Rand.Next(0, Width);
                            y2 = Height + _Rand.Next(20, 60);
                            break;
                    }
                    _ActiveObjs.Add(new GameObject(ActType.Nomal, ShapeType.Ellipse, x2, y2, maxMove, size, speed2, Brushes.Red, life, 0, 0));
                }
                _GroupPw++;
            }

            if (_LeveL % _MineLv == 0)
            {
                for (int i = 0; i < 4 + (_MinePw / 2); i++)
                {
                    int size = _Rand.Next(8, 10);
                    int maxMove = _Rand.Next(10, 15);
                    float speed = _Rand.Next(850, 1000) - (size * 50) * lifeSpeedFx;
                    float speed2 = speed / maxMove;
                    int life = SecToTick(_MineLv * 0.5F);// +_Rand.Next(0, 5);
                    int x2 = 0, y2 = 0, lockPotX = 0, lockPotY = 0;
                    switch (_Rand.Next(1, 4))
                    {
                        case 1:
                            x2 = -_Rand.Next(20, 60);
                            y2 = _Rand.Next(0, Height);
                            lockPotX = _Rand.Next(_RectInside.Left + 20, _RectInside.Left + _RectInside.Width / 2);
                            lockPotY = _Rand.Next(_RectInside.Top + 20, _RectInside.Top + _RectInside.Height - 20);
                            break;
                        case 2:
                            x2 = Width + _Rand.Next(20, 60);
                            y2 = _Rand.Next(0, Height);
                            lockPotX = _Rand.Next(_RectInside.Left + _RectInside.Width / 2, _RectInside.Left + _RectInside.Width - 20);
                            lockPotY = _Rand.Next(_RectInside.Top + 20, _RectInside.Top + _RectInside.Height - 20);
                            break;
                        case 3:
                            x2 = _Rand.Next(0, Width);
                            y2 = -_Rand.Next(20, 60);
                            lockPotX = _Rand.Next(_RectInside.Left + 20, _RectInside.Left + _RectInside.Width - 20);
                            lockPotY = _Rand.Next(_RectInside.Top + 20, _RectInside.Top + _RectInside.Height / 2);
                            break;
                        case 4:
                            x2 = _Rand.Next(0, Width);
                            y2 = Height + _Rand.Next(20, 60);
                            lockPotX = _Rand.Next(_RectInside.Left + 20, _RectInside.Left + _RectInside.Width - 20);
                            lockPotY = _Rand.Next(_RectInside.Top + _RectInside.Height / 2, _RectInside.Height - 20);
                            break;
                    }
                    _ActiveObjs.Add(new GameObject(ActType.Mine, ShapeType.Image, x2, y2, maxMove, size, speed2, Brushes.Firebrick, life, lockPotX, lockPotY) { Image = RunningBox.Properties.Resources.Mine });
                }
                _MinePw++;
            }

            int x1 = 0, y1 = 0;
            switch (_Rand.Next(1, 4))
            {
                case 1:
                    x1 = -_Rand.Next(20, 60);
                    y1 = _Rand.Next(0, Height);
                    break;
                case 2:
                    x1 = Width + _Rand.Next(20, 60);
                    y1 = _Rand.Next(0, Height);
                    break;
                case 3:
                    x1 = _Rand.Next(0, Width);
                    y1 = -_Rand.Next(20, 60);
                    break;
                case 4:
                    x1 = _Rand.Next(0, Width);
                    y1 = Height + _Rand.Next(20, 60);
                    break;
            }

            if (_LeveL % _SmartLv == 0)
            {
                int size = _Rand.Next(6, 8);
                int maxMove = _Rand.Next(10, 15);
                float speed = _Rand.Next(850, 1000) - (size * 50) * lifeSpeedFx;
                float speed2 = speed / maxMove;
                int life = SecToTick(7F * lifeTimeFx);
                _ActiveObjs.Add(new GameObject(ActType.Smart, ShapeType.Ellipse, x1, y1, maxMove, size, speed2, Brushes.Fuchsia, life, 0, 0));
            }
            else if (_LeveL % _FastLv == 0)
            {
                int size = _Rand.Next(3, 4);
                int maxMove = _Rand.Next(8, 15);
                float speed = _Rand.Next(800, 900) - (size * 50) * lifeSpeedFx;
                float speed2 = speed / maxMove;
                int life = SecToTick(4.5F * lifeTimeFx);
                _ActiveObjs.Add(new GameObject(ActType.Fast, ShapeType.Ellipse, x1, y1, maxMove, size, speed2, Brushes.Blue, life, 0, 0));
            }
            else
            {
                int size = _Rand.Next(3, 6);
                int maxMove = _Rand.Next(15, 30);
                float speed = _Rand.Next(700, 850) - (size * 50) * lifeSpeedFx;
                float speed2 = speed / maxMove;
                int life = SecToTick(3.5F * lifeTimeFx);
                _ActiveObjs.Add(new GameObject(ActType.Nomal, ShapeType.Ellipse, x1, y1, maxMove, size, speed2, Brushes.Red, life, 0, 0));
            }
        }

        private void TimerMove_Tick(object sender, EventArgs e)
        {
            if (!_Start) return;

            if (!_End)
            {
                //Score
                _Score += _SlowON ? _LeveL / 2 : _LeveL;

                //Slow
                if (_SlowON)
                {
                    _Energy -= _SlowEnergy;
                    if (_Energy <= 0)
                    {
                        _Energy = 0;
                        _SlowON = false;
                    }
                }
                else
                {
                    if (_Energy < _MaxEnergy) _Energy += _EnergyGet;
                }

                //DarkScreen
                if (_DarkAni > 0)
                {
                    _DarkAni += 2;
                    if (_DarkAni > 200)
                    {
                        _DarkAni = 0;
                    }
                }
            }
            else
            {
                _EndTick--;
                if (_EndTick <= 0)
                {
                    _Start = false;
                    _End = false;
                }
            }

            float speedFx = _SlowON ? 0.3F : 1;

            //rerect
            if (_RerectTick > 0)
            {
                if (_RerectTick >= 300)
                {
                    _RerectTick = 0;
                }
                else
                {
                    if (_RerectTick < 40)
                    {
                        _RectInside = new Rectangle(_RectInside.X + _RerectValue.X, _RectInside.Y + _RerectValue.Y,
                                                    _RectInside.Width - _RerectValue.Width, _RectInside.Height - _RerectValue.Height);
                    }
                    else if (_RerectTick > 260)
                    {
                        _RectInside = new Rectangle(_RectInside.X - _RerectValue.X, _RectInside.Y - _RerectValue.Y,
                                                   _RectInside.Width + _RerectValue.Width, _RectInside.Height + _RerectValue.Height);
                    }

                    _RerectTick++;
                }
            }

            //void
            List<VoidObject> liRemoveVoid = new List<VoidObject>();
            for (int i = 0; i < _VoidObjs.Count; i++)
            {
                VoidObject vo = _VoidObjs[i];
                vo.FadeTick++;
                if (vo.FadeTick > vo.MaxFadeTick)
                {
                    if (vo.Size <= 1)
                    {
                        liRemoveVoid.Add(vo);
                    }
                    else
                    {
                        vo.FadeTick = 0;
                        vo.Size--;
                    }
                }
            }
            foreach (VoidObject vo in liRemoveVoid)
            {
                _VoidObjs.Remove(vo);
            }

            //hider
            PointF hiderPot = new PointF(0, 0);
            if (_HiderAlive)
            {
                if (_HiderObj.Moves.Count >= _HiderObj.MaxMoves)
                {
                    _HiderObj.Moves.RemoveAt(0);
                }

                float x1 = _HiderObj.X;
                float y1 = _HiderObj.Y;
                double dist1 = ((Math.Abs(_MousePot.X - x1) * 2 + Math.Abs(_MousePot.Y - y1) * 2) / 120) + 0.4F;
                double rota1 = PointRotation(new PointF(x1, y1), _MousePot);
                double moveX1 = Math.Cos(rota1 / 180 * Math.PI) * dist1;
                double moveY1 = Math.Sin(rota1 / 180 * Math.PI) * dist1;

                if (x1 < _RectInside.Left)
                {
                    moveX1 = Math.Abs(moveX1) * 2;
                }
                else if (x1 > _RectInside.Left + _RectInside.Width)
                {
                    moveX1 = -Math.Abs(moveX1) * 2;
                }

                if (y1 < _RectInside.Top)
                {
                    moveY1 = Math.Abs(moveY1) * 2;
                }
                else if (y1 > _RectInside.Top + _RectInside.Height)
                {
                    moveY1 = -Math.Abs(moveY1) * 2;
                }

                if (_HiderObj.JetTime == 0 && _JetON && _Energy > _JetEnergy)
                {
                    _Energy -= _JetEnergy;
                    moveX1 *= 5;
                    moveY1 *= 5;
                    moveX1 += Math.Cos(rota1 / 180 * Math.PI) * 15F;
                    moveY1 += Math.Sin(rota1 / 180 * Math.PI) * 15F;
                    _HiderObj.JetTime = _HiderObj.MaxMoves;
                }

                if (_HiderObj.JetTime > 0)
                {
                    _HiderObj.JetTime--;
                    _VoidObjs.Add(new VoidObject((int)x1, (int)y1, _HiderObj.Size, 3, _BrushHalfBlack));
                }
                _JetON = false;

                _HiderObj.Moves.Add(new PointF((float)moveX1, (float)moveY1));
                foreach (PointF pt in _HiderObj.Moves)
                {
                    x1 += pt.X * speedFx;
                    y1 += pt.Y * speedFx;
                }

                hiderPot = new PointF(x1, y1);
                _HiderObj.X = (int)x1;
                _HiderObj.Y = (int)y1;
            }

            //seeker
            List<GameObject> liRemoveObj = new List<GameObject>();
            for (int i = 0; i < _ActiveObjs.Count; i++)
            {
                GameObject go = _ActiveObjs[i];
                //checkIntersects
                if (_HiderAlive && go.ActType != ActType.Leave && go.Rect.IntersectsWith(_HiderObj.Rect))
                {
                    SetEnd(go);
                    return;
                }

                //checkLife
                if (go.ActType == ActType.Leave)
                {
                    go.FadeTime -= go.FadeSpeed;
                    if (go.FadeTime <= 0)
                    {
                        liRemoveObj.Add(go);
                    }
                }
                else
                {
                    go.LifeTime--;
                    if (go.LifeTime <= 0)
                    {
                        KillObject(go);
                    }
                }

                //move
                if (go.Moves.Count >= go.MaxMoves)
                {
                    go.Moves.RemoveAt(0);
                }

                float x2 = go.X;
                float y2 = go.Y;
                switch (go.ActType)
                {
                    case ActType.Nomal:
                        {
                            if (_HiderAlive)
                            {
                                double rota2 = PointRotation(new PointF(x2, y2), hiderPot);
                                double moveX2 = Math.Cos(rota2 / 180 * Math.PI) * (go.Speed / 100F);
                                double moveY2 = Math.Sin(rota2 / 180 * Math.PI) * (go.Speed / 100F);
                                if (go.JetTime > 0)
                                {
                                    go.JetTime--;
                                    if (go.JetType == 2)
                                    {
                                        _VoidObjs.Add(new VoidObject((int)x2, (int)y2, go.Size, 3, _BrushHalfRed));
                                    }
                                }
                                else
                                {
                                    double randNum = _Rand.NextDouble() * 100;
                                    if (randNum < 0.5)
                                    {
                                        go.JetTime = go.MaxMoves;
                                        go.JetType = 2;
                                        moveX2 *= 30;
                                        moveY2 *= 30;
                                    }
                                    else if (randNum < 3)
                                    {
                                        go.JetTime = go.MaxMoves;
                                        go.JetType = 1;
                                        moveX2 *= 10;
                                        moveY2 *= 10;
                                    }
                                }
                                go.Moves.Add(new PointF((float)moveX2, (float)moveY2));
                            }
                            foreach (PointF pt in go.Moves)
                            {
                                x2 += pt.X * speedFx;
                                y2 += pt.Y * speedFx;
                            }
                        }
                        break;
                    case ActType.Fast:
                        {
                            if (_HiderAlive)
                            {
                                double rota2 = PointRotation(new PointF(x2, y2), hiderPot);
                                double moveX2 = Math.Cos(rota2 / 180 * Math.PI) * (go.Speed / 100F);
                                double moveY2 = Math.Sin(rota2 / 180 * Math.PI) * (go.Speed / 100F);
                                if (go.JetTime > 0)
                                {
                                    go.JetTime--;
                                    if (go.JetType == 2)
                                    {
                                        _VoidObjs.Add(new VoidObject((int)x2, (int)y2, go.Size, 3, _BrushHalfBlue));
                                    }
                                }
                                else
                                {
                                    double randNum = _Rand.NextDouble() * 100;
                                    if (randNum < 0.8)
                                    {
                                        go.JetTime = go.MaxMoves;
                                        go.JetType = 2;
                                        moveX2 *= 20;
                                        moveY2 *= 20;
                                    }
                                    else if (randNum < 3)
                                    {
                                        go.JetTime = go.MaxMoves;
                                        go.JetType = 1;
                                        moveX2 *= 10;
                                        moveY2 *= 10;
                                    }
                                }
                                go.Moves.Add(new PointF((float)moveX2, (float)moveY2));
                            }
                            foreach (PointF pt in go.Moves)
                            {
                                x2 += pt.X * speedFx;
                                y2 += pt.Y * speedFx;
                            }
                        }
                        break;
                    case ActType.Smart:
                        {
                            if (_HiderAlive)
                            {
                                double rota2 = PointRotation(new PointF(x2, y2), _MousePot);
                                double moveX2 = Math.Cos(rota2 / 180 * Math.PI) * (go.Speed / 100F);
                                double moveY2 = Math.Sin(rota2 / 180 * Math.PI) * (go.Speed / 100F);
                                if (go.JetTime > 0)
                                {
                                    go.JetTime--;
                                }
                                else
                                {
                                    double randNum = _Rand.NextDouble() * 100;
                                    if (randNum < 3)
                                    {
                                        go.JetTime = go.MaxMoves;
                                        go.JetType = 1;
                                        moveX2 *= 10;
                                        moveY2 *= 10;
                                    }
                                }
                                go.Moves.Add(new PointF((float)moveX2, (float)moveY2));
                            }
                            foreach (PointF pt in go.Moves)
                            {
                                x2 += pt.X * speedFx;
                                y2 += pt.Y * speedFx;
                            }
                        }
                        break;
                    case ActType.Mine:
                    case ActType.Leave:
                        {
                            double rota2 = PointRotation(new PointF(x2, y2), go.LockPoint);
                            double moveX2 = Math.Cos(rota2 / 180 * Math.PI) * (go.Speed / 100F);
                            double moveY2 = Math.Sin(rota2 / 180 * Math.PI) * (go.Speed / 100F);
                            go.Moves.Add(new PointF((float)moveX2, (float)moveY2));

                            foreach (PointF pt in go.Moves)
                            {
                                x2 += pt.X * speedFx;
                                y2 += pt.Y * speedFx;
                            }

                            //float moveX3 = 0, moveY3 = 0;
                            //foreach (PointF pt in go.Moves)
                            //{
                            //    moveX3 += pt.X * speedFx;
                            //    moveY3 += pt.Y * speedFx;
                            //}

                            //if (Math.Sqrt(Math.Pow(x2 - go.LockPoint.X, 2) + Math.Pow(y2 - go.LockPoint.Y, 2)) <
                            //    Math.Sqrt(Math.Pow(moveX3, 2) + Math.Pow(moveY3, 2)))
                            //{
                            //    x2 = go.LockPoint.X;
                            //    y2 = go.LockPoint.Y;
                            //}
                            //else
                            //{
                            //    x2 += moveX3;
                            //    y2 += moveY3;
                            //}
                        }
                        break;
                    //case ActType.Leave:
                    //    foreach (PointF pt in go.Moves)
                    //    {
                    //        x2 += pt.X * speedFx;
                    //        y2 += pt.Y * speedFx;
                    //    }
                    //    break;
                }

                go.X = (int)x2;
                go.Y = (int)y2;

                //checkIntersects
                if (_HiderAlive && go.ActType != ActType.Leave && go.Rect.IntersectsWith(_HiderObj.Rect))
                {
                    SetEnd(go);
                    return;
                }
            }

            foreach (GameObject go in liRemoveObj)
            {
                _ActiveObjs.Remove(go);
            }
            Drawing();
        }

        private void RunningBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_Start)
            {
                SetStart(e.X, e.Y);
            }
            else
            {
                switch (e.Button)
                {
                    case System.Windows.Forms.MouseButtons.Left:
                        if (!_JetON)
                        {
                            _JetON = true;
                        }
                        break;
                    case System.Windows.Forms.MouseButtons.Right:
                        if (!_SlowON && _Energy > _LowEnergy)
                        {
                            _SlowON = true;
                        }
                        break;
                }
            }
        }

        private void RunningBox_MouseMove(object sender, MouseEventArgs e)
        {
            _MousePot = e.Location;
        }

        private void RunningBox_MouseEnter(object sender, EventArgs e)
        {
            if (_Start)
            {
                Cursor.Hide();
            }
        }

        private void RunningBox_MouseLeave(object sender, EventArgs e)
        {
            if (_Start)
            {
                Cursor.Show();
            }
        }

        public void SetStart(int potX, int potY)
        {
            _LeveL = 0;
            _HiderObj = new GameObject(ActType.Hider, ShapeType.Ellipse, potX, potY, 8, 3, 0, Brushes.Black, 0, 0, 0);
            _HiderAlive = true;
            _VoidObjs.Clear();
            _ActiveObjs.Clear();
            _Energy = _MaxEnergy;
            _Score = 0;
            _GroupPw = 0;
            _MinePw = 0;
            _DarkAni = 0;
            _RerectTick = 0;
            _SlowON = false;
            _JetON = false;
            _Start = true;
            _RectInside = new Rectangle(50, 50, Width - 100, Height - 100);

            if (_BackImage != null) _BackImage.Dispose();
            if (_BackGraphics != null) _BackGraphics.Dispose();
            if (_ControlGraphics != null) _ControlGraphics.Dispose();

            _BackImage = new Bitmap(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            _BackGraphics = Graphics.FromImage(_BackImage);
            _ControlGraphics = CreateGraphics();

            _BackGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Cursor.Hide();
        }

        private void KillObject(GameObject go)
        {
            switch (go.ActType)
            {
                case ActType.Mine:
                    go.ShapeType = ShapeType.Ellipse;
                    go.Size *= 8;
                    if (_HiderAlive && go.Rect.IntersectsWith(_HiderObj.Rect))
                    {
                        SetEnd(go);
                        return;
                    }
                    break;
            }
            go.ActType = ActType.Leave;

        }

        private void SetEnd(GameObject endBy)
        {
            _HiderAlive = false;

            if (endBy.ActType == ActType.Mine)
            {
                KillObject(endBy);
            }

            int x1 = _HiderObj.X;
            int y1 = _HiderObj.Y;
            double rota = PointRotation(new PointF(x1, y1), new PointF(_MousePot.X, _MousePot.Y));
            for (int i = 0; i < 15; i++)
            {
                int speed = _Rand.Next(300, 900);
                int live = _Rand.Next(300, 600);
                double rota2 = rota + (_Rand.NextDouble() - 0.5) * 20;
                int x2 = x1 + (int)(Math.Cos(rota2 / 180 * Math.PI) * 1000);
                int y2 = y1 + (int)(Math.Sin(rota2 / 180 * Math.PI) * 1000);
                _ActiveObjs.Add(new GameObject(ActType.Leave, ShapeType.Rectangle, x1, y1, 1, 1, speed, _HiderObj.Brush, 0, x2, y2) { FadeTime = live });
            }
            _HiderObj = null;
            _End = true;
            _EndTick = _EndTickMax;
            _DarkAni = 0;
            Cursor.Show();
        }

        public int SecToTick(float sec)
        {
            return (int)(sec * 1000 / _TimerMove.Interval);
        }

        Pen _PenRect = new Pen(Color.LightGreen, 2);
        Pen _PenHider = new Pen(Color.Black, 2);
        Stopwatch sw = new Stopwatch();
        private void Drawing()
        {
            sw.Restart();
            _BackGraphics.Clear(Color.White);
            _BackGraphics.ResetTransform();
            if (_End && _EndTick > _EndTickMax - (_EndTickMax / 3))
            {
                int shakeX = _Rand.Next(-10, 10);
                int shakeY = _Rand.Next(-10, 10);
                _BackGraphics.TranslateTransform(shakeX, shakeY, System.Drawing.Drawing2D.MatrixOrder.Append);
            }

            if (_RectInside != null)
            {
                //_BackGraphics.FillRectangle(Brushes.White, _RectInside);
                _BackGraphics.DrawRectangle(_PenRect, _RectInside);
            }

            if (_DarkAni > 0)
            {
                int nLv = (100 - Math.Abs(_DarkAni - 100)) * 3;
                if (nLv > 255) nLv = 255;
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(nLv, 0, 0, 0)))
                {
                    _BackGraphics.FillRectangle(sb, 0, 0, Width, Height);
                }
            }
            _BackGraphics.FillRectangle(_Energy < _LowEnergy ? Brushes.Red : Brushes.Black, _RectEngery.X + 2, _RectEngery.Y + 2, (_RectEngery.Width - 4) * _Energy / _MaxEnergy, _RectEngery.Height - 4);
            _BackGraphics.DrawRectangle(Pens.Black, _RectEngery);
            _BackGraphics.DrawString(_Score.ToString(), Font, Brushes.Black, _RectEngery.X + _RectEngery.Width + 10, _RectEngery.Y);
            foreach (VoidObject vo in _VoidObjs)
            {
                _BackGraphics.FillEllipse(vo.Brush, vo.Rect);
            }

            if (_Start && _HiderAlive)
            {
                switch (_HiderObj.ShapeType)
                {
                    case ShapeType.Rectangle:
                        _BackGraphics.FillRectangle(_HiderObj.Brush, _HiderObj.Rect);
                        break;
                    case ShapeType.Ellipse:
                        _BackGraphics.DrawEllipse(_PenHider, _HiderObj.Rect);
                        break;
                    case ShapeType.Image:
                        _BackGraphics.DrawImage(_HiderObj.Image, _HiderObj.Rect);
                        break;
                }
            }

            foreach (GameObject bo in _ActiveObjs)
            {
                Rectangle rtgDraw = Rectangle.Empty;
                Brush brushDraw = null;
                bool newBrush = false;
                switch (bo.ActType)
                {
                    case ActType.Leave:
                        rtgDraw = bo.Rect;

                        Color co = ((SolidBrush)bo.Brush).Color;
                        int alpha = bo.FadeTime;
                        if (alpha > 255) alpha = 255;
                        brushDraw = new SolidBrush(Color.FromArgb(alpha, co.R, co.G, co.B));
                        newBrush = true;
                        break;
                    case ActType.Mine:
                        if (bo.LifeTime < 50)
                        {
                            int sizeFix = bo.LifeTime / 2 % 5;
                            //rtgDraw = new Rectangle(bo.Rect.Left + _Rand.Next(0, 6) - 3, bo.Rect.Top + _Rand.Next(0, 6) - 3, bo.Rect.Width, bo.Rect.Height);
                            rtgDraw = new Rectangle(bo.Rect.Left - sizeFix, bo.Rect.Top - sizeFix, bo.Rect.Width + sizeFix * 2, bo.Rect.Height + sizeFix * 2);
                            brushDraw = bo.Brush;
                        }
                        else
                        {
                            goto default;
                        }
                        break;
                    default:
                        rtgDraw = bo.Rect;
                        brushDraw = bo.Brush;
                        break;
                }

                switch (bo.ShapeType)
                {
                    case ShapeType.Rectangle:
                        _BackGraphics.FillRectangle(brushDraw, rtgDraw);
                        break;
                    case ShapeType.Ellipse:
                        _BackGraphics.FillEllipse(brushDraw, rtgDraw);
                        break;
                    case ShapeType.Image:
                        _BackGraphics.DrawImage(bo.Image, rtgDraw);
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
                // FindForm().Text = ((int)(1F / sw.Elapsed.Ticks * TimeSpan.TicksPerMillisecond * 1000)).ToString();
                _FPSTic = 10;
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

        private enum ActType
        {
            Hider = 0,
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
            public List<PointF> Moves { get; set; }
            public ActType ActType { get; set; }
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


            public GameObject(ActType actType, ShapeType shapeType, int x, int y, int maxMoves, int size, float speed, Brush brush, int lifeTime, int lockPotX, int lockPotY)
            {
                ActType = actType;
                ShapeType = shapeType;
                MaxMoves = maxMoves;
                X = x;
                Y = y;
                LifeTime = lifeTime;
                Size = size;
                Speed = speed;
                Moves = new List<PointF>();
                Brush = brush;
                LockPoint = new PointF(lockPotX, lockPotY);
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


            public VoidObject(int x, int y, int size, int maxFadeTick, Brush brush)
            {
                X = x;
                Y = y;
                Size = size;
                MaxFadeTick = maxFadeTick;
                Brush = brush;
            }
        }
    }
}
