using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 護盾技能
    /// </summary>
    public class SkillShield : SkillBase
    {
        private PropertyCollision _Collision;
        private ObjectUI _Effect;

        public SkillShield(int costEnergy, int costEnergyPerSec, int channeled, int cooldown)
        {
            Status = SkillStatus.Disabled;
            CostEnergy = costEnergy;
            CostEnergyPerSec = costEnergyPerSec;
            ChanneledLimit = channeled;
            CooldownLimit = cooldown;
        }

        public override void DoBeforeActionMove()
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

                        for (int i = 0; i < Owner.Propertys.Count; i++)
                        {
                            PropertyCollision collision = Owner.Propertys[i] as PropertyCollision;
                            if (collision != null)
                            {
                                _Collision = collision;
                            }
                        }

                        if (_Collision == null)
                        {
                            Break();
                            return;
                        }
                        _Collision.CollisionPower += 1;

                        _Effect = new ObjectUI(ContentAlignment.MiddleCenter, 0, 0, Owner.Layout.RectWidth + 6, Owner.Layout.RectHeight + 6, new DrawPolygon(Color.Black, Color.FromArgb(150, 255, 255, 200), 6, 1));
                        _Effect.Layout.DependTarget = new TargetObject(Owner);
                        Owner.Scene.UIObjects.Add(_Effect);
                        Status = SkillStatus.Channeled;
                    }
                    break;
            }
        }

        public override void DoBeforeEnd(SkillEndType endType)
        {
            switch (endType)
            {
                case SkillEndType.ChanneledBreak:
                case SkillEndType.Finish:
                    {
                        _Collision.CollisionPower -= 1;
                        _Collision = null;
                        _Effect.Status = ObjectStatus.Dead;
                    }
                    break;
            }

        }
        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <returns>繪圖物件</returns>
        public override IDrawSkill GetDrawObject(Color color)
        {
            DrawSkillSprint drawObject = new DrawSkillSprint(color, this);
            return drawObject;
        }

        public override void DoBeforeAction() { }
        public override void DoBeforeActionPlan() { }
        public override void DoAfterAction() { }
        public override void DoBeforeDraw(Graphics g) { }
        public override void DoAfterDraw(Graphics g) { }
        public override void DoBeforeActionEnergyGet() { }
        public override void DoAfterDead(ObjectActive killer, ObjectDeadType deadType) { }
        public override void DoUseWhenEfficacy(ITarget target) { }
    }
}
