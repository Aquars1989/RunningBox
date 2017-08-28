
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
        public EffectStatus Status { get; private set; }

        /// <summary>
        /// 渲染色彩
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// 渲染時間計時器(毫秒)
        /// </summary>
        public CounterObject DurationTime { get; private set; }

        /// <summary>
        /// 渲染啟用時間計時器(毫秒)
        /// </summary>
        public CounterObject EnablingTime { get; private set; }

        /// <summary>
        /// 渲染消退時間計時器(毫秒)
        /// </summary>
        public CounterObject DisablingTime { get; private set; }

        /// <summary>
        /// 新增逐漸改變畫面顏色的特效
        /// </summary>
        /// <param name="color">要繪製的顏色</param>
        /// <param name="durationTime">渲染時間(毫秒),小於0為永久</param>
        /// <param name="enablingTime">渲染啟用時間(毫秒)</param>
        /// <param name="disablingTime">渲染消退時間(毫秒)</param>
        public EffectDyeing(Color color, int durationTime, int enablingTime, int disablingTime)
        {
            CanBreak = true;
            Status = EffectStatus.Enabling;
            Color = color;
            DurationTime = new CounterObject(durationTime);
            EnablingTime = new CounterObject(enablingTime);
            DisablingTime = new CounterObject(disablingTime);
        }

        public void DoAfterRound()
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingTime.IsFull)
                    {
                        Status = EffectStatus.Enabled;
                        goto case EffectStatus.Enabled;
                    }
                    else
                    {
                        EnablingTime.Value += Scene.SceneIntervalOfRound;
                    }
                    break;
                case EffectStatus.Enabled:
                    if (DurationTime.IsFull)
                    {
                        Status = EffectStatus.Disabling;
                        goto case EffectStatus.Disabling;
                    }
                    else
                    {
                        DurationTime.Value += Scene.SceneIntervalOfRound;
                    }
                    break;
                case EffectStatus.Disabling:
                    if (DisablingTime.IsFull)
                    {
                        Status = EffectStatus.Disabled;
                    }
                    else
                    {
                        DisablingTime.Value += Scene.SceneIntervalOfRound;
                    }
                    break;
            }
        }

        public void DoBeforeDrawFloor(Graphics g)
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingTime.Value > 0)
                    {
                        int alpha = (int)(EnablingTime.GetRatio() * Color.A + 0.5F);
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
                    if (DisablingTime.Value > 0)
                    {
                        int alpha = (int)((1 - DisablingTime.GetRatio()) * Color.A + 0.5F);

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
