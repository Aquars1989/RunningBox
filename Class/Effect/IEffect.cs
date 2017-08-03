
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public enum EffectStatus
    {
        Alive = 0,
        Dead = 2,
    }

    public interface IEffect
    {
        EffectStatus Status { get; set; }
        void DoBeforeAction();
        void DoAfterAction();
        void DoBeforeDraw(Graphics g);
        void DoBeforeDrawObject(Graphics g);
        void DoAfterDraw(Graphics g);
        void End();
    }
}
