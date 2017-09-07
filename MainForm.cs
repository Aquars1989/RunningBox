using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Controls.Add(new SceneStand() { Dock = DockStyle.Fill, Skill1 = new SkillBait(5000, 1500, 1500, 200) });
            return;
            Controls.Add(new SceneSkill() { Dock = DockStyle.Fill });
            return;
            Controls.Add( new SceneWelcome() { Dock = DockStyle.Fill });
        }
    }
}
