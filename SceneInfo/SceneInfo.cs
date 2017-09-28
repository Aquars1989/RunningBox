using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 記錄場景資訊
    /// </summary>
    /// <typeparam name="T">場景物件類別</typeparam>
    public class SceneInfo<T> : ISceneInfo
        where T : SceneGaming, new()
    {
        private SceneLevelInfo[] SceneLevels { get; set; }

        /// <summary>
        /// 取得場景名稱
        /// </summary>
        public string SceneName { get; private set; }

        /// <summary>
        /// 取得場景ID
        /// </summary>
        public string SceneID { get; private set; }

        /// <summary>
        /// 取得場景最大等級
        /// </summary>
        public int MaxLevel { get; private set; }

        /// <summary>
        /// 取得場景挑戰次數
        /// </summary>
        public int CountOfChallenge { get; private set; }

        /// <summary>
        /// 取得場景挑戰時間(毫秒)
        /// </summary>
        public long TimeOfChallenge { get; private set; }

        /// <summary>
        /// 取得場景最高存活時間
        /// </summary>
        public int HighPlayingTime { get; private set; }

        /// <summary>
        /// 取得場景最高分數
        /// </summary>
        public int HighScore { get; private set; }

        /// <summary>
        /// 取得場景完成關卡數
        /// </summary>
        public int CountOfComplete { get; private set; }

        /// <summary>
        /// 新增場景資訊記錄
        /// </summary>
        /// <param name="sceneID">場景名稱</param>
        /// <param name="sceneID">場景ID</param>
        /// <param name="levelMax">最大關卡數</param>
        /// <param name="playingTimeLimit">預設時間限制(毫秒)</param>
        public SceneInfo(string sceneName, string sceneID, int levelMax, int playingTimeLimit)
        {
            SceneName = sceneName;
            SceneID = sceneID;
            MaxLevel = levelMax;
            SceneLevels = new SceneLevelInfo[MaxLevel];
            for (int i = 0; i < MaxLevel; i++)
            {
                SceneLevels[i] = new SceneLevelInfo(SceneID, i + 1, playingTimeLimit);
            }
        }

        /// <summary>
        /// 建立場景實體
        /// </summary>
        /// <param name="level">關卡等級</param>
        /// <returns>場景實體</returns>
        public SceneGaming CreateScene(int level)
        {
            if (level < 1 || level > MaxLevel) return null;
            int idx = level - 1;

            return new T()
            {
                SceneID = this.SceneID,
                Level = level,
                PlayingTimeLimit = SceneLevels[idx].PlayingTimeLimit
            };
        }

        /// <summary>
        /// 取得指定等級是否完成
        /// </summary>
        /// <param name="level">指定等級</param>
        /// <returns>是否完成</returns>
        public bool GetComplete(int level)
        {
            if (level < 1 || level > MaxLevel) return false;
            int idx = level - 1;
            return SceneLevels[idx].Complete;
        }

        /// <summary>
        /// 取得指定等級的挑戰次數
        /// </summary>
        /// <param name="level">指定等級</param>
        /// <returns>挑戰次數</returns>
        public int GetCountOfChallenge(int level)
        {
            if (level < 1 || level > MaxLevel) return 0;
            int idx = level - 1;
            return SceneLevels[idx].CountOfChallenge;
        }

        /// <summary>
        /// 取得指定等級的挑戰時間
        /// </summary>
        /// <param name="level">指定等級</param>
        /// <returns>挑戰時間</returns>
        public long GetTimeOfChallenge(int level)
        {
            if (level < 1 || level > MaxLevel) return 0;
            int idx = level - 1;
            return SceneLevels[idx].TimeOfChallenge;
        }

        /// <summary>
        /// 取得指定等級的最高存活時間
        /// </summary>
        /// <param name="level">指定等級</param>
        /// <returns>最高存活時間</returns>
        public int GetHighPlayingTime(int level)
        {
            if (level < 1 || level > MaxLevel) return 0;
            int idx = level - 1;
            return SceneLevels[idx].HighPlayingTime;
        }

        /// <summary>
        /// 取得指定等級的最高分數
        /// </summary>
        /// <param name="level">指定等級</param>
        /// <returns>最高分數</returns>
        public int GetHighScore(int level)
        {
            if (level < 1 || level > MaxLevel) return 0;
            int idx = level - 1;
            return SceneLevels[idx].HighScore;
        }

        /// <summary>
        /// 設定關卡資訊
        /// </summary>
        /// <param name="level">關卡等級</param>
        /// <param name="countOfChallenge">挑戰次數</param>
        /// <param name="timeOfChallenge">挑戰時間</param>
        /// <param name="highSurviveTime">最高存活時間</param>
        /// <param name="highScore">最高分數</param>
        /// <param name="complete">是否完成</param>
        public void SetValue(int level, int countOfChallenge, long timeOfChallenge, int highSurviveTime, int highScore, bool complete)
        {
            if (level < 1 || level > MaxLevel) return;

            int idx = level - 1;
            CountOfChallenge += countOfChallenge - SceneLevels[idx].CountOfChallenge;
            TimeOfChallenge += timeOfChallenge - SceneLevels[idx].TimeOfChallenge;
            HighScore += highScore - SceneLevels[idx].HighScore;
            CountOfComplete += (complete ? 1 : 0) - (SceneLevels[idx].Complete ? 1 : 0);

            SceneLevels[idx].CountOfChallenge = countOfChallenge;
            SceneLevels[idx].TimeOfChallenge = timeOfChallenge;
            SceneLevels[idx].PlayingTimeLimit = highSurviveTime;
            SceneLevels[idx].HighScore = highScore;
            SceneLevels[idx].Complete = complete;
        }

        /// <summary>
        /// 結算成績
        /// </summary>
        /// <param name="level">關卡等級</param>
        /// <param name="playingTime">本次存活時間</param>
        /// <param name="score">本次分數</param>
        /// <param name="complete">是否完成</param>
        public void Settlement(int level, int playingTime, int score, bool complete)
        {
            if (level < 1 || level > MaxLevel) return;

            int idx = level - 1;
            CountOfChallenge++;
            TimeOfChallenge += playingTime;

            SceneLevels[idx].CountOfChallenge++;
            SceneLevels[idx].TimeOfChallenge += playingTime;

            if (playingTime > SceneLevels[idx].HighPlayingTime)
            {
                HighPlayingTime += playingTime - SceneLevels[idx].HighPlayingTime;
                SceneLevels[idx].HighPlayingTime = playingTime;
            }

            if (score > SceneLevels[idx].HighScore)
            {
                HighScore += score - SceneLevels[idx].HighScore;
                SceneLevels[idx].HighScore = score;
            }

            if (!SceneLevels[idx].Complete && complete)
            {
                CountOfComplete++;
                SceneLevels[idx].Complete = complete;
            }
        }

        /// <summary>
        /// 結算成績
        /// </summary>
        /// <param name="playInfo">場景挑戰記錄</param>
        public void Settlement(ScenePlayingInfo playInfo)
        {
            if (playInfo.SceneID != SceneID) return;
            Settlement(playInfo.Level, playInfo.PlayingTime.Value, playInfo.Score, playInfo.Complete);
        }
    }
}
