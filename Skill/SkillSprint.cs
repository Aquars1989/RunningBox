using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 衝刺技能,提升最後一次移動距離
    /// </summary>
    public class SkillSprint : SkillBase
    {
        /// <summary>
        /// 衝刺距離倍數(以所有者速度為基準)
        /// </summary>
        public float SpeedMultiple { get; set; }

        /// <summary>
        /// 衝刺距離常數
        /// </summary>
        public int SpeedConstant { get; set; }

        /// <summary>
        /// 是否加上冒煙特效
        /// </summary>
        public bool Smoking { get; set; }

        /// <summary>
        /// 新增衝刺技能,提升最後一次移動距離
        /// </summary>
        /// <param name="costEnergy">耗費能量</param>
        /// <param name="cooldown">冷卻時間(毫秒)</param>
        /// <param name="speedMultiple">衝刺距離倍數(以所有者速度為基準)</param>
        /// <param name="speedConstant">衝刺距離常數</param>
        /// <param name="smoking">是否加上冒煙特效</param>
        public SkillSprint(int costEnergy, int cooldown, int speedMultiple, int speedConstant, bool smoking)
        {
            Status = SkillStatus.Disabled;
            CostEnergy = costEnergy;
            Cooldown = new CounterObject(cooldown);
            SpeedMultiple = speedMultiple;
            SpeedConstant = speedConstant;
            Smoking = smoking;
        }

        public override void DoBeforeActionMove()
        {
            switch (Status)
            {
                case SkillStatus.Enabled:
                    {
                        if (Owner == null || Owner.MoveObject.Target is TargetNull)
                        {
                            Break();
                            return;
                        }


                        double direction = Function.GetAngle(Owner.Layout.CenterX, Owner.Layout.CenterY, Owner.MoveObject.Target.X, Owner.MoveObject.Target.Y);
                        float speed = (Owner.MoveObject.Speed * SpeedMultiple) + SpeedConstant;
                        Owner.MoveObject.AddOffset(Function.GetOffsetPoint(0, 0, direction, speed));

                        if (Smoking)
                        {
                            Owner.Propertys.Add(new PropertySmoking(Owner.Scene.Round(Owner.MoveObject.OffsetsLimit), Owner.Scene.Sec(0.2F)));
                        }
                        Status = SkillStatus.Cooldown;
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
        public override void DoAfterEnd(SkillEndType endType) { }
    }
}
