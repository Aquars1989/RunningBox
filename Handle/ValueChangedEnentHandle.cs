using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 處理值變更的事件
    /// </summary>
    /// <param name="sender">觸發物件</param>
    /// <param name="oldVlaue">舊值</param>
    /// <param name="newValue">新值</param>
    public delegate void ValueChangedEnentHandle(object sender,object oldVlaue,object newValue);
}
