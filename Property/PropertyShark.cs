using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 震動特性,擁有此特性的物件會震動
    /// </summary>
    class PropertyShark : PropertyBase
    {
        /// <summary>
        /// 震動的強度
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// 效果是否隨時間減弱
        /// </summary>
        public bool Weaken { get; set; }

        /// <summary>
        /// 新增旋轉特性,擁有此特性的物件會旋轉
        /// </summary>
        /// <param name="duration">持續時間(毫秒),小於0為永久</param>
        /// <param name="rotatingPerSec">震動的強度</param>
        /// <param name="weaken">效果是否隨時間減弱</param>
        public PropertyShark(int duration, int power, bool weaken)
        {
            DurationTime.Limit = duration;
            Power = power;
            Weaken = weaken;
        }

        public override void DoBeforeDraw(Graphics g)
        {
            int power = Power;
            if (Weaken)
            {
                Power = (int)(Power * DurationTime.GetRatio());
            }
            _OffsetX = Global.Rand.Next(-Power, Power);
            _OffsetY = Global.Rand.Next(-Power, Power);
            Owner.Layout.X += _OffsetX;
            Owner.Layout.Y += _OffsetY;
            base.DoBeforeDraw(g);
        }

        public override void DoAfterDraw(Graphics g)
        {
            Owner.Layout.X -= _OffsetX;
            Owner.Layout.Y -= _OffsetY;
            _OffsetX = 0;
            _OffsetY = 0;
            base.DoAfterDraw(g);
        }

        public override void DoBeforeEnd(PropertyEndType endType)
        {
            Owner.Layout.X -= _OffsetX;
            Owner.Layout.Y -= _OffsetY;
            _OffsetX = 0;
            _OffsetY = 0;
            base.DoBeforeEnd(endType);
        }

        private float _OffsetX;
        private float _OffsetY;
    }
}
