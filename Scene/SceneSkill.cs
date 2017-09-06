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
        private DrawUISkillFrame Skill1;
        private DrawUISkillFrame Skill2;
        private SkillBase[] _Skills;

        private ObjectUI[] _SkillIcons;
        private ObjectUI[] _SkillInfos;
        ObjectUI _CommandOK;
        ObjectUI _CommandCancel;
        public SceneSkill()
        {
            InitializeComponent();
            _Skills = new SkillBase[]
            {
                new SkillSprint(3500, Sec(1), 0, 6000, true),
                new SkillShield(1, 6000, 0, Sec(1F), Sec(2.5F)),
                new SkillShockwave(5000, 0, Sec(1F), Sec(2F), Sec(0.1F), 5000, 300),
                new SkillBulletTime(1000, 8000, -1, Sec(5), 1)
            };

            int len = _Skills.Length;
            _SkillIcons = new ObjectUI[len];
            _SkillInfos = new ObjectUI[len];
            for (int i = 0; i < len; i++)
            {
                int left = i % 2 * 260 + 50;
                int top = i / 2 * 120 + 50;
                DrawBase skillDraw = _Skills[i].GetDrawObject(Color.FromArgb(120, 60, 0));
                _SkillIcons[i] = new ObjectUI(left, top, 75, 75, new DrawUISkillFrame(Color.FromArgb(210, 180, 50), SkillKeyType.None, skillDraw) { StaticMode = true });
                _SkillIcons[i].Click += IconClick;

                DrawBase infoDraw = _Skills[i].GetInfoObject(Color.FromArgb(180, 80, 0), Color.FromArgb(255, 255, 240), Color.FromArgb(210, 180, 50));
                _SkillInfos[i] = new ObjectUI(left + 85, top, 150, 75, infoDraw);

                // UIObjects.Add(_SkillIcons[i]);
                UIObjects.Add(_SkillInfos[i]);
            }

            Font commandFont = new System.Drawing.Font("微軟正黑體", 22);
            StringFormat commandFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            _CommandOK = new ObjectUI(0, 0, 150, 50, new DrawUIString(Color.Black, Color.Empty, Color.Black, "繼續", commandFont, commandFormat));
            _CommandCancel = new ObjectUI(0, 0, 150, 50, new DrawUIString(Color.Black, Color.Empty, Color.Black, "返回", commandFont, commandFormat));
            UIObjects.Add(_CommandOK);
            UIObjects.Add(_CommandCancel);

            _CommandOK.Click += (x, e) =>
            {
                OnGoScene(new SceneStand()
                {
                    Skill1 = Skill1 == null ? null : (Skill1.IconDrawObject as DrawSkillBase).BindingSkill,
                    Skill2 = Skill2 == null ? null : (Skill2.IconDrawObject as DrawSkillBase).BindingSkill
                });
            };
        }

        protected override void OnReLayout()
        {
            base.OnReLayout();
            _CommandOK.Layout.Y = Height - _CommandOK.Layout.Height - 80;
            _CommandCancel.Layout.Y = Height - _CommandCancel.Layout.Height - 80;
            _CommandOK.Layout.X = Width - _CommandCancel.Layout.Width - 80;
            _CommandCancel.Layout.X = 80;
        }

        public void IconClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                DrawUISkillFrame selectSkill = (sender as ObjectUI).DrawObject as DrawUISkillFrame;
                if (Skill1 != null)
                {
                    Skill1.DrawButton = SkillKeyType.None;
                }

                if (Skill2 == selectSkill)
                {
                    Skill2.DrawButton = SkillKeyType.None;
                    Skill2 = null;
                }
                selectSkill.DrawButton = SkillKeyType.MouseButtonLeft;
                Skill1 = selectSkill;
            }
            else
            {
                DrawUISkillFrame selectSkill = (sender as ObjectUI).DrawObject as DrawUISkillFrame;
                if (Skill2 != null)
                {
                    Skill2.DrawButton = SkillKeyType.None;
                }

                if (Skill1 == selectSkill)
                {
                    Skill1.DrawButton = SkillKeyType.None;
                    Skill1 = null;
                }
                selectSkill.DrawButton = SkillKeyType.MouseButtonRight;
                Skill2 = selectSkill;
            }
        }
    }
}
