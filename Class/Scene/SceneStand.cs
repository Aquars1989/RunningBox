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
        private int _LevelOfShrink = 8;

        private int _GroupCount = 0;
        private int _MineCount = 0;
        private int _MineExpand = 8;

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
                int limitX = (int)(scaleX * GameRectangle.Width / 3 / shrinkRounds);
                int limitY = (int)(scaleY * GameRectangle.Height / 3 / shrinkRounds);

                int limitLeft = Global.Rand.Next(0, limitX);
                int limitTop = Global.Rand.Next(0, limitY);
                int limitRight = limitX - limitLeft;
                int limitDown = limitY - limitTop;

                Padding shrinkPerRound = new Padding(limitLeft, limitTop, limitRight, limitDown);
                EffectObjects.Add(new EffectShrink(shrinkPerRound, 100, 20));
            }

            if (Level % _LeveLOfMine == 0)
            {
                for (int i = 0; i < _MineCount; i++)
                {
                    int size = Global.Rand.Next(8, 10);
                    int movesCount = Global.Rand.Next(10, 15);
                    float speed = Global.Rand.Next(850, 1000) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(4);// +_Rand.Next(0, 5);

                    Point enterPoint = Point.Empty;
                    int targetX = 0, targetY = 0;
                    int padding = 20;
                    switch (Global.Rand.Next(1, 4))
                    {
                        case 1:
                            enterPoint = GetEnterPoint(EnumDirection.Left);
                            targetX = Global.Rand.Next(GameRectangle.Left + padding, GameRectangle.Left + GameRectangle.Width / 2);
                            targetY = Global.Rand.Next(GameRectangle.Top + padding, GameRectangle.Top + GameRectangle.Height - padding);
                            break;
                        case 2:
                            enterPoint = GetEnterPoint(EnumDirection.Right);
                            targetX = Global.Rand.Next(GameRectangle.Left + GameRectangle.Width / 2, GameRectangle.Left + GameRectangle.Width - 20);
                            targetY = Global.Rand.Next(GameRectangle.Top + padding, GameRectangle.Top + GameRectangle.Height - padding);
                            break;
                        case 3:
                            enterPoint = GetEnterPoint(EnumDirection.Top);
                            targetX = Global.Rand.Next(GameRectangle.Left + padding, GameRectangle.Left + GameRectangle.Width - padding);
                            targetY = Global.Rand.Next(GameRectangle.Top + padding, GameRectangle.Top + GameRectangle.Height / 2);
                            break;
                        case 4:
                            enterPoint = GetEnterPoint(EnumDirection.Bottom);
                            targetX = Global.Rand.Next(GameRectangle.Left + padding, GameRectangle.Left + GameRectangle.Width - padding);
                            targetY = Global.Rand.Next(GameRectangle.Top + GameRectangle.Height / 2, GameRectangle.Height - padding);
                            break;
                    }

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawImage(Color.Black, Properties.Resources.Mine), new TargetPoint(targetX, targetY));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    GameObjects.Add(newObject);
                }
                _MineCount++;
            }

            if (Level % _LevelOfGroup == 0)
            {
                for (int i = 0; i < _GroupCount; i++)
                {
                    int size = Global.Rand.Next(3, 6);
                    int movesCount = Global.Rand.Next(15, 30);
                    float speed = Global.Rand.Next(600, 700) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(6F * lifeLevel) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawBrush(Color.Red, DrawShape.Ellipse), new TargetObject(PlayerObject));
                    newObject.Skills.Add(new SkillSprint(0, 50, 5, false) { AutoCastObject = new AutoCastNormal(2.5F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    GameObjects.Add(newObject);
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

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawBrush(Color.Fuchsia, DrawShape.Ellipse), new TargetTrackPoint(this));
                    newObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    GameObjects.Add(newObject);
                }
                else if (Level % _LevelOfCatcherFaster == 0)
                {
                    int size = Global.Rand.Next(3, 4);
                    int movesCount = Global.Rand.Next(8, 15);
                    float speed = Global.Rand.Next(800, 900) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(4.5F * lifeLevel);

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawBrush(Color.Blue, DrawShape.Ellipse), new TargetObject(PlayerObject));
                    newObject.Skills.Add(new SkillSprint(0, 70, 10, true) { AutoCastObject = new AutoCastNormal(1F) });
                    newObject.Skills.Add(new SkillSprint(0, 40, 5, false) { AutoCastObject = new AutoCastNormal(3F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    GameObjects.Add(newObject);
                }
                else
                {
                    int size = Global.Rand.Next(3, 6);
                    int movesCount = Global.Rand.Next(15, 30);
                    float speed = Global.Rand.Next(700, 850) - (size * 50) * speedLevel;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(3.5F * lifeLevel);

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawBrush(Color.Red, DrawShape.Ellipse), new TargetObject(PlayerObject));
                    newObject.Skills.Add(new SkillSprint(0, 120, 15, true) { AutoCastObject = new AutoCastNormal(0.4F) });
                    newObject.Skills.Add(new SkillSprint(0, 50, 5, false) { AutoCastObject = new AutoCastNormal(2.5F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    GameObjects.Add(newObject);
                }
            }
        }

        public override void DoAfterStart()
        {
            _MineCount = _DefaultMineCount;
            _GroupCount = _DefaultGroupCount;
            base.DoAfterStart();
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
