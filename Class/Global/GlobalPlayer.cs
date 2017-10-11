using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace RunningBox
{
    class GlobalPlayer
    {
        public static string PlayerName;
        public static string PlayerID;
        public static string UUID;

        static GlobalPlayer()
        {
            ManagementObjectCollection instances = new ManagementClass("Win32_ComputerSystemProduct").GetInstances();
            foreach (ManagementObject mo in instances)
            {
                UUID = mo.Properties["UUID"].Value.ToString();
                break;
            }
        }

        public static void WriteRegistry()
        {
            string regAddr = Global.RegistryAddr + "PlayInfo\\";
            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(regAddr))
            {
                byte[] playerName = Encoding.UTF8.GetBytes(PlayerName);
                byte[] playerID = Encoding.UTF8.GetBytes(PlayerID);
                registryKey.SetValue("PlayerName", Function.EncryptByte(playerName, "Player_" + UUID, PlayerID, false));
                registryKey.SetValue("PlayerID", Function.EncryptByte(playerID, "Player_" + UUID, "PlayerID", false));
                registryKey.Close();
            }
        }

        public static void ReadRegistry()
        {
            string regAddr = Global.RegistryAddr + "PlayInfo\\";
            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(regAddr))
            {
                byte[] playerName = registryKey.GetValue("PlayerName", null) as byte[];
                byte[] playerID = registryKey.GetValue("PlayerID", null) as byte[];
                playerID = Function.EncryptByte(playerID, "Player_" + UUID, "PlayerID", true);
                if (playerID != null)
                {
                    PlayerID = Encoding.UTF8.GetString(playerID);
                }
                if (string.IsNullOrWhiteSpace(PlayerID)) PlayerID = "";

                playerName = Function.EncryptByte(playerName, "Player_" + UUID, PlayerID, true);
                if (playerName != null)
                {
                    PlayerName = Encoding.UTF8.GetString(playerName);
                }
                registryKey.Close();
            }
        }
    }
}
