using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件每回合會在原位置留下smoke物件
    /// </summary>
    class PropertySmoking : PropertyBase
    {
        /// <summary>
        /// 新增冒煙特性，擁有此特性的物件每回合會在原位置留下smoke物件
        /// </summary>
        /// <param name="durationRound">持續回合數</param>
        /// <param name="size">smoke物件尺寸</param>
        /// <param name="life">smoke物件生命週期n,每n回合尺寸會-1直到消失</param>
        public PropertySmoking(int durationRound, int size, int life)
        {
            Status = PropertyStatus.Enabled;
            DurationRoundMax = durationRound;
        }

        public override void DoAfterAction()
        {
            if (Owner.DrawObject == null) return;

            IDraw drawSmoke = Owner.DrawObject.Copy();
            drawSmoke.Opacity *= 0.2F;
            Owner.ParentCollection.Add(new ObjectSmoke(Owner.X, Owner.Y, Owner.Size, 3, drawSmoke));
        }

        public override void DoBeforeActionMove() { }
        public override void DoBeforeAction() { }
        public override void DoBeforeActionPlan() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoBeforeEnd(PropertyEndType endType) { }
    }
}
