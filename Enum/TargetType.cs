
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 目標纇型
    /// </summary>
    public enum TargetType
    {
        /// <summary>
        /// 無
        /// </summary>
        None,

        /// <summary>
        /// 座標點
        /// </summary>
        Point,

        /// <summary>
        /// 遊戲物件
        /// </summary>
        GameObejct,

        /// <summary>
        /// 位置配置物件
        /// </summary>
        Layout,

        /// <summary>
        /// 場景
        /// </summary>
        Scene
    }
}
