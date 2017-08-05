
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class EffectDyeing : IEffect
    {
        public bool CanBreak { get; set; }
        public SceneBase Scene { get; set; }
        public EffectStatus Status { get; set; }
        public Color Color { get; set; }
        public int DurationRounds { get; set; }
        public int EnablingRoundsMax { get; private set; }
        public int DisablingRoundsMax { get; private set; }
        public int EnablingRounds { get; set; }
        public int DisablingRounds { get; set; }

        public EffectDyeing(Color color,int duration, int enablingRounds, int disablingRounds)
        {
            CanBreak = true;
            Status = EffectStatus.Enabling;
            Color = color;
            DurationRounds = duration;
            EnablingRoundsMax = enablingRounds;
            EnablingRounds = enablingRounds;
            DisablingRoundsMax = disablingRounds;
            DisablingRounds = disablingRounds;
        }

        public void DoAfterRound()
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingRounds <= 0)
                    {
                        Status = EffectStatus.Enabled;
                        goto case EffectStatus.Enabled;
                    }
                    EnablingRounds--;
                    break;
                case EffectStatus.Enabled:
                    if (DurationRounds <= 0)
                    {
                        Status = EffectStatus.Disabling;
                        goto case EffectStatus.Disabling;
                    }
                    DurationRounds--;
                    break;
                case EffectStatus.Disabling:
                    if (DisablingRounds <= 0)
                    {
                        Status = EffectStatus.Disabled;
                    }
                    DisablingRounds--;
                    break;
            }
        }

        public void DoBeforeDrawUI(Graphics g)
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingRoundsMax > 0)
                    {
                        int alpha = (int)((float)(EnablingRoundsMax - EnablingRounds) / EnablingRoundsMax * Color.A);
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
                        int alpha = (int)((float)(DisablingRounds) / DisablingRoundsMax * Color.A);

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
    }
}
