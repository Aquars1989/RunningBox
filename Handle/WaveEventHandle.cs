using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 處理關卡事件
    /// </summary>
    /// <param name="value">事件參數值</param>
    /// <returns>回傳是否引發事件</returns>
    public delegate void WaveEventHandle(int value);
}
