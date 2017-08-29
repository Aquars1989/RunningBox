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
    public partial class SceneStand : SceneGaming
    {
        private float _SpeedFix = 1;
        private float _LifeFix = 1;

        public SceneStand()
        {
            InitializeComponent();

            //場景:畫面變黑暗
            WaveEvents.Add("@Dark", (n) =>
                {
                    EffectObjects.Add(new EffectDyeing(Color.Black, Sec(Math.Max(n - 1, 0)), Sec(0.5F), Sec(0.5F)));
                });

            //場景:邊界縮小
            WaveEvents.Add("@Shrink", (n) =>
            {
                double scaleX = Global.Rand.NextDouble();
                double scaleY = 1 - scaleX;

                int limitX = (int)(scaleX * MainRectangle.Width * 0.4F);
                int limitY = (int)(scaleY * MainRectangle.Height * 0.4F);
                int limitLeft = Global.Rand.Next(0, limitX);
                int limitTop = Global.Rand.Next(0, limitY);
                int limitRight = limitX - limitLeft;
                int limitDown = limitY - limitTop;

                Padding shrinkPerRound = new Padding(limitLeft, limitTop, limitRight, limitDown);
                EffectObjects.Add(new EffectShrink(shrinkPerRound, Sec(Math.Max(n - 1, 0)), Sec(0.5F), Sec(0.5F)));
            });

            //物件:追捕者
            WaveEvents.Add("Catcher", (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(7, 13);
                    int movesCount = Global.Rand.Next(15, 30);
                    float speed = (Global.Rand.Next(500, 650) - (size * 30)) * _SpeedFix;
                    float speedPerMove = speed / movesCount;
                    int life = Sec(3.5F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, size, speedPerMove, life, League.Ememy, ShapeType.Ellipse, new DrawBrush(Color.Red, ShapeType.Ellipse), new TargetObject(PlayerObject));
                    newObject.Skills.Add(new SkillSprint(0, Sec(1.5F), 15, 0, true) { AutoCastObject = new AutoCastNormal(0.4F) });
                    newObject.Skills.Add(new SkillSprint(0, Sec(0.5F), 5, 0, false) { AutoCastObject = new AutoCastNormal(3F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 200, 600, Sec(0.2F), Sec(0.5F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, null));
                    GameObjects.Add(newObject);
                }
            });

            //物件:快速追捕者 速度更快 更常使用加速
            WaveEvents.Add("Faster", (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(6, 9);
                    int movesCount = Global.Rand.Next(8, 12);
                    float speed = (Global.Rand.Next(600, 700) - (size * 30)) * _SpeedFix;
                    float speedPerMove = speed / movesCount;
                    int life = Sec(4.5F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, size, speedPerMove, life, League.Ememy, ShapeType.Ellipse, new DrawBrush(Color.Blue, ShapeType.Ellipse), new TargetObject(PlayerObject));
                    newObject.Skills.Add(new SkillSprint(0, Sec(1F), 10, 0, true) { AutoCastObject = new AutoCastNormal(1F) });
                    newObject.Skills.Add(new SkillSprint(0, Sec(0.5F), 5, 0, false) { AutoCastObject = new AutoCastNormal(3F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 200, 600, Sec(0.2F), Sec(0.5F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, null));
                    GameObjects.Add(newObject);
                }
            });

            //物件:包圍者 與普通一致但不會大幅加速
            WaveEvents.Add("Group", (n) =>
            {
                int roundIdx = Global.Rand.Next(4);
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(7, 13);
                    int movesCount = Global.Rand.Next(15, 30);
                    float speed = (Global.Rand.Next(500, 600) - (size * 30)) * _SpeedFix;
                    float speedPerMove = speed / movesCount;
                    int life = Sec(6F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint((EnumDirection)roundIdx);

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, size, speedPerMove, life, League.Ememy, ShapeType.Ellipse, new DrawBrush(Color.Red, ShapeType.Ellipse), new TargetObject(PlayerObject));
                    newObject.Skills.Add(new SkillSprint(0, 50, 5, 0, false) { AutoCastObject = new AutoCastNormal(2.5F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 200, 600, Sec(0.2F), Sec(0.5F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, null));
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });

            //物件:攔截者 不會加速 但速度會越來越快
            WaveEvents.Add("Blocker", (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(14, 17);
                    int movesCount = 2;
                    float speed = 100 * _SpeedFix;
                    float speedPerMove = speed / movesCount;
                    int life = Sec(6F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, size, speedPerMove, life, League.Ememy, ShapeType.Ellipse, new DrawPolygon(Color.Fuchsia, Color.Fuchsia, 3, 1, 0, 3), new TargetTrackPoint(this));
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 200, 600, Sec(0.2F), Sec(0.5F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(10, null));
                    newObject.Propertys.Add(new PropertySpeeded(-1, 40));
                    GameObjects.Add(newObject);
                }
            });

            //物件:地雷
            WaveEvents.Add("Mine", (n) =>
            {
                int roundIdx = Global.Rand.Next(4);
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(16, 20);
                    int movesCount = Global.Rand.Next(10, 15);
                    float speed = Global.Rand.Next(200, 300);
                    float speedPerMove = speed / movesCount;
                    int life = Sec(4);

                    Point enterPoint = Point.Empty;
                    int targetX = 0, targetY = 0;
                    //int padding = 20;
                    switch (roundIdx)
                    {
                        case 0:
                            enterPoint = GetEnterPoint(EnumDirection.Left);
                            targetX = MainRectangle.Left + Global.Rand.Next(MainRectangle.Width / 2);
                            targetY = MainRectangle.Top + Global.Rand.Next(MainRectangle.Height);
                            break;
                        case 1:
                            enterPoint = GetEnterPoint(EnumDirection.Right);
                            targetX = MainRectangle.Left + MainRectangle.Width / 2 + Global.Rand.Next(MainRectangle.Width / 2);
                            targetY = MainRectangle.Top + Global.Rand.Next(MainRectangle.Height);
                            break;
                        case 2:
                            enterPoint = GetEnterPoint(EnumDirection.Top);
                            targetX = MainRectangle.Left + Global.Rand.Next(MainRectangle.Width);
                            targetY = MainRectangle.Top + Global.Rand.Next(MainRectangle.Height / 2);
                            break;
                        case 3:
                            enterPoint = GetEnterPoint(EnumDirection.Bottom);
                            targetX = MainRectangle.Left + Global.Rand.Next(MainRectangle.Width);
                            targetY = MainRectangle.Top + MainRectangle.Height / 2 + Global.Rand.Next(MainRectangle.Height / 2);
                            break;
                    }

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, size, speedPerMove, life, League.Ememy, ShapeType.Ellipse, new DrawImage(Color.Black, Properties.Resources.Mine), new TargetPoint(targetX, targetY));
                    newObject.Propertys.Add(new PropertyDeadExplosion(10, 0, 1, League.None, Color.Firebrick, 0.15F, 0.1F, ObjectDeadType.Collision | ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, null));
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });
        }

        public override void SetWave()
        {
            //                                 12345678901234567890123456789012345678901234567890
            //Waves.Add(new WaveLine("Catcher", "111 11111 1111 1111  111 1111 1111 1111   11 11111  "));
            //Waves.Add(new WaveLine("Faster ", "   1    1    1    11   1    1    1    111  1        "));
            //Waves.Add(new WaveLine("Blocker", "1            1             1             11         "));
            Waves.Add(new WaveLine("Group  ", "     4           5           6         7       8    "));
            Waves.Add(new WaveLine("Mine   ", " 8        4            5           6        7       "));
            //Waves.Add(new WaveLine("@Dark  ", "              +++               +++                 "));
            Waves.Add(new WaveLine("@Shrink", "33   +++     +++     +++     +++     +++     +++    "));
        }

        public override ObjectActive CreatePlayerObject(int potX, int potY)
        {
            ObjectPlayer PlayerObject = new ObjectPlayer(potX, potY, 8, 7, 7, 170, League.Player, new DrawPen(Color.Black, ShapeType.Ellipse, 2), new TargetTrackPoint(this));
            //SkillSprint skill1 = new SkillSprint(3500, Sec(1), 0, 1000, true);
            SkillShield skill1 = new SkillShield(1, 6000, 0, Sec(1F), Sec(3));
            SkillBulletTime skill2 = new SkillBulletTime(1000, 8000, -1, Sec(5), 1);
            PlayerObject.Skills.Add(skill1);
            PlayerObject.Skills.Add(skill2);
            PlayerObject.Propertys.Add(new PropertyCollision(1, null));
            PlayerObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 200, 600, Sec(0.2F), Sec(0.5F)));
            return PlayerObject;
        }

        public override void DoAfterStart()
        {
            _SpeedFix = 1;
            _LifeFix = 1;
        }

        public override void DoAfterWave()
        {
            _SpeedFix = 1F + Wave * 0.015F;
            _LifeFix = 1F + Wave * 0.01F;
        }

        private void RunningBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsStart)
            {
                switch (e.Button)
                {
                    case System.Windows.Forms.MouseButtons.Left:
                        UsePlayerSkill1();
                        break;
                    case System.Windows.Forms.MouseButtons.Right:
                        UsePlayerSkill2();
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

        public override void DoAfterEnd()
        {
        }
    }
}
