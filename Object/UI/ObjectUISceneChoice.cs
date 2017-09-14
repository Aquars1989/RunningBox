using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 基礎容器介面
    /// </summary>
    public class ObjectUISceneChoice : ObjectUIPanel
    {
        private ObjectUI _UIBack;
        private ObjectUI _UIGroup1;
        private ObjectUI _UIGroup2;

        public ObjectUISceneChoice(int x, int y, int width, int height)
            : base(x, y, width, height, DrawNull.Value)
        {
            _UIBack = new ObjectUI(20, 20, 80, 40, new DrawUITextFrame(Color.Black, Color.Gray, Color.AliceBlue, Color.DarkSlateBlue, 1, 8, "返回", new Font("微軟正黑體", 18), GlobalFormat.MiddleCenter));
            _UIGroup1 = new ObjectUI(DirectionType.Center, 0, 0, 200, 200, DrawNull.Value, new MoveStraight(this, 1, 3000, 1, 100, 1F));
            _UIGroup2 = new ObjectUI(DirectionType.Center, 0, 0, 200, 200, DrawNull.Value, new MoveStraight(this, 1, 3000, 1, 100, 1F));
            _UIGroup1.DrawObject = new DrawUITextFrame(Color.Black, Color.White, Color.FromArgb(255, 255, 220), Color.DarkSlateBlue, 2, 12, "生存100秒", new Font("標楷體", 18), GlobalFormat.MiddleLeft);
            _UIGroup2.DrawObject = new DrawUITextFrame(Color.Black, Color.White, Color.WhiteSmoke, Color.DarkSlateBlue, 2, 12, "", new Font("標楷體", 18), GlobalFormat.MiddleLeft);

            _UIBack.Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
            _UIGroup1.Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
            _UIGroup2.Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
            _UIBack.Layout.Depend.SetObject(this);
            _UIGroup1.Layout.Depend.SetObject(this);
            _UIGroup2.Layout.Depend.SetObject(this);
            _UIGroup1.Layout.X = -_UIGroup1.Layout.Width;
            _UIGroup2.Layout.X = width + _UIGroup1.Layout.Width;
            _UIGroup1.Layout.Y = height / 2;
            _UIGroup2.Layout.Y = height / 2;
     
            _UIGroup1.MoveObject.Target.SetOffsetByXY(-130,0);
            _UIGroup2.MoveObject.Target.SetOffsetByXY(130,0);
            _UIBack.Click += (s, e) =>
                {
                    Mode = 0;
                };

            _UIGroup1.Click += (s, e) =>
                {
                    Mode = 1;
                };

            UIObjects.Add(_UIBack);
            UIObjects.Add(_UIGroup1);
            UIObjects.Add(_UIGroup2);
        }


        private int _Mode = 0;
        public int Mode
        {
            get { return _Mode; }
            set
            {
                _Mode = value;
                switch (_Mode)
                {
                    case 0:
                        _UIGroup1.MoveObject.Target.SetOffsetByXY(-130, 0);
                        _UIGroup2.MoveObject.Target.SetOffsetByXY(130, 0);
                        break;
                    case 1:
                        _UIGroup1.MoveObject.Target.SetOffsetByXY(-1000 + 130, 0);
                        _UIGroup2.MoveObject.Target.SetOffsetByXY(1000 - 130, 0);
                        _UIBack.Visible = true;
                        _UIBack.DrawObject.Colors.Opacity = 0;
                        break;
                }
            }
        }

        protected override void OnAfterAction()
        {
            switch (_Mode)
            {
                case 1:
                    if (_UIBack.DrawObject.Colors.Opacity < 1)
                    {
                        _UIBack.DrawObject.Colors.Opacity += 0.05F;
                    }
                    break;
                case 0:
                    if (_UIBack.DrawObject.Colors.Opacity > 0)
                    {
                        _UIBack.DrawObject.Colors.Opacity -= 0.1F;
                        if (_UIBack.DrawObject.Colors.Opacity <= 0)
                        {
                            _UIBack.Visible = false;
                        }
                    }
                    break;
            }
            base.OnAfterAction();
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
        }
    }
}
