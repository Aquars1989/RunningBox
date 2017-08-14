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
        private float _SpeedFix = 1;
        private float _LifeFix = 1;

        public SceneStand()
        {
            InitializeComponent();

            //場景:畫面變黑暗
            WaveEvents.Add("@Dark", (n) =>
                {
                    EffectObjects.Add(new EffectDyeing(Color.Black, WaveToRounds(n), 20, 20));
                });

            //場景:邊界縮小
            WaveEvents.Add("@Shrink", (n) =>
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
                EffectObjects.Add(new EffectShrink(shrinkPerRound, WaveToRounds(n), shrinkRounds));
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
                    newObject.Skills.Add(new SkillSprint(0, 120, 15, true) { AutoCastObject = new AutoCastNormal(0.4F) });
                    newObject.Skills.Add(new SkillSprint(0, 50, 5, false) { AutoCastObject = new AutoCastNormal(2.5F) });
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
                    newObject.Skills.Add(new SkillSprint(0, 70, 10, true) { AutoCastObject = new AutoCastNormal(1F) });
                    newObject.Skills.Add(new SkillSprint(0, 40, 5, false) { AutoCastObject = new AutoCastNormal(3F) });
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
                    float speed = (Global.Rand.Next(450, 550) - (size * 50)) * _SpeedFix;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(6 * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawBrush(Color.Red, DrawShape.Ellipse), new TargetObject(PlayerObject));
                    newObject.Skills.Add(new SkillSprint(0, 50, 5, false) { AutoCastObject = new AutoCastNormal(2.5F) });
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
                    int movesCount = Global.Rand.Next(6, 10);
                    float speed = Global.Rand.Next(200, 250) * _SpeedFix;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(7F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, movesCount, size, speedPerMove, life, League.Ememy, new DrawBrush(Color.Fuchsia, DrawShape.Ellipse), new TargetTrackPoint(this));
                    newObject.Propertys.Add(new PropertyDeadBroken(15, ObjectDeadType.Collision));
                    newObject.Propertys.Add(new PropertyDeadCollapse(10, 4, ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    newObject.Propertys.Add(new PropertySpeeded(-1, 5));
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
                    float speed = (Global.Rand.Next(850, 1000) - (size * 50)) * _SpeedFix;
                    float speedPerMove = speed / movesCount;
                    int life = SecToRounds(4);

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
                            targetX = Global.Rand.Next(GameRectangle.Left + GameRectangle.Width / 2, GameRectangle.Left + GameRectangle.Width - padding);
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
                    newObject.Propertys.Add(new PropertyDeadExplosion(10, 0, Color.Firebrick, ObjectDeadType.Collision | ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1, new TargetObject(PlayerObject)));
                    GameObjects.Add(newObject);
                }
            });
        }

        public override void SetWave()
        {
            //                                 12345678901234567890123456789012345678901234567890
            Waves.Add(new WaveLine("Catcher", "111 11111 1111 1111  111 1111 1111 1111   11 11111  "));
            Waves.Add(new WaveLine("Faster ", "   1    1    1    11   1    1    1    111  1        "));
            Waves.Add(new WaveLine("Blocker", "             1             1             11         "));
            Waves.Add(new WaveLine("Group  ", "     4           5           6         7       8    "));
            Waves.Add(new WaveLine("Mine   ", "3        4            5           6        7       "));
            Waves.Add(new WaveLine("@Dark  ", "              +++               +++                 "));
            Waves.Add(new WaveLine("@Shrink", "     +++     +++     +++     +++     +++     +++    "));
        }

        public override void DoAfterStart()
        {
            _SpeedFix = 1;
            _LifeFix = 1;
            base.DoAfterStart();
        }

        public override void DoAfterWave()
        {
            _SpeedFix = 1F + Level * 0.05F;
            _LifeFix = 1F + Level * 0.05F;
            base.DoAfterWave();
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
                        UsePlayerSkill(1);
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
