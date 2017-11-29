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
        #region ===== 事件 =====
        /// <summary>
        /// 發生於技能集合變更
        /// </summary>
        public event ValueChangedEnentHandle<SkillCollection> SkillsChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於技能集合變更
        /// </summary>
        protected virtual void OnSkillsChanged(SkillCollection oldValue, SkillCollection newValue)
        {
            if (oldValue != null)
            {
                oldValue.BindingUnlock();
                oldValue.Binding(Scene);
            }

            if (newValue != null)
            {
                newValue.Binding(this,true);
            }

            if (SkillsChanged != null)
            {
                SkillsChanged(this, oldValue, newValue);
            }
        }
        #endregion

        #region ===== 屬性 =====
        /// <summary>
        /// 能量計數物件
        /// </summary>
        public CounterObject Energy { get; private set; }

        /// <summary>
        /// 每秒自動恢復的能量數
        /// </summary>
        public int EnergyGetPerSec { get; set; }

        private SkillCollection _Skills;
        /// <summary>
        /// 活動物件擁有的技能群組
        /// </summary>
        public SkillCollection Skills
        {
            get { return _Skills; }
            set
            {
                if (_Skills == value) return;
                SkillCollection oldValue = _Skills;
                _Skills = value;
                OnSkillsChanged(oldValue, value);
            }
        }
        #endregion

        /// <summary>
        /// 使用繪製物件和移動物件建立互動性活動物件
        /// </summary>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectActive(DrawBase drawObject, MoveBase moveObject)
            : base(drawObject, moveObject)
        {
            Skills = new SkillCollection();
            Energy = new CounterObject(Global.DefaultEnergyLimit, Global.DefaultEnergyLimit, false);
            EnergyGetPerSec = Global.DefaultEnergyGetPerSec;
        }

        /// <summary>
        /// 使用指定的配置建立互動性活動物件
        /// </summary>
        /// <param name="layout">配置資訊</param>
        /// <param name="life">存活時間,小於0為永久</param>
        /// <param name="leage">物件所屬陣營,供技能或特性判定</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectActive(LayoutSet layout, int life, LeagueType leage, DrawBase drawObject, MoveBase moveObject)
            : this(drawObject, moveObject)
        {
            Layout.CollisonShape = layout.CollisonShape;
            Layout.Anchor = layout.Anchor;
            Layout.X = layout.X;
            Layout.Y = layout.Y;
            Layout.Width = layout.Width;
            Layout.Height = layout.Height;
            League = leage;
            Life.Limit = life;
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
            Layout.Anchor = DirectionType.Center;
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
            Skills.AllDoAutoCast();
            // 回合前
            Skills.AllDoBeforeAction();
            Propertys.AllDoBeforeAction();
            // 能量調整前
            Skills.AllDoBeforeActionEnergyGet();
            Propertys.AllDoBeforeActionEnergyGet();
            // 能量調整
            ActionEnergyGet();
            // 移動規劃前
            Skills.AllDoBeforeActionPlan();
            Propertys.AllDoBeforeActionPlan();
            // 移動規劃
            MoveObject.Plan();
            // 移動動作前
            Skills.AllDoBeforeActionMove();
            Propertys.AllDoBeforeActionMove();
            // 移動動作
            MoveObject.Move();
            // 回合後
            Skills.AllDoAfterAction();
            Propertys.AllDoAfterAction();
            // 結算
            Settlement();
            Skills.AllSettlement();
            Propertys.AllSettlement();

            OnAfterAction();
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
        public override void Kill(ObjectBase killer, ObjectDeadType deadType)
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
            if (Visible)
            {
                UIOffSetY = 0; 
                Skills.AllDoBeforeDraw(g);
                Propertys.AllDoBeforeDraw(g);
                DrawObject.Draw(g, Layout.Rectangle);
                Propertys.AllDoAfterDraw(g);
                Skills.AllDoAfterDraw(g);
            }
        }

        protected override void OnDispose()
        {
            Skills.Clear();
            base.OnDispose();
        }
    }
}
