using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            //for (int i = 0; i < 10; i++)
            //    MessageBox.Show(Function.GetRandName());
            //DataTable dtck = null;
            //if (Global.SQL.Run(out dtck, "SELECT * FROM SceneInfo"))
            //{
            //    MessageBox.Show(dtck.Rows.Count.ToString());
            //}
            //Global.SQL.CloseConn();
            //return;
            Controls.Add(new SceneMain() { Dock = DockStyle.Fill });
            //Controls.Add(new SceneStand() { Dock = DockStyle.Fill, Skill1 = new SkillBait(5000, 1500, 1500, 200) });
            return;
            Controls.Add(new SceneSkill() { Dock = DockStyle.Fill });
            return;
            Controls.Add(new SceneWelcome() { Dock = DockStyle.Fill });
        }
    }
}
