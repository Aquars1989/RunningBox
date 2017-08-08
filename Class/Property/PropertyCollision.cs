using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    class PropertyCollision : PropertyBase
    {
        public int CollisionPower { get; set; }

        public PropertyCollision(int collisionPower)
        {
            Status = PropertyStatus.Enabled;
            CollisionPower = collisionPower;
            DurationRoundMax = 0;
        }

        public override void DoAfterAction()
        {
            foreach (ObjectBase objectBase in Owner.Collection)
            {
                ObjectActive objectActive = objectBase as ObjectActive;
                if (objectActive != null && objectActive.League != Owner.League && objectActive.Rectangle.IntersectsWith(Owner.Rectangle))
                {
                    int collisionPower = -1;
                    foreach (PropertyBase properBase in objectActive.Propertys)
                    {
                        PropertyCollision propertyCollision = properBase as PropertyCollision;
                        if (propertyCollision != null)
                        {
                            collisionPower = Math.Max(collisionPower, propertyCollision.CollisionPower);
                        }
                    }

                    if (collisionPower < 0) continue;
                    if (collisionPower >= CollisionPower)
                    {
                        Owner.Kill(objectActive);
                    }

                    if (CollisionPower >= collisionPower)
                    {
                        objectActive.Kill(Owner);
                    }
                }
            }
        }

        public override void DoBeforeAction() { }
        public override void DoBeforeActionMove() { }
        public override void DoBeforeActionPlan() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoAfterDead(ObjectActive killer) { }
        public override void DoBeforeEnd(PropertyEndType endType) { }
    }
}
