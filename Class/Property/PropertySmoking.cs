using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    class PropertySmoking : PropertyBase
    {
        public PropertySmoking(int durationRound, int size, int life)
        {
            Status = PropertyStatus.Enabled;
            DurationRoundMax = durationRound;
        }

        public override void DoAfterAction()
        {
            Color smokeColor = Color.FromArgb(100, 0, 0, 0);
            Owner.Scene.GameObjects.Add(new ObjectSmoke(Owner.X, Owner.Y, Owner.Size, smokeColor, 3));
        }

        public override void DoBeforeActionMove() { }
        public override void DoBeforeAction() { }
        public override void DoBeforeActionPlan() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoAfterDead(ObjectActive killer) { }
        public override void DoBeforeEnd(PropertyEndType endType) { }
    }
}
