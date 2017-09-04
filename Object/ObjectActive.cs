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
        /// 存活時間計數器(毫秒)
        /// </summary>
        public CounterObject Life { get; private set; }

        /// <summary>
        /// 能量計數物件
        /// </summary>
        public CounterObject Energy { get; private set; }

        /// <summary>
        /// 物件所屬陣營,供技能或特性判定
        /// </summary>
        public LeagueType League { get; set; }

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
        /// 使用繪製物件和移動物件建立互動性活動物件
        /// </summary>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectActive(DrawBase drawObject, MoveBase moveObject)
            : base(drawObject, moveObject)
        {
            Skills = new SkillCollection(this);
            Propertys = new PropertyCollection(this);
            Life = new CounterObject(-1);
            Energy = new CounterObject(10000, 10000, false);
            EnergyGetPerSec = 2000;
        }

        /// <summary>
        /// 建立一個互動性活動物件
        /// </summary>
        /// <param name="x">物件中心位置X</param>
        /// <param name="y">物件中心位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="life">存活時間,小於0為永久</param>
        /// <param name="collisonShape">碰撞形狀</param>
        /// <param name="leage">物件所屬陣營,供技能或特性判定</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectActive(float x, float y, int width, int height, int life, LeagueType leage, ShapeType collisonShape, DrawBase drawObject, MoveBase moveObject)
            : this(drawObject, moveObject)
        {
            Layout.CollisonShape = collisonShape;
            Layout.Anchor = ContentAlignment.MiddleCenter;
            Layout.X = x;
            Layout.Y = y;
            Layout.Width = width;
            Layout.Height = height;
            League = leage;
            Life.Limit = life;
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

            MoveObject.Plan();

            Skills.AllDoBeforeActionMove();
            Propertys.AllDoBeforeActionMove();

            MoveObject.Move();

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
            Energy.Value += (int)(EnergyGetPerSec / Scene.SceneRoundPerSec + 0.5F);
        }

        /// <summary>
        /// 殺死此物件
        /// </summary>
        /// <param name="killer">殺手物件</param>
        /// <param name="deadType">死亡類型</param>
        public override void Kill(ObjectActive killer, ObjectDeadType deadType)
        {
            if (Status == ObjectStatus.Alive)
            {
                Status = ObjectStatus.Dead;
                Propertys.AllDoAfterDead(killer, deadType);
                Skills.AllDoAfterDead(killer, deadType);
                OnDead(this, killer, deadType);
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
}
