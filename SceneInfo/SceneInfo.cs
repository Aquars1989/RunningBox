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
        public SceneInfo(string sceneName, string sceneID, int levelMax)
        {
            SceneName = sceneName;
            SceneID = sceneID;
            MaxLevel = levelMax;
            SceneLevels = new SceneLevelInfo[MaxLevel];
            for (int i = 0; i < MaxLevel; i++)
            {
                SceneLevels[i] = new SceneLevelInfo(SceneID, i + 1);
            }
        }

        /// <summary>
        /// 建立場景實體
        /// </summary>
        /// <param name="level">關卡等級</param>
        /// <returns>場景實體</returns>
        public SceneGaming CreateScene(int level)
        {
            return new T()
            {
                SceneID = this.SceneID,
                Level = level
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
        /// <param name="highScore">最高分數</param>
        /// <param name="complete">是否完成</param>
        public void SetValue(int level, int countOfChallenge, long timeOfChallenge, int highScore, bool complete)
        {
            if (level < 1 || level > MaxLevel) return;

            int idx = level - 1;
            CountOfChallenge += countOfChallenge - SceneLevels[idx].CountOfChallenge;
            TimeOfChallenge += timeOfChallenge - SceneLevels[idx].TimeOfChallenge;
            HighScore += highScore - SceneLevels[idx].HighScore;
            CountOfComplete += (complete ? 1 : 0) - (SceneLevels[idx].Complete ? 1 : 0);

            SceneLevels[idx].CountOfChallenge = countOfChallenge;
            SceneLevels[idx].TimeOfChallenge = timeOfChallenge;
            SceneLevels[idx].HighScore = highScore;
            SceneLevels[idx].Complete = complete;
        }

        /// <summary>
        /// 結算成績
        /// </summary>
        /// <param name="level">關卡等級</param>
        /// <param name="timeOfChallenge">本次挑戰時間</param>
        /// <param name="score">本次分數</param>
        /// <param name="complete">是否完成</param>
        public void Settlement(int level, long timeOfChallenge, int score, bool complete)
        {
            if (level < 1 || level > MaxLevel) return;

            int idx = level - 1;
            CountOfChallenge++;
            TimeOfChallenge += timeOfChallenge;

            SceneLevels[idx].CountOfChallenge++;
            SceneLevels[idx].TimeOfChallenge += timeOfChallenge;

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
    }
}
