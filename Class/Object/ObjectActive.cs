using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public abstract class ObjectActive : ObjectBase
    {
        public int Energy { get; set; }
        public int EnergyMax { get; set; }
        public int EnergyGetPerRound { get; set; }
        public SkillCollection Skills { get; set; }

        public ObjectActive()
        {
            Skills = new SkillCollection(this);
        }

        public override void Action()
        {
            Skills.AllDoBeforeAction();
            Skills.AllDoBeforeActionEnergyGet();
            ActionEnergyGet();
            Skills.AllDoBeforeActionPlan();
            ActionPlan();
            Skills.AllDoBeforeActionMove();
            ActionMove();
            Skills.AllDoBeforeAction();
        }

        protected virtual void ActionEnergyGet()
        {
            Energy += EnergyGetPerRound;
            if (Energy > EnergyMax)
            {
                Energy = EnergyMax;
            }
        }

        /// <summary>
        /// 繪製物件
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public override void Draw(Graphics g)
        {
            Skills.AllDoBeforeDraw(g);
            DrawSelf(g);
            Skills.AllDoAfterDraw(g);
        }
    }
}
