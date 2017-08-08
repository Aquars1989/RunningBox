using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    class SkillSprint : SkillBase
    {
        public SkillSprint()
        {
            Status = SkillStatus.Disabled;
            CostEnargy = 100;
            CostEnargyPerRound = 0;
            CooldownMax = 20;
            ChanneledRoundMax = 20;
        }

        public override void DoBeforeActionMove()
        {
            if (Owner == null || Owner.Target == null)
            {
                Break();
                return;
            }

            switch (Status)
            {
                case SkillStatus.Enabled:
                    {
                        int lastMove = Owner.Moves.Count - 1;
                        if (lastMove < 0) return;

                        double direction = Function.PointRotation(Owner.X, Owner.Y, Owner.Target.X, Owner.Target.Y);
                        float moveX = (float)Math.Cos(direction / 180 * Math.PI) * 25;
                        float moveY = (float)Math.Sin(direction / 180 * Math.PI) * 25;
                        Owner.Moves.Add(new PointF(moveX, moveY));

                        Owner.Propertys.Add(new PropertySmoking(20, 10, 5));
                        Status = SkillStatus.Cooldown;
                        ChanneledRound = 0;
                    }
                    break;
            }
        }

        public override void DoBeforeAction() { }
        public override void DoBeforeActionPlan() { }
        public override void DoAfterAction() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoAfterDead(ObjectActive killer) { }
        public override void DoUseWhenEfficacy(ITarget target) { }
        public override void DoBeforeBreak() { }
    }
}
