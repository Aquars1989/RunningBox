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
        Dictionary<string, int> _ScoreDetail = new Dictionary<string, int>();
        /// <summary>
        /// 取得分數明細
        /// </summary>
        public IReadOnlyDictionary<string, int> ScoreDetail
        {
            get { return _ScoreDetail; }
        }

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
        public int Score { get; private set; }

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

        /// <summary>
        /// 新增分數
        /// </summary>
        /// <param name="from">分數來源</param>
        /// <param name="score">分數</param>
        public void AddScore(string from, int score)
        {
            if (_ScoreDetail.ContainsKey(from))
            {
                _ScoreDetail[from] += score;
            }
            else
            {
                _ScoreDetail.Add(from, score);
            }
            Score += score;
        }
    }
}
