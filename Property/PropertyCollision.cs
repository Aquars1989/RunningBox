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
        public PropertyCollision(int collisionPower, ObjectBase target = null)
        {
            Affix = SpecialStatus.Collision | SpecialStatus.Movesplit;
            Target.SetObject(target);
            CollisionPower = collisionPower;
        }

        public override void DoActionMoving()
        {
            if (Owner.Status == ObjectStatus.Alive && Status == PropertyStatus.Enabled)
            {
                for (int i = 0; i < Owner.Container.Count; i++)
                {
                    ObjectBase objectActive = Owner.Container[i];
                    if (objectActive.Status != ObjectStatus.Alive || Function.IsFriendly(objectActive.League, Owner.League)) continue;

                    // 限定目標
                    if (Target.TargetType == TargetType.GameObejct)
                    {
                        if (objectActive == Target.Object)
                        {
                            i = Owner.Container.Count;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    // 特殊狀態判定 具碰撞 非鬼魂
                    if ((objectActive.Propertys.Affix & SpecialStatus.Collision) != SpecialStatus.Collision ||
                        (objectActive.Propertys.Affix & SpecialStatus.Ghost) == SpecialStatus.Ghost)
                    {
                        continue;
                    }

                    // 碰撞判定
                    if (!Function.IsCollison(Owner.Layout, objectActive.Layout)) continue;

                    int colliderPower = -1;
                    for (int j = 0; j < objectActive.Propertys.Count; j++)
                    {
                        PropertyCollision checkCollision = objectActive.Propertys[j] as PropertyCollision;
                        if (checkCollision != null && checkCollision.Status == PropertyStatus.Enabled)
                        {
                            if (checkCollision.Target.TargetType != TargetType.GameObejct || checkCollision.Target.Object == Owner)
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
            base.DoActionMoving();
        }

        protected override void OnTargetObjectChanged(ITargetability oldValue, ITargetability newValue)
        {
            if (Target.TargetType != TargetType.GameObejct)
            {
                Target.ClearObject();
            }
            base.OnTargetObjectChanged(oldValue, newValue);
        }
    }
}
