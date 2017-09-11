using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 處理繪製物件繪圖事件
    /// </summary>
    /// <param name="g">Graphics物件</param>
    /// <param name="rectangle">繪置區域</param>
    public delegate void DrawObjectEnentHandle(object sender, Graphics g, Rectangle rectangle);
}
