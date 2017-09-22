using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 旋轉鎖定特性,擁有此特性的物件會面向到目標
    /// </summary>
    class PropertyRotateTarget : PropertyBase
    {
        /// <summary>
        /// 每秒旋轉角度
        /// </summary>
        public float RotatingPerSec { get; set; }

        /// <summary>
        /// 是否忽略阻力
        /// </summary>
        public bool IgnoreResistance { get; set; }

        /// <summary>
        /// 新增旋轉鎖定特性,擁有此特性的物件會面向到目標
        /// </summary>
        /// <param name="duration">持續時間(毫秒),小於0為永久</param>
        /// <param name="rotatingPerSec">每秒旋轉角度</param>
        /// <param name="ignoreResistance">是否忽略阻力</param>
        public PropertyRotateTarget(int duration, float rotatingPerSec, bool ignoreResistance)
        {
            DurationTime.Limit = duration;
            RotatingPerSec = rotatingPerSec;
            IgnoreResistance = ignoreResistance;
        }

        public override void DoAfterAction()
        {
            if (Status == PropertyStatus.Enabled)
            {
                float rotate = RotatingPerSec / Scene.SceneRoundPerSec;
                
             
                Owner.DrawObject.Rotate(RotatingPerSec / Scene.SceneRoundPerSec, IgnoreResistance);
            }
            base.DoAfterAction();
        }
    }
}
