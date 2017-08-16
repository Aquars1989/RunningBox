
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
        public SceneGaming Scene { get; set; }

        /// <summary>
        /// 特效狀態
        /// </summary>
        public EffectStatus Status { get; set; }

        /// <summary>
        /// 渲染色彩
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// 渲染回合數最大值
        /// </summary>
        public int DurationRoundMax { get; set; }

        /// <summary>
        /// 渲染回合數計數
        /// </summary>
        public int DurationRound { get; set; }

        /// <summary>
        /// 渲染啟用回合數最大值
        /// </summary>
        public int EnablingRoundsMax { get; private set; }

        /// <summary>
        /// 渲染啟用回合數計數
        /// </summary>
        public int EnablingRounds { get; set; }

        /// <summary>
        /// 渲染消退回合數最大值
        /// </summary>
        public int DisablingRoundsMax { get; private set; }

        /// <summary>
        /// 渲染消退回合數計數
        /// </summary>
        public int DisablingRounds { get; set; }

        /// <summary>
        /// 新增逐漸改變畫面顏色的特效
        /// </summary>
        /// <param name="color">要繪製的顏色</param>
        /// <param name="duration">渲染回合數,小於0為永久</param>
        /// <param name="enablingRounds">渲染啟用回合數</param>
        /// <param name="disablingRounds">渲染消退回合數</param>
        public EffectDyeing(Color color, int duration, int enablingRounds, int disablingRounds)
        {
            CanBreak = true;
            Status = EffectStatus.Enabling;
            Color = color;
            DurationRoundMax = duration;
            EnablingRoundsMax = enablingRounds;
            DisablingRoundsMax = disablingRounds;
        }

        public void DoAfterRound()
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingRounds >= EnablingRoundsMax)
                    {
                        Status = EffectStatus.Enabled;
                        goto case EffectStatus.Enabled;
                    }
                    EnablingRounds++;
                    break;
                case EffectStatus.Enabled:
                    if (DurationRoundMax >= 0 && DurationRound >= DurationRoundMax)
                    {
                        Status = EffectStatus.Disabling;
                        goto case EffectStatus.Disabling;
                    }
                    DurationRound++;
                    break;
                case EffectStatus.Disabling:
                    if (DisablingRounds >= DisablingRoundsMax)
                    {
                        Status = EffectStatus.Disabled;
                    }
                    DisablingRounds++;
                    break;
            }
        }

        public void DoBeforeDrawBack(Graphics g)
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingRoundsMax > 0)
                    {
                        int alpha = (int)((float)(EnablingRounds) / EnablingRoundsMax * Color.A);
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
                    if (DisablingRoundsMax > 0)
                    {
                        int alpha = (int)((float)(DisablingRoundsMax - DisablingRounds) / DisablingRoundsMax * Color.A);

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
