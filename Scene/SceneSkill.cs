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
        private static Font _TitleFont = new Font("標楷體", 30);
        private static Rectangle _TitleRect = new Rectangle(20, 40, 300, 50);
        private static LinearGradientBrush _TitleBrush = new LinearGradientBrush(_TitleRect, Color.FromArgb(210, 130, 50), Color.FromArgb(120, 60, 0), 135);

        private DrawUISkillFrame Skill1;
        private DrawUISkillFrame Skill2;
        private SkillBase[] _Skills;

        private ObjectUI[] _UISkillIcons;
        private ObjectUI[] _UISkillInfos;

        private ObjectUI _UICommandOK;
        private ObjectUI _UICommandCancel;

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
                _UISkillIcons[i].GetFocus += (s, e) =>
                {
                    (s as ObjectUI).DrawObject.Colors.SetColor("Border", Color.Chocolate);
                    (s as ObjectUI).DrawObject.Scale = 1.1F;
                };
                _UISkillIcons[i].LostFocus += (s, e) =>
                {
                    (s as ObjectUI).DrawObject.Colors.SetColor("Border", Color.FromArgb(210, 180, 50));
                    (s as ObjectUI).DrawObject.Scale = 1F;
                };
                _UISkillIcons[i].Click += IconClick;

                DrawBase infoDraw = _Skills[i].GetInfoObject(Color.Chocolate, Color.Cornsilk, Color.FromArgb(180, 210, 180, 50));
                _UISkillInfos[i] = new ObjectUI(left + 85, top + 6, 170, 75, infoDraw);

                _UISkillIcons[i].Propertys.Add(new PropertyShadow(4, 6, 1, 1, 0.2F) { RFix = -0.3F, GFix = -0.3F, BFix = -0.3F });
                UIObjects.Add(_UISkillIcons[i]);
                UIObjects.Add(_UISkillInfos[i]);
            }

            DrawUITextFrame drawCommandOK = new DrawUITextFrame(Color.Black, Color.WhiteSmoke, Color.FromArgb(150, 255, 255, 255), Color.Black, 2, 10, "繼續", Global.CommandFont, GlobalFormat.MiddleCenter);
            DrawUITextFrame drawCommandCancel = new DrawUITextFrame(Color.Black, Color.WhiteSmoke, Color.FromArgb(150, 255, 255, 255), Color.Black, 2, 10, "返回", Global.CommandFont, GlobalFormat.MiddleCenter);
            DrawUITextFrame drawCommandOKHover = new DrawUITextFrame(Color.Black, Color.WhiteSmoke, Color.FromArgb(200, 255, 255, 220), Color.Black, 2, 10, "繼續", Global.CommandFont, GlobalFormat.MiddleCenter);
            DrawUITextFrame drawCommandCancelHover = new DrawUITextFrame(Color.Black, Color.WhiteSmoke, Color.FromArgb(200, 255, 255, 220), Color.Black, 2, 10, "返回", Global.CommandFont, GlobalFormat.MiddleCenter);
            _UICommandOK = new ObjectUI(0, 0, 150, 50, drawCommandOK);
            _UICommandCancel = new ObjectUI(0, 0, 150, 50, drawCommandCancel);
            _UICommandOK.DrawObjectHover = drawCommandOKHover;
            _UICommandCancel.DrawObjectHover = drawCommandCancelHover;
            _UICommandOK.Propertys.Add(new PropertyShadow(-4, 4) { RFix = 0, GFix = 0, BFix = 0 });
            _UICommandCancel.Propertys.Add(new PropertyShadow(4, 4) { RFix = 0, GFix = 0, BFix = 0 });

            UIObjects.Add(_UICommandOK);
            UIObjects.Add(_UICommandCancel);

            _UICommandOK.Click += (x, e) =>
            {
                SceneGaming scene = GlobalScenes.ChoiceScene.CreateScene(GlobalScenes.ChoiceLevel);
                scene.Skill1 = Skill1 == null ? null : (Skill1.DrawObjectInside as DrawSkillBase).BindingSkill;
                scene.Skill2 = Skill2 == null ? null : (Skill2.DrawObjectInside as DrawSkillBase).BindingSkill;
                OnGoScene(scene);
            };

            _UICommandCancel.Click += (x, e) =>
            {
                OnGoScene(new SceneMain() { });
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


        private int Animaiton;
        protected override void OnDrawFloor(Graphics g)
        {
            Animaiton++;
            int aniMax = 60;
            int aniHalf = aniMax / 2;
            if (Animaiton >= aniMax)
            {
                Animaiton %= aniMax;
            }

            float ratio = (Math.Abs(aniHalf - Animaiton) / (float)aniHalf);
            int drawHeight = (int)(Height * 0.15F);
            Rectangle drawRect1 = new Rectangle(0, 0, Width, drawHeight);
            Rectangle drawRect2 = new Rectangle(0, Height - drawHeight, Width, drawHeight);
            using (LinearGradientBrush brush1 = new LinearGradientBrush(drawRect1, Color.FromArgb(180, 200 + (int)(ratio * 55), 255, 190), Color.Empty, 90F))
            using (LinearGradientBrush brush2 = new LinearGradientBrush(drawRect2, Color.FromArgb(180, 200 + (int)(ratio * 55), 255, 190), Color.Empty, 270F))
            {
                g.FillRectangle(brush1, drawRect1);
                g.FillRectangle(brush2, drawRect2);
            }

            base.OnDrawFloor(g);
        }

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