﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 衝擊波技能
    /// </summary>
    public class SkillShockwave : SkillBase
    {
        private PropertyUI _MiniBar;   // 迷你條棒

        /// <summary>
        /// 辨識碼
        /// </summary>
        public override SkillID ID
        {
            get { return SkillID.Shockwave; }
        }

        /// <summary>
        /// 說明文字
        /// </summary>
        public override string Info
        {
            get { return string.Format("產生衝擊波擊退物件\n獎勵：擊退目標", PushSycle.Limit / 1000F); }
        }

        /// <summary>
        /// 釋放周期(毫秒)
        /// </summary>
        public CounterObject PushSycle { get; private set; }

        /// <summary>
        /// 擊退力量
        /// </summary>
        public int PushPower { get; set; }

        /// <summary>
        /// 擊退範圍
        /// </summary>
        public int Range { get; set; }

        /// <summary>
        /// 新增衝擊波技能,擊退周圍物件
        /// </summary>
        /// <param name="costEnergy">耗費能量</param>
        /// <param name="costEnergyPerSec">每秒耗費能量</param>
        /// <param name="channeled">最大引導時間(毫秒)</param>
        /// <param name="cooldown">冷卻時間(毫秒)</param>
        /// <param name="pushSycle">釋放周期(毫秒)</param>
        /// <param name="pushPower">擊退力量</param>
        /// <param name="range">擊退範圍</param>
        public SkillShockwave(int costEnergy, int costEnergyPerSec, int channeled, int cooldown, int pushSycle, int pushPower, int range)
        {
            Status = SkillStatus.Disabled;
            CostEnergy = costEnergy;
            CostEnergyPerSec = costEnergyPerSec;
            Channeled = new CounterObject(channeled);
            Cooldown = new CounterObject(cooldown);
            PushSycle = new CounterObject(pushSycle);
            PushPower = pushPower;
            Range = range;
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
                        Status = SkillStatus.Channeled;
                        _MiniBar = new PropertyUI(-1, new Size(30, 6), new DrawUICounterBar(Color.FromArgb(160, 210, 100), Color.Black, Color.White, 1, true, Channeled));
                        Owner.Propertys.Add(_MiniBar);
                        PushSycle.Value = Scene.SceneIntervalOfRound;
                        break;
                    }
                case SkillStatus.Channeled:
                    if (PushSycle.IsFull)
                    {
                        LayoutSet checkRange = new LayoutSet()
                        {
                            CollisonShape = ShapeType.Ellipse,
                            Anchor = DirectionType.Center,
                            X = Owner.Layout.X,
                            Y = Owner.Layout.Y,
                            Width = Range,
                            Height = Range
                        };

                        int hitCount = 0;
                        for (int i = 0; i < Owner.Container.Count; i++)
                        {
                            ObjectBase objectActive = Owner.Container[i];
                            if (objectActive == Owner) continue;

                            // 範圍判定
                            if (!Function.IsCollison(objectActive.Layout, checkRange)) continue;

                            double angle = Function.GetAngle(checkRange.CenterX, checkRange.CenterY, objectActive.Layout.CenterX, objectActive.Layout.CenterY);
                            double pushPower = PushPower * Function.GetDistance(checkRange.CenterX, checkRange.CenterY, objectActive.Layout.CenterX, objectActive.Layout.CenterY) / Range;
                            objectActive.MoveObject.AddToNextOffset(objectActive.MoveObject.GetOffsetByAngle(angle, PushPower));
                            hitCount++;
                        }

                        if (hitCount > 0)
                        {
                            if (hitCount > 4)
                                hitCount = 4;
                            SceneGaming scene = Scene as SceneGaming;
                            if (scene != null)
                            {
                                scene.AddScoreToPlayer("震盪衝擊", hitCount * 15);
                            }
                        }

                        Color waveColor = Color.FromArgb(150, Owner.DrawObject.MainColor);
                        ObjectWave wave = new ObjectWave(0, 0, 0, 0, Range, Range, Scene.Sec(0.15F), Scene.Sec(0.1F), new DrawPen(waveColor, ShapeType.Ellipse, 1), MoveNull.Value);
                        wave.Layout.Depend.SetObject(Owner);
                        wave.DiffusionOpacity = 0.1F;
                        wave.FadeOpacity = 0.5F;
                        Owner.Container.Add(wave);
                        PushSycle.Value = 0;
                    }
                    else
                    {
                        PushSycle.Value += Scene.SceneIntervalOfRound;
                    }
                    break;
            }

            base.DoBeforeActionMove();
        }

        public override void DoAfterEnd(SkillEndType endType)
        {
            switch (endType)
            {
                case SkillEndType.ChanneledBreak:
                case SkillEndType.Finish:
                    {
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
            DrawSkillShockwave drawObject = new DrawSkillShockwave(color, this);
            return drawObject;
        }
    }
}
