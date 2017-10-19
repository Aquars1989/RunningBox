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
    public partial class SceneMain : SceneBase
    {
        private ObjectUI _UITopbar;
        private ObjectUI _UIInfo;
        private ObjectUI _UIDarkCover;
        private ObjectUINameSetting _UIPlayerNameSetting;
        private ObjectUISceneChoice _UISceneChoice;
        protected override void OnReLayout()
        {
            base.OnReLayout();

            _UITopbar.Layout.Width = Width;
            _UISceneChoice.Layout.Width = Width;
            _UISceneChoice.Layout.Height = Height - _UITopbar.Layout.Height;
            _UIDarkCover.Layout.Width = Width;
            _UIDarkCover.Layout.Height = Height;
            _UIPlayerNameSetting.Layout.X = Width / 2;
            _UIPlayerNameSetting.MoveObject.Target.SetObject(new PointObject(_UIPlayerNameSetting.Layout.X, Height / 2));
        }

        protected override void OnLoadComplete()
        {
            _UITopbar = new ObjectUI(0, 0, 0, 90, new DrawUIFrame(Color.Wheat, Color.DarkSlateBlue, 1, 0));

            _UIInfo = new ObjectUIPlayerInfo(DirectionType.TopLeft, 5, 5, MoveNull.Value);
            _UIInfo.Click += (x, e) => { PlayerInfoShow = true; };

            _UIDarkCover = new ObjectUI(0, 0, 150, 15, new DrawBrush(Color.FromArgb(100, 0, 0, 0), ShapeType.Rectangle)) { Visible = false };

            _UIPlayerNameSetting = new ObjectUINameSetting(DirectionType.Center, 5, 5, new MoveStraight(null, 1, 800, 1, 100, 1F)) { Visible = false };
            _UIPlayerNameSetting.Close += (x, e) => { PlayerInfoShow = false; };
            _UISceneChoice = new ObjectUISceneChoice(0, 90, Width, Height - _UITopbar.Layout.Height);
            _UISceneChoice.SceneChoice += (x, i, l) =>
            {
                GlobalScenes.ChoiceScene = i;
                GlobalScenes.ChoiceLevel = l;
                OnGoScene(new SceneSkill());
            };
            _UISceneChoice.Mode = GlobalScenes.ChoiceScene == null ? 0 : 1;

            UIObjects.Add(_UITopbar);
            UIObjects.Add(_UISceneChoice);
            UIObjects.Add(_UIInfo);
            UIObjects.Add(_UIDarkCover);
            UIObjects.Add(_UIPlayerNameSetting);

            if (string.IsNullOrWhiteSpace(GlobalPlayer.PlayerName))
            {
                PlayerInfoShow = true;
            }
            base.OnLoadComplete();
        }

        public SceneMain()
        {
            InitializeComponent();
        }


        protected override void OnAfterRound()
        {
            if (_PlayerInfoShow)
            {
                if (_UIDarkCover.DrawObject.Colors.Opacity < 1)
                {
                    _UIDarkCover.DrawObject.Colors.Opacity += 0.05F;
                }
            }
            else
            {
                if (_UIDarkCover.DrawObject.Colors.Opacity > 0)
                {
                    _UIDarkCover.DrawObject.Colors.Opacity -= 0.1F;
                    if (_UIDarkCover.DrawObject.Colors.Opacity <= 0)
                    {
                        _UIDarkCover.Visible = false;
                    }
                }
            }

            base.OnAfterRound();
        }

        private bool _PlayerInfoShow;
        private bool PlayerInfoShow
        {
            get { return _PlayerInfoShow; }
            set
            {
                _PlayerInfoShow = value;
                if (_PlayerInfoShow)
                {
                    _UIPlayerNameSetting.Layout.Y = Height / 2 - 100;
                    _UIDarkCover.DrawObject.Colors.Opacity = 0;
                    _UIDarkCover.Visible = true;
                    _UIPlayerNameSetting.Visible = true;
                }
                else
                {
                    _UIPlayerNameSetting.Visible = false;
                }
            }
        }
    }
}
