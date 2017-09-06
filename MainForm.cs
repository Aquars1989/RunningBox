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
            //Controls.Add(new SceneStand() { Dock = DockStyle.Fill });
            SceneSkill s = new SceneSkill() { Dock = DockStyle.Fill };
            s.GoScene += SceneToScene;
            Controls.Add(s);
            return;
            SceneWelcome senceWelcome = new SceneWelcome() { Dock = DockStyle.Fill };
            senceWelcome.GoScene += SceneToScene;
            Controls.Add(senceWelcome);
        }

        private void SceneToScene(object sender, SceneBase scene)
        {
            SceneBase senderSence = sender as SceneBase;
            Controls.Remove(senderSence);
            scene.Dock = DockStyle.Fill;
            scene.GoScene += SceneToScene;
            Controls.Add(scene);
            senderSence.Dispose();
        }
    }
}
