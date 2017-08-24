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
        private float _SceneSlow;//實際減慢的速度
        private ObjectUI _MiniBar;

        /// <summary>
        /// 減慢程度(1為原速度的50%)
        /// </summary>
        public float SlowRate { get; set; }

        /// <summary>
        /// 新增子彈時間技能 降低世界速度
        /// </summary>
        /// <param name="costEnergy">耗費能量</param>
        /// <param name="costEnergyPerSec">每秒耗費能量</param>
        /// <param name="channeled">最大引導時間(毫秒</param>
        /// <param name="cooldown">冷卻時間(毫秒)</param>
        /// <param name="slowRate">減慢程度(1為原速度的50%</param>
        public SkillBulletTime(int costEnergy, int costEnergyPerSec, int channeled, int cooldown, float slowRate)
        {
            Status = SkillStatus.Disabled;
            CostEnergy = costEnergy;
            CostEnergyPerSec = costEnergyPerSec;
            ChanneledLimit = channeled;
            CooldownLimit = cooldown;
            SlowRate = slowRate;
        }

        public override void DoUseWhenEfficacy(ITarget target)
        {
            Break();
        }

        public override void DoBeforeAction()
        {
            switch (Status)
            {
                case SkillStatus.Enabled:
                    {
                        _SceneSlow = SlowRate;
                        Owner.Scene.SceneSlow += _SceneSlow;
                        _MiniBar = new ObjectUI(ContentAlignment.MiddleCenter, 0, 20, 30, 6, new DrawUiEnergyBar(Colors.EnergyBar, 1, Owner));
                        _MiniBar.Layout.DependTarget = new TargetObject(Owner);
                        Owner.Scene.UIObjects.Add(_MiniBar);
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
                        Owner.Scene.SceneSlow -= _SceneSlow;
                        _MiniBar.Status = ObjectStatus.Dead;
                    }
                    break;
            }
        }

        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <returns>繪圖物件</returns>
        public override IDrawSkill GetDrawObject(Color color)
        {
            DrawSkillBulletTime drawObject = new DrawSkillBulletTime(color, this );
            return drawObject;
        }

        public override void DoBeforeActionPlan() { }
        public override void DoAfterAction() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType) { }
        public override void DoBeforeActionMove() { }
    }
}
