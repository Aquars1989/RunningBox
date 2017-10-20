using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 散彈技能
    /// </summary>
    public class SkillShotgun : SkillBase
    {
        /// <summary>
        /// 辨識碼
        /// </summary>
        public override SkillID ID
        {
            get { return SkillID.SkillShotgun; }
        }

        /// <summary>
        /// 說明文字
        /// </summary>
        public override string Info
        {
            get { return string.Format("發射{0:N0}顆子彈", BulletCount); }
        }

        /// <summary>
        /// 散彈數量
        /// </summary>
        public int BulletCount { get; set; }

        /// <summary>
        /// 殺傷力
        /// </summary>
        public int AttackPower { get; set; }

        /// <summary>
        /// 擴散角度
        /// </summary>
        public int Radiation { get; set; }

        /// <summary>
        /// 新增散彈技能
        /// </summary>
        /// <param name="bulletCount">散彈數量</param>
        /// <param name="attackPower">殺傷力</param>
        /// <param name="costEnergy">耗費能量</param>
        /// <param name="cooldown">冷卻時間(毫秒)</param>
        public SkillShotgun(int bulletCount, int attackPower, int radiation, int costEnergy, int cooldown)
        {
            Status = SkillStatus.Disabled;
            BulletCount = bulletCount;
            AttackPower = attackPower;
            Radiation = radiation;
            CostEnergy = costEnergy;
            Cooldown = new CounterObject(cooldown);
        }

        /// <summary>
        /// 技能生效
        /// </summary>
        public override void DoBeforeAction()
        {
            switch (Status)
            {
                case SkillStatus.Enabled:
                    {
                        if (Owner == null)
                        {
                            Break();
                            return;
                        }

                        double angle = Function.GetAngle(0, 0, Owner.MoveObject.MoveX, Owner.MoveObject.MoveY);
                        float partAngle = Radiation / (float)BulletCount;
                        float shotAngle = (float)angle - (partAngle * BulletCount / 2);
                        for (int i = 0; i < BulletCount; i++)
                        {
                            MoveStraight bulletMove = new MoveStraight(null, 1, 600, 1, 0, 1);
                            ObjectActive bullet = new ObjectActive(Owner.Layout.CenterX, Owner.Layout.CenterY, 5, 5, Scene.Sec(0.5F), Owner.League, ShapeType.Ellipse, new DrawBrush(Owner.DrawObject.MainColor, ShapeType.Ellipse), bulletMove);
                            bullet.Propertys.Add(new PropertyCollision(AttackPower));
                            bullet.Propertys.Add(new PropertyDeadBroken(5, 2, 2, ObjectDeadType.All, 360, 40, 100, Scene.Sec(0.2F), Scene.Sec(0.3F)));
                            bulletMove.Target.SetOffsetByAngle(shotAngle, 1000);
                            bulletMove.Target.SetObject(bullet);
                            Owner.Container.Add(bullet);
                            shotAngle += partAngle;
                        }
                        Status = SkillStatus.Cooldown;
                    }
                    break;
            }
            base.DoBeforeAction();
        }

        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <returns>繪圖物件</returns>
        public override DrawSkillBase GetDrawObject(Color color)
        {
            DrawSkillShield drawObject = new DrawSkillShield(color, this);
            return drawObject;
        }
    }
}
