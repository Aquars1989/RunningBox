using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    class Global
    {
        /// <summary>
        /// MySQL連線物件
        /// </summary>
        public static SQLobject SQL = new SQLobject("server=104.199.150.7;uid=client;Password=playforfun;database=RunningBox");

        /// <summary>
        /// 是否開啟線上功能
        /// </summary>
        public static bool Online = false;

        /// <summary>
        /// 機碼路徑
        /// </summary>
        public static string RegistryAddr = "SOFTWARE\\RunningBox\\";
        
        /// <summary>
        /// Debug模式
        /// </summary>
        public static bool DebugMode = false;

        /// <summary>
        /// 公用亂數產生器
        /// </summary>
        public static Random Rand = new Random(Guid.NewGuid().GetHashCode());
        
        /// <summary>
        /// 預設按鈕字體
        /// </summary>
        public static Font CommandFont = new System.Drawing.Font("微軟正黑體", 20, FontStyle.Bold);
        
        /// <summary>
        /// 預設每回合時間(毫秒)
        /// </summary>
        public static int DefaultIntervalOfRound = 20;
        
        /// <summary>
        /// 預設每波時間
        /// </summary>
        public static int DefaultIntervalOfWave = 1000;
        
        /// <summary>
        /// 預設結束延遲時間
        /// </summary>
        public static int DefaultEndDelayLimit = 1000;
        
        /// <summary>
        /// 預設邊界
        /// </summary>
        public static Padding DefaultMainRectanglePadding = new Padding(80, 80, 80, 80);
        
        /// <summary>
        /// 預設能量上限值
        /// </summary>
        public static int DefaultEnergyLimit = 10000;
        
        public static int DefaultEnergyGetPerSec = 1500;
    }
}
