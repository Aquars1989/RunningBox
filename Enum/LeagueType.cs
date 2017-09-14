
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 活動物件所屬陣營
    /// </summary>
    public enum LeagueType
    {
        /// <summary>
        /// 無陣營,對所有陣營都為友好
        /// </summary>
        None = 0,

        /// <summary>
        /// 玩家陣營
        /// </summary>
        Player = 1,

        /// <summary>
        /// 敵對陣營1
        /// </summary>
        Ememy1 = 2,

        /// <summary>
        /// 敵對陣營2
        /// </summary>
        Ememy2 = 3,

        /// <summary>
        /// 對所有陣營包含自身都為敵對
        /// </summary>
        Chaos = 10,
    }
}
