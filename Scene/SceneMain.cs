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
        private ObjectUINameSetting _UIPlayerNameSetting;
        private ObjectUISceneChoice _UISceneChoice;
        private ObjectUIHighScore _UIHightScore;
        protected override void OnReLayout()
        {
            base.OnReLayout();

            _UITopbar.Layout.Width = Width;
            _UISceneChoice.Layout.Width = Width;
            _UISceneChoice.Layout.Height = Height - _UITopbar.Layout.Height;
        }

        protected override void OnLoadComplete()
        {
            _UITopbar = new ObjectUI(0, 0, 0, 90, new DrawUIFrame(Color.Wheat, Color.DarkSlateBlue, 1, 0));

            _UIInfo = new ObjectUIPlayerInfo(DirectionType.TopLeft, 5, 5, MoveNull.Value);
            _UIInfo.Click += (x, e) => { PlayerInfoShow(); };

            _UISceneChoice = new ObjectUISceneChoice(0, 90, Width, Height - _UITopbar.Layout.Height);
            _UISceneChoice.SceneChoice += (x, i, l) =>
            {
                GlobalScenes.ChoiceScene = i;
                GlobalScenes.ChoiceLevel = l;
                OnGoScene(new SceneSkill());
            };
            _UISceneChoice.SceneHightScoreClick += (x, i, l) =>
            {
                HightScoreShow(i);
            };
            _UISceneChoice.Mode = GlobalScenes.ChoiceScene == null ? 0 : 1;

            UIObjects.Add(_UITopbar);
            UIObjects.Add(_UISceneChoice);
            UIObjects.Add(_UIInfo);

            if (string.IsNullOrWhiteSpace(GlobalPlayer.PlayerName))
            {
                PlayerInfoShow();
            }
            base.OnLoadComplete();
        }

        public SceneMain()
        {
            InitializeComponent();
        }

        public void PlayerInfoShow()
        {
            if (_UIPlayerNameSetting != null) return;

            LockScene(Color.FromArgb(100, 0, 0, 0), 3);
            _UIPlayerNameSetting = new ObjectUINameSetting(DirectionType.Center, 5, 5, new MoveStraight(null, 1, 800, 1, 100, 1F));
            _UIPlayerNameSetting.Close += (x, e) =>
            {
                PlayerInfoHide();
            };
            _UIPlayerNameSetting.Layout.X = Width / 2;
            _UIPlayerNameSetting.Layout.Y = Height / 2 - 100;
            _UIPlayerNameSetting.MoveObject.Target.SetObject(new PointObject(_UIPlayerNameSetting.Layout.X, Height / 2));
            UIObjects.Add(_UIPlayerNameSetting);
        }

        public void PlayerInfoHide()
        {
            if (_UIPlayerNameSetting == null) return;

            _UIPlayerNameSetting.Kill(null, ObjectDeadType.Clear);
            _UIPlayerNameSetting = null;
            UnlockScene(6);
        }

        public void HightScoreShow(ISceneInfo sceneInfo)
        {
            if (_UIHightScore != null) return;

            LockScene(Color.FromArgb(100, 0, 0, 0), 3);
            _UIHightScore = new ObjectUIHighScore(DirectionType.Center, 5, 5, MoveNull.Value, sceneInfo);
            _UIHightScore.Close += (x, e) =>
            {
                HightScoreHide();
            };
            _UIHightScore.Layout.X = Width / 2;
            _UIHightScore.Layout.Y = Height / 2;
            UIObjects.Add(_UIHightScore);
        }

        public void HightScoreHide()
        {
            if (_UIHightScore == null) return;

            _UIHightScore.Kill(null, ObjectDeadType.Clear);
            _UIHightScore = null;
            UnlockScene(6);
        }
    }
}
