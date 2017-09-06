﻿using System;
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
        private ObjectActive _ShieldObject; //護盾物件
        private PropertyUI _MiniBar;        //迷你條棒+幽靈屬性

        /// <summary>
        /// 說明文字
        /// </summary>
        public override string Info
        {
            get { return string.Format("產生強度{0:N0}的護盾,護盾被摧毀之前本體不會受到傷害", CollisionPower); }
        }

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

                        int effectWidth = Owner.Layout.RectWidth + 12;
                        int effectHeight = Owner.Layout.RectHeight + 12;
                        effectWidth += effectWidth % 2;
                        effectHeight += effectHeight % 2;

                        Color color = Owner.DrawObject.Color;
                        Color colorBack = Color.FromArgb(50, color.R, color.G, color.G);
                        _ShieldObject = new ObjectActive(0, 0, effectWidth, effectHeight, -1, Owner.League, ShapeType.Ellipse, new DrawPolygon(colorBack, color, 6, 1, 0, 360), MoveNull.Value);
                        _ShieldObject.Propertys.Add(new PropertyDeadBroken(new DrawPolygon(Color.Empty, color, 2, 1, 0, 360), 6, 10, 10, ObjectDeadType.Collision, 360, 100, 150, Owner.Scene.Sec(0.4F), Owner.Scene.Sec(0.6F)));
                        _ShieldObject.Propertys.Add(new PropertyDeadCollapse(new DrawPolygon(Color.Empty, color, 2, 1, 0, 360), 1, Owner.Scene.Sec(0.2F), 10, 10, ObjectDeadType.LifeEnd, 100, 200, Owner.Scene.Sec(0.2F), Owner.Scene.Sec(0.3F)));
                        _ShieldObject.Propertys.Add(new PropertyCollision(1));
                        _ShieldObject.Layout.DependTarget = new TargetObject(Owner);
                        //_ShieldObject.Dead += (x, e, t) => { this.Break(); };
                        Owner.Container.Add(_ShieldObject);

                        _MiniBar = new PropertyUI(-1, new Size(30, 6), new DrawUICounterBar(Color.FromArgb(160, 210, 100), Color.Black, Color.White, 1, true, Channeled));
                        _MiniBar.Affix = SpecialStatus.Ghost;   //增加幽靈屬性
                        Owner.Propertys.Add(_MiniBar);
                        Status = SkillStatus.Channeled;
                    }
                    break;
                case SkillStatus.Channeled:
                    if (_ShieldObject.Status != ObjectStatus.Alive)
                    {
                        this.Break();
                    }
                    break;
            }
        }

        public override void DoAfterEnd(SkillEndType endType)
        {
            switch (endType)
            {
                case SkillEndType.ChanneledBreak:
                    {
                        _ShieldObject.Kill(null, ObjectDeadType.LifeEnd);
                        _MiniBar.End(PropertyEndType.Break);
                    }
                    break;
                case SkillEndType.Finish:
                    {
                        _ShieldObject.Kill(null, ObjectDeadType.LifeEnd);
                        _MiniBar.End(PropertyEndType.Finish);
                    }
                    break;
            }
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
