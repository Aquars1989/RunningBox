using System;
using System.Drawing;

namespace RunningBox
{
    /// <summary>
    /// 特效介面
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// 是否可被中斷
        /// </summary>
        bool CanBreak { get; set; }

        /// <summary>
        /// 作用場景物件
        /// </summary>
        SceneBase Scene { get; set; }

        /// <summary>
        /// 特效狀態
        /// </summary>
        EffectStatus Status { get;}

        /// <summary>
        /// 物件活動前執行動作
        /// </summary>
        void DoBeforeRound();

        /// <summary>
        /// 物件活動後執行動作
        /// </summary>
        void DoAfterRound();

        /// <summary>
        /// 背景繪製前執行動作
        /// </summary>
        void DoBeforeDraw(Graphics g);

        /// <summary>
        /// 繪製背景前執行動作
        /// </summary>
        void DoBeforeDrawFloor(Graphics g);

        /// <summary>
        /// 背景繪製後，物件繪製前執行動作
        /// </summary>
        void DoBeforeDrawObject(Graphics g);

        /// <summary>
        /// 繪製UI前執行動作
        /// </summary>
        void DoBeforeDrawUI(Graphics g);

        /// <summary>
        /// 繪製後執行動作
        /// </summary>
        void DoAfterDraw(Graphics g);

        /// <summary>
        /// 中斷特效
        /// </summary>
        void Break();
    }

    /// <summary>
    /// 特效狀態
    /// </summary>
    public enum EffectStatus
    {
        /// <summary>
        /// 失效，等待回收
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// 生效中
        /// </summary>
        Enabling = 1,

        /// <summary>
        /// 生效
        /// </summary>
        Enabled = 2,

        /// <summary>
        /// 失效中
        /// </summary>
        Disabling = 3
    }
}
