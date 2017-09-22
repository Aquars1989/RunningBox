using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 誘餌技能,放出分身
    /// </summary>
    public class SkillBait : SkillBase
    {
        /// <summary>
        /// 說明文字
        /// </summary>
        public override string Info
        {
            get { return "釋放誘餌吸引注意\n誘餌可被摧毀"; }
        }

        /// <summary>
        /// 誘餌生命(毫秒)
        /// </summary>
        public int BaitLife { get; set; }

        /// <summary>
        /// 誘餌速度
        /// </summary>
        public int BaitSpeed { get; set; }

        /// <summary>
        /// 新增誘餌技能,放出分身
        /// </summary>
        /// <param name="costEnergy">耗費能量</param>
        /// <param name="cooldown">冷卻時間(毫秒)</param>
        /// <param name="baitLife">誘餌生命(毫秒)</param>
        /// <param name="baitSpeed">誘餌速度</param>
        public SkillBait(int costEnergy, int cooldown, int baitLife, int baitSpeed)
        {
            Status = SkillStatus.Disabled;
            CostEnergy = costEnergy;
            Cooldown = new CounterObject(cooldown);
            BaitLife = baitLife;
            BaitSpeed = baitSpeed;
        }

        /// <summary>
        /// 技能生效
        /// </summary>
        public override void DoBeforeActionMove()
        {
            switch (Status)
            {
                case SkillStatus.Enabled:
                    {
                        if (Owner == null || Owner.MoveObject.Target.TargetType == TargetType.None)
                        {
                            Break();
                            return;
                        }

                        //新增誘餌物件
                        double angle = Function.GetAngle(Owner.Layout.CenterX, Owner.Layout.CenterY, Owner.MoveObject.Target.X, Owner.MoveObject.Target.Y);

                        MoveStraight move = new MoveStraight(null, Owner.MoveObject.Resistance, BaitSpeed, 1, 0, 1F);
                        move.Target.SetOffsetByAngle(angle, 1000);

                        DrawBase draw = Owner.DrawObject.Copy();
                        draw.Colors.Opacity = 0.6F;

                        ObjectActive bait = new ObjectActive(Owner.Layout, BaitLife, Owner.League, draw, move);
                        move.Target.SetObject(bait);

                        bait.Propertys.Add(new PropertyCollision(0)); //強度碰撞

                        //新增雜訊物件
                        int noiseWidth = Owner.Layout.Width + 2;
                        int noiseHeight = (int)(Owner.Layout.Height * 1.3F + 0.5F) + 5;
                        DrawNoise drawNoise = new DrawNoise(Owner.DrawObject.MainColor, Color.White, 1);
                        ObjectWave noise = new ObjectWave(0, 0, noiseWidth, noiseHeight, noiseWidth * 3, noiseHeight * 3, -1, 0, drawNoise, MoveNull.Value);
                        noise.DiffusionOpacity = 0;
                        noise.Layout.Depend.SetObject(bait);
                        Owner.Container.Add(bait);
                        Owner.Container.Add(noise);

                        //將目標設為誘餌
                        for (int i = 0; i < Owner.Container.Count; i++)
                        {
                            ObjectBase objectBase = Owner.Container[i];
                            if (objectBase.Status != ObjectStatus.Alive || Function.IsFriendly(objectBase.League, Owner.League)) continue;
                            if (objectBase.MoveObject.Target.Object == Owner)
                            {
                                objectBase.MoveObject.Target.SetObject(bait);
                            }
                        }

                        bait.Dead += (s, e, t) =>
                            {
                                noise.DiffusionTime.Limit = Scene.Sec(0.2F);
                                noise.DiffusionTime.Value = 0;

                                if (Owner == null) return;
                                for (int i = 0; i < Owner.Container.Count; i++)
                                {
                                    ObjectBase objectBase = Owner.Container[i];
                                    if (objectBase.Status != ObjectStatus.Alive) continue;
                                    if (objectBase.MoveObject.Target.Object == s)
                                    {
                                        objectBase.MoveObject.Target.SetObject(Owner);
                                    }
                                }
                            };
                        Status = SkillStatus.Cooldown;
                    }
                    break;
            }

            base.DoBeforeActionMove();
        }

        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <returns>繪圖物件</returns>
        public override DrawSkillBase GetDrawObject(Color color)
        {
            DrawSkillBait drawObject = new DrawSkillBait(color, this);
            return drawObject;
        }
    }
}
