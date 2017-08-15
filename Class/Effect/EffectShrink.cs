using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 逐漸限縮場地範圍的特效
    /// </summary>
    class EffectShrink : IEffect
    {
        /// <summary>
        /// 特效是否可被中斷
        /// </summary>
        public bool CanBreak { get; set; }

        /// <summary>
        /// 作用場景物件
        /// </summary>
        public SceneBase Scene { get; set; }

        /// <summary>
        /// 特效狀態
        /// </summary>
        public EffectStatus Status { get; private set; }

        /// <summary>
        /// 縮小持續的回合數最大值
        /// </summary>
        public int DurationRoundMax { get; set; }

        /// <summary>
        /// 縮小持續的回合數計數
        /// </summary>
        public int DurationRound { get; set; }

        /// <summary>
        /// 用來縮小/還原的回合數量最大值
        /// </summary>
        public int ShrinkRoundMax { get; private set; }

        /// <summary>
        /// 用來縮小/還原的回合數量計數
        /// </summary>
        public int ShrinkRound { get; set; }

        /// <summary>
        /// 每回合縮小的邊界
        /// </summary>
        public Padding ShrinkPerRound { get; set; }

        /// <summary>
        /// 新增逐漸限縮場地範圍的特效
        /// </summary>
        /// <param name="shrinkPerRound">每回合縮小的邊界</param>
        /// <param name="duration">縮小持續的回合數,小於0為永久</param>
        /// <param name="shrinkRounds">用來縮小的回合數量</param>
        public EffectShrink(Padding shrinkPerRound, int duration, int shrinkRounds)
        {
            CanBreak = true;
            Status = EffectStatus.Enabling;
            ShrinkPerRound = shrinkPerRound;
            DurationRoundMax = duration;
            ShrinkRoundMax = shrinkRounds;
            ShrinkRound = 0;
        }

        public void DoAfterRound()
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    Scene.GameRectangle = new Rectangle
                        (
                        Scene.GameRectangle.Left + ShrinkPerRound.Left,
                        Scene.GameRectangle.Top + ShrinkPerRound.Top,
                        Scene.GameRectangle.Width - ShrinkPerRound.Horizontal,
                        Scene.GameRectangle.Height - ShrinkPerRound.Vertical
                        );

                    if (ShrinkRound >= ShrinkRoundMax)
                    {
                        Status = EffectStatus.Enabled;
                        goto case EffectStatus.Enabled;
                    }
                    ShrinkRound++;
                    break;
                case EffectStatus.Enabled:
                    if (DurationRoundMax >= 0 && DurationRound >= DurationRoundMax)
                    {
                        Status = EffectStatus.Disabling;
                        goto case EffectStatus.Disabling;
                    }
                    DurationRound++;
                    break;
                case EffectStatus.Disabling:
                    Scene.GameRectangle = new Rectangle
                      (
                      Scene.GameRectangle.Left - ShrinkPerRound.Left,
                      Scene.GameRectangle.Top - ShrinkPerRound.Top,
                      Scene.GameRectangle.Width + ShrinkPerRound.Horizontal,
                      Scene.GameRectangle.Height + ShrinkPerRound.Vertical
                      );

                    if (ShrinkRound <= 0)
                    {
                        Status = EffectStatus.Disabled;
                        break;
                    }
                    ShrinkRound--;
                    break;
            }
        }

        public void Break()
        {
            if (CanBreak)
            {
                Status = EffectStatus.Disabling;
            }
        }

        public void DoBeforeDraw(Graphics g) { }
        public void DoBeforeDrawObject(Graphics g) { }
        public void DoBeforeDrawBack(Graphics g) { }
        public void DoAfterDraw(Graphics g) { }
        public void DoBeforeRound() { }
        public void DoBeforeDrawUI(Graphics g) { }
    }
}
