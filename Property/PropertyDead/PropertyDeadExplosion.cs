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
        /// 爆炸的撞擊力量,撞擊力量小於等於此值會被毀滅
        /// </summary>
        public int CollisionPower { get; set; }

        /// <summary>
        /// 爆炸的陣營判定,符合此陣營不會被傷害
        /// </summary>
        public LeagueType CollisionLeague { get; set; }

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
        /// <param name="collisionPower">爆炸的撞擊力量,撞擊力量小於等於此值會被摧毀</param>
        /// <param name="collisionLeague">爆炸的陣營判定,符合此陣營不會被傷害</param>
        /// <param name="color">爆炸顏色</param>
        /// <param name="ownerScaleFix">快爆炸時的大小調整倍數</param>
        /// <param name="ownerRFix">快爆炸時的紅色調整倍數</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        public PropertyDeadExplosion(float rangeMultiple, int rangeConstant, int collisionPower, LeagueType collisionLeague, Color color, float ownerScaleFix, float ownerRFix, ObjectDeadType deadType)
        {
            Status = PropertyStatus.Enabled;
            DeadType = deadType;
            RangeMultiple = rangeMultiple;
            RangeConstant = rangeConstant;
            CollisionPower = collisionPower;
            CollisionLeague = collisionLeague;
            Color = color;
            OwnerScaleFix = ownerScaleFix;
            OwnerRFix = ownerRFix;
        }


        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType)
        {
            if ((DeadType & deadType) != deadType) return;

            Owner.Scene.EffectObjects.Add(new EffectShark(Owner.Scene.Sec(0.2F), 5));
            int explosionSize = (int)(RangeMultiple * (Owner.Layout.RectWidth + Owner.Layout.RectHeight) / 2) + RangeConstant;

            ObjectScrap explosionObject = new ObjectScrap(Owner.Layout.CenterX, Owner.Layout.CenterY, explosionSize, explosionSize, Owner.Scene.Sec(0.6F), Color, MoveNull.Value);
            Owner.Container.Add(explosionObject);

            for (int i = 0; i < Owner.Container.Count; i++)
            {
                ObjectActive objectActive = Owner.Container[i] as ObjectActive;
                if (objectActive == null || objectActive.Status != ObjectStatus.Alive || objectActive.League == CollisionLeague) continue;

                //碰撞判定
                if (!Function.IsCollison(objectActive.Layout, explosionObject.Layout)) continue;

                //檢查目標有無碰撞特性
                int colliderPower = -1;
                for (int j = 0; j < objectActive.Propertys.Count; j++)
                {
                    PropertyCollision checkCollision = objectActive.Propertys[j] as PropertyCollision;
                    if (checkCollision != null && checkCollision.Status == PropertyStatus.Enabled)
                    {
                        colliderPower = Math.Max(colliderPower, checkCollision.CollisionPower);
                    }
                }

                if (colliderPower < 0) continue;
                if (colliderPower <= CollisionPower)
                {
                    objectActive.Kill(Owner, ObjectDeadType.Collision);
                }
            }
        }

        public override void DoBeforeDraw(Graphics g)
        {
            _OwnerScaleFix = 0;
            _OwnerRFix = 0;

            if ((DeadType & ObjectDeadType.LifeEnd) == ObjectDeadType.LifeEnd && Owner.Life.Limit >= 0 && Owner.DrawObject != null)
            {
                int life = Owner.Life.Limit - Owner.Life.Value;
                if (life < Owner.Scene.Sec(1F))
                {
                    int animation = (life / Owner.Scene.Sec(0.05F)) % 5;
                    int animation2 = (life / Owner.Scene.Sec(0.05F));
                    if (animation2 > 15) animation2 = 15;
                    _OwnerScaleFix = animation * OwnerScaleFix;
                    _OwnerRFix = animation * OwnerRFix;
                    Owner.DrawObject.Scale += _OwnerScaleFix;
                    Owner.DrawObject.RFix += _OwnerRFix;

                    int explosionSize = (int)(RangeMultiple * (Owner.Layout.RectWidth + Owner.Layout.RectHeight) / 2) + RangeConstant;
                    Rectangle rectRange = new Rectangle((int)(Owner.Layout.CenterX - explosionSize / 2), (int)(Owner.Layout.CenterY - explosionSize / 2), explosionSize, explosionSize);
                    using (Pen pen = new Pen(Color.FromArgb(60 - animation2 * 4, 255, 50, 50)))
                    {
                        g.DrawEllipse(pen, rectRange);
                    }
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
