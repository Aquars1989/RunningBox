using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 衝刺技能,提升最後一次移動距離
    /// </summary>
    public class SkillSprint : SkillBase
    {
        /// <summary>
        /// 衝刺距離倍數
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// 是否加上冒煙特效
        /// </summary>
        public bool Smoke { get; set; }

        /// <summary>
        /// 新增衝刺技能,提升最後一次移動距離
        /// </summary>
        /// <param name="costEnargy">耗費能量</param>
        /// <param name="cooldown">冷卻回合數</param>
        /// <param name="power">衝刺距離倍數</param>
        /// <param name="smoke">是否加上冒煙特效</param>
        public SkillSprint(int costEnargy, int cooldown, int power, bool smoke)
        {
            Status = SkillStatus.Disabled;
            CostEnargy = costEnargy;
            CooldownRoundMax = cooldown;
            Power = power;
            Smoke = smoke;

            
        }

        public override void DoBeforeActionMove()
        {
            switch (Status)
            {
                case SkillStatus.Enabled:
                    {
                        if (Owner == null || Owner.Target == null)
                        {
                            Break();
                            return;
                        }

                        int lastMove = Owner.Moves.Count - 1;
                        if (lastMove < 0) return;

                        double direction = Function.PointRotation(Owner.X, Owner.Y, Owner.Target.X, Owner.Target.Y);
                        float moveX = (float)Math.Cos(direction / 180 * Math.PI) * Power;
                        float moveY = (float)Math.Sin(direction / 180 * Math.PI) * Power;
                        Owner.Moves.Add(new PointF(moveX, moveY));

                        if (Smoke)
                        {
                            Owner.Propertys.Add(new PropertySmoking(Owner.MaxMoves, Owner.Size, 5));
                        }
                        Status = SkillStatus.Cooldown;
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
        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType) { }
        public override void DoUseWhenEfficacy(ITarget target) { }
        public override void DoBeforeEnd(SkillEndType endType) { }
    }
}
