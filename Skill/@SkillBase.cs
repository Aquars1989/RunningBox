
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 技能基礎物件
    /// </summary>
    public abstract class SkillBase
    {
        //說明文字用
        private static Font _InfoFont = new Font("微軟正黑體", 12);

        /// <summary>
        /// 說明文字
        /// </summary>
        public abstract string Info { get; }

        /// <summary>
        /// 自動施放物件
        /// </summary>
        public AutoCastBase AutoCastObject { get; set; }

        /// <summary>
        /// 技能目標
        /// </summary>
        public ITarget Target { get; set; }

        /// <summary>
        /// 技能所有人
        /// </summary>
        public ObjectActive Owner { get; set; }

        /// <summary>
        /// 技能冷卻時間計數器(毫秒)
        /// </summary>
        public CounterObject Cooldown { get; protected set; }

        /// <summary>
        /// 引導型技能引導時間計數器(毫秒)
        /// </summary>
        public CounterObject Channeled { get; protected set; }

        /// <summary>
        /// 技能耗費能量
        /// </summary>
        public int CostEnergy { get; set; }

        /// <summary>
        /// 引導型技能每秒耗費能量
        /// </summary>
        public int CostEnergyPerSec { get; set; }

        private SkillStatus _Status;
        /// <summary>
        /// 技能狀態
        /// </summary>
        public SkillStatus Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value) return;

                _Status = value;
                switch (_Status)
                {
                    case SkillStatus.Cooldown:
                        Cooldown.Value = 0;
                        break;
                    case SkillStatus.Channeled:
                        Channeled.Value = 0;
                        break;
                }
            }
        }

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
                    else if (Owner.Energy.Value > CostEnergy)
                    {
                        Owner.Energy.Value -= CostEnergy;
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
                    if (Channeled.IsFull)
                    {
                        Status = SkillStatus.Cooldown;
                        DoAfterEnd(SkillEndType.Finish);
                        goto case SkillStatus.Cooldown;
                    }
                    else
                    {
                        Channeled.Value += Owner.Scene.SceneIntervalOfRound;
                    }

                    int costEnergy = (int)(CostEnergyPerSec / Owner.Scene.SceneRoundPerSec + 0.5F);
                    if (Owner.Energy.Value >= costEnergy)
                    {
                        Owner.Energy.Value -= costEnergy;
                    }
                    else
                    {
                        Status = SkillStatus.Cooldown;
                        DoAfterEnd(SkillEndType.ChanneledBreak);
                        goto case SkillStatus.Cooldown;
                    }
                    break;
                case SkillStatus.Cooldown:
                    if (Cooldown.IsFull)
                    {
                        Status = SkillStatus.Disabled;
                    }
                    else
                    {
                        Cooldown.Value += Owner.Scene.SceneIntervalOfRound;
                    }
                    break;
            }
        }

        /// <summary>
        /// 中斷技能
        /// </summary>
        public virtual void Break()
        {
            switch (Status)
            {
                case SkillStatus.Enabled:
                    Owner.Energy.Value += CostEnergy;
                    Status = SkillStatus.Disabled;
                    DoAfterEnd(SkillEndType.CastBreak);
                    break;
                case SkillStatus.Channeled:
                    Status = SkillStatus.Cooldown;
                    DoAfterEnd(SkillEndType.ChanneledBreak);
                    break;
            }
        }

        /// <summary>
        /// 重設技能
        /// </summary>
        public virtual void Reset()
        {
            Break();
            Status = SkillStatus.Disabled;
        }

        /// <summary>
        /// 當技能生效時又使用技能時的動作
        /// </summary>
        public virtual void DoUseWhenEfficacy(ITarget target) { }

        /// <summary>
        /// 物件活動前執行動作
        /// </summary>
        public virtual void DoBeforeAction() { }

        /// <summary>
        /// 物件能量調整前執行動作
        /// </summary>
        public virtual void DoBeforeActionEnergyGet() { }

        /// <summary>
        /// 物件規劃活動前執行動作
        /// </summary>
        public virtual void DoBeforeActionPlan() { }

        /// <summary>
        /// 物件移動前執行動作
        /// </summary>
        public virtual void DoBeforeActionMove() { }

        /// <summary>
        /// 物件活動後執行動作
        /// </summary>
        public virtual void DoAfterAction() { }

        /// <summary>
        /// 繪製前執行動作
        /// </summary>
        public virtual void DoBeforeDraw(Graphics g) { }

        /// <summary>
        /// 繪製後執行動作
        /// </summary>
        public virtual void DoAfterDraw(Graphics g) { }

        /// <summary>
        /// 死亡後執行動作
        /// </summary>
        public virtual void DoAfterDead(ObjectActive killer, ObjectDeadType deadType)
        {
            Break();
        }

        /// <summary>
        /// 技能結束卻後(包含中斷)執行
        /// </summary>
        public virtual void DoAfterEnd(SkillEndType endType) { }

        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <returns>繪圖物件</returns>
        public abstract DrawSkillBase GetDrawObject(Color color);

        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <returns>繪圖物件</returns>
        public virtual DrawUITextFrame GetInfoObject(Color color, Color backColor, Color borderColor)
        {
            DrawUITextFrame result = new DrawUITextFrame(color, Color.WhiteSmoke, backColor, borderColor, 2, 10, Info, _InfoFont, GlobalFormat.TopLeft);
            return result;
        }
    }
}
