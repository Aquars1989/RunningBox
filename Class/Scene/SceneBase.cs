
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    public class SceneBase : UserControl
    {
        public float WorldSpeed { get; set; }
        public Point TrackPoint { get; set; }

        public ObjectBase PlayerObject { get; set; }
        public ObservableCollection<ObjectBase> GameObjects { get; private set; }
        public Rectangle GameRectangle { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
        
        public SceneBase()
        {
            WorldSpeed = 1;
            GameObjects = new ObservableCollection<ObjectBase>();
            GameObjects.CollectionChanged += (x, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    (e.NewItems[0] as ObjectBase).Scene=this;
                }
            };
        }
    }
}
