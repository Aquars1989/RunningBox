﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 碰撞特性,兩個具碰撞屬性的物件接觸時,強度小的會被消滅,如果強度相同則兩者都會消滅
    /// </summary>
    class PropertyCollision : PropertyBase
    {
        /// <summary>
        /// 碰撞強度
        /// </summary>
        public int CollisionPower { get; set; }

        /// <summary>
        /// 新增碰撞特性,兩個具碰撞屬性的物件接觸時,強度小的會被消滅,如果強度相同則兩者都會消滅
        /// </summary>
        /// <param name="collisionPower">碰撞強度</param>
        /// <param name="target">鎖定目標,不為null時只針對特定目標碰撞</param>
        public PropertyCollision(int collisionPower, TargetObject target)
        {
            Status = PropertyStatus.Enabled;
            Target = target;
            CollisionPower = collisionPower;
            DurationRoundMax = 0;
        }

        public override void DoAfterAction()
        {
            if (Owner.Status == ObjectStatus.Alive && Status == PropertyStatus.Enabled)
            {
                for (int i = 0; i < Owner.ParentCollection.Count; i++)
                {
                    ObjectActive objectActive = Owner.ParentCollection[i] as ObjectActive;
                    if (objectActive != null && objectActive.Status == ObjectStatus.Alive && objectActive.League != Owner.League && objectActive.Rectangle.IntersectsWith(Owner.Rectangle))
                    {
                        int colliderPower = -1;
                        for (int j = 0; j < objectActive.Propertys.Count; j++)
                        {
                            PropertyCollision colliderCollision = objectActive.Propertys[j] as PropertyCollision;
                            if (colliderCollision != null && colliderCollision.Status == PropertyStatus.Enabled)
                            {
                                TargetObject target = colliderCollision.Target as TargetObject;
                                if (target == null || target.Targer == Owner)
                                {
                                    colliderPower = Math.Max(colliderPower, colliderCollision.CollisionPower);
                                }
                            }
                        }

                        if (colliderPower < 0) continue;
                        if (colliderPower >= CollisionPower)
                        {
                            Owner.Kill(objectActive, ObjectDeadType.Collision);
                        }

                        if (CollisionPower >= colliderPower)
                        {
                            objectActive.Kill(Owner, ObjectDeadType.Collision);
                        }
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
        public override void DoBeforeEnd(PropertyEndType endType) { }
    }
}
