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

        /// <summary>
        /// 護盾撞擊力量
        /// </summary>
        public int CollisionPower { get; private set; }

        /// <summary>
        /// 新增護盾技能,護盾破除之前無敵
        /// </summary>
        /// <param name="collisionPower">護盾撞擊力量</param>
        /// <param name="costEnergy">耗費能量</param>
        /// <param name="costEnergyPerSec">每秒耗費能量</param>
        /// <param name="channeled">最大引導時間(毫秒</param>
        /// <param name="cooldown">冷卻時間(毫秒)</param>
        public SkillShield(int collisionPower, int costEnergy, int costEnergyPerSec, int channeled, int cooldown)
        {
            Status = SkillStatus.Disabled;
            CollisionPower = collisionPower;
            CostEnergy = costEnergy;
            CostEnergyPerSec = costEnergyPerSec;
            Channeled = new CounterObject(channeled);
            Cooldown = new CounterObject(cooldown);
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
                        _Collision.Status = PropertyStatus.Pause;

                        int effectWidth = Owner.Layout.RectWidth + 12;
                        int effectHeight = Owner.Layout.RectHeight + 12;
                        effectWidth += effectWidth % 2;
                        effectHeight += effectHeight % 2;
                        _ShieldObject = new ObjectActive(0, 0, 0, effectWidth, effectHeight, 0, -1, Owner.League, ShapeType.Ellipse, new DrawPolygon(Color.FromArgb(170, 170, 0), Color.FromArgb(200, 255, 255, 200), 6, 1, 0, 5), null);
                        _ShieldObject.Propertys.Add(new PropertyDeadBroken(1, 3, 3, ObjectDeadType.All, 360, 150, 300, Owner.Scene.Sec(0.15F), Owner.Scene.Sec(0.25F)));
                        _ShieldObject.Propertys.Add(new PropertyCollision(1, null));
                        _ShieldObject.Layout.DependTarget = new TargetObject(Owner);
                        _ShieldObject.Dead += (x, e, t) => { this.Break(); };
                        Owner.Container.Add(_ShieldObject);

                        _MiniBar = new PropertyUI(-1, new Size(30, 6), new DrawUICounterBar(Color.FromArgb(160, 210, 100), 1, true, Channeled));
                        Owner.Propertys.Add(_MiniBar);
                        Status = SkillStatus.Channeled;
                    }
                    break;
            }
        }

        public override void DoAfterEnd(SkillEndType endType)
        {
            switch (endType)
            {
                case SkillEndType.ChanneledBreak:
                case SkillEndType.Finish:
                    {
                        _Collision.Status = PropertyStatus.Enabled;
                        _Collision = null;
                        _ShieldObject.Kill(null, ObjectDeadType.LifeEnd);
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
