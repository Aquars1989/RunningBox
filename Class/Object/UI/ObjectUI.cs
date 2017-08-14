using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 基礎介面物件
    /// </summary>
    public class ObjectUI : ObjectBase
    {
        public bool Enabled { get; set; }

        public override void Action() { }
    }
}
