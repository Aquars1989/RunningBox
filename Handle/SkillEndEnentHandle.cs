﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
   /// <summary>
   /// 處理技能結束事件
   /// </summary>
    /// <param name="sender">觸發物件</param>
   /// <param name="endType">結束方式</param>
    public delegate void SkillEndEnentHandle(object sender, SkillEndType endType);
}
