using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    class EffectShrink : IEffect
    {
        public bool CanBreak { get; set; }
        public SceneBase Scene { get; set; }
        public EffectStatus Status { get; private set; }
        public int DurationRounds { get; set; }
        public int DhrinkRoundsMax { get; private set; }
        public int DhrinkRounds { get; set; }
        public Padding ShrinkPerRound { get; set; }

        public EffectShrink(Padding shrinkPerRound, int duration, int shrinkRounds)
        {
            CanBreak = true;
            Status = EffectStatus.Enabling;
            ShrinkPerRound = shrinkPerRound;
            DurationRounds = duration;
            DhrinkRoundsMax = shrinkRounds;
            DhrinkRounds = 0;
        }

        public void Break()
        {
            if (CanBreak)
            {
                Status = EffectStatus.Disabling;
            }
        }


        public void DoAfterRound()
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    Scene.GameRectangle = new Rectangle
                        (
                        Scene.GameRectangle.Left + ShrinkPerRound.Left,
                        Scene.GameRectangle.Top + ShrinkPerRound.Top,
                        Scene.GameRectangle.Width - ShrinkPerRound.Horizontal,
                        Scene.GameRectangle.Height - ShrinkPerRound.Vertical
                        );

                    if (DhrinkRounds >= DhrinkRoundsMax)
                    {
                        Status = EffectStatus.Enabled;
                        goto case EffectStatus.Enabled;
                    }
                    DhrinkRounds++;
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
                    Scene.GameRectangle = new Rectangle
                      (
                      Scene.GameRectangle.Left - ShrinkPerRound.Left,
                      Scene.GameRectangle.Top - ShrinkPerRound.Top,
                      Scene.GameRectangle.Width + ShrinkPerRound.Horizontal,
                      Scene.GameRectangle.Height + ShrinkPerRound.Vertical
                      );

                    if (DhrinkRounds <= 0)
                    {
                        Status = EffectStatus.Disabled;
                    }
                    DhrinkRounds--;
                    break;
            }
        }

        public void DoBeforeDraw(Graphics g) { }
        public void DoBeforeDrawObject(Graphics g) { }
        public void DoBeforeDrawUI(Graphics g) { }
        public void DoAfterDraw(Graphics g) { }
        public void DoBeforeRound() { }
    }
}
