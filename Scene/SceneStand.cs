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
                    EffectObjects.Add(new EffectDyeing(Color.Black, Wave(Math.Max(n - 1, 0)), Wave(0.5F), Wave(0.5F)));
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
                EffectObjects.Add(new EffectShrink(shrinkPerRound, Wave(Math.Max(n - 1, 0)), Wave(0.5F), Wave(0.5F)));
            });

            //物件:追捕者
            WaveEvents.Add("Catcher", (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(7, 12);
                    int offsetLimit = size + Global.Rand.Next(5, 10);
                    float speed = Global.Rand.Next(320, 380) * _SpeedFix;
                    float weight = 0.3F + size * 0.1F;
                    int life = Sec(3.5F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();
                    MoveStraight moveObject = new MoveStraight(new TargetObject(PlayerObject), weight, speed, offsetLimit, 100, 0.5F);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy, ShapeType.Ellipse, new DrawBrush(Color.Red, ShapeType.Ellipse), moveObject);
                    newObject.Skills.Add(new SkillSprint(0, Sec(1.5F), 15, 0, true) { AutoCastObject = new AutoCastNormal(0.4F) });
                    newObject.Skills.Add(new SkillSprint(0, Sec(0.5F), 5, 0, false) { AutoCastObject = new AutoCastNormal(3F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 200, 600, Sec(0.2F), Sec(0.5F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    GameObjects.Add(newObject);
                }
            });

            //物件:快速追捕者 速度更快 更常使用加速
            WaveEvents.Add("Faster", (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(6, 8);
                    int offsetLimit = size + Global.Rand.Next(2, 4);
                    float speed = Global.Rand.Next(400, 460) * _SpeedFix;
                    float weight = 0.3F + size * 0.1F;
                    int life = Sec(4.5F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    MoveStraight moveObject = new MoveStraight(new TargetObject(PlayerObject), weight, speed, offsetLimit, 100, 0.5F);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy, ShapeType.Ellipse, new DrawBrush(Color.Blue, ShapeType.Ellipse), moveObject);
                    newObject.Skills.Add(new SkillSprint(0, Sec(1F), 8, 0, true) { AutoCastObject = new AutoCastNormal(1F) });
                    newObject.Skills.Add(new SkillSprint(0, Sec(0.5F), 4, 0, false) { AutoCastObject = new AutoCastNormal(3F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 200, 600, Sec(0.2F), Sec(0.5F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    GameObjects.Add(newObject);
                }
            });

            //物件:包圍 四面八方的直線前進物件
            WaveEvents.Add("Meteor", (n) =>
            {
                int roundIdx = Global.Rand.Next(4);
                for (int i = 0; i < n; i++)
                {
                    int size = 10;
                    int movesCount = 6;
                    float speed = 500 * _SpeedFix;
                    float weight = 0.3F + size * 0.1F;
                    int life = Sec(5F + 0.2F * i);
                    Point enterPoint = GetEnterPoint((DirectionType)roundIdx);

                    double angel = Function.GetAngle(enterPoint.X, enterPoint.Y, PlayerObject.Layout.CenterX, PlayerObject.Layout.CenterY) + Global.Rand.Next(-20, 20);
                    TargetOffset targetObject = new TargetOffset(TargetNull.Value, angel, 1000F);
                    MoveStraight moveObject = new MoveStraight(targetObject, weight, speed, movesCount, 0, 1F);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy, ShapeType.Ellipse, new DrawBrush(Color.Orchid, ShapeType.Ellipse), moveObject);
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 200, 600, Sec(0.2F), Sec(0.5F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertySmoking(-1, Sec(0.2F)));
                    newObject.Propertys.Add(new PropertyFreeze(Sec(0.2F * i)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyOutClear());
                    targetObject.Target = new TargetObject(newObject);
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });

            //物件:水平牆壁
            WaveEvents.Add("WallA", (n) =>
            {
                List<ObjectActive> objects = new List<ObjectActive>();
                for (int i = 0; i < MainRectangle.Height + 60; i += 30)
                {
                    int movesCount = 6;
                    float speed = 700 * _SpeedFix;
                    float weight = 3;
                    int life = Sec(5F);
                    TargetOffset targetObject = new TargetOffset(TargetNull.Value, 0, 1000F);
                    MoveStraight moveObject = new MoveStraight(targetObject, weight, speed, movesCount, 0, 1F);
                    ObjectActive newObject = new ObjectActive(-50, MainRectangle.Top + i - 20, 5, 28, life, LeagueType.Ememy, ShapeType.Rectangle, new DrawBrush(Color.Orchid, ShapeType.Rectangle), moveObject);
                    newObject.Propertys.Add(new PropertyDeadBroken(new DrawBrush(Color.Orchid, ShapeType.Rectangle), 15, 6, 6, ObjectDeadType.Collision | ObjectDeadType.LifeEnd, 20, 200, 600, Sec(0.8F), Sec(1.4F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyOutClear());
                    targetObject.Target = new TargetObject(newObject);
                    GameObjects.Add(newObject);
                    objects.Add(newObject);
                }

                int clearCount = 2;
                int clearIndex = Global.Rand.Next(1, objects.Count - 2 - clearCount + 1);
                for (int i = 0; i < clearCount; i++)
                {
                    objects[clearIndex + i].Kill(null, ObjectDeadType.Clear);
                }

            });

            //物件:序列 排列成直線的追捕者
            WaveEvents.Add("Series", (n) =>
            {
                Point enterPoint = GetEnterPoint();
                int size = Global.Rand.Next(10, 12);
                int movesCount = 6;
                float speed = Global.Rand.Next(300, 400) * _SpeedFix;
                float weight = 0.3F + size * 0.1F;
                int life = Sec(6F * _LifeFix) + Global.Rand.Next(0, 5);

                ObjectActive target = PlayerObject;
                ObjectActive lastObject = null;
                for (int i = 0; i < n; i++)
                {
                    MoveStraight moveObject = new MoveStraight(new TargetObject(target), weight, speed, movesCount, 30, 1F);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy, ShapeType.Ellipse, new DrawBrush(Color.DarkOrange, ShapeType.Ellipse), moveObject);
                    newObject.Propertys.Add(new PropertyFreeze(Sec(i * 0.05F)));
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 200, 600, Sec(0.2F), Sec(0.5F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    GameObjects.Add(newObject);
                    target = newObject;
                    speed += 10;
                    life -= 50;
                    if (lastObject != null)
                    {
                        lastObject.Dead += (x, e, t) =>
                        {
                            newObject.MoveObject.Target = (x as ObjectActive).MoveObject.Target;
                        };
                    }
                    lastObject = newObject;
                }
            });

            //物件:攔截者 不會加速 但速度會越來越快
            WaveEvents.Add("Blocker", (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(14, 17);
                    int movesCount = 2;
                    float weight = 0.3F + size * 0.1F;
                    float speed = 200 * _SpeedFix;
                    int life = Sec(6F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint();

                    MoveStraight moveObject = new MoveStraight(new TargetObject(PlayerObject), weight, speed, movesCount, 100, 0.5F);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy, ShapeType.Ellipse, new DrawPolygon(Color.Fuchsia, Color.Fuchsia, 3, 1, 0, 360), moveObject);
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 200, 600, Sec(0.2F), Sec(0.5F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(10));
                    newObject.Propertys.Add(new PropertySpeeded(-1, 100));
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
                    float weight = 0.3F + size * 0.1F;
                    int life = Sec(4);

                    Point enterPoint = Point.Empty;
                    int targetX = 0, targetY = 0;
                    switch (roundIdx)
                    {
                        case 0:
                            enterPoint = GetEnterPoint(DirectionType.Left);
                            targetX = MainRectangle.Left + Global.Rand.Next(MainRectangle.Width / 2);
                            targetY = MainRectangle.Top + Global.Rand.Next(MainRectangle.Height);
                            break;
                        case 1:
                            enterPoint = GetEnterPoint(DirectionType.Right);
                            targetX = MainRectangle.Left + MainRectangle.Width / 2 + Global.Rand.Next(MainRectangle.Width / 2);
                            targetY = MainRectangle.Top + Global.Rand.Next(MainRectangle.Height);
                            break;
                        case 2:
                            enterPoint = GetEnterPoint(DirectionType.Top);
                            targetX = MainRectangle.Left + Global.Rand.Next(MainRectangle.Width);
                            targetY = MainRectangle.Top + Global.Rand.Next(MainRectangle.Height / 2);
                            break;
                        case 3:
                            enterPoint = GetEnterPoint(DirectionType.Bottom);
                            targetX = MainRectangle.Left + Global.Rand.Next(MainRectangle.Width);
                            targetY = MainRectangle.Top + MainRectangle.Height / 2 + Global.Rand.Next(MainRectangle.Height / 2);
                            break;
                    }

                    MoveStraight moveObject = new MoveStraight(new TargetPoint(targetX, targetY), weight, speed, movesCount, 100, 0.5F);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy, ShapeType.Ellipse, new DrawImage(Color.Black, Properties.Resources.Mine), moveObject);
                    newObject.Propertys.Add(new PropertyDeadExplosion(10, 0, 1, LeagueType.None, Color.Firebrick, 0.15F, 0.1F, ObjectDeadType.Collision | ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });
        }

        public override void SetWave()
        {
            //                                 123456789012345678901234567890123456789012345678901234
            //Waves.Add(new WaveLine("Catcher", "111111 111111 111111 111111 111111 111111 111111 11111"));
            //Waves.Add(new WaveLine("Faster ", "1      1      1             1      1             1     "));
            //Waves.Add(new WaveLine("Blocker", "1                   1                    1            "));
            Waves.Add(new WaveLine("WallA  ", "1          4               5                  6    "));
            //Waves.Add(new WaveLine("Mine   ", "         4            5           6        7          "));
            //Waves.Add(new WaveLine("@Dark  ", "              +++               +++            +++    "));
            //Waves.Add(new WaveLine("@Shrink", "        +++              +++              +++         "));
        }

        public override ObjectActive CreatePlayerObject(int potX, int potY)
        {
            MovePlayer moveObject = new MovePlayer(new TargetTrackPoint(this), 1, 200, 8);
            ObjectPlayer PlayerObject = new ObjectPlayer(potX, potY, 8, 7, 7, 170, LeagueType.Player, new DrawPen(Color.Black, ShapeType.Ellipse, 2), moveObject);
            PlayerObject.Propertys.Add(new PropertyCollision(1));
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
            _SpeedFix = 1F + WaveNo.Value * 0.015F;
            _LifeFix = 1F + WaveNo.Value * 0.01F;
        }

        public override void DoAfterEnd() { }
    }
}
