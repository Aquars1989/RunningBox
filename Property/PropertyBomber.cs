using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件週期性會在原位置產生四散的炸彈
    /// </summary>
    class PropertyBomber : PropertyBase
    {
        /// <summary>
        /// 產生炸彈週期
        /// </summary>
        public CounterObject BuildTime { get; private set; }

        /// <summary>
        /// 每次產生炸彈數量
        /// </summary>
        public int BombCount { get; set; }

        /// <summary>
        /// 炸彈寬度
        /// </summary>
        public int BombWidth { get; set; }

        /// <summary>
        /// 炸彈高度
        /// </summary>
        public int BombHeight { get; set; }

        /// <summary>
        /// 炸彈移動速度最大值
        /// </summary>
        public int BombSpeedMax { get; set; }

        /// <summary>
        /// 炸彈移動速度最小值
        /// </summary>
        public int BombSpeedMin { get; set; }

        /// <summary>
        /// 炸彈生命週期最大值
        /// </summary>
        public int BombLifeMax { get; set; }

        /// <summary>
        /// 炸彈生命週期最小值
        /// </summary>
        public int BombLifeMin { get; set; }

        /// <summary>
        /// 炸彈外觀範本繪圖物件
        /// </summary>
        public DrawBase BombDrawObject { get; set; }

        /// <summary>
        /// 新增碎裂特性且指定炸彈外觀，擁有此特性的物件每回合會在原位置產生四散的炸彈
        /// </summary>
        /// <param name="duration">持續時間(毫秒),小於0為永久</param>
        /// <param name="buildTime">生產週期(毫秒)</param>
        /// <param name="bombDrawObject">炸彈外觀範本繪圖物件</param>
        /// <param name="bombCount">每次產生炸彈數量</param>
        /// <param name="bombWidth">炸彈寬度</param>
        /// <param name="bombHeight">炸彈高度</param>
        /// <param name="bombSpeedMax">炸彈移動速度最大值</param>
        /// <param name="bombSpeedMin">炸彈移動速度最小值</param>
        /// <param name="bombLifeMax">炸彈生命週期最大值</param>
        /// <param name="bombLifeMin">炸彈生命週期最小值</param>
        public PropertyBomber(DrawBase bombDrawObject, int duration, int buildTime, int bombCount, int bombWidth, int bombHeight, int bombSpeedMin, int bombSpeedMax, int bombLifeMin, int bombLifeMax)
            : this(duration, buildTime, bombCount, bombWidth, bombHeight, bombSpeedMin, bombSpeedMax, bombLifeMin, bombLifeMax)
        {
            BombDrawObject = bombDrawObject;
        }

        /// <summary>
        /// 新增碎裂特性且指定炸彈顏色，擁有此特性的物件每回合會在原位置產生四散的炸彈
        /// </summary>
        /// <param name="duration">持續時間(毫秒),小於0為永久</param>
        /// <param name="buildTime">生產週期(毫秒)</param>
        /// <param name="color">產生炸彈顏色</param>
        /// <param name="bombCount">每回合產生炸彈數量</param>
        /// <param name="bombWidth">炸彈寬度</param>
        /// <param name="bombHeight">炸彈高度</param>
        /// <param name="bombSpeedMax">炸彈移動速度最大值</param>
        /// <param name="bombSpeedMin">炸彈移動速度最小值</param>
        /// <param name="bombLifeMax">炸彈生命週期最大值</param>
        /// <param name="bombLifeMin">炸彈生命週期最小值</param>
        public PropertyBomber(Color color, int duration, int buildTime, int bombCount, int bombWidth, int bombHeight, int bombSpeedMin, int bombSpeedMax, int bombLifeMin, int bombLifeMax)
            : this(duration, buildTime, bombCount, bombWidth, bombHeight, bombSpeedMin, bombSpeedMax, bombLifeMin, bombLifeMax)
        {
            BombDrawObject = new DrawBrush(color, ShapeType.Ellipse);
        }

        /// <summary>
        /// 新增碎裂特性，擁有此特性的物件每回合會在原位置產生四散的炸彈
        /// </summary>
        /// <param name="duration">持續時間(毫秒),小於0為永久</param>
        /// <param name="buildTime">生產週期(毫秒)</param>
        /// <param name="bombCount">每回合產生炸彈數量</param>
        /// <param name="bombWidth">炸彈寬度</param>
        /// <param name="bombHeight">炸彈高度</param>
        /// <param name="bombSpeedMax">炸彈移動速度最大值</param>
        /// <param name="bombSpeedMin">炸彈移動速度最小值</param>
        /// <param name="bombLifeMax">炸彈生命週期最大值</param>
        /// <param name="bombLifeMin">炸彈生命週期最小值</param>
        public PropertyBomber(int duration, int buildTime, int bombCount, int bombWidth, int bombHeight, int bombSpeedMin, int bombSpeedMax, int bombLifeMin, int bombLifeMax)
        {
            BuildTime = new CounterObject(buildTime);
            DurationTime.Limit = duration;
            BombCount = bombCount;
            BombWidth = bombWidth;
            BombHeight = bombHeight;
            BombSpeedMax = bombSpeedMax;
            BombSpeedMin = bombSpeedMin;
            BombLifeMax = bombLifeMax;
            BombLifeMin = bombLifeMin;
            BreakAfterDead = false;
        }

        public override void DoAfterAction()
        {
            if (BuildTime.IsFull)
            {
                for (int i = 0; i < BombCount; i++)
                {
                    int speed = Global.Rand.Next(BombSpeedMin, Math.Max(BombSpeedMin, BombSpeedMax) + 1);
                    int life = Global.Rand.Next(BombLifeMin, Math.Max(BombLifeMin, BombLifeMax) + 1);
                    double bombDirection = Global.Rand.NextDouble() * 360;

                    MoveStraight moveObject = new MoveStraight(null, 1, speed, 1, 0, 1);
                    moveObject.Target.SetOffsetByAngle(bombDirection, 1000);

                    DrawBase bombDraw;
                    if (BombDrawObject == null)
                    {
                        bombDraw = new DrawPolygon(Color.Empty, Owner.DrawObject.MainColor, 2, 2, Global.Rand.Next(0, 360)) { RotateEnabled = true };

                    }
                    else
                    {
                        bombDraw = BombDrawObject.Copy();
                    }

                    ObjectBase newObject = new ObjectBase(bombDraw, moveObject);
                    newObject.Layout.Anchor = DirectionType.Center;
                    newObject.Layout.CollisonShape = ShapeType.Ellipse;
                    newObject.Layout.Width = BombWidth;
                    newObject.Layout.Height = BombHeight;
                    newObject.Layout.X = Owner.Layout.CenterX;
                    newObject.Layout.Y = Owner.Layout.CenterY;
                    newObject.League = Owner.League;
                    newObject.Life.Limit = life;

                    newObject.Propertys.Add(new PropertyRotate(-1, Global.Rand.Next(280, 360), false, true));
                    newObject.Propertys.Add(new PropertyDeadExplosion(8, 0, 1, LeagueType.Ememy1, Color.FromArgb(180, 225, 70, 40), 0, 1, 2, ObjectDeadType.All) { DrawRange = false });
                    newObject.Propertys.Add(new PropertyCollision(1));

                    moveObject.Target.SetObject(newObject);
                    Owner.Container.Add(newObject);
                }
                BuildTime.Value = 0;
            }
            else
            {
                BuildTime.Value += Scene.SceneIntervalOfRound;
            }
            base.DoAfterAction();
        }
    }
}
