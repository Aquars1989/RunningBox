﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件死亡時會造成爆炸
    /// </summary>
    class PropertyDeadExplosion : PropertyBase
    {
        private float _OwnerScaleFix = 0;
        private float _OwnerRFix = 0;

        /// <summary>
        /// 爆炸範圍倍數(以所有者大小為基準)
        /// </summary>
        public float RangeMultiple { get; set; }

        /// <summary>
        /// 爆炸範圍常數
        /// </summary>
        public int RangeConstant { get; set; }

        /// <summary>
        /// 爆炸顏色
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// 快爆炸時的大小調整倍數
        /// </summary>
        public float OwnerScaleFix { get; set; }

        /// <summary>
        /// 快爆炸時的紅色調整倍數
        /// </summary>
        public float OwnerRFix { get; set; }


        /// <summary>
        /// 符合指定的死亡方式才會觸發
        /// </summary>
        public ObjectDeadType DeadType { get; set; }

        /// <summary>
        /// 新增爆炸特性,擁有此特性的物件死亡時會造成爆炸
        /// </summary>
        /// <param name="rangeMultiple">爆炸範圍倍數(以所有者大小為基準)</param>
        /// <param name="rangeConstant">爆炸範圍常數</param>
        /// <param name="color">爆炸顏色</param>
        /// <param name="ownerScaleFix">快爆炸時的大小調整倍數</param>
        /// <param name="ownerRFix">快爆炸時的紅色調整倍數</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        public PropertyDeadExplosion(float rangeMultiple, int rangeConstant, Color color, float ownerScaleFix, float ownerRFix, ObjectDeadType deadType)
        {
            Status = PropertyStatus.Enabled;
            DeadType = deadType;
            RangeMultiple = rangeMultiple;
            RangeConstant = rangeConstant;
            Color = color;
            OwnerScaleFix = ownerScaleFix;
            OwnerRFix = ownerRFix;
        }


        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType)
        {
            if ((DeadType & deadType) != deadType) return;

            int size = (int)(RangeMultiple * Owner.Size) + RangeConstant;
            Rectangle explosionRect = new Rectangle((int)Owner.X - size, (int)Owner.Y - size, size * 2, size * 2);
            Owner.ParentCollection.Add(new ObjectScrap(Owner.X, Owner.Y, size, 0, 20, 0, Color));

            for (int i = 0; i < Owner.ParentCollection.Count; i++)
            {
                ObjectActive objectActive = Owner.ParentCollection[i] as ObjectActive;
                if (objectActive != null && objectActive.Status == ObjectStatus.Alive && objectActive.Rectangle.IntersectsWith(explosionRect))
                {
                    for (int j = 0; j < objectActive.Propertys.Count; j++)
                    {
                        PropertyCollision colliderCollision = objectActive.Propertys[j] as PropertyCollision;
                        if (colliderCollision != null && colliderCollision.Status == PropertyStatus.Enabled)
                        {
                            objectActive.Kill(Owner, ObjectDeadType.Collision);
                        }
                    }
                }
            }
        }

        public override void DoBeforeDraw(Graphics g)
        {
            _OwnerScaleFix = 0;
            _OwnerRFix = 0;

            if ((DeadType & ObjectDeadType.LifeEnd) == ObjectDeadType.LifeEnd && Owner.LifeRoundMax >= 0 && Owner.DrawObject != null)
            {
                int life = Owner.LifeRoundMax - Owner.LifeRound;
                if (life < 60)
                {
                    _OwnerScaleFix = ((life / 2) % 5) * OwnerScaleFix;
                    _OwnerRFix = ((life / 2) % 5) * OwnerRFix;
                    Owner.DrawObject.Scale += _OwnerScaleFix;
                    Owner.DrawObject.RFix += _OwnerRFix;
                }
            }
            else
            {
                _OwnerScaleFix = 0;
                _OwnerRFix = 0;
            }
        }

        public override void DoAfterDraw(Graphics g)
        {
            if ((DeadType & ObjectDeadType.LifeEnd) == ObjectDeadType.LifeEnd && Owner.DrawObject != null)
            {
                Owner.DrawObject.Scale -= _OwnerScaleFix;
                Owner.DrawObject.RFix -= _OwnerRFix;
            }
        }

        public override void DoBeforeActionMove() { }
        public override void DoBeforeAction() { }
        public override void DoBeforeActionPlan() { }
        public override void DoAfterAction() { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoBeforeEnd(PropertyEndType endType) { }
    }
}
