using System;
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
        public PropertyCollision(int collisionPower, TargetObject target)
            : base(target)
        {
            Affix = SpecialStatus.Collision;
            CollisionPower = collisionPower;
        }

        /// <summary>
        /// 新增碰撞特性,兩個具碰撞屬性的物件接觸時,強度小的會被消滅,如果強度相同則兩者都會消滅
        /// </summary>
        /// <param name="collisionPower">碰撞強度</param>
        public PropertyCollision(int collisionPower)
            : base(TargetNull.Value)
        {
            Affix = SpecialStatus.Collision;
            CollisionPower = collisionPower;
        }

        public override void DoAfterAction()
        {
            if (Owner.Status == ObjectStatus.Alive && Status == PropertyStatus.Enabled)
            {
                for (int i = 0; i < Owner.Container.Count; i++)
                {
                    ObjectActive objectActive = Owner.Container[i] as ObjectActive;
                    if (objectActive == null || objectActive.Status != ObjectStatus.Alive || objectActive.League == Owner.League) continue;

                    //限定目標
                    if (Target != TargetNull.Value)
                    {
                        TargetObject targetObject = (Target as TargetObject);
                        if (targetObject != null && objectActive == targetObject.Target)
                        {
                            i = Owner.Container.Count;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //特殊狀態判定 具碰撞 非鬼魂
                    if ((objectActive.Propertys.Affix & SpecialStatus.Collision) != SpecialStatus.Collision ||
                        (objectActive.Propertys.Affix & SpecialStatus.Ghost) == SpecialStatus.Ghost)
                    {
                        continue;
                    }

                    //碰撞判定
                    if (!Function.IsCollison(Owner.Layout, objectActive.Layout)) continue;

                    int colliderPower = -1;
                    for (int j = 0; j < objectActive.Propertys.Count; j++)
                    {
                        PropertyCollision checkCollision = objectActive.Propertys[j] as PropertyCollision;
                        if (checkCollision != null && checkCollision.Status == PropertyStatus.Enabled)
                        {
                            TargetObject checkTarget = checkCollision.Target as TargetObject;
                            if (checkTarget == null || checkTarget.Target == Owner)
                            {
                                colliderPower = Math.Max(colliderPower, checkCollision.CollisionPower);
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

        public override void DoBeforeAction() { }
        public override void DoBeforeActionMove() { }
        public override void DoBeforeActionPlan() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoBeforeEnd(PropertyEndType endType) { }

        protected override void OnTargetChanged()
        {
            if (Target != TargetNull.Value && !(Target is TargetObject))
            {
                Target = TargetNull.Value;
            }
            base.OnTargetChanged();
        }
    }
}
