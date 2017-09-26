using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 場景資訊畫面
    /// </summary>
    public class ObjectUISceneInfo : ObjectUIPanel
    {
        /// <summary>
        /// 發生於場景被選取時
        /// </summary>
        public event SceneInfoEnentHandle SceneChoice;

        /// <summary>
        /// 發生於場景被選取時
        /// </summary>
        /// <param name="sceneInfo">場景資訊</param>
        /// <param name="level">關卡等級</param>
        protected virtual void OnSceneChoice(ISceneInfo sceneInfo, int level)
        {
            if (SceneChoice != null)
            {
                SceneChoice(this, sceneInfo, level);
            }
        }

        private Font _ItemFont = new Font("微軟正黑體", 12);

        private ISceneInfo _BindingScene;
        public ISceneInfo BindingScene
        {
            get { return _BindingScene; }
            set
            {
                _BindingScene = value;
                UIObjects.Clear();

                if (_BindingScene == null) return;

                int itemWidth = (Layout.Width - 30) / 6;
                int itemHeight = 25;
                int left = 15;
                int top = Layout.Height - itemHeight - 5;
                for (int i = 0; i < _BindingScene.MaxLevel; i++)
                {
                    int idx = i + 1;
                    DrawUITextFrame drawObject = new DrawUITextFrame(Color.Black, Color.Empty, Color.Empty, Color.Black, 1, 0, idx.ToString(), _ItemFont, GlobalFormat.MiddleCenter);
                    ObjectUI newObject = new ObjectUI(left, top, itemWidth, itemHeight, drawObject);
                    newObject.Layout.Depend.SetObject(this);
                    newObject.Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
                    UIObjects.Add(newObject);
                    left += itemWidth;

                    newObject.Click += (x, e) =>
                    {
                        OnSceneChoice(BindingScene, idx);
                    };
                }
            }
        }


        public ObjectUISceneInfo(DirectionType anchor, int x, int y, MoveBase moveObject)
            : base(anchor, x, y, 250, 100, new DrawUIFrame(Color.LightSalmon, Color.CornflowerBlue, 2, 20), moveObject)
        {
        }
    }
}
