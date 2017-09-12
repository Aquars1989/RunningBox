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
        ObjectUI _Info = new ObjectUI(5, 5, 200, 80, new DrawCustom());

        ObjectUI _DarkCover = new ObjectUI(0, 0, 150, 15, new DrawBrush(Color.FromArgb(100, 0, 0, 0), ShapeType.Rectangle)) { Visible = false };
        ObjectUI _Info2 = new ObjectUIPlayer(ContentAlignment.MiddleCenter, 5, 5, 400, 400,  new MoveStraight(TargetNull.Value, 1, 800, 1, 100, 1F)) { Visible = false };

        protected override void OnReLayout()
        {
            base.OnReLayout();

            _DarkCover.Layout.Width = Width;
            _DarkCover.Layout.Height = Height;
            _Info2.Layout.X = Width / 2;
            (_Info2.MoveObject as MoveStraight).Target = new TargetPoint((int)(_Info2.Layout.X), (int)(Height / 2));
        }

        public SceneMain()
        {
            InitializeComponent();

            _Info.DrawObject.BeforeDraw += (x, g, r) =>
            {
                g.DrawRectangle(Pens.CornflowerBlue, r);

            };

            _Info2.DrawObject.BeforeDraw += (x, g, r) =>
            {
                g.FillRectangle(Brushes.White, r);
                g.DrawRectangle(Pens.CornflowerBlue, r);

            };

            _Info.Click += (x, e) =>
            {
                _Info2.Layout.Y = Height / 2 - 100;
                _Info2.DrawObject.Colors.Opacity = 0;
                _DarkCover.DrawObject.Colors.Opacity = 0;
                _DarkCover.Visible = true;
                _Info2.Visible = true;
            };

            _DarkCover.AfterAction += (x, e) =>
            {
                _DarkCover.DrawObject.Colors.Opacity += 0.05F;
            };
            UIObjects.Add(_Info);
            UIObjects.Add(_DarkCover);
            UIObjects.Add(_Info2);
        }
    }
}
