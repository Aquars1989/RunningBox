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
    class MoveNull : MoveBase
    {
        public static MoveNull Value = new MoveNull();

        /// <summary>
        /// 表示無效的移動物件
        /// </summary>
        private MoveNull() : base(TargetNull.Value, 1, 0, 0) { }
        public override void Plan() { }
        public override void Move() { }
    }
}
