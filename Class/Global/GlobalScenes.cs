using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    class GlobalScenes
    {
        public static SkillBase ChoiceSkill1;
        public static SkillBase ChoiceSkill2;
        public static ISceneInfo ChoiceScene;
        public static int ChoiceLevel;

        public static SceneInfoManager Scenes = new SceneInfoManager();

        static GlobalScenes()
        {
            Scenes.AddScene(new SceneInfo<SceneStand>("標準", "Stand", 6, 60000));
            Scenes.AddScene(new SceneInfo<SceneRhythm>("節奏", "Rhythm", 6, 60000));
        }
    }
}
