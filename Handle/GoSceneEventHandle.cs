using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 處理場景跳轉事件
    /// </summary>
    /// <param name="sender">觸發物件</param>
    /// <param name="scene">跳轉場景</param>
    public delegate void GoSceneEventHandle(object sender, SceneBase scene);
}
