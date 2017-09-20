
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 畫面震動的特效
    /// </summary>
    public class EffectShark : EffectBase
    {
        /// <summary>
        /// 震動的強度
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// 新增畫面震動的特效
        /// </summary>
        /// <param name="durationTime">震動持續的時間(毫秒),小於0為永久</param>
        /// <param name="power">震動的強度</param>
        public EffectShark(int durationTime, int power) :
            base(0, durationTime, 0)
        {
            Power = power;
        }

        public override void DoBeforeDraw(Graphics g)
        {
            int shakeX = Global.Rand.Next(-Power, Power);
            int shakeY = Global.Rand.Next(-Power, Power);
            g.TranslateTransform(shakeX, shakeY, System.Drawing.Drawing2D.MatrixOrder.Append);

            base.DoBeforeDraw(g);
        }
    }
}
