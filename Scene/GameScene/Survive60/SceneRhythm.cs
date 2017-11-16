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
    public partial class SceneRhythm : SceneGaming
    {
        private float _SpeedFix = 1;
        private float _LifeFix = 1;

        public SceneRhythm()
        {
            InitializeComponent();

            //場景:畫面變黑暗
            WaveEvents.Add("@Dark", (n) =>
                {
                    EffectObjects.Add(new EffectDyeing(Color.Black, Wave(0.1F), Wave(Math.Max(n, 0)), Wave(0.1F)));
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
                EffectObjects.Add(new EffectShrink(shrinkPerRound, Wave(0.1F), Wave(Math.Max(n, 0)), Wave(0.1F)));
            });

            //物件:追捕者
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

            //物件:水平牆壁
            WaveEvents.Add("WallA", (n) =>
            {
                List<ObjectActive> objects = new List<ObjectActive>();
                for (int i = 0; i < MainRectangle.Height + 60; i += 30)
                {
                    int movesCount = 6;
                    float weight = 3;
                    int life = -1;// Sec(10F);
                    MoveStraight moveObject = new MoveStraight(null, weight, 0, movesCount, 0, 1F);
                    moveObject.Target.SetOffsetByXY(1000F, 0);
                    DrawBrush drawObject = new DrawBrush(Color.Orchid, ShapeType.Rectangle) { RotateEnabled = false };
                    //DrawPolygon drawObject = new DrawPolygon(Color.Orchid, Color.Orchid, 2, 5, 0) { RotateEnabled = false };
                    ObjectActive newObject = new ObjectActive(-50, MainRectangle.Top + i - 20, 5, 28, life, LeagueType.Ememy1, ShapeType.Ellipse, drawObject, moveObject);
                    //newObject.Propertys.Add(new PropertyRotate(-1, 780, false, true));
                    //newObject.Propertys.Add(new PropertyDeadBroken(new DrawBrush(Color.Orchid, ShapeType.Rectangle), 15, 6, 6, ObjectDeadType.Collision | ObjectDeadType.LifeEnd, 20, 200, 600, Sec(0.6F), Sec(1.2F)));
                    newObject.Skills.Add(new SkillSprint(0, Sec(1F), 0, (int)(10000 * (10 + n) / 10F), false) { AutoCastObject = new AutoCastNormal(100) });
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

            //物件:箭雨(水平)
            WaveEvents.Add("Arrow", (n) =>
            {
                int baseTop = Global.Rand.Next(MainRectangle.Top - 20, MainRectangle.Top + MainRectangle.Height + 20 - ((n - 1) * 30));
                for (int i = 0; i < n; i++)
                {
                    int movesCount = 12;
                    float weight = 3;
                    int life = -1;// Sec(10F);
                    MoveStraight moveObject = new MoveStraight(null, weight, 500, movesCount, 0, 1F);
                    moveObject.Target.SetOffsetByXY(1000F, 0);
                    DrawPolygon drawObject = new DrawPolygon(Color.Orchid, Color.Empty, 3, 1, 90F) { RotateEnabled = false };
                    ObjectActive newObject = new ObjectActive(-30, baseTop, 25, 25, life, LeagueType.Ememy1, ShapeType.Ellipse, drawObject, moveObject);
                    newObject.Skills.Add(new SkillSprint(0, Sec(1F), 0, (int)(400000), false) { AutoCastObject = new AutoCastNormal(100), Status = SkillStatus.Cooldown });
                    newObject.Propertys.Add(new PropertySpeeded(Sec(1F), -300));
                    newObject.Propertys.Add(new PropertyDeadBrokenShaping(15, 6, 6, ObjectDeadType.Collision | ObjectDeadType.LifeEnd, 20, 100, 300, Sec(0.6F), Sec(1.2F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    newObject.Propertys.Add(new PropertyOutClear());
                    moveObject.Target.SetObject(newObject);

                    GameObjects.Add(newObject);
                    baseTop += 30;
                }
            });

            //物件:青蛙
            WaveEvents.Add("Frog", (n) =>
            {
                int roundIdx = Global.Rand.Next(4);
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(9, 11);
                    int movesCount = 6;
                    float weight = 0.3F + size * 0.1F;
                    int life = Sec(6F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint(roundIdx);

                    MoveStraight moveObject = new MoveStraight(PlayerObject, weight, 0, movesCount, 100, 0.5F);
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, new DrawBrush(Color.Red, ShapeType.Ellipse), moveObject);
                    newObject.Skills.Add(new SkillSprint(0, Sec(1F), 0, (int)(10000 * _SpeedFix), false) { AutoCastObject = new AutoCastNormal(100) });
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 150, 400, Sec(0.5F), Sec(0.9F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), Sec(0.01F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });
        }

        public override void SetWave()
        {
            switch (Level)
            {
                case 1:
                    //                                    12345678901234567890123456789012345678901234567890
                    Waves.Add(new WaveLine("Frog     ", "1111 111111 111111 111111 111111 111111 111111 111"));
                    Waves.Add(new WaveLine("Arrow  ", " 23456789A      ++++                     ++++       "));
                    break;
                case 2:
                    //                                    12345678901234567890123456789012345678901234567890
                    Waves.Add(new WaveLine("Frog   ", "1111 111111 111111 111111 111111 111111 111111 111"));
                    Waves.Add(new WaveLine("@Dark     ", "          ++                    ++                "));
                    break;
            }

        }

        public override ObjectActive CreatePlayerObject(int potX, int potY)
        {
            MovePlayer moveObject = new MovePlayer(this, 1, 250, 8);
            ObjectPlayer PlayerObject = new ObjectPlayer(potX, potY, 8, 7, 7, 170, LeagueType.Player, new DrawPen(Color.Black, ShapeType.Ellipse, 2), moveObject);

            PlayerObject.Propertys.Add(new PropertyCollision(1));
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
            _SpeedFix = 0.8F;
            _LifeFix = 1F;
        }

        public override void DoAfterWave()
        {
            _SpeedFix = 1F + WaveNo.Value * 0.01F;
            _LifeFix = 1F + WaveNo.Value * 0.01F;
        }

        public override void DoAfterEnd() { }
    }
}
