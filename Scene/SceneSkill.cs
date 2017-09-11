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
using System.Drawing.Drawing2D;

namespace RunningBox
{
    public partial class SceneSkill : SceneBase
    {
        private DrawUISkillFrame Skill1;
        private DrawUISkillFrame Skill2;
        private SkillBase[] _Skills;

        private ObjectUI[] _SkillIcons;
        private ObjectUI[] _SkillInfos;
        private ObjectUI _CommandOK = new ObjectUI(0, 0, 150, 50, new DrawUIString(Color.Black, Color.Empty, Color.Black, 2, "繼續", Global.CommandFont, Global.CommandFormat));
        private ObjectUI _CommandCancel = new ObjectUI(0, 0, 150, 50, new DrawUIString(Color.Black, Color.Empty, Color.Black, 2, "返回", Global.CommandFont, Global.CommandFormat));

        public SceneSkill()
        {
            InitializeComponent();
            _Skills = new SkillBase[]
            {
                new SkillSprint(3500, Sec(1), 0, 6000, true),
                new SkillShield(1, 6000, 0, Sec(1F), Sec(2.5F)),
                new SkillShockwave(4000, 0, Sec(1F), Sec(2.5F), Sec(0.1F), 4000, 300),
                new SkillBulletTime(1000, 8000, -1, Sec(5), 1),
                new SkillBait(6000, Sec(1.5F), Sec(1.5F), 200) 
            };

            int len = _Skills.Length;
            _SkillIcons = new ObjectUI[len];
            _SkillInfos = new ObjectUI[len];
            for (int i = 0; i < len; i++)
            {
                int left = i % 2 * 270 + 30;
                int top = i / 2 * 100 + 120;
                DrawBase skillDraw = _Skills[i].GetDrawObject(Color.FromArgb(120, 60, 0));
                _SkillIcons[i] = new ObjectUI(left, top, 75, 75, new DrawUISkillFrame(Color.FromArgb(210, 180, 50), SkillKeyType.None, skillDraw) { StaticMode = true });
                _SkillIcons[i].Click += IconClick;

                DrawBase infoDraw = _Skills[i].GetInfoObject(Color.FromArgb(180, 80, 0), Color.FromArgb(255, 255, 240), Color.FromArgb(210, 180, 50));
                _SkillInfos[i] = new ObjectUI(left + 85, top, 170, 75, infoDraw);

                UIObjects.Add(_SkillIcons[i]);
                UIObjects.Add(_SkillInfos[i]);
            }
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

            _CommandCancel.Click += (x, e) =>
            {
                OnGoScene(new SceneWelcome());
            };
        }

        protected override void OnReLayout()
        {
            base.OnReLayout();
            _CommandOK.Layout.Y = Height - _CommandOK.Layout.Height - 70;
            _CommandCancel.Layout.Y = Height - _CommandCancel.Layout.Height - 70;
            _CommandOK.Layout.X = Width - _CommandCancel.Layout.Width - 80;
            _CommandCancel.Layout.X = 80;
        }

        private static Font _TitleFont = new Font("標楷體", 30);
        private static Rectangle _TitleRect = new Rectangle(20, 40, 300, 50);

        private static LinearGradientBrush _TitleBrush = new LinearGradientBrush(_TitleRect, Color.FromArgb(210, 130, 50), Color.FromArgb(120, 60, 0), 135);
        protected override void OnAfterDrawUI(Graphics g)
        {
            g.DrawString("技能設定", _TitleFont, Brushes.DarkGray, _TitleRect.X + 1, _TitleRect.Y + 1);
            g.DrawString("技能設定", _TitleFont, _TitleBrush, _TitleRect);
            base.OnAfterDrawUI(g);
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