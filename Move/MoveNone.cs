using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 表示無效的移動物件
    /// </summary>
    class MoveNone : MoveBase
    {
        /// <summary>
        /// 表示無效的移動物件
        /// </summary>
        public MoveNone() : base(new TargetPoint(0, 0)) 
        {
        //todo add targetnone
        }
        public override void Plan() { }
        public override void Move() { }
    }
}
