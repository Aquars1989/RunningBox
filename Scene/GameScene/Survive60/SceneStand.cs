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
        /// <summary>
        /// 預設敵人速度調整值
        /// </summary>
        private float _DefaultSpeedFix = 1;

        /// <summary>
        /// 預設敵人存活時間調整值
        /// </summary>
        private float _DefaultLifeFix = 1;

        /// <summary>
        /// 敵人速度調整值
        /// </summary>
        private float _SpeedFix;

        /// <summary>
        /// 敵人存活時間調整值
        /// </summary>
        private float _LifeFix;

        public SceneStand()
        {
            InitializeComponent();

            // 場景:畫面變黑暗
            WaveEvents.Add("@Dark", (n) =>
                {
                    EffectObjects.Add(new EffectDyeing(Color.Black, Wave(0.5F), Wave(Math.Max(n - 1, 0)), Wave(0.5F)));
                });

            // 場景:邊界縮小
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
                EffectObjects.Add(new EffectShrink(shrinkPerRound, Wave(0.5F), Wave(Math.Max(n - 1, 0)), Wave(0.5F)));
            });

            // 物件:追捕者
            WaveEvents.Add("Catcher", (n) =>
            {
                int roundIdx = Global.Rand.Next(4);
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(7, 12);
                    int offsetLimit = size + Global.Rand.Next(5, 10);
                    float speed = Global.Rand.Next(320, 380) * _SpeedFix;
                    float weight = 0.3F + size * 0.1F;
                    int life = Sec(3.5F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint(roundIdx);

                    MoveStraight moveObject = new MoveStraight(PlayerObject, weight, speed, offsetLimit, 100, 0.5F);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, new DrawBrush(Color.Red, ShapeType.Ellipse), moveObject);
                    newObject.Skills.Add(new SkillSprint(0, Sec(1.5F), 15, 0, true) { AutoCastObject = new AutoCastNormal(0.4F) });
                    newObject.Skills.Add(new SkillSprint(0, Sec(0.5F), 5, 0, false) { AutoCastObject = new AutoCastNormal(3F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 30, 150, 400, Sec(0.5F), Sec(0.9F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), Sec(0.01F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });

            // 物件:快速追捕者 速度更快 更常使用加速
            WaveEvents.Add("Faster", (n) =>
            {
                int roundIdx = Global.Rand.Next(4);
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(6, 8);
                    int offsetLimit = size + Global.Rand.Next(2, 4);
                    float speed = Global.Rand.Next(400, 460) * _SpeedFix;
                    float weight = 0.3F + size * 0.1F;
                    int life = Sec(4.5F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint(roundIdx);

                    MoveStraight moveObject = new MoveStraight(PlayerObject, weight, speed, offsetLimit, 100, 0.5F);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, new DrawBrush(Color.Blue, ShapeType.Ellipse), moveObject);
                    newObject.Skills.Add(new SkillSprint(0, Sec(1F), 8, 0, true) { AutoCastObject = new AutoCastNormal(1F) });
                    newObject.Skills.Add(new SkillSprint(0, Sec(0.5F), 4, 0, false) { AutoCastObject = new AutoCastNormal(3F) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 150, 400, Sec(0.5F), Sec(0.9F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), Sec(0.01F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });

            // 物件:流星 四面八方的直線前進物件
            WaveEvents.Add("Meteor", (n) =>
            {
                int roundIdx = Global.Rand.Next(4);
                for (int i = 0; i < n; i++)
                {
                    int size = 10;
                    int movesCount = 6;
                    float speed = 500 * _SpeedFix;
                    float weight = 0.3F + size * 0.1F;
                    int life = Sec(10F);
                    Point enterPoint = GetEnterPoint(roundIdx);

                    double angel = Function.GetAngle(enterPoint.X, enterPoint.Y, PlayerObject.Layout.CenterX, PlayerObject.Layout.CenterY) + Global.Rand.Next(-20, 20);
                    MoveStraight moveObject = new MoveStraight(null, weight, speed, movesCount, 0, 1F);
                    moveObject.Target.SetOffsetByAngle(angel, 1000F);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, new DrawBrush(Color.Orchid, ShapeType.Ellipse), moveObject);
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 150, 400, Sec(0.5F), Sec(0.9F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), Sec(0.01F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertySmoking(-1, Sec(0.2F)));
                    newObject.Propertys.Add(new PropertyFreeze(Sec(0.2F * i)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    newObject.Propertys.Add(new PropertyOutClear());
                    moveObject.Target.SetObject(newObject);
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });

            // 物件:水平牆壁(有缺口)
            WaveEvents.Add("WallA", (n) =>
            {
                List<ObjectActive> objects = new List<ObjectActive>();
                for (int i = 0; i < MainRectangle.Height + 60; i += 30)
                {
                    int movesCount = 6;
                    float speed = 700 * (10 + n) / 10F;
                    float weight = 3;
                    int life = Sec(10F);
                    MoveStraight moveObject = new MoveStraight(null, weight, speed, movesCount, 0, 1F);
                    moveObject.Target.SetOffsetByXY(1000F, 0);
                    DrawBrush drawObject = new DrawBrush(Color.Orchid, ShapeType.Rectangle) { RotateEnabled = false };
                    //DrawPolygon drawObject = new DrawPolygon(Color.Orchid, Color.Orchid, 2, 5, 0) { RotateEnabled = false };
                    ObjectActive newObject = new ObjectActive(-50, MainRectangle.Top + i - 20, 5, 28, life, LeagueType.Ememy1, ShapeType.Ellipse, drawObject, moveObject);
                    //newObject.Propertys.Add(new PropertyRotate(-1, 780, false, true));
                    //newObject.Propertys.Add(new PropertyDeadBroken(new DrawBrush(Color.Orchid, ShapeType.Rectangle), 15, 6, 6, ObjectDeadType.Collision | ObjectDeadType.LifeEnd, 20, 200, 600, Sec(0.6F), Sec(1.2F)));
                    newObject.Propertys.Add(new PropertyDeadBrokenShaping(15, 6, 6, ObjectDeadType.Collision | ObjectDeadType.LifeEnd, 20, 100, 300, Sec(0.6F), Sec(1.2F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    newObject.Propertys.Add(new PropertyOutClear());
                    moveObject.Target.SetObject(newObject);
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

            // 物件:水平牆壁(交錯)
            WaveEvents.Add("WallB", (n) =>
            {
                List<ObjectActive> objects = new List<ObjectActive>();
                int midRand = 50;
                int cot = MainRectangle.Height + 60 / 30;
                int offsetMid = 100 / cot;
                for (int i = 0; i < MainRectangle.Height + 60; i += 30)
                {
                    int movesCount = 6;
                    float speed = 700 * (10 + n) / 10F;
                    float weight = 3;
                    int life = Sec(10F);
                    int enterX;
                    int moveX;
                    if (Global.Rand.Next(100) > midRand)
                    {
                        enterX = -50;
                        moveX = 1000;
                        midRand += offsetMid;
                    }
                    else
                    {
                        enterX = Width + 50;
                        moveX = -1000;
                        midRand -= offsetMid;
                    }

                    MoveStraight moveObject = new MoveStraight(null, weight, speed, movesCount, 0, 1F);
                    moveObject.Target.SetOffsetByXY(moveX, 0);
                    DrawPolygon drawObject = new DrawPolygon(Color.Orchid, Color.Orchid, 2, 5, 0) { RotateEnabled = true };
                    ObjectActive newObject = new ObjectActive(enterX, MainRectangle.Top + i - 20, 28, 28, life, LeagueType.Ememy1, ShapeType.Ellipse, drawObject, moveObject);
                    newObject.Propertys.Add(new PropertyRotate(-1, 780, false, true));
                    newObject.Propertys.Add(new PropertyDeadBrokenShaping(15, 6, 6, ObjectDeadType.Collision | ObjectDeadType.LifeEnd, 20, 100, 300, Sec(0.6F), Sec(1.2F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    newObject.Propertys.Add(new PropertyOutClear());
                    moveObject.Target.SetObject(newObject);
                    GameObjects.Add(newObject);
                    objects.Add(newObject);
                }
            });

            // 物件:序列 排列成直線的追捕者
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
                    MoveStraight moveObject = new MoveStraight(target, weight, speed, movesCount, 30, 1F);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, new DrawBrush(Color.DarkOrange, ShapeType.Ellipse), moveObject);
                    newObject.Propertys.Add(new PropertyFreeze(Sec(i * 0.05F)));
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 40, 150, 400, Sec(0.5F), Sec(0.9F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.2F), Sec(0.01F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    GameObjects.Add(newObject);
                    target = newObject;
                    speed += 10;
                    life -= 50;
                    if (lastObject != null)
                    {
                        lastObject.Dead += (x, e, t) =>
                        {
                            newObject.MoveObject.Target.SetObject((x as ObjectActive).MoveObject.Target.Object);
                        };
                    }
                    lastObject = newObject;
                }
            });

            // 物件:轟炸機
            WaveEvents.Add("Bomber", (n) =>
            {
                int roundIdx = Global.Rand.Next(4);
                for (int i = 0; i < n; i++)
                {
                    Point enterPoint = GetEnterPoint(roundIdx);
                    int size = Global.Rand.Next(28, 34);
                    int movesCount = 6;
                    float speed = Global.Rand.Next(300, 380);
                    float weight = 0.3F + size * 0.1F;
                    double angle = Function.GetAngle(enterPoint.X, enterPoint.Y, MainRectangle.Left + MainRectangle.Width / 2, MainRectangle.Top + MainRectangle.Height / 2) + Global.Rand.Next(-20, 20);
                    int life = Sec(10F);

                    MoveStraight moveObject = new MoveStraight(null, weight, speed, movesCount, 30, 1F);
                    DrawPic drawObject = new DrawPic(Color.Black, Properties.Resources.Bomber, (float)angle);
                    drawObject.Colors.RFix = 0.7F;
                    drawObject.Colors.BFix = 0.5F;
                    //DrawPolygon drawObject = new DrawPolygon(Color.SlateBlue, Color.SlateBlue, 3, 1, (float)angle);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, drawObject, moveObject);
                    moveObject.Target.SetOffsetByAngle(angle, 1000);
                    moveObject.Target.SetObject(newObject);

                    newObject.Propertys.Add(new PropertyAlert(-1));
                    newObject.Propertys.Add(new PropertyDeadBroken(30, 2, 2, ObjectDeadType.Collision, 40, 150, 400, Sec(0.5F), Sec(0.9F)));
                    newObject.Propertys.Add(new PropertyCollision(10));
                    newObject.Propertys.Add(new PropertyShadow(4, 6));
                    newObject.Propertys.Add(new PropertyBomber(-1, Sec(0.2F), 3, 8, 8, 5, 80, Sec(1F), Sec(1.4F)));
                    newObject.Propertys.Add(new PropertyOutClear());
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });

            // 物件:攔截者 不會加速 但速度會越來越快
            WaveEvents.Add("Blocker", (n) =>
            {
                int roundIdx = Global.Rand.Next(4);
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(14, 17);
                    int movesCount = 2;
                    float weight = 0.3F + size * 0.1F;
                    float speed = 200 * _SpeedFix;
                    int life = Sec(6F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint(roundIdx);

                    MoveStraight moveObject = new MoveStraight(PlayerObject, weight, speed, movesCount, 100, 0.5F);
                    DrawPolygon drawObject = new DrawPolygon(Color.Fuchsia, Color.Fuchsia, 3, 1, 0) { RotateEnabled = true, Resistance = weight };
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, drawObject, moveObject);
                    newObject.Propertys.Add(new PropertyRotate(-1, 360, false, false));
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 60, 150, 400, Sec(0.5F), Sec(0.9F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), Sec(0.01F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(10));
                    newObject.Propertys.Add(new PropertySpeeded(-1, 100));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });

            // 物件:地雷
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

                    MoveStraight moveObject = new MoveStraight(new PointObject(targetX, targetY), weight, speed, movesCount, 100, 0.5F);
                    DrawPic drawObject = new DrawPic(Color.Black, Properties.Resources.Mine, 0) { RotateEnabled = true, Resistance = weight };
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, drawObject, moveObject);
                    newObject.Propertys.Add(new PropertyRotate(-1, 280, false, false));
                    newObject.Propertys.Add(new PropertyDeadExplosion(10, 0, 1, LeagueType.None, Color.FromArgb(180, 225, 70, 40), 0.15F, 0.1F, 5, ObjectDeadType.Collision | ObjectDeadType.LifeEnd));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });

            // 物件:分裂地雷
            WaveEvents.Add("MineSplit", (n) =>
            {
                int size = Global.Rand.Next(28, 30);
                int movesCount = Global.Rand.Next(10, 15);
                float speed = Global.Rand.Next(200, 300);
                float weight = 0.3F + size * 0.1F;
                int life = Sec(4);

                Point enterPoint = GetEnterPoint();
                int targetX = MainRectangle.Left + MainRectangle.Width / 2 + Global.Rand.Next(-40, 40);
                int targetY = MainRectangle.Top + MainRectangle.Height / 2 + Global.Rand.Next(-40, 40);

                MoveStraight moveObject = new MoveStraight(new PointObject(targetX, targetY), weight, speed, movesCount, 100, 0.5F);
                DrawPic drawObject = new DrawPic(Color.Black, Properties.Resources.Mine, 0) { RotateEnabled = true, Resistance = weight };
                drawObject.Colors.BFix = 0.5F;
                ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, drawObject, moveObject);
                newObject.Propertys.Add(new PropertyRotate(-1, 280, false, false));
                newObject.Propertys.Add(new PropertyDeadExplosion(10, 0, 1, LeagueType.None, Color.FromArgb(180, 225, 70, 40), 0.15F, 0.1F, 5, ObjectDeadType.Collision | ObjectDeadType.LifeEnd));
                newObject.Propertys.Add(new PropertyCollision(1));
                newObject.Propertys.Add(new PropertyShadow(2, 3));
                newObject.Dead += (x, e, t) =>
                {
                    float partAngle = 360F / n;
                    float baseAngle = Global.Rand.Next(360);
                    for (int i = 0; i < n; i++)
                    {
                        int size2 = Global.Rand.Next(16, 18);
                        float speed2 = Global.Rand.Next(150, 250);
                        float weight2 = 0.3F + size2 * 0.1F;
                        int life2 = Sec(1.5F);
                        MoveStraight moveObject2 = new MoveStraight(null, weight2, speed2, movesCount, 0, 1F);
                        moveObject2.Target.SetOffsetByAngle(baseAngle, 1000);
                        DrawPic drawObject2 = new DrawPic(Color.Black, Properties.Resources.Mine, 0) { RotateEnabled = true, Resistance = weight2 };
                        drawObject2.Colors.BFix = 0.5F;
                        ObjectActive newObject2 = new ObjectActive(x.Layout.CenterX, x.Layout.CenterY, size2, size2, life2, LeagueType.Ememy1, ShapeType.Ellipse, drawObject2, moveObject2);
                        newObject2.Propertys.Add(new PropertyRotate(-1, 280, false, false));
                        newObject2.Propertys.Add(new PropertyDeadExplosion(10, 0, 1, LeagueType.None, Color.FromArgb(180, 225, 70, 40), 0.15F, 0.1F, 5, ObjectDeadType.Collision | ObjectDeadType.LifeEnd));
                        newObject2.Propertys.Add(new PropertyCollision(1));
                        newObject2.Propertys.Add(new PropertyShadow(2, 3));
                        moveObject2.Target.SetObject(newObject2);

                        newObject2.Dead += (x2, e2, t2) =>
                        {
                            float baseAngle2 = Global.Rand.Next(360);
                            for (int j = 0; j < n; j++)
                            {
                                int size3 = Global.Rand.Next(8, 10);
                                float speed3 = Global.Rand.Next(150, 250);
                                float weight3 = 0.3F + size3 * 0.1F;
                                int life3 = Sec(1.5F);
                                MoveStraight moveObject3 = new MoveStraight(null, weight3, speed3, movesCount, 0, 1F);
                                moveObject3.Target.SetOffsetByAngle(baseAngle2, 1000);
                                DrawPic drawObject3 = new DrawPic(Color.Black, Properties.Resources.Mine, 0) { RotateEnabled = true, Resistance = weight3 };
                                drawObject3.Colors.BFix = 0.5F;
                                ObjectActive newObject3 = new ObjectActive(x2.Layout.CenterX, x2.Layout.CenterY, size3, size3, life3, LeagueType.Ememy1, ShapeType.Ellipse, drawObject3, moveObject3);
                                newObject3.Propertys.Add(new PropertyRotate(-1, 280, false, false));
                                newObject3.Propertys.Add(new PropertyDeadExplosion(10, 0, 1, LeagueType.None, Color.FromArgb(180, 225, 70, 40), 0.15F, 0.1F, 5, ObjectDeadType.Collision | ObjectDeadType.LifeEnd));
                                newObject3.Propertys.Add(new PropertyCollision(1));
                                newObject3.Propertys.Add(new PropertyShadow(2, 3));
                                moveObject3.Target.SetObject(newObject2);
                                GameObjects.Add(newObject3);
                                baseAngle2 += partAngle;
                            }
                        };

                        GameObjects.Add(newObject2);
                        baseAngle += partAngle;
                    }
                };
                GameObjects.Add(newObject);
            });
        }

        public override void SetWave()
        {
            switch (Level)
            {
                case 1:
                    //                                    12345678901234567890123456789012345678901234567890
                    Waves.Add(new WaveLine("Catcher   ", "1111 111111 111111 111111 111111 111111 111111 111"));
                    Waves.Add(new WaveLine("Faster    ", "    1      1      1      1      1      1      1   "));
                    Waves.Add(new WaveLine("Blocker   ", "                    1                     1       "));
                    Waves.Add(new WaveLine("Mine      ", "        4                  5                  6   "));
                    Waves.Add(new WaveLine("@Shrink   ", "              ++++                     ++++       "));
                    Waves.Add(new WaveLine("WallA   ", "1111 111111 111111 111111 111111 111111 111111 111"));
                    break;
                case 2:
                    //                                    12345678901234567890123456789012345678901234567890
                    Waves.Add(new WaveLine("Catcher   ", "1111 111111 111111 111111 111111 111111 111111 111"));
                    Waves.Add(new WaveLine("Faster    ", "    1      1      1      1      1      1      1   "));
                    Waves.Add(new WaveLine("Blocker   ", "        1           1            1          1     "));
                    Waves.Add(new WaveLine("Mine      ", "              4               5                  6"));
                    Waves.Add(new WaveLine("Meteor    ", "   4                4                  8          "));
                    Waves.Add(new WaveLine("@Dark     ", "          ++                    ++                "));
                    break;
            }

        }

        public override ObjectActive CreatePlayerObject(int potX, int potY)
        {
            MovePlayer moveObject = new MovePlayer(this, 1, 250, 8);
            ObjectPlayer PlayerObject = new ObjectPlayer(potX, potY, 8, 7, 7, 170, LeagueType.Player, new DrawPen(Color.Black, ShapeType.Ellipse, 2), moveObject);

            if (!DeveloperOptions.Player_Ghost)
            {
                PlayerObject.Propertys.Add(new PropertyCollision(DeveloperOptions.Player_GodMode ? 10000 : 1));
            }
            PlayerObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 30, 150, 400, Sec(0.5F), Sec(0.9F)));
            PlayerObject.Propertys.Add(new PropertyShadow(2, 3));
            return PlayerObject;
        }

        protected override void OnAfterRound()
        {
            if (IsStart && !IsEnding)
            {
                PlayingInfo.Score += 10 + (WaveNo.Value) / 10;
            }
            base.OnAfterRound();
        }

        public override void DoAfterStart()
        {
            _SpeedFix = _DefaultSpeedFix;
            _LifeFix = _DefaultLifeFix;
        }

        public override void DoAfterWave()
        {
            _SpeedFix = _DefaultSpeedFix + WaveNo.Value * 0.01F;
            _LifeFix = _DefaultLifeFix + WaveNo.Value * 0.01F;
        }

        public override void DoAfterEnd() { }
    }
}
