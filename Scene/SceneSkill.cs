using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace RunningBox
{
    public partial class SceneSkill : SceneBase
    {
        ObjectUI[] SkillIcons;

        public SceneSkill()
        {
            InitializeComponent();

            SkillIcons = new ObjectUI[8];
            for (int i = 0; i < 8; i++)
            {
                SkillIcons[i] = new ObjectUI(50, 50 + i * 80, 75, 75, new DrawUISkillFrame(Color.Black, SkillKeyType.None));
                SkillIcons[i].Click += IconClick;
                UIObjects.Add(SkillIcons[i]);
            }
        }

        public void IconClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ((sender as ObjectUI).DrawObject as DrawUISkillFrame).DrawButton = SkillKeyType.MouseButtonLeft;
            }
            else
            {
                ((sender as ObjectUI).DrawObject as DrawUISkillFrame).DrawButton = SkillKeyType.MouseButtonRight;
            }
        }
    }
}
