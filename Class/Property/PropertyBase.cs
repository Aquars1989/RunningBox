
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 特性基礎物件
    /// </summary>
    public abstract class PropertyBase
    {
        /// <summary>
        /// 特性目標
        /// </summary>
        public ITarget Target { get; set; }

        /// <summary>
        /// 特性所有者
        /// </summary>
        public ObjectActive Owner { get; set; }

        /// <summary>
        /// 特性狀態
        /// </summary>
        public PropertyStatus Status { get; set; }

        /// <summary>
        /// 特性持續時間最大值(毫秒),小於0為永久
        /// </summary>
        public int DurationLimit { get; set; }

        /// <summary>
        /// 特性持續時間計數(毫秒)
        /// </summary>
        public int DurationTicks { get; set; }

        public PropertyBase()
        {
            DurationLimit = -1;
        }

        /// <summary>
        /// 在回合動作最後執行
        /// </summary>
        public virtual void Settlement()
        {
            if (Status == PropertyStatus.Enabled)
            {
                if (DurationLimit >= 0 && DurationTicks >= DurationLimit)
                {
                    DoBeforeEnd(PropertyEndType.Finish);
                    Status = PropertyStatus.Disabled;
                }
                DurationTicks += Owner.Scene.SceneIntervalOfRound;
            }
        }

        /// <summary>
        /// 取消特性效果
        /// </summary>
        public virtual void Break()
        {
            if (Status == PropertyStatus.Disabled) return;

            DoBeforeEnd(PropertyEndType.Break);
            Status = PropertyStatus.Disabled;
        }

        /// <summary>
        /// 所有者死亡後執行動作
        /// </summary>
        public virtual void DoAfterDead(ObjectActive killer, ObjectDeadType deadType)
        {
            Break();
        }

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
        /// 特性結束前執行
        /// </summary>
        public abstract void DoBeforeEnd(PropertyEndType endType);
    }

    /// <summary>
    /// 特性狀態
    /// </summary>
    public enum PropertyStatus
    {
        /// <summary>
        /// 失效，等待回收
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// 生效
        /// </summary>
        Enabled = 1
    }

    /// <summary>
    /// 特性結束類型
    /// </summary>
    public enum PropertyEndType
    {
        /// <summary>
        /// 持續時間結束
        /// </summary>
        Finish = 0,

        /// <summary>
        /// 被中斷
        /// </summary>
        Break = 1
    }
}
