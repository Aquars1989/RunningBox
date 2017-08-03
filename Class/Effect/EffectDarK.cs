
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class EffectShark : IEffect
    {
        public EffectStatus Status { get; set; }
        public int Life { get; set; }
        public int Power { get; set; }

        public EffectShark(int life, int power)
        {
            Status = EffectStatus.Alive;
            Life = life;
            Power = power;
        }

        public void DoBeforeAction() { }
        public void DoAfterAction()
        {
            if (Status == EffectStatus.Alive)
            {
                Life--;
                if (Life <= 0)
                {
                    End();
                }
            }
        }

        public void DoBeforeDraw(Graphics g)
        {
            int shakeX = Global.Rand.Next(-Power, Power);
            int shakeY = Global.Rand.Next(-Power, Power);
            g.TranslateTransform(shakeX, shakeY, System.Drawing.Drawing2D.MatrixOrder.Append);
        }

        public void DoBeforeDrawObject(Graphics g) { }
        public void DoAfterDraw(Graphics g) { }

        public void End()
        {
            Status = EffectStatus.Dead;
        }
    }
}
