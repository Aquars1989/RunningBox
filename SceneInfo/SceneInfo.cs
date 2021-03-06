﻿using System;
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
        /// <summary>
        /// 關卡資訊
        /// </summary>
        private SceneLevelInfo[] SceneLevels { get; set; }

        /// <summary>
        /// 發生於資訊變更
        /// </summary>
        public event EventHandler InfoChanged;

        /// <summary>
        /// 發生於資訊變更
        /// </summary>
        public void OnInfoChanged()
        {
            if (InfoChanged != null)
            {
                InfoChanged(this, new EventArgs());
            }
        }

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
                SceneInfo = this,
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
        /// 取得指定等級的時間限制
        /// </summary>
        /// <param name="level">指定等級</param>
        /// <returns>時間限制</returns>
        public int GetPlayingTimeLimit(int level)
        {
            if (level < 1 || level > MaxLevel) return 0;
            int idx = level - 1;
            return SceneLevels[idx].PlayingTimeLimit;
        }

        /// <summary>
        /// 設定關卡資訊
        /// </summary>
        /// <param name="level">關卡等級</param>
        /// <param name="countOfChallenge">挑戰次數</param>
        /// <param name="timeOfChallenge">挑戰時間</param>
        /// <param name="highPlayingTime">最高存活時間</param>
        /// <param name="highScore">最高分數</param>
        /// <param name="complete">是否完成</param>
        public void SetValue(int level, int countOfChallenge, long timeOfChallenge, int highPlayingTime, int highScore, bool complete)
        {
            if (level < 1 || level > MaxLevel) return;

            int idx = level - 1;
            CountOfChallenge += countOfChallenge - SceneLevels[idx].CountOfChallenge;
            TimeOfChallenge += timeOfChallenge - SceneLevels[idx].TimeOfChallenge;
            HighPlayingTime += highPlayingTime - SceneLevels[idx].HighPlayingTime;
            HighScore += highScore - SceneLevels[idx].HighScore;
            CountOfComplete += (complete ? 1 : 0) - (SceneLevels[idx].Complete ? 1 : 0);

            SceneLevels[idx].CountOfChallenge = countOfChallenge;
            SceneLevels[idx].TimeOfChallenge = timeOfChallenge;
            SceneLevels[idx].HighPlayingTime = highPlayingTime;
            SceneLevels[idx].HighScore = highScore;
            SceneLevels[idx].Complete = complete;
            OnInfoChanged();
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

            SceneLevels[idx].WriteRegistry();
            OnInfoChanged();
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

        /// <summary>
        /// 將指定關卡等級資訊寫入機碼
        /// </summary>
        /// <param name="level">關卡等級</param>
        public void WriteRegistry(int level)
        {
            if (level < 1 || level > MaxLevel) return;

            int idx = level - 1;
            SceneLevels[idx].WriteRegistry();
        }

        /// <summary>
        /// 從機碼讀出指定關卡等級資訊
        /// </summary>
        /// <param name="level">關卡等級</param>
        public void ReadRegistry(int level)
        {
            if (level < 1 || level > MaxLevel) return;

            int idx = level - 1;
            int oldCountOfChallenge = SceneLevels[idx].CountOfChallenge;
            long oldTimeOfChallenge = SceneLevels[idx].TimeOfChallenge;
            int oldHighPlayingTime = SceneLevels[idx].HighPlayingTime;
            int oldHighScore = SceneLevels[idx].HighScore;
            bool oldComplete = SceneLevels[idx].Complete;
            SceneLevels[idx].ReadRegistry();
            CountOfChallenge += SceneLevels[idx].CountOfChallenge - oldCountOfChallenge;
            TimeOfChallenge += SceneLevels[idx].TimeOfChallenge - oldTimeOfChallenge;
            HighPlayingTime += SceneLevels[idx].HighPlayingTime - oldHighPlayingTime;
            HighScore += SceneLevels[idx].HighScore - oldHighScore;
            CountOfComplete += (SceneLevels[idx].Complete ? 1 : 0) - (oldComplete ? 1 : 0);
            OnInfoChanged();
        }

        /// <summary>
        /// 將所有關卡等級資訊寫入機碼
        /// </summary>
        public void AllWriteRegistry()
        {
            for (int i = 1; i <= MaxLevel; i++)
            {
                SceneLevels[i].WriteRegistry();
            }
        }

        /// <summary>
        /// 從機碼讀出所有關卡等級資訊
        /// </summary>
        public void AllReadRegistry()
        {
            CountOfChallenge = 0;
            TimeOfChallenge = 0;
            HighPlayingTime = 0;
            HighScore = 0;
            CountOfComplete = 0;

            for (int i = 0; i < MaxLevel; i++)
            {
                SceneLevels[i].ReadRegistry();
                CountOfChallenge += SceneLevels[i].CountOfChallenge;
                TimeOfChallenge += SceneLevels[i].TimeOfChallenge;
                HighPlayingTime += SceneLevels[i].HighPlayingTime;
                HighScore += SceneLevels[i].HighScore;
                CountOfComplete += (SceneLevels[i].Complete ? 1 : 0);
            }
            OnInfoChanged();
        }
    }
}
