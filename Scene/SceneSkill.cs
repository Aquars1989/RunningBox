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

        private ObjectUI[] _UISkillIcons;
        private ObjectUI[] _UISkillInfos;

        private ObjectUI _UICommandOK = new ObjectUI(0, 0, 150, 50, new DrawUITextFrame(Color.Black, Color.WhiteSmoke, Color.Empty, Color.Black, 2, 10, "繼續", Global.CommandFont, Global.CommandFormat));
        private ObjectUI _UICommandCancel = new ObjectUI(0, 0, 150, 50, new DrawUITextFrame(Color.Black, Color.WhiteSmoke, Color.Empty, Color.Black, 2, 10, "返回", Global.CommandFont, Global.CommandFormat));

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
            _UISkillIcons = new ObjectUI[len];
            _UISkillInfos = new ObjectUI[len];
            for (int i = 0; i < len; i++)
            {
                int left = i % 2 * 270 + 30;
                int top = i / 2 * 100 + 120;
                DrawBase skillDraw = _Skills[i].GetDrawObject(Color.FromArgb(120, 60, 0));
                _UISkillIcons[i] = new ObjectUI(left, top, 75, 75, new DrawUISkillFrame(Color.White, Color.FromArgb(210, 180, 50), 2, 10, SkillKeyType.None, skillDraw) { StaticMode = true });
                _UISkillIcons[i].Click += IconClick;

                DrawBase infoDraw = _Skills[i].GetInfoObject(Color.FromArgb(180, 80, 0), Color.FromArgb(255, 255, 240), Color.FromArgb(210, 180, 50));
                _UISkillInfos[i] = new ObjectUI(left + 85, top, 170, 75, infoDraw);

                UIObjects.Add(_UISkillIcons[i]);
                UIObjects.Add(_UISkillInfos[i]);
            }
            UIObjects.Add(_UICommandOK);
            UIObjects.Add(_UICommandCancel);

            _UICommandOK.Click += (x, e) =>
            {
                OnGoScene(new SceneStand()
                {
                    Skill1 = Skill1 == null ? null : (Skill1.DrawObjectInside as DrawSkillBase).BindingSkill,
                    Skill2 = Skill2 == null ? null : (Skill2.DrawObjectInside as DrawSkillBase).BindingSkill
                });
            };

            _UICommandCancel.Click += (x, e) =>
            {
                OnGoScene(new SceneWelcome());
            };
        }

        protected override void OnReLayout()
        {
            base.OnReLayout();
            _UICommandOK.Layout.Y = Height - _UICommandOK.Layout.Height - 70;
            _UICommandCancel.Layout.Y = Height - _UICommandCancel.Layout.Height - 70;
            _UICommandOK.Layout.X = Width - _UICommandCancel.Layout.Width - 80;
            _UICommandCancel.Layout.X = 80;
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