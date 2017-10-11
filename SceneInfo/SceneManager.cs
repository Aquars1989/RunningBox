using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 管理場景資訊管理物件
    /// </summary>
    public class SceneInfoManager
    {
        private bool _GetInfo = false;
        private Dictionary<string, ISceneInfo> _Items = new Dictionary<string, ISceneInfo>();

        private int _CountOfLevel;
        /// 取得所有關卡數量
        /// </summary>
        public int CountOfLevel
        {
            get
            {
                RefreshInfo();
                return _CountOfLevel;
            }
        }

        private int _CountOfChallenge;
        /// <summary>
        /// 取得場景挑戰次數
        /// </summary>
        public int CountOfChallenge
        {
            get
            {
                RefreshInfo();
                return _CountOfChallenge;
            }
        }

        private long _TimeOfChallenge;
        /// <summary>
        /// 取得場景挑戰時間(毫秒)
        /// </summary>
        public long TimeOfChallenge
        {
            get
            {
                RefreshInfo();
                return _TimeOfChallenge;
            }
        }

        private long _HighPlayingTime;
        /// <summary>
        /// 取得場景最高存活時間
        /// </summary>
        public long HighPlayingTime
        {
            get
            {
                RefreshInfo();
                return _HighPlayingTime;
            }
        }

        private int _HighScore;
        /// <summary>
        /// 取得場景最高分數
        /// </summary>
        public int HighScore
        {
            get
            {
                RefreshInfo();
                return _HighScore;
            }
        }

        private int _CountOfComplete;
        /// <summary>
        /// 取得場景完成關卡數
        /// </summary>
        public int CountOfComplete
        {
            get
            {
                RefreshInfo();
                return _CountOfComplete;
            }
        }

        /// <summary>
        /// 增加場景資訊管理物件
        /// </summary>
        /// <param name="sceneSet">場景資訊</param>
        public void AddScene(ISceneInfo sceneSet)
        {
            _Items.Add(sceneSet.SceneID, sceneSet);
            sceneSet.InfoChanged += (x, e) =>
            {
                _GetInfo = false;
            };
            _GetInfo = false;
        }

        /// <summary>
        /// 取得場景資訊群組
        /// </summary>
        /// <returns>場景資訊群組</returns>
        public Dictionary<string, ISceneInfo>.ValueCollection GetItems()
        {
            return _Items.Values;
        }

        /// <summary>
        /// 將所有場景資訊寫入機碼
        /// </summary>
        public void AllWriteRegistry()
        {
            foreach (ISceneInfo sceneInfo in _Items.Values)
            {
                sceneInfo.AllWriteRegistry();
            }
        }

        /// <summary>
        /// 從機碼讀出所有場景資訊
        /// </summary>
        public void AllReadRegistry()
        {
            foreach (ISceneInfo sceneInfo in _Items.Values)
            {
                sceneInfo.AllReadRegistry();
            }
        }

        private void RefreshInfo()
        {
            if (_GetInfo) return;

            _CountOfLevel = 0;
            _CountOfChallenge = 0;
            _TimeOfChallenge = 0;
            _HighPlayingTime = 0;
            _HighScore = 0;
            _CountOfComplete = 0;
            foreach (ISceneInfo sceneInfo in _Items.Values)
            {
                _CountOfLevel += sceneInfo.MaxLevel;
                _CountOfChallenge += sceneInfo.CountOfChallenge;
                _TimeOfChallenge += sceneInfo.TimeOfChallenge;
                _HighPlayingTime += sceneInfo.HighPlayingTime;
                _HighScore += sceneInfo.HighScore;
                _CountOfComplete += sceneInfo.CountOfComplete;
            }
            _GetInfo = true;
        }
    }
}
