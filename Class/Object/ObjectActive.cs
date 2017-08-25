using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 基礎可互動活動物件
    /// </summary>
    public class ObjectActive : ObjectBase
    {
        /// <summary>
        /// 存活時間計數(毫秒)
        /// </summary>
        public int LifeTicks { get; set; }

        /// <summary>
        /// 存活時間最大值(毫秒),小於0為永久
        /// </summary>
        public int LifeLimit { get; set; }

        /// <summary>
        /// 物件所屬陣營,供技能或特性判定
        /// </summary>
        public League League { get; set; }

        /// <summary>
        /// 追尋目標
        /// </summary>
        public ITarget Target { get; set; }

        /// <summary>
        /// 移動調整值紀錄
        /// </summary>
        public List<PointF> Moves { get; set; }

        /// <summary>
        /// 最大調整值紀錄數量
        /// </summary>
        public int MaxMoves { get; set; }

        /// <summary>
        /// 移動速度,決定每個移動調整值的距離
        /// </summary>
        public float Speed { get; set; }

        private int _Energy;
        /// <summary>
        /// 能量,使用技能用
        /// </summary>
        public int Energy
        {
            get { return _Energy; }
            set
            {
                _Energy = value;
                if (_Energy < 0) _Energy = 0;
                else if (_Energy > EnergyMax) _Energy = EnergyMax;
            }
        }

        /// <summary>
        /// 能量上限,能量不可高於此數值
        /// </summary>
        public int EnergyMax { get; set; }

        /// <summary>
        /// 每秒自動恢復的能量數
        /// </summary>
        public int EnergyGetPerSec { get; set; }


        /// <summary>
        /// 活動物件擁有的技能群組
        /// </summary>
        public SkillCollection Skills { get; set; }

        /// <summary>
        /// 活動物件擁有的特性群組
        /// </summary>
        public PropertyCollection Propertys { get; set; }

        /// <summary>
        /// 給UI特性使用
        /// </summary>
        public int UIOffSetY { get; set; }

        /// <summary>
        /// 建立一個互動性活動物件
        /// </summary>
        /// <param name="x">物件中心位置X</param>
        /// <param name="y">物件中心位置Y</param>
        /// <param name="maxMoves">最大調整值紀錄數量</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="speed">速度</param>
        /// <param name="life">存活時間,小於0為永久</param>
        /// <param name="leage">物件所屬陣營,供技能或特性判定</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="target">追蹤目標</param>
        public ObjectActive(float x, float y, int maxMoves, int width, int height, float speed, int life, League leage, IDraw drawObject, ITarget target)
            : this()
        {
            Layout.Anchor = ContentAlignment.MiddleCenter;
            Layout.X = x;
            Layout.Y = y;
            Layout.Width = width;
            Layout.Height = height;

            Status = ObjectStatus.Alive;
            MaxMoves = maxMoves;
            Speed = speed;
            LifeLimit = life;
            Target = target;
            League = leage;
            DrawObject = drawObject;
        }

        /// <summary>
        /// 建立一個互動性活動物件
        /// </summary>
        public ObjectActive()
        {
            Skills = new SkillCollection(this);
            Propertys = new PropertyCollection(this);
            Moves = new List<PointF>();
            EnergyMax = 10000;
            Energy = 10000;
            EnergyGetPerSec = 2000;
        }

        /// <summary>
        /// 活動物件每回合動作
        /// </summary>
        public override void Action()
        {
            UIOffSetY = 0;
            Skills.AllDoAutoCast();

            Skills.AllDoBeforeAction();
            Propertys.AllDoBeforeAction();

            Skills.AllDoBeforeActionEnergyGet();
            Propertys.AllDoBeforeActionEnergyGet();

            ActionEnergyGet();

            Skills.AllDoBeforeActionPlan();
            Propertys.AllDoBeforeActionPlan();

            ActionPlan();

            Skills.AllDoBeforeActionMove();
            Propertys.AllDoBeforeActionMove();

            ActionMove();

            Skills.AllDoAfterAction();
            Propertys.AllDoAfterAction();

            Skills.AllSettlement();
            Propertys.AllSettlement();

            Propertys.ClearAllDisabled();
        }

        /// <summary>
        /// 物件能量調整
        /// </summary>
        protected virtual void ActionEnergyGet()
        {
            Energy += (int)(EnergyGetPerSec / Scene.SceneRoundPerSec + 0.5F);
        }

        /// <summary>
        /// 物件在回合內進行的規劃活動
        /// </summary>
        protected virtual void ActionPlan()
        {
            if (Target != null)
            {
                double direction = Function.GetAngle(Layout.CenterX, Layout.CenterY, Target.X, Target.Y);
                Moves.Add(GetMovePoint(direction, Speed));
            }
        }

        /// <summary>
        /// 物件在回合內進行的移動活動
        /// </summary>
        protected virtual void ActionMove()
        {
            if (Moves.Count > MaxMoves)
            {
                Moves.RemoveRange(0, Moves.Count - MaxMoves);
            }

            float moveTotalX = 0;
            float moveTotalY = 0;
            foreach (PointF pt in Moves)
            {
                moveTotalX += pt.X;
                moveTotalY += pt.Y;
            }

            Layout.X += moveTotalX / Scene.SceneSlow;
            Layout.Y += moveTotalY / Scene.SceneSlow;

            LifeTicks += Scene.SceneIntervalOfRound;
            if (LifeLimit >= 0 && LifeTicks >= LifeLimit)
            {
                Kill(null, ObjectDeadType.LifeEnd);
            }

        }

        /// <summary>
        /// 殺死此物件
        /// </summary>
        /// <param name="killer">殺手物件</param>
        public override void Kill(ObjectActive killer, ObjectDeadType deadType)
        {
            if (Status == ObjectStatus.Alive)
            {
                base.Kill(killer, deadType);
                Skills.AllDoAfterDead(killer, deadType);
                Propertys.AllDoAfterDead(killer, deadType);
            }
        }

        /// <summary>
        /// 繪製物件
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public override void Draw(Graphics g)
        {
            Skills.AllDoBeforeDraw(g);
            Propertys.AllDoBeforeDraw(g);
            base.Draw(g);
            Propertys.AllDoAfterDraw(g);
            Skills.AllDoAfterDraw(g);
        }
    }

    /// <summary>
    /// 活動物件所屬陣營
    /// </summary>
    public enum League
    {
        None,
        Player,
        Ememy
    }
}
