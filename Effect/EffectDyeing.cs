
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 逐漸改變畫面顏色的特效
    /// </summary>
    public class EffectDyeing : EffectBase
    {
        /// <summary>
        /// 渲染色彩
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// 新增逐漸改變畫面顏色的特效
        /// </summary>
        /// <param name="color">要繪製的顏色</param>
        /// <param name="durationTime">渲染時間(毫秒),小於0為永久</param>
        /// <param name="enablingTime">渲染啟用時間(毫秒)</param>
        /// <param name="disablingTime">渲染消退時間(毫秒)</param>
        public EffectDyeing(Color color, int enablingTime, int durationTime, int disablingTime)
            : base(enablingTime, durationTime, disablingTime)
        {
            Status = EffectStatus.Enabling;
            Color = color;
        }

        public override void DoBeforeDrawFloor(Graphics g)
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingTime.Value > 0)
                    {
                        int alpha = (int)(EnablingTime.GetRatio() * Color.A + 0.5F);
                        if (alpha < 0) alpha = 0;
                        else if (alpha > 255) alpha = 255;
                        using (Brush brush = new SolidBrush(Color.FromArgb(alpha, Color)))
                        {
                            g.FillRectangle(brush, Scene.ClientRectangle);
                        }
                    }
                    break;
                case EffectStatus.Enabled:
                    using (Brush brush = new SolidBrush(Color))
                    {
                        g.FillRectangle(brush, Scene.ClientRectangle);
                    }
                    break;
                case EffectStatus.Disabling:
                    if (DisablingTime.Value > 0)
                    {
                        int alpha = (int)((1 - DisablingTime.GetRatio()) * Color.A + 0.5F);

                        if (alpha < 0) alpha = 0;
                        else if (alpha > 255) alpha = 255;
                        using (Brush brush = new SolidBrush(Color.FromArgb(alpha, Color)))
                        {
                            g.FillRectangle(brush, Scene.ClientRectangle);
                        }
                    }
                    break;
            }

            base.DoBeforeDrawFloor(g);
        }
    }
}
