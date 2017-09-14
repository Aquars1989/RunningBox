using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件死亡時會逐漸縮小
    /// </summary>
    class PropertyDeadSmoke : PropertyBase
    {
        /// <summary>
        /// 縮小時間計時器(毫秒)
        /// </summary>
        public CounterObject ShrinkTime { get; private set; }

        /// <summary>
        /// 符合指定的死亡方式才會觸發
        /// </summary>
        public ObjectDeadType DeadType { get; set; }

        /// <summary>
        /// 新增煙霧特性,擁有此特性的物件死亡時會逐漸縮小
        /// </summary>
        /// <param name="shrinkTime">縮小時間(毫秒)</param>
        /// <param name="deadType">符合指定的死亡方式才會觸發</param>
        public PropertyDeadSmoke( int shrinkTime, ObjectDeadType deadType)
        {
            DeadType = deadType;
            ShrinkTime = new CounterObject(shrinkTime);
            Affix = SpecialStatus.Remain;
        }

        public override void DoAfterAction()
        {
            if (Owner.Status == ObjectStatus.Dead)
            {
                if (ShrinkTime.IsFull)
                {
                    End(PropertyEndType.Finish);
                }
                else
                {
                    Owner.Layout.Scale = 1F - ShrinkTime.GetRatio();
                    ShrinkTime.Value += Owner.Scene.SceneIntervalOfRound;
                }
            }
        }
    }
}
