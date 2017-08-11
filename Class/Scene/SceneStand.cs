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
    public partial class SceneStand : SceneBase
    {
        private const int _DefaultGroupCount = 6;
        private const int _DefaultMineCount = 4;


        private const int _EnergyForSprint = 350;
        private const int _EnergyForSlow = 350;
        private const int _EnergyForSlowPerRound = 15;

        private Timer _TimerOfBuild = new Timer() { Enabled = true, Interval = 1000 };

        private int _LevelOfDark = 22;
        private int _LevelOfGroup = 15;
        private int _LevelOfInterceptor = 12;
        private int _LeveLOfMine = 8;
        private int _LevelOfCatcherFaster = 3;
        private int _LevelOfShrink = 6;

        private int _GroupCount = 0;
        private int _MineCount = 0;
        private int _MineExpand = 8;

        private Graphics _ControlGraphics;
        private Graphics _BackGraphics;
        private Bitmap _BackImage;

        private int _TickOfLimited = 0;
        private Padding _LimitedPerRound = new Padding();

        public SceneStand()
        {
            InitializeComponent();

            _TimerOfBuild.Tick += TimerOfBuild_Tick;
        }

        private void TimerOfBuild_Tick(object sender, EventArgs e)
        {
            if (!IsStart) return;

            Level++;

            float lifeLevel = (100F + Level) / 100F;
            float speedLevel = (70F + Level) / 100F;

            if (Level % _LevelOfDark == 0)
            {
                EffectObjects.Add(new EffectDyeing(Color.Black, 200, 20, 20));
            }

            if (Level % _LevelOfShrink == 0)
            {
                double scaleX = Global.Rand.NextDouble();
                double scaleY = 1 - scaleX;

                int shrinkRounds = 20;
                int limitX = (int)(scaleX * GameRectangle.Width / 2 / shrinkRounds);
                int limitY = (int)(scaleY * GameRectangle.Height / 2 / shrinkRounds);

                int limitLeft = Global.Rand.Next(0, limitX);
                int limitTop = Global.Rand.Next(0, limitY);
                int limitRight = limitX - limitLeft;
                int limitDown = limitY - limitTop;

                Padding shrinkPerRound = new Padding(limitLeft, limitTop, limitRight, limitDown);
                EffectObjects.Add(new EffectShrink(shrinkPerRound, 100, 20));
            }

            //if (_LeveL % _LeveLOfMine == 0)
            //{
            //    for (int i = 0; i < _MineCount; i++)
            //    {
            //        int size = _Rand.Next(8, 10);
            //        int movesCount = _Rand.Next(10, 15);
            //        float speed = _Rand.Next(850, 1000) - (size * 50) * speedLevel;
            //        float speedPerMove = speed / movesCount;
            //        int life = SecToRound(_LeveLOfMine * 0.5F);// +_Rand.Next(0, 5);
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
                    int size = Global.Rand.Next(3, 6);
                    int movesCount = Global.Rand.Next(15, 30);
                    float speed = Global.Rand.Next(700, 850) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(6F * lifeLevel) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();
                    ObjectActive newObject = new ObjectActive()
                    {
                        X = enterPoint.X,
                        Y = enterPoint.Y,
                        MaxMoves = movesCount,
                        Size = size,
                        Speed = speedPerMove,
                        LifeRoundMax = life,
                        Target = new TargetObject(PlayerObject),
                        DrawObject = new DrawBrush(Color.Red, DrawShape.Ellipse),
                        League = League.Ememy
                    };
                }
                _GroupCount++;
            }
            else
            {
                Point enterPoint = GetEnterPoint();
                if (Level % _LevelOfInterceptor == 0)
                {
                    int size = Global.Rand.Next(6, 8);
                    int movesCount = Global.Rand.Next(6, 10);
                    float speed = Global.Rand.Next(250, 300) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(7F * lifeLevel);
                    //GameObjects.Add(new ObjectCatcherInterceptor(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, Color.Fuchsia));
                }
                else if (Level % _LevelOfCatcherFaster == 0)
                {
                    int size = Global.Rand.Next(3, 4);
                    int movesCount = Global.Rand.Next(8, 15);
                    float speed = Global.Rand.Next(800, 900) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(4.5F * lifeLevel);
                    //GameObjects.Add(new ObjectCatcherFaster(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, Color.Blue, new TargetObject(PlayerObject)));
                }
                else
                {
                    int size = Global.Rand.Next(3, 6);
                    int movesCount = Global.Rand.Next(15, 30);
                    float speed = Global.Rand.Next(700, 850) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(3.5F * lifeLevel);

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new TargetObject(PlayerObject))
                    {
                        League = League.Ememy,
                        DrawObject = new DrawBrush(Color.Red, DrawShape.Ellipse),
                        EnergyMax = 1000,
                        Energy = 1000,
                        EnergyGetPerRound = 10,
                    };
                    newObject.Skills.Add(new SkillSprint(0, 100, 10, true) { AutoCastObject = new AutoCastNormal(1F) });
                    newObject.Skills.Add(new SkillSprint(0, 50, 5, false) { AutoCastObject = new AutoCastNormal(3F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));

                    GameObjects.Add(newObject);
                }
            }
        }


        private void RunningBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsStart)
            {
                switch (e.Button)
                {
                    case System.Windows.Forms.MouseButtons.Left:
                        UsePlayerSkill(0);
                        break;
                    case System.Windows.Forms.MouseButtons.Right:
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
            if (IsStart)
            {
                Cursor.Hide();
            }
        }

        private void RunningBox_MouseLeave(object sender, EventArgs e)
        {
            if (IsStart)
            {
                Cursor.Show();
            }
        }
    }
}
