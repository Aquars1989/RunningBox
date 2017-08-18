﻿
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 畫面震動的特效
    /// </summary>
    public class EffectShark : IEffect
    {
        /// <summary>
        /// 特效是否可被中斷
        /// </summary>
        public bool CanBreak { get; set; }

        /// <summary>
        /// 作用場景物件
        /// </summary>
        public SceneBase Scene { get; set; }

        /// <summary>
        /// 特效狀態
        /// </summary>
        public EffectStatus Status { get; private set; }

        /// <summary>
        /// 震動持續的時間最大值(毫秒)
        /// </summary>
        public int DurationLimit { get; set; }

        /// <summary>
        /// 震動持續的時間計數(毫秒)
        /// </summary>
        public int DurationTicks { get; set; }

        /// <summary>
        /// 震動的強度
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// 新增畫面震動的特效
        /// </summary>
        /// <param name="duration">震動持續的時間(毫秒),小於0為永久</param>
        /// <param name="power">震動的強度</param>
        public EffectShark(int duration, int power)
        {
            CanBreak = true;
            Status = EffectStatus.Enabled;
            DurationLimit = duration;
            Power = power;
        }

        public void DoAfterRound()
        {
            if (Status == EffectStatus.Enabled)
            {
                if (DurationLimit >= 0 && DurationTicks >= DurationLimit)
                {
                    Status = EffectStatus.Disabled;
                }
                DurationTicks += Scene.IntervalOfRound;
            }
        }

        public void DoBeforeDraw(Graphics g)
        {
            int shakeX = Global.Rand.Next(-Power, Power);
            int shakeY = Global.Rand.Next(-Power, Power);
            g.TranslateTransform(shakeX, shakeY, System.Drawing.Drawing2D.MatrixOrder.Append);
        }

        public void Break()
        {
            if (CanBreak)
            {
                Status = EffectStatus.Disabled;
            }
        }

        public void DoBeforeRound() { }
        public void DoBeforeDrawBack(Graphics g) { }
        public void DoBeforeDrawObject(Graphics g) { }
        public void DoAfterDraw(Graphics g) { }
        public void DoBeforeDrawUI(Graphics g) { }
    }
}
