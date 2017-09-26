using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 記錄場景關卡資訊
    /// </summary>
    public class SceneLevelInfo
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
        /// 挑戰次數
        /// </summary>
        public int CountOfChallenge { get; set; }

        /// <summary>
        /// 挑戰時間(毫秒)
        /// </summary>
        public long TimeOfChallenge { get; set; }

        /// <summary>
        /// 最高分數
        /// </summary>
        public int HighScore { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// 場景關卡等級資訊
        /// </summary>
        /// <param name="sceneID">場景ID</param>
        /// <param name="level">關卡等級</param>
        public SceneLevelInfo(string sceneID, int level)
        {
            SceneID = SceneID;
            Level = level;
        }

        public byte[] GetByte()
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] sceneID = Encoding.ASCII.GetBytes(SceneID);
                ms.Write(sceneID, 0, sceneID.Length);
                ms.Write(BitConverter.GetBytes(Level), 0, 4);
                ms.Write(BitConverter.GetBytes(CountOfChallenge), 0, 4);
                ms.Write(BitConverter.GetBytes(TimeOfChallenge), 0, 8);
                ms.Write(BitConverter.GetBytes(HighScore), 0, 4);
                ms.Write(BitConverter.GetBytes(Complete), 0, 1);
                result = ms.ToArray();
                ms.Close();
            }
            return Function.EncryptByte(result, Global.UUID, Global.PlayerName, false);
        }
    }
}
