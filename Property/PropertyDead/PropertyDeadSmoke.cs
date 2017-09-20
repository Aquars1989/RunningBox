using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件死亡時會逐漸縮小(放大)並淡出
    /// </summary>
    class PropertyDeadSmoke : PropertyBase
    {
        private bool _Enabled = false;
        private float _BaseScale;
        private float _BaseOpacity;

        /// <summary>
        /// 縮小(放大)淡出時間計時器(毫秒)
        /// </summary>
        public CounterObject FadeTime { get; private set; }

        /// <summary>
        /// 最終大小比例
        /// </summary>
        public float FinelScale { get; private set; }

        /// <summary>
        /// 最終透明度
        /// </summary>
        public float FinelOpacity { get; private set; }

        /// <summary>
        /// 符合指定的死亡方式才會觸發
        /// </summary>
        public ObjectDeadType DeadType { get; set; }

        /// <summary>
        ///擁有此特性的物件死亡時會逐漸縮小(放大)並淡出
        /// </summary>
        /// <param name="fadeTime">縮小/淡出時間計時器(毫秒)</param>
        /// <param name="fadeTime">計時器結束時大小比例</param>
        /// <param name="fadeTime">計時器結束時透明度</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        public PropertyDeadSmoke(int fadeTime, float finelScale, float finelOpacity, ObjectDeadType deadType)
        {
            DeadType = deadType;
            FadeTime = new CounterObject(fadeTime);
            FinelScale = finelScale;
            FinelOpacity = finelOpacity;
            BreakAfterDead = false;
        }

        public override void DoAfterDead(ObjectBase killer, ObjectDeadType deadType)
        {
            if ((DeadType & deadType) != deadType) return;

            Affix = SpecialStatus.Remain;
            FadeTime.Value = 0;
            _Enabled = true;
            _BaseScale = Owner.Layout.Scale;
            _BaseOpacity = Owner.DrawObject.Colors.Opacity;
            base.DoAfterDead(killer, deadType);
        }

        public override void DoAfterAction()
        {
            if (Owner.Status == ObjectStatus.Dead && _Enabled)
            {
                if (FadeTime.IsFull)
                {
                    Affix = SpecialStatus.None;
                    _Enabled = false;
                }
                else
                {
                    if (FinelScale != 1)
                    {
                        Owner.Layout.Scale = _BaseScale * (1F - FadeTime.GetRatio() * (1F - FinelScale));
                    }

                    if (FinelOpacity != 1)
                    {
                        Owner.DrawObject.Colors.Opacity = _BaseOpacity * (1F - FadeTime.GetRatio() * (1F - FinelOpacity));
                    }
                    FadeTime.Value += Owner.Scene.SceneIntervalOfRound;
                }
            }

            base.DoAfterAction();
        }
    }
}
