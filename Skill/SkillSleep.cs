using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 睡眠技能,恢復能量
    /// </summary>
    public class SkillSleep : SkillBase
    {
        private PropertyUI _MiniBar;   //迷你條棒
        private float _OwnerSpeed;

        /// <summary>
        /// 辨識碼
        /// </summary>
        public override SkillID ID
        {
            get { return SkillID.Sleep; }
        }

        /// <summary>
        /// 說明文字
        /// </summary>
        public override string Info
        {
            get { return "恢復能量,施放時無法移動"; }
        }

        public CounterObject Animation { get; set; }

        /// <summary>
        /// 每秒取得能量
        /// </summary>
        public int GetEnergyPerSec { get; set; }

        /// <summary>
        /// 新增睡眠技能,恢復能量
        /// </summary>
        /// <param name="costEnergy">耗費能量</param>
        /// <param name="cooldown">冷卻時間(毫秒)</param>
        /// <param name="channeled">持續時間(毫秒)</param>
        public SkillSleep(int getEnergyPerSec, int channeled, int cooldown)
        {
            Status = SkillStatus.Disabled;
            GetEnergyPerSec = getEnergyPerSec;
            Channeled = new CounterObject(channeled);
            Cooldown = new CounterObject(cooldown);
            Animation = new CounterObject(100);
        }

        public override void Recast(ITargetability target)
        {
            Break();
            base.Recast(target);
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

                        Owner.Skills.AllBreak();
                        _OwnerSpeed = Owner.MoveObject.Speed;
                        Owner.MoveObject.Speed = 0;
                        Status = SkillStatus.Channeled;
                        _MiniBar = new PropertyUI(-1, new Size(30, 6), new DrawUICounterBar(Color.FromArgb(80, 210, 140), Color.Black, Color.White, 1, true, Channeled));
                        Owner.Propertys.Add(_MiniBar);
                    }
                    break;
                case SkillStatus.Channeled:
                    int getEnergy = (int)(GetEnergyPerSec / Scene.SceneRoundPerSec + 0.5F);
                    Owner.Energy.Value += getEnergy;

                    if (Animation.IsFull)
                    {
                        MoveStraight moveZZZ = new MoveStraight(null, 1, 150, 3, 100, 1F);
                        DrawCustom drawZZZ = new DrawCustom();
                        drawZZZ.Colors.SetColor("Main", Color.DarkGreen);
                        drawZZZ.AfterDraw += (x, g, r) =>
                        {
                            Pen pen = drawZZZ.Colors.GetPen("Main");
                            Point[] drawPots = { new Point(r.Left, r.Top),
                                                 new Point(r.Left + r.Width, r.Top),
                                                 new Point(r.Left, r.Top + r.Height),
                                                 new Point(r.Left + r.Width, r.Top + r.Height)};
                            g.DrawLines(pen, drawPots);
                        };
                        ObjectSmoke objectZZZ = new ObjectSmoke(Owner.Layout.CenterX, Owner.Layout.CenterY, 4, 4, Scene.Sec(1), 3F, 0, drawZZZ, moveZZZ);
                        moveZZZ.Target.SetOffsetByAngle(315 + Global.Rand.Next(-20, 20), 50);
                        moveZZZ.Target.SetObject(objectZZZ);
                        Owner.Container.Add(objectZZZ);
                        Animation.Value = 0;
                    }
                    Animation.Value += Scene.SceneIntervalOfRound;
                    break;
            }
            base.DoBeforeAction();
        }

        private static Font _FontZZZ = new Font("微軟正黑體", 12);

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
                        _MiniBar.Break();
                    }
                    break;
            }

            base.DoAfterEnd(endType);
        }

        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <returns>繪圖物件</returns>
        public override DrawSkillBase GetDrawObject(Color color)
        {
            DrawSkillSleep drawObject = new DrawSkillSleep(color, this);
            return drawObject;
        }
    }
}
