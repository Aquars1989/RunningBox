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

            Controls.Add(new SceneWelcome() { Dock = DockStyle.Fill });
            //Controls.Add(new SceneStand() { Dock = DockStyle.Fill });
        }
    }
}
