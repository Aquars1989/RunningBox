using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件超出區域後被清除
    /// </summary>
    class PropertyOutClear : PropertyBase
    {
        private bool _InRect = false;

        /// <summary>
        /// 新增出界清除特性,擁有此特性的物件超出區域後被清除
        /// </summary>
        public PropertyOutClear() { }

        public override void DoAfterAction()
        {
            bool inRect = Owner.Layout.LeftTopX < Owner.Scene.Width &&
                          Owner.Layout.LeftTopY < Owner.Scene.Height &&
                          Owner.Layout.LeftTopX + Owner.Layout.Width > 0 &&
                          Owner.Layout.LeftTopY + Owner.Layout.Height > 0;
            if (_InRect == inRect) return;
            if (_InRect)
            {
                Owner.Kill(null, ObjectDeadType.Clear);
            }
            else
            {
                _InRect = inRect;
            }
        }
    }
}
