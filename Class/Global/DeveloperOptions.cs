using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    class DeveloperOptions
    {
        /// <summary>
        /// 角色碰撞關閉
        /// </summary>
        public static bool Player_Ghost = false;

        /// <summary>
        /// 角色無敵
        /// </summary>
        public static bool Player_GodMode = true;

        /// <summary>
        /// 取消CD時間
        /// </summary>
        public static bool Player_NoCooldown = true;

        /// <summary>
        /// 取消能量消耗
        /// </summary>
        public static bool Player_NoCast = true;
    }
}
