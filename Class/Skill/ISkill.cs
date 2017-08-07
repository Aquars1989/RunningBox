
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
        /// 生效中
        /// </summary>
        Enabling = 0,

        /// <summary>
        /// 生效
        /// </summary>
        Enabled = 1,

        /// <summary>
        /// 失效中
        /// </summary>
        Disabling = 2,

        /// <summary>
        /// 失效
        /// </summary>
        Disabled = 3
    }

    public interface ISkill
    {
        ObjectBase Owner { get; set; }
        SkillStatus Status { get; set; }
        int Cooldown { get; set; }
        int CostEnargy { get; set; }
        int CostEnargyPerRound { get; set; }

        void Use();

        /// <summary>
        /// 物件活動前執行動作
        /// </summary>
        void DoBeforeAction();

        /// <summary>
        /// 物件能量調整前執行動作
        /// </summary>
        void DoBeforeActionEnergyGet();

        /// <summary>
        /// 物件規劃活動前執行動作
        /// </summary>
        void DoBeforeActionPlan();

        /// <summary>
        /// 物件移動前執行動作
        /// </summary>
        void DoBeforeActionMove();

        /// <summary>
        /// 物件活動後執行動作
        /// </summary>
        void DoAfterAction();

        /// <summary>
        /// 繪製前執行動作
        /// </summary>
        void DoBeforeDraw(Graphics g);

        /// <summary>
        /// 繪製後執行動作
        /// </summary>
        void DoAfterDraw(Graphics g);

        /// <summary>
        /// 死亡後執行動作
        /// </summary>
        void DoAfterDead(ObjectActive killer);

        /// <summary>
        /// 中斷技能
        /// </summary>
        void Break();
    }
}
