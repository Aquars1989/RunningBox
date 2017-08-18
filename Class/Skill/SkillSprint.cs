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
        /// 衝刺距離倍數(以所有者速度為基準)
        /// </summary>
        public float SpeedMultiple { get; set; }

        /// <summary>
        /// 衝刺距離常數
        /// </summary>
        public int SpeedConstant { get; set; }

        /// <summary>
        /// 是否加上冒煙特效
        /// </summary>
        public bool Smoke { get; set; }

        /// <summary>
        /// 新增衝刺技能,提升最後一次移動距離
        /// </summary>
        /// <param name="costEnargy">耗費能量</param>
        /// <param name="cooldown">冷卻時間(毫秒)</param>
        /// <param name="speedMultiple">衝刺距離倍數(以所有者速度為基準)</param>
        /// <param name="speedConstant">衝刺距離常數</param>
        /// <param name="smoke">是否加上冒煙特效</param>
        public SkillSprint(int costEnargy, int cooldown, int speedMultiple, int speedConstant, bool smoke)
        {
            Status = SkillStatus.Disabled;
            CostEnargy = costEnargy;
            CooldownLimit = cooldown;
            SpeedMultiple = speedMultiple;
            SpeedConstant = speedConstant;
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

                        double direction = Function.GetAngle(Owner.X, Owner.Y, Owner.Target.X, Owner.Target.Y);
                        float speed = (Owner.Speed * SpeedMultiple) + SpeedConstant;
                        Owner.Moves.Add(Owner.GetMovePoint(direction, speed));

                        if (Smoke)
                        {
                            Owner.Propertys.Add(new PropertySmoking(Owner.MaxMoves, Owner.Size, Owner.Scene.SecToRounds(0.06F)));
                        }
                        Status = SkillStatus.Cooldown;
                    }
                    break;
            }
        }

        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="drawButton">繪製熱鍵</param>
        /// <returns>繪圖物件</returns>
        public override DrawIconBase GetDrawObject(Color color, EnumSkillButton drawButton)
        {
            DrawIconSprint drawObject = new DrawIconSprint(color, drawButton) { BindingSkill = this };
            return drawObject;
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
