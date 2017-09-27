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
        public static ISceneInfo ChoiceScene;
        public static int ChoiceLevel;

        public static SceneInfoManager Scenes = new SceneInfoManager();

        static GlobalScenes()
        {
            Scenes.AddScene(new SceneInfo<SceneStand>("標準","Stand", 6));
            Scenes.AddScene(new SceneInfo<SceneStand>("標準", "Stand2", 6));
            Scenes.AddScene(new SceneInfo<SceneStand>("標準", "Stand3", 6));
            Scenes.AddScene(new SceneInfo<SceneStand>("標準", "Stand4", 6));
        }
    }
}
