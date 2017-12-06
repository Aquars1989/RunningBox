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
    public partial class SceneDisco : SceneGaming
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

        public SceneDisco()
        {
            InitializeComponent();

            // 場景:畫面變黑暗
            WaveEvents.Add("@Dark", (n) =>
                {
                    EffectObjects.Add(new EffectDyeing(Color.Black, Wave(0.1F), Wave(Math.Max(n, 0)), Wave(0.1F)));
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
                EffectObjects.Add(new EffectShrink(shrinkPerRound, Wave(0.1F), Wave(Math.Max(n, 0)), Wave(0.1F)));
            });

            // 物件:追捕者
            WaveEvents.Add("Catcher", (n) =>
            {
                int roundIdx = Global.Rand.Next(4);
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(9, 11);
                    int offsetLimit = 10;
                    float speed = Global.Rand.Next(200, 240) * _SpeedFix;
                    float weight = 0.3F + size * 0.1F;
                    int life = Sec(6F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint(roundIdx);

                    MoveFrog moveObject = new MoveFrog(PlayerObject, weight, speed, offsetLimit, Sec(1F));
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, new DrawBrush(Color.Red, ShapeType.Ellipse), moveObject);
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 150, 400, Sec(0.5F), Sec(0.9F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), Sec(0.01F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });

            // 物件:快速追捕者 移動間格減半
            WaveEvents.Add("Faster", (n) =>
            {
                int roundIdx = Global.Rand.Next(4);
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(6, 8);
                    int offsetLimit = 10;
                    float speed = Global.Rand.Next(275, 320) * _SpeedFix;
                    float weight = 0.3F + size * 0.1F;
                    int life = Sec(4.5F * _LifeFix) + Global.Rand.Next(0, 5);
                    Point enterPoint = GetEnterPoint(roundIdx);

                    MoveFrog moveObject = new MoveFrog(PlayerObject, weight, speed, offsetLimit, Sec(0.5F));
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, new DrawBrush(Color.Blue, ShapeType.Ellipse), moveObject);
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 150, 400, Sec(0.5F), Sec(0.9F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), Sec(0.01F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    GameObjects.Add(newObject);
                    roundIdx = ++roundIdx % 4;
                }
            });

            // 物件:蜻蜓 固定方向且會左右擺動
            WaveEvents.Add("Dragonfly", (n) =>
            {
                for (int i = 0; i < n; i++)
                {
                    int size = Global.Rand.Next(14, 18);
                    int offsetLimit = 10;
                    float speed = Global.Rand.Next(220, 260) * _SpeedFix;
                    float weight = 0.3F + size * 0.1F;
                    int life = -1;
                    Point enterPoint = GetEnterPoint(DirectionType.Right);

                    MoveFrog moveObject = new MoveFrog(null, weight, speed, offsetLimit, Sec(1F));
                    DrawPolygon drawObject = new DrawPolygon(Color.BlueViolet, Color.BlueViolet, 3, 0, 0) { RotateEnabled = true };
                    ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, drawObject, moveObject);
                    newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 150, 400, Sec(0.5F), Sec(0.9F)));
                    newObject.Propertys.Add(new PropertyDeadCollapse(1, Sec(0.6F), Sec(0.01F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, Sec(0.15F), Sec(0.25F)));
                    newObject.Propertys.Add(new PropertyCollision(1));
                    newObject.Propertys.Add(new PropertyShadow(2, 3));
                    newObject.Propertys.Add(new PropertyOutClear());
                    newObject.Propertys.Add(new PropertyDelay(Sec(0.5F), new PropertyDrunken(-1, Sec(1F), -45, 45, Global.Rand.Next(2) == 0 ? 100 : -100)));
                    newObject.Propertys.Add(new PropertyRotateTarget(-1, 400, true));
                    moveObject.Target.SetObject(newObject);
                    moveObject.Target.SetOffsetByXY(-1000, 0);
                    GameObjects.Add(newObject);
                }
            });

            // 物件:水平牆壁
            WaveEvents.Add("WallA", (n) =>
            {
                List<ObjectActive> objects = new List<ObjectActive>();
                for (int i = 0; i < MainRectangle.Height + 60; i += 30)
                {
                    int movesCount = 6;
                    float speed = 300 * (10 + n) / 10F;
                    float weight = 3;
                    int life = -1;
                    MoveFrog moveObject = new MoveFrog(null, weight, speed, movesCount, Sec(1));
                    moveObject.Target.SetOffsetByXY(1000F, 0);
                    DrawBrush drawObject = new DrawBrush(Color.Orchid, ShapeType.Rectangle) { RotateEnabled = false };
                    ObjectActive newObject = new ObjectActive(-50, MainRectangle.Top + i - 20, 5, 28, life, LeagueType.Ememy1, ShapeType.Ellipse, drawObject, moveObject);
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

            // 物件:箭雨(水平)
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

            // 物件:炸彈陣列
            WaveEvents.Add("BoomGrid", (n) =>
            {
                List<ObjectActive> objects = new List<ObjectActive>();
                for (int i = 0; i < MainRectangle.Height + 60; i += 120)
                {
                    int movesCount = 6;
                    float speed = 300 * (10 + n) / 10F;
                    float weight = 3;
                    int life = -1;
                    MoveFrog moveObject = new MoveFrog(null, weight, speed, movesCount, Sec(1));
                    moveObject.Target.SetOffsetByXY(1000F, 0);
                    DrawBrush drawObject = new DrawBrush(Color.Orchid, ShapeType.Rectangle) { RotateEnabled = false };
                    ObjectActive newObject = new ObjectActive(-50, MainRectangle.Top + i - 20, 5, 28, life, LeagueType.Ememy1, ShapeType.Ellipse, drawObject, moveObject);
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
        }

        public override void SetWave()
        {
            switch (Level)
            {
                case 1:
                    //                                    12345678901234567890123456789012345678901234567890
                    Waves.Add(new WaveLine("Catcher   ", "1111 111111 111111 111111 111111 111111 111111 111"));
                    Waves.Add(new WaveLine("Faster    ", "    1      1      1      1      1      1      1   "));
                    Waves.Add(new WaveLine("Dragonfly ", "        8          A          C          E        "));
                    Waves.Add(new WaveLine("@Shrink   ", " ++          ++          ++          ++         ++"));
                    break;
                case 2:
                    //                                    12345678901234567890123456789012345678901234567890
                    Waves.Add(new WaveLine("Frog      ", "1111 111111 111111 111111 111111 111111 111111 111"));
                    Waves.Add(new WaveLine("Faster    ", "    1      1      1      1      1      1      1   "));
                    Waves.Add(new WaveLine("Dragonfly ", "        8          A          C          E        "));
                    Waves.Add(new WaveLine("Arrow     ", "     4                   5                  6     "));
                    Waves.Add(new WaveLine("@Dark     ", "             ++                    ++             "));
                    break;
                case 3:
                    //                                    12345678901234567890123456789012345678901234567890
                    Waves.Add(new WaveLine("Frog      ", "1111 111111 111111 111111 111111 111111 111111 111"));
                    Waves.Add(new WaveLine("Faster    ", "    1      1      1      1      1      1      1   "));
                    Waves.Add(new WaveLine("Dragonfly ", "                 8               9                "));
                    Waves.Add(new WaveLine("Arrow     ", "         4               5                 6      "));
                    Waves.Add(new WaveLine("WallA     ", "     2       2       2      2  2  2  2  2    22222"));
                    break;
                case 4:
                    //                                    12345678901234567890123456789012345678901234567890
                    Waves.Add(new WaveLine("Frog      ", "1111 111111 111111 111111 111111 111111 111111 111"));
                    Waves.Add(new WaveLine("Faster    ", "    1      1      1      1      1      1      1   "));
                    Waves.Add(new WaveLine("Dragonfly ", "       3      4      5      6      7      8     9 "));
                    Waves.Add(new WaveLine("Arrow     ", "   2      3      4      5      6      7      8    "));
                    Waves.Add(new WaveLine("@Dark     ", "             ++                    ++             "));
                    break;
                case 5:
                    //                                    12345678901234567890123456789012345678901234567890
                    Waves.Add(new WaveLine("Frog      ", "1111 111111 111111 111111 111111 111111 111111 111"));
                    Waves.Add(new WaveLine("Faster    ", "    1      1      1      1      1      1      1   "));
                    Waves.Add(new WaveLine("Dragonfly ", "  4   4   5   5   6   6   7   7   8   8   9   9555"));
                    Waves.Add(new WaveLine("Arrow     ", "    1     1     1    1    1   1   1    1    1   1 "));
                    break;
                case 6:
                    //                                    12345678901234567890123456789012345678901234567890
                    Waves.Add(new WaveLine("Frog      ", "1111 111111 111111 111111 111111 111111 111111 111"));
                    Waves.Add(new WaveLine("Faster    ", "    1      1      1      1      1      1      1   "));
                    Waves.Add(new WaveLine("Dragonfly ", "33333333333333333333333333333333333333333333333333"));
                    Waves.Add(new WaveLine("Arrow     ", "11111111111111111111111111111111111111111111111111"));
                    Waves.Add(new WaveLine("Arrow     ", "           111111111111111111111111111111111111111"));
                    Waves.Add(new WaveLine("Arrow     ", "                      1111111111111111111111111111"));
                    Waves.Add(new WaveLine("Arrow     ", "                                 11111111111111111"));
                    Waves.Add(new WaveLine("Arrow     ", "                                            111111"));
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

        int _EnergyFillTime;
        int _EnergyFillTimeKeep;
        protected override void OnAfterRound()
        {
            if (IsStart && !IsEnding)
            {
                AddScoreToPlayer("存活", SceneIntervalOfRound);

                if (PlayerObject.Energy.IsFull)
                {
                    _EnergyFillTime += SceneIntervalOfRound * (1 + _EnergyFillTimeKeep / 3000);
                    _EnergyFillTimeKeep += SceneIntervalOfRound;
                }
                else
                {
                    _EnergyFillTimeKeep = 0;
                }
            }
            base.OnAfterRound();
        }

        public override void DoAfterStart()
        {
            _SpeedFix = _DefaultSpeedFix;
            _LifeFix = _DefaultLifeFix;
            _EnergyFillTime = 0;
            _EnergyFillTimeKeep = 0;

            Color[] backColors = {
                                     Color.FromArgb(235,255,235),
                                     Color.FromArgb(235,235,255),
                                     Color.FromArgb(255,255,235),
                                     Color.FromArgb(255,235,220)
                                 };
            EffectObjects.Add(new EffectDyeingRotate(backColors, -1, Sec(1), Sec(0.2F)));
        }

        public override void DoAfterWave()
        {
            _SpeedFix = _DefaultSpeedFix + WaveNo.Value * 0.005F;
            _LifeFix = _DefaultLifeFix + WaveNo.Value * 0.01F;
            AddScoreToPlayer("經歷波數", WaveNo.Value * 15);
            if (_EnergyFillTime > 0)
            {
                AddScoreToPlayer("能量滿溢", _EnergyFillTime / 30);
                _EnergyFillTime = 0;
            }
        }



        public override void DoAfterEnd() { }
    }
}
