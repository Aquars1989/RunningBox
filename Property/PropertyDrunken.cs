using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件往目標移動時會產生偏移
    /// </summary>
    class PropertyDrunken : PropertyBase
    {
        private int _NowAngle = 0;
        private bool _Reverse = false;

        /// <summary>
        /// 方向改變間格
        /// </summary>
        public CounterObject ChangeTime { get; private set; }

        /// <summary>
        /// 方向偏移最小值
        /// </summary>
        public int OffsetAngleMin { get; set; }

        /// <summary>
        /// 方向偏移最大值
        /// </summary>
        public int OffsetAngleMax { get; set; }

        /// <summary>
        /// 每次偏移值,0為每次偏移後為隨機角度
        /// </summary>
        public int AnglePerChange { get; set; }

        /// <summary>
        /// 新增瘋狗擺頭特性,擁有此特性的物件往目標移動時會產生偏移
        /// </summary>
        /// <param name="durationTime">持續時間</param>
        /// <param name="changeTime">方向改變間格(毫秒)</param>
        /// <param name="offsetAngleMin">方向偏移最小值</param>
        /// <param name="offsetAngleMax">方向偏移最大值</param>
        /// <param name="anglePerChange">每次偏移值,0為每次偏移後為隨機角度</param>
        public PropertyDrunken(int durationTime, int changeTime, int offsetAngleMin, int offsetAngleMax, int anglePerChange)
        {
            DurationTime.Limit = durationTime;
            OffsetAngleMin = offsetAngleMin;
            OffsetAngleMax = offsetAngleMax;
            AnglePerChange = anglePerChange;
            ChangeTime = new CounterObject(changeTime);
        }

        public override void DoBeforeAction()
        {
            if (ChangeTime.IsFull)
            {
                if (AnglePerChange == 0)
                {
                    _NowAngle = Global.Rand.Next(OffsetAngleMin, OffsetAngleMax);
                }
                else
                {
                    _NowAngle += _Reverse ? -AnglePerChange : AnglePerChange;
                    if (_NowAngle < OffsetAngleMin)
                    {
                        _NowAngle = OffsetAngleMin;
                        _Reverse = !_Reverse;
                    }
                    else if (_NowAngle > OffsetAngleMax)
                    {
                        _NowAngle = OffsetAngleMax;
                        _Reverse = !_Reverse;
                    }
                }
                ChangeTime.Value = 0;
            }
            ChangeTime.Value += Scene.SceneIntervalOfRound;

            Owner.MoveObject.AngleOffset = _NowAngle;
            base.DoBeforeAction();
        }

        public override void DoBeforeEnd(PropertyEndType endType)
        {
            Owner.MoveObject.AngleOffset = 0;
            base.DoBeforeEnd(endType);
        }
    }
}
