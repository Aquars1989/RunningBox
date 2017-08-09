using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public abstract class ObjectActive : ObjectBase
    {
        public League League { get; set; }
        public ITarget Target { get; set; }
        public List<PointF> Moves { get; set; }
        public int MaxMoves { get; set; }
        public float Speed { get; set; }

        private int _Energy;
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
        public int EnergyMax { get; set; }
        public int EnergyGetPerRound { get; set; }
        public SkillCollection Skills { get; set; }
        public PropertyCollection Propertys { get; set; }

        public ObjectActive()
        {
            Skills = new SkillCollection(this);
            Propertys = new PropertyCollection(this);
        }

        public override void Action()
        {
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
            Energy += EnergyGetPerRound;
        }

        /// <summary>
        /// 物件在回合內進行的規劃活動
        /// </summary>
        protected virtual void ActionPlan()
        {
            if (Target != null)
            {
                double direction = Function.PointRotation(X, Y, Target.X, Target.Y);
                float moveX = (float)Math.Cos(direction / 180 * Math.PI) * (Speed / 100F);
                float moveY = (float)Math.Sin(direction / 180 * Math.PI) * (Speed / 100F);
                Moves.Add(new PointF(moveX, moveY));
            }
        }

        /// <summary>
        /// 物件在回合內進行的移動
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

            X += moveTotalX * Scene.WorldSpeed;
            Y += moveTotalY * Scene.WorldSpeed;
        }

        /// <summary>
        /// 殺死此物件
        /// </summary>
        /// <param name="killer">殺手物件</param>
        public override void Kill(ObjectActive killer)
        {
            Skills.AllDoAfterDead(killer);
            Propertys.AllDoAfterDead(killer);
            base.Kill(killer);
        }

        /// <summary>
        /// 繪製物件
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public override void Draw(Graphics g)
        {
            Skills.AllDoBeforeDraw(g);
            base.Draw(g);
            Skills.AllDoAfterDraw(g);
        }
    }

    public enum League
    {
        Player,
        Ememy
    }
}
