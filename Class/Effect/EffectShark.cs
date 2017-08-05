
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class EffectShark : IEffect
    {
        public bool CanBreak { get; set; }
        public SceneBase Scene { get; set; }
        public EffectStatus Status { get; private set; }
        public int DurationRounds { get; set; }
        public int Power { get; set; }

        public EffectShark(int duration, int power)
        {
            CanBreak = true;
            Status = EffectStatus.Enabled;
            DurationRounds = duration;
            Power = power;
        }

        public void DoAfterRound()
        {
            if (Status == EffectStatus.Enabled)
            {
                DurationRounds--;
                if (DurationRounds <= 0)
                {
                    Status = EffectStatus.Disabled;
                }
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
        public void DoBeforeDrawUI(Graphics g) { }
        public void DoBeforeDrawObject(Graphics g) { }
        public void DoAfterDraw(Graphics g) { }
    }
}
