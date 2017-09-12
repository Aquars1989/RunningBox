
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 基本場景物件
    /// </summary>
    public class SceneNull : SceneBase
    {
        public static SceneNull Value = new SceneNull();
        protected override void OnLoadComplete() { }
    }
}
