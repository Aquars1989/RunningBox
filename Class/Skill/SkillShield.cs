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
        private ObjectActive _ShieldObject;
        private PropertyUI _MiniBar;

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

                        int effectWidth = Owner.Layout.RectWidth + 12;
                        int effectHeight = Owner.Layout.RectHeight + 12;
                        effectWidth += effectWidth % 2;
                        effectHeight += effectHeight % 2;
                        _ShieldObject = new ObjectActive(0, 0, 0, effectWidth, effectHeight, 0, -1, Owner.League, new DrawPolygon(Color.Black, Color.FromArgb(200, 255, 255, 200), 6, 1), null);
                        _ShieldObject.Propertys.Add(new PropertyCollision(10, null));
                        _ShieldObject.Layout.DependTarget = new TargetObject(Owner);
                        Owner.Container.Add(_ShieldObject);

                        _MiniBar = new PropertyUI(-1, new Size(30, 6), new DrawUIChanneledBar(Color.FromArgb(160, 210, 100), 1, this));
                        Owner.Propertys.Add(_MiniBar);
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
                        _ShieldObject.Status = ObjectStatus.Dead;
                        _MiniBar.Status = PropertyStatus.Disabled;
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
