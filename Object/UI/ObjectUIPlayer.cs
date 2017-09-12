using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 基礎容器介面
    /// </summary>
    public class ObjectUIPlayer : ObjectUIPanel
    {
        private DrawUITextFrame _DrawPlayerName;

        public ObjectUIPlayer(ContentAlignment anchor, int x, int y, int width, int height, MoveBase moveObject)
            : base(anchor, x, y, width, height, new DrawUIFrame(Color.White, Color.Black, 2, 10), moveObject)
        {
            StringFormat sf = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            _DrawPlayerName = new DrawUITextFrame(Color.RoyalBlue, Color.LightSteelBlue, Color.AliceBlue, Color.LightSteelBlue, 2, 10, "", new Font("標楷體", 16), sf) { TextPadding = new Padding(10, 5, 0, 5) };
            ObjectUI UICommandBuildName = new ObjectUI(width - 80, 40, 40, 40, new DrawUIFrame(Color.White, Color.Black, 2, 10));
            ObjectUI UIPlayerName = new ObjectUI(40, 40, width - 140, 40, _DrawPlayerName);

            UICommandBuildName.Layout.DependTarget = new TargetObject(this) { Anchor = ContentAlignment.TopLeft };
            UIPlayerName.Layout.DependTarget = new TargetObject(this) { Anchor = ContentAlignment.TopLeft };

            UICommandBuildName.Click += (s, e) => { _DrawPlayerName.Text = Function.GetRandName(); };
            UIObjects.Add(UICommandBuildName);
            UIObjects.Add(UIPlayerName);
        }
    }
}
