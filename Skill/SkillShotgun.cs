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
        private ITargetability _OwnerTarget;
        private float _OwnerSpeed;

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
            Channeled = new CounterObject(-1);
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
                        _OwnerSpeed = Owner.MoveObject.Speed;
                        _OwnerTarget = Owner.MoveObject.Target.Object;

                        Owner.MoveObject.Target.ClearObject();
                        //Owner.MoveObject.Target.SetOffsetByAngle(angle, 100);
                        Owner.MoveObject.Speed = 20;

                        Status = SkillStatus.Channeled;
                        Cursor.Position = Scene.PointToScreen(new Point((int)Owner.Layout.CenterX, (int)Owner.Layout.CenterY));
                    }
                    break;
            }
            base.DoBeforeAction();
        }

        public override void Release(ITargetability target)
        {
            if (Status == SkillStatus.Channeled)
            {
                PointF targetPoint = Scene.GetTargetPoint(DirectionType.Center);
                double angle = Function.GetAngle(Owner.Layout.CenterX, Owner.Layout.CenterY, targetPoint.X, targetPoint.Y);
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
                Owner.MoveObject.AddToNextOffset(Function.GetOffsetPoint(0, 0, angle + 180, 500));
                Cursor.Position = Scene.PointToScreen(new Point((int)Owner.Layout.CenterX, (int)Owner.Layout.CenterY));
                Status = SkillStatus.Cooldown;
                OnEnd(SkillEndType.Finish);
            }
            base.Release(target);
        }

        /// <summary>
        /// 技能失效
        /// </summary>
        public override void DoAfterEnd(SkillEndType endType)
        {
            switch (endType)
            {
                case SkillEndType.ChanneledBreak:
                case SkillEndType.Finish:
                    {
                        Owner.MoveObject.Speed = _OwnerSpeed;
                        Owner.MoveObject.Target.SetObject(_OwnerTarget);
                        //Owner.MoveObject.Target.ClearOffset();
                    }
                    break;
            }
            base.DoAfterEnd(endType);
        }

        public override void DoBeforeDraw(Graphics g)
        {
            if (Status == SkillStatus.Channeled)
            {
                PointF targetPoint = Scene.GetTargetPoint(DirectionType.Center);
                double angle = Function.GetAngle(Owner.Layout.CenterX, Owner.Layout.CenterY, targetPoint.X, targetPoint.Y);
                g.FillPie(Brushes.LightSteelBlue, Owner.Layout.CenterX - 50, Owner.Layout.CenterY - 50, 100, 100, (float)angle - Radiation / 2, Radiation);
                g.DrawLine(Pens.IndianRed, targetPoint.X - 5, targetPoint.Y, targetPoint.X + 5, targetPoint.Y);
                g.DrawLine(Pens.IndianRed, targetPoint.X, targetPoint.Y - 5, targetPoint.X, targetPoint.Y + 5);
            }
            base.DoBeforeDraw(g);
        }

        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <returns>繪圖物件</returns>
        public override DrawSkillBase GetDrawObject(Color color)
        {
            DrawSkillShotGun drawObject = new DrawSkillShotGun(color, this);
            return drawObject;
        }
    }
}
