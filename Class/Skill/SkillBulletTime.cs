using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 子彈時間 降低世界速度 不影響生命計時
    /// </summary>
    public class SkillBulletTime : SkillBase
    {
        private float _WorldSpeedSlow = 0;
        /// <summary>
        /// 速度調整
        /// </summary>
        public float SlowRate { get; set; }

        /// <summary>
        /// 新增子彈時間技能 降低世界速度 不影響生命計時
        /// </summary>
        /// <param name="costEnargy">耗費能量</param>
        /// <param name="costEnargyPerRound">每回合耗費能量</param>
        /// <param name="channeledRound">最大引導時間</param>
        /// <param name="cooldown">冷卻時間</param>
        /// <param name="rate">減慢程度1=100%</param>
        public SkillBulletTime(int costEnargy, int costEnargyPerRound, int channeledRound, int cooldown, float slowRate)
        {
            Status = SkillStatus.Disabled;
            CostEnargy = costEnargy;
            CostEnargyPerRound = costEnargyPerRound;
            ChanneledRoundMax = channeledRound;
            CooldownRoundMax = cooldown;
            SlowRate = slowRate;
        }

        public override void DoBeforeAction()
        {
            switch (Status)
            {
                case SkillStatus.Enabled:
                    {
                        _WorldSpeedSlow = SlowRate;
                        Owner.Scene.WorldSpeedSlow += _WorldSpeedSlow;
                        Status = SkillStatus.Channeled;
                    }
                    break;
            }
        }


        public override void DoBeforeEnd(SkillEndType endType)
        {
            switch (endType)
            {
                case SkillEndType.ChanneledBreak:
                case SkillEndType.Finish:
                    {
                        Owner.Scene.WorldSpeedSlow -= _WorldSpeedSlow;
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
            DrawIconBulletTime drawObject = new DrawIconBulletTime(color, drawButton) { BindingSkill = this };
            return drawObject;
        }

        public override void DoBeforeActionPlan() { }
        public override void DoAfterAction() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType) { }
        public override void DoUseWhenEfficacy(ITarget target) { }
        public override void DoBeforeActionMove() { }
    }
}
