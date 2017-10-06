using Microsoft.Win32;
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
        /// 遊戲時間限制(毫秒)
        /// </summary>
        public int PlayingTimeLimit { get; set; }

        /// <summary>
        /// 最高存活時間(毫秒)
        /// </summary>
        public int HighPlayingTime { get; set; }

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
        /// <param name="playingTimeLimit">遊戲時間限制</param>
        public SceneLevelInfo(string sceneID, int level, int playingTimeLimit)
        {
            SceneID = sceneID;
            Level = level;
            PlayingTimeLimit = playingTimeLimit;
        }

        /// <summary>
        /// 匯出二進位資料
        /// </summary>
        /// <returns>二進位資料</returns>
        public byte[] OutPutBytes()
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                byte[] sceneID = Encoding.ASCII.GetBytes(SceneID.PadRight(10, ' '));
                writer.Write(sceneID);
                writer.Write(Level);
                writer.Write(CountOfChallenge);
                writer.Write(TimeOfChallenge);
                writer.Write(HighPlayingTime);
                writer.Write(HighScore);
                writer.Write(Complete);
                result = ms.ToArray();
                writer.Close();
                ms.Close();
            }
            return Function.EncryptByte(result, Global.UUID, GlobalPlayer.PlayerName, false);
        }

        /// <summary>
        /// 解析二進位資料
        /// </summary>
        public void InPutBytes(byte[] input)
        {
            if (input == null) return;

            byte[] infoBytes = Function.EncryptByte(input, Global.UUID, GlobalPlayer.PlayerName, true);
            if (infoBytes == null || infoBytes.Length < 35) return;

            using (MemoryStream ms = new MemoryStream(infoBytes))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                byte[] sceneIDByte = reader.ReadBytes(10);
                string sceneID = Encoding.ASCII.GetString(sceneIDByte).Trim();
                int level = reader.ReadInt32();
                if (sceneID == SceneID && level == Level)
                {
                    CountOfChallenge = reader.ReadInt32();
                    TimeOfChallenge = reader.ReadInt64();
                    HighPlayingTime = reader.ReadInt32();
                    HighScore = reader.ReadInt32();
                    Complete = reader.ReadBoolean();
                }
                reader.Close();
                ms.Close();
            }
        }

        /// <summary>
        /// 寫入機碼
        /// </summary>
        public void WriteRegistry()
        {
            string regAddr = Global.RegistryAddr + "SceneInfo\\";
            string regName = SceneID + "_" + Level.ToString().PadLeft(2, '0');
            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(regAddr))
            {
                byte[] bytes = OutPutBytes();
                registryKey.SetValue(regName, bytes);
                registryKey.Close();
            }
        }

        /// <summary>
        /// 讀取機碼
        /// </summary>
        public void ReadRegistry()
        {
            string regAddr = Global.RegistryAddr + "SceneInfo\\";
            string regName = SceneID + "_" + Level.ToString().PadLeft(2, '0');
            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(regAddr))
            {
                byte[] bytes = registryKey.GetValue(regName, null) as byte[];
                InPutBytes(bytes);
                registryKey.Close();
            }
        }
    }
}
