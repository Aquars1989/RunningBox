using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 新增挑戰記錄
    /// </summary>
    public class ScenePlayingInfo
    {
        /// <summary>
        /// 場景ID
        /// </summary>
        public string SceneID { get; private set; }

        /// <summary>
        /// 關卡等級
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 本次存活時間
        /// </summary>
        public CounterObject PlayingTime { get; private set; }

        /// <summary>
        /// 本次分數
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 本次是否完成
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// 場景關卡等級資訊
        /// </summary>
        /// <param name="sceneID">場景ID</param>
        /// <param name="level">關卡等級</param>
        /// <param name="playingTime">遊戲時間限制(毫秒)</param>
        public ScenePlayingInfo(string sceneID, int level, int playingTime)
        {
            PlayingTime = new CounterObject(playingTime, 0, false);
            SceneID = sceneID;
            Level = level;
        }
    }
}
