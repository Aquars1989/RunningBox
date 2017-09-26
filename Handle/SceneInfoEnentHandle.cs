using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 處理場景資訊相關事件
    /// </summary>
    /// <param name="sender">觸發物件</param>
    /// <param name="sceneInfo">場景資訊</param>
    /// <param name="level">觀卡等級</param>
    public delegate void SceneInfoEnentHandle(object sender, ISceneInfo sceneInfo,int level);
}
