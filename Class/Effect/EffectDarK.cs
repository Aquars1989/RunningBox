
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class EffectDark : IEffect
    {
        public SceneBase Scene { get; set; }
        public EffectStatus Status { get; set; }
        public int DurationRounds { get; set; }
        public int EnablingRoundsMax { get; private set; }
        public int DisablingRoundsMax { get; private set; }
        public int EnablingRounds { get; set; }
        public int DisablingRounds { get; set; }

        public EffectDark(int duration, int enablingRounds, int disablingRounds)
        {
            Status = EffectStatus.Enabling;
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
                        Break();
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

        public void DoBeforeDrawObject(Graphics g)
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingRoundsMax > 0)
                    {
                        int alpha = (int)(EnablingRounds / EnablingRoundsMax * 255F);

                        if (alpha < 0) alpha = 0;
                        else if (alpha > 255) alpha = 255;
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                        {
                            g.FillRectangle(brush, Scene.ClientRectangle);
                        }
                    }
                    break;
                case EffectStatus.Enabled:
                    g.FillRectangle(Brushes.Black, Scene.ClientRectangle);
                    break;
                case EffectStatus.Disabling:
                    if (DisablingRoundsMax > 0)
                    {
                        int alpha = (int)((EnablingRoundsMax - DisablingRounds) / EnablingRoundsMax * 255F);

                        if (alpha < 0) alpha = 0;
                        else if (alpha > 255) alpha = 255;
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                        {
                            g.FillRectangle(brush, Scene.ClientRectangle);
                        }
                    }
                    break;
            }
        }

        public void Break()
        {
            Status = EffectStatus.Disabled;
        }

        public void DoBeforeRound() { }
        public void DoBeforeDraw(Graphics g) { }
        public void DoAfterDraw(Graphics g) { }
    }
}
