
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能狀態
    /// </summary>
    public enum SkillStatus
    {
        /// <summary>
        /// 失效
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// 生效
        /// </summary>
        Enabled = 1,

        /// <summary>
        /// 引導中
        /// </summary>
        Channeled = 2,

        /// <summary>
        /// 冷卻中
        /// </summary>
        Cooldown = 3
    }

    public abstract class SkillBase
    {
        public ITarget Target { get; set; }
        public ObjectActive Owner { get; set; }
        public SkillStatus Status { get; set; }
        public int CooldownMax { get; set; }
        public int Cooldown { get; set; }
        public int CostEnargy { get; set; }
        public int CostEnargyPerRound { get; set; }
        public int ChanneledRoundMax { get; set; }
        public int ChanneledRound { get; set; }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="target">技能目標</param>
        public virtual void Use(ITarget target)
        {
            switch (Status)
            {
                case SkillStatus.Channeled:
                    DoUseWhenEfficacy(target);
                    break;
                case SkillStatus.Disabled:
                    if (Owner == null)
                    {
                        Status = SkillStatus.Enabled;
                        Target = target;
                    }
                    else if (Owner.Energy > CostEnargy)
                    {
                        Owner.Energy -= CostEnargy;
                        Status = SkillStatus.Enabled;
                        Target = target;
                    }
                    break;
            }

        }

        /// <summary>
        /// 在回合動作最後執行
        /// </summary>
        public virtual void Settlement()
        {
            switch (Status)
            {
                case SkillStatus.Channeled:
                    if (ChanneledRoundMax <= 0 || ChanneledRound < ChanneledRoundMax)
                    {
                        ChanneledRound++;
                    }
                    else
                    {
                        Break();
                        goto case SkillStatus.Cooldown;
                    }

                    if (Owner.Energy >= CostEnargyPerRound)
                    {
                        Owner.Energy -= CostEnargyPerRound;
                    }
                    else
                    {
                        Break();
                        goto case SkillStatus.Cooldown;
                    }
                    break;
                case SkillStatus.Cooldown:
                    if (Cooldown >= CooldownMax)
                    {
                        Status = SkillStatus.Disabled;
                    }
                    Cooldown++;
                    break;
            }
        }

        /// <summary>
        /// 中斷技能
        /// </summary>
        public virtual void Break()
        {
            DoBeforeBreak();
            switch (Status)
            {
                case SkillStatus.Enabled:
                    Owner.Energy += CostEnargy;
                    Status = SkillStatus.Cooldown;
                    Cooldown = 0;
                    break;
                case SkillStatus.Channeled:
                    Status = SkillStatus.Cooldown;
                    Cooldown = 0;
                    break;
            }

        }

        /// <summary>
        /// 當技能生效時又使用技能時的動作
        /// </summary>
        public abstract void DoUseWhenEfficacy(ITarget target);

        /// <summary>
        /// 物件活動前執行動作
        /// </summary>
        public abstract void DoBeforeAction();

        /// <summary>
        /// 物件能量調整前執行動作
        /// </summary>
        public abstract void DoBeforeActionEnergyGet();

        /// <summary>
        /// 物件規劃活動前執行動作
        /// </summary>
        public abstract void DoBeforeActionPlan();

        /// <summary>
        /// 物件移動前執行動作
        /// </summary>
        public abstract void DoBeforeActionMove();

        /// <summary>
        /// 物件活動後執行動作
        /// </summary>
        public abstract void DoAfterAction();
        
        /// <summary>
        /// 繪製前執行動作
        /// </summary>
        public abstract void DoBeforeDraw(Graphics g);

        /// <summary>
        /// 繪製後執行動作
        /// </summary>
        public abstract void DoAfterDraw(Graphics g);

        /// <summary>
        /// 死亡後執行動作
        /// </summary>
        public abstract void DoAfterDead(ObjectActive killer);

        /// <summary>
        /// 技能中斷前執行(未發動前中斷,引導時中斷)
        /// </summary>
        public abstract void DoBeforeBreak();
    }
}
