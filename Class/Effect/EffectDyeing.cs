
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 逐漸改變畫面顏色的特效
    /// </summary>
    public class EffectDyeing : IEffect
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
        public EffectStatus Status { get; set; }

        /// <summary>
        /// 渲染色彩
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// 渲染的時間最大值(毫秒)
        /// </summary>
        public int DurationLimit { get; set; }

        /// <summary>
        /// 渲染的時間計數(毫秒)
        /// </summary>
        public int DurationTicks { get; set; }

        /// <summary>
        /// 渲染啟用時間最大值(毫秒)
        /// </summary>
        public int EnablingLimit { get; private set; }

        /// <summary>
        /// 渲染啟用時間計數(毫秒)
        /// </summary>
        public int EnablingTicks { get; set; }

        /// <summary>
        /// 渲染消退時間最大值(毫秒)
        /// </summary>
        public int DisablingLimit { get; private set; }

        /// <summary>
        /// 渲染消退時間計數(毫秒)
        /// </summary>
        public int DisablingTicks { get; set; }

        /// <summary>
        /// 新增逐漸改變畫面顏色的特效
        /// </summary>
        /// <param name="color">要繪製的顏色</param>
        /// <param name="duration">渲染時間(毫秒),小於0為永久</param>
        /// <param name="enablingTime">渲染啟用時間(毫秒)</param>
        /// <param name="disablingTime">渲染消退時間(毫秒)</param>
        public EffectDyeing(Color color, int duration, int enablingTime, int disablingTime)
        {
            CanBreak = true;
            Status = EffectStatus.Enabling;
            Color = color;
            DurationLimit = duration;
            EnablingLimit = enablingTime;
            DisablingLimit = disablingTime;
        }

        public void DoAfterRound()
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingTicks >= EnablingLimit)
                    {
                        Status = EffectStatus.Enabled;
                        goto case EffectStatus.Enabled;
                    }
                    EnablingTicks += Scene.IntervalOfRound;
                    break;
                case EffectStatus.Enabled:
                    if (DurationLimit >= 0 && DurationTicks >= DurationLimit)
                    {
                        Status = EffectStatus.Disabling;
                        goto case EffectStatus.Disabling;
                    }
                    DurationTicks += Scene.IntervalOfRound;
                    break;
                case EffectStatus.Disabling:
                    if (DisablingTicks >= DisablingLimit)
                    {
                        Status = EffectStatus.Disabled;
                    }
                    DisablingTicks+= Scene.IntervalOfRound;;
                    break;
            }
        }

        public void DoBeforeDrawBack(Graphics g)
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingLimit > 0)
                    {
                        int alpha = (int)((float)(EnablingTicks) / EnablingLimit * Color.A);
                        if (alpha < 0) alpha = 0;
                        else if (alpha > 255) alpha = 255;
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, Color.R, Color.G, Color.B)))
                        {
                            g.FillRectangle(brush, Scene.ClientRectangle);
                        }
                    }
                    break;
                case EffectStatus.Enabled:
                    using (SolidBrush brush = new SolidBrush(Color))
                    {
                        g.FillRectangle(brush, Scene.ClientRectangle);
                    }
                    break;
                case EffectStatus.Disabling:
                    if (DisablingLimit > 0)
                    {
                        int alpha = (int)((float)(DisablingLimit - DisablingTicks) / DisablingLimit * Color.A);

                        if (alpha < 0) alpha = 0;
                        else if (alpha > 255) alpha = 255;
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, Color.R, Color.G, Color.B)))
                        {
                            g.FillRectangle(brush, Scene.ClientRectangle);
                        }
                    }
                    break;
            }
        }

        public void Break()
        {
            if (CanBreak)
            {
                Status = EffectStatus.Disabling;
            }
        }

        public void DoBeforeRound() { }
        public void DoBeforeDraw(Graphics g) { }
        public void DoBeforeDrawObject(Graphics g) { }
        public void DoAfterDraw(Graphics g) { }
        public void DoBeforeDrawUI(Graphics g) { }
    }
}
