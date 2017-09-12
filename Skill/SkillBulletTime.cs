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
        private PropertyUI _MiniBar;

        /// <summary>
        /// 說明文字
        /// </summary>
        public override string Info
        {
            get { return string.Format("減緩時間流逝速度{0:P0}", SlowRate); }
        }

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
            Channeled = new CounterObject(channeled);
            Cooldown = new CounterObject(cooldown);
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
                        _MiniBar = new PropertyUI(-1, new Size(30, 6), new DrawUICounterBar(GlobalColors.EnergyBar, Color.Black, Color.White, 1, false, Owner.Energy));
                        Owner.Propertys.Add(_MiniBar);
                        Status = SkillStatus.Channeled;
                    }
                    break;
            }
        }


        public override void DoAfterEnd(SkillEndType endType)
        {
            switch (endType)
            {
                case SkillEndType.ChanneledBreak:
                    {
                        Owner.Scene.SceneSlow -= _SceneSlow;
                        _MiniBar.End(PropertyEndType.Break);
                    }
                    break;
                case SkillEndType.Finish:
                    {
                        Owner.Scene.SceneSlow -= _SceneSlow;
                        _MiniBar.End(PropertyEndType.Finish);
                    }
                    break;
            }
        }

        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <returns>繪圖物件</returns>
        public override DrawSkillBase GetDrawObject(Color color)
        {
            DrawSkillBulletTime drawObject = new DrawSkillBulletTime(color, this);
            return drawObject;
        }
    }
}
