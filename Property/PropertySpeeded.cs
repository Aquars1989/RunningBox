﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件每回合速度會增加
    /// </summary>
    class PropertySpeeded : PropertyBase
    {
        /// <summary>
        /// 已提升的速度
        /// </summary>
        private float _SpeededToatl = 0;

        /// <summary>
        /// 每秒速度提升
        /// </summary>
        public float SpeededPerSec { get; set; }

        /// <summary>
        /// 新增加速特性,擁有此特性的物件速度會逐漸增加
        /// </summary>
        /// <param name="durationRound">持續時間(毫秒),小於0為永久</param>
        /// <param name="speededPerSec">每秒速度增加</param>
        public PropertySpeeded(int durationRound, float speededPerSec)
        {
            DurationTime.Limit = durationRound;
            SpeededPerSec = speededPerSec;
        }

        public override void DoAfterAction()
        {
            float speeded = SpeededPerSec / Scene.SceneRoundPerSec;
            Owner.MoveObject.Speed += speeded;
            _SpeededToatl += speeded;

            base.DoAfterAction();
        }

        public override void DoBeforeEnd(PropertyEndType endType)
        {
            Owner.MoveObject.Speed -= _SpeededToatl;

            base.DoBeforeEnd(endType);
        }
    }
}
