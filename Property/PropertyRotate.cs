using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 旋轉特性,擁有此特性的物件會旋轉
    /// </summary>
    class PropertyRotate : PropertyBase
    {
        /// <summary>
        /// 每秒旋轉角度
        /// </summary>
        public float RotatingPerSec { get; set; }

        /// <summary>
        /// 效果是否隨時間減弱
        /// </summary>
        public bool Weaken { get; set; }

        /// <summary>
        /// 是否忽略阻力
        /// </summary>
        public bool IgnoreResistance { get; set; }

        /// <summary>
        /// 新增旋轉特性,擁有此特性的物件會旋轉
        /// </summary>
        /// <param name="duration">持續時間(毫秒),小於0為永久</param>
        /// <param name="rotatingPerSec">每秒旋轉角度</param>
        /// <param name="weaken">效果是否隨時間減弱</param>
        /// <param name="ignoreResistance">是否忽略阻力</param>
        public PropertyRotate(int duration, float rotatingPerSec, bool weaken, bool ignoreResistance)
        {
            DurationTime.Limit = duration;
            RotatingPerSec = rotatingPerSec;
            Weaken = weaken;
            IgnoreResistance = ignoreResistance;
        }

        public override void DoAfterAction()
        {
            if (Status == PropertyStatus.Enabled)
            {
                float rotate = RotatingPerSec / Scene.SceneRoundPerSec;
                if (Weaken)
                {
                    rotate *= 1 - DurationTime.GetRatio();
                }
                Owner.DrawObject.Rotate(rotate, IgnoreResistance);
            }
            base.DoAfterAction();
        }
    }
}
