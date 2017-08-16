using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    class Global
    {
        public static bool DebugMode = false;
        public static Random Rand = new Random();
        public static int DefaultIntervalOfRound = 20;
    }

    /// <summary>
    /// 技能按鈕列舉
    /// </summary>
    public enum EnumSkillButton
    {
        None = 0,
        MouseButtonLeft = 1,
        MouseButtonRight = 2
    }

    /// <summary>
    /// 方向列舉
    /// </summary>
    public enum EnumDirection
    {
        Left = 0,
        Right = 1,
        Top = 2,
        Bottom = 3
    }
}
