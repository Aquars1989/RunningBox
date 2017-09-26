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
        /// <param name="sceneID">場景ID</param>
        /// <param name="levelMax">最大關卡數</param>
        public SceneInfo(string sceneID, int levelMax)
        {
            SceneID = sceneID;
            MaxLevel = levelMax;
            SceneLevels = new SceneLevelInfo[MaxLevel];
            for (int i = 0; i < MaxLevel; i++)
            {
                SceneLevels[i] = new SceneLevelInfo(SceneID, i + 1);
            }
        }

        public SceneGaming CreateScene(int level)
        {
            return new T()
            {
                SceneID = this.SceneID,
                Level = level
            };
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
