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
            Controls.Add(new SceneStand() { Dock = DockStyle.Fill });
            return;
            SceneWelcome senceWelcome = new SceneWelcome() { Dock = DockStyle.Fill };
            senceWelcome.GoSence += (x, e) =>
            {
                Controls.Remove(senceWelcome);
                SceneStand SceneStand = new SceneStand() { Dock = DockStyle.Fill };
                Controls.Add(SceneStand);
                senceWelcome.Dispose();
            };
            Controls.Add(senceWelcome);
        }
    }
}
