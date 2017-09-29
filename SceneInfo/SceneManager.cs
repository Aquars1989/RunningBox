﻿using Microsoft.Win32;
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
        private Dictionary<string, ISceneInfo> _Items = new Dictionary<string, ISceneInfo>();

        /// <summary>
        /// 增加場景資訊管理物件
        /// </summary>
        /// <param name="sceneSet">場景資訊</param>
        public void AddScene(ISceneInfo sceneSet)
        {
            _Items.Add(sceneSet.SceneID, sceneSet);
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
    }
}
