using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 圖示繪圖介面
    /// </summary>
    public class ObjectUIIcon : ObjectUI
    {
        public ObjectUIIcon(DrawIconBase drawObject)
        {
            DrawObject = drawObject;
        }
    }
}
