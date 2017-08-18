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
                    int size = Global.Rand.Next(3, 6);
                    int movesCount = Global.Rand.Next(15, 30);
                    float speed = (Global.Rand.Next(500, 650) - (size * 50)) * _SpeedFix;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(3.5F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawBrush(Color.Red, DrawShape.Ellipse), new TargetObject(PlayerObject));
                    newObject.Skills.Add(new SkillSprint(0, 120, 10, 0, true) { AutoCastObject = new AutoCastNormal(0.4F) });
                    newObject.Skills.Add(new SkillSprint(0, 50, 5, 0, false) { AutoCastObject = new AutoCastNormal(3F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    GameObjects.Add(newObject);
                }
            });

            //物件:快速追捕者 速度更快 更常使用加速
            WaveEvents.Add("Faster", (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(3, 4);
                    int movesCount = Global.Rand.Next(8, 15);
                    float speed = (Global.Rand.Next(600, 750) - (size * 50)) * _SpeedFix;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(4.5F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawBrush(Color.Blue, DrawShape.Ellipse), new TargetObject(PlayerObject));
                    newObject.Skills.Add(new SkillSprint(0, 70, 8, 0, true) { AutoCastObject = new AutoCastNormal(1F) });
                    newObject.Skills.Add(new SkillSprint(0, 40, 4, 0, false) { AutoCastObject = new AutoCastNormal(3F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    GameObjects.Add(newObject);
                }
            });

            //物件:包圍者 與普通一致但不會大幅加速
            WaveEvents.Add("Group", (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(3, 6);
                    int movesCount = Global.Rand.Next(15, 30);
                    float speed = (Global.Rand.Next(350, 450) - (size * 50)) * _SpeedFix;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(6 * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawBrush(Color.Red, DrawShape.Ellipse), new TargetObject(PlayerObject));
                    newObject.Skills.Add(new SkillSprint(0, 50, 5, 0, false) { AutoCastObject = new AutoCastNormal(2.5F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    GameObjects.Add(newObject);
                }
            });

            //物件:攔截者 不會加速 但速度會越來越快
            WaveEvents.Add("Blocker", (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(6, 8);
                    int movesCount = 2;
                    float speed = 100 * _SpeedFix;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(6F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawBrush(Color.Fuchsia, DrawShape.Ellipse), new TargetTrackPoint(this));
                    newObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    newObject.Propertys.Add(new PropertySpeeded(-1, 1));
                    GameObjects.Add(newObject);
                }
            });

            //物件:地雷
            WaveEvents.Add("Mine", (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(8, 10);
                    int movesCount = Global.Rand.Next(10, 15);
                    float speed = Global.Rand.Next(850, 1000) - (size * 50);
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(4);

                    Point enterPoint = Point.Empty;
                    int targetX = 0, targetY = 0;
                    int padding = 20;
                    switch (Global.Rand.Next(1, 4))
                    {
                        case 1:
                            enterPoint = GetEnterPoint(EnumDirection.Left);
                            targetX = Global.Rand.Next(MainRectangle.Left + padding, MainRectangle.Left + MainRectangle.Width / 2);
                            targetY = Global.Rand.Next(MainRectangle.Top + padding, MainRectangle.Top + MainRectangle.Height - padding);
                            break;
                        case 2:
                            enterPoint = GetEnterPoint(EnumDirection.Right);
                            targetX = Global.Rand.Next(MainRectangle.Left + MainRectangle.Width / 2, MainRectangle.Left + MainRectangle.Width - padding);
                            targetY = Global.Rand.Next(MainRectangle.Top + padding, MainRectangle.Top + MainRectangle.Height - padding);
                            break;
                        case 3:
                            enterPoint = GetEnterPoint(EnumDirection.Top);
                            targetX = Global.Rand.Next(MainRectangle.Left + padding, MainRectangle.Left + MainRectangle.Width - padding);
                            targetY = Global.Rand.Next(MainRectangle.Top + padding, MainRectangle.Top + MainRectangle.Height / 2);
                            break;
                        case 4:
                            enterPoint = GetEnterPoint(EnumDirection.Bottom);
                            targetX = Global.Rand.Next(MainRectangle.Left + padding, MainRectangle.Left + MainRectangle.Width - padding);
                            targetY = Global.Rand.Next(MainRectangle.Top + MainRectangle.Height / 2, MainRectangle.Height - padding);
                            break;
                    }

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawImage(Color.Black, Properties.Resources.Mine), new TargetPoint(targetX, targetY));
                    newObject.Propertys.Add(new PropertyDeadExplosion(10, 0, 1, League.None, Color.Firebrick, 0.15F, 0.1F, ObjectDeadType.Collision | ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    GameObjects.Add(newObject);
                }
            });
        }

        public override void SetWave()
        {
            //                                 12345678901234567890123456789012345678901234567890
            Waves.Add(new WaveLine("Catcher", "111 11111 1111 1111  111 1111 1111 1111   11 11111  "));
            //Waves.Add(new WaveLine("Faster ", "   1    1    1    11   1    1    1    111  1        "));
            //Waves.Add(new WaveLine("Blocker", "             1             1             11         "));
            //Waves.Add(new WaveLine("Group  ", "     4           5           6         7       8    "));
            //Waves.Add(new WaveLine("Mine   ", " 3        4            5           6        7       "));
            //Waves.Add(new WaveLine("@Dark  ", "              +++               +++                 "));
            Waves.Add(new WaveLine("@Shrink", "33   +++     +++     +++     +++     +++     +++    "));
        }

        public override ObjectActive CreatePlayerObject(int potX, int potY)
        {
            ObjectPlayer PlayerObject = new ObjectPlayer(potX, potY, 8, 3, 150, League.Player, new DrawPen(Color.Black, DrawShape.Ellipse, 2), new TargetTrackPoint(this));
            SkillSprint skill1 = new SkillSprint(350, SecToRounds(1), 0, 1000, true);
            SkillBulletTime skill2 = new SkillBulletTime(200, 5, SecToRounds(3), SecToRounds(5), 1);
            PlayerObject.Skills.Add(skill1);
            PlayerObject.Skills.Add(skill2);
            PlayerObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
            //PlayerObject.Propertys.Add(new PropertyCollision(1, null));
            return PlayerObject;
        }

        public override void DoAfterStart()
        {
            _SpeedFix = 1;
            _LifeFix = 1;
        }

        public override void DoAfterWave()
        {
            _SpeedFix = 1F + Level * 0.015F;
            _LifeFix = 1F + Level * 0.01F;
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
