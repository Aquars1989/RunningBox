using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class WaveCatcherNomal
    {
        public static void Active(SceneGaming scene, int n, float speedFix, float lifeFix)
        {
            int roundIdx = Global.Rand.Next(4);
            for (int i = 0; i < n; i++)
            {
                int size = Global.Rand.Next(7, 12);
                int offsetLimit = size + Global.Rand.Next(5, 10);
                float speed = Global.Rand.Next(320, 380) * speedFix;
                float weight = 0.3F + size * 0.1F;
                int life = scene.Sec(3.5F * lifeFix) + Global.Rand.Next(0, 5);
                Point enterPoint = scene.GetEnterPoint(roundIdx);

                MoveStraight moveObject = new MoveStraight(scene.PlayerObject, weight, speed, offsetLimit, 100, 0.5F);
                ObjectActive newObject = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, life, LeagueType.Ememy1, ShapeType.Ellipse, new DrawBrush(Color.Red, ShapeType.Ellipse), moveObject);
                newObject.Skills.Add(new SkillSprint(0, scene.Sec(1.5F), 15, 0, true) { AutoCastObject = new AutoCastNormal(0.4F) });
                newObject.Skills.Add(new SkillSprint(0, scene.Sec(0.5F), 5, 0, false) { AutoCastObject = new AutoCastNormal(3F) });
                newObject.Propertys.Add(new PropertyDeadBroken(15, 2, 2, ObjectDeadType.Collision, 20, 200, 600, scene.Sec(0.2F), scene.Sec(0.5F)));
                newObject.Propertys.Add(new PropertyDeadCollapse(1, scene.Sec(0.6F), 2, 2, ObjectDeadType.LifeEnd, 50, 100, scene.Sec(0.15F), scene.Sec(0.25F)));
                newObject.Propertys.Add(new PropertyCollision(1));
                newObject.Propertys.Add(new PropertyShadow(2, 3));
                scene.GameObjects.Add(newObject);
                roundIdx = ++roundIdx % 4;
            }
        }
    }

    public interface IWave
    {
        static void Active(SceneGaming scene, int n, float speedFix, float lifeFix); 
    }
}
