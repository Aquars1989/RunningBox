
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
        /// 震動持續的時間計時器(毫秒)
        /// </summary>
        public CounterObject DurationTime { get; private set; }

        /// <summary>
        /// 震動的強度
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// 新增畫面震動的特效
        /// </summary>
        /// <param name="durationTime">震動持續的時間(毫秒),小於0為永久</param>
        /// <param name="power">震動的強度</param>
        public EffectShark(int durationTime, int power)
        {
            CanBreak = true;
            Status = EffectStatus.Enabled;
            DurationTime = new CounterObject(durationTime);
            Power = power;
        }

        public override void DoAfterRound()
        {
            switch (Status)
            {
                case EffectStatus.Enabled:
                    if (DurationTime.IsFull)
                    {
                        OnEnd(EffectEndType.Finish, EffectStatus.Disabled);
                    }
                    else
                    {
                        DurationTime.Value += Scene.SceneIntervalOfRound;
                    }
                    break;
            }

            base.DoAfterRound();
        }

        public override void DoBeforeDraw(Graphics g)
        {
            int shakeX = Global.Rand.Next(-Power, Power);
            int shakeY = Global.Rand.Next(-Power, Power);
            g.TranslateTransform(shakeX, shakeY, System.Drawing.Drawing2D.MatrixOrder.Append);

            base.DoBeforeDraw(g);
        }

        protected override void OnEnd(EffectEndType endType)
        {
            Status = EffectStatus.Disabling;

            if (End != null)
            {
                End(this, endType);
            }
        }
    }
}
