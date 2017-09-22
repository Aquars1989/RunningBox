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
        /// <summary>
        /// 發生於特性集合變更
        /// </summary>
        public event ValueChangedEnentHandle<ObjectCollection> BackObjectsChanged;

        /// <summary>
        /// 發生於特性集合變更
        /// </summary>
        protected virtual void OnBackObjectsChanged(ObjectCollection oldValue, ObjectCollection newValue)
        {
            if (oldValue != null)
            {
                oldValue.BindingUnlock();
                oldValue.Binding(Scene);
            }

            if (newValue != null)
            {
                newValue.Binding(this, true);
            }

            if (BackObjectsChanged != null)
            {
                BackObjectsChanged(this, oldValue, newValue);
            }
        }

        private ObjectCollection _BackObjects;
        /// <summary>
        /// 背景層
        /// </summary>
        public ObjectCollection BackObjects
        {
            get { return _BackObjects; }
            private set
            {
                if (_BackObjects == value) return;
                ObjectCollection oldValue = _BackObjects;
                _BackObjects = value;
                OnBackObjectsChanged(oldValue, value);
            }
        }

        private ObjectUI _UIBack;
        private ObjectUI _UIGroup1;
        private ObjectUI _UIGroup2;

        public ObjectUISceneChoice(int x, int y, int width, int height)
            : base(x, y, width, height, new DrawBrush(Color.White, ShapeType.Rectangle))
        {
            BackObjects = new ObjectCollection();
            _UIBack = new ObjectUI(20, 20, 80, 40, new DrawUITextFrame(Color.Black, Color.Gray, Color.LightYellow, Color.WhiteSmoke, 1, 8, "返回", new Font("微軟正黑體", 18), GlobalFormat.MiddleCenter));
            _UIGroup1 = new ObjectUI(DirectionType.Center, 0, 0, 200, 200, DrawNull.Value, new MoveStraight(this, 1, 3000, 1, 100, 1F));
            _UIGroup2 = new ObjectUI(DirectionType.Center, 0, 0, 200, 200, DrawNull.Value, new MoveStraight(this, 1, 3000, 1, 100, 1F));
            _UIGroup1.DrawObject = new DrawUITextFrame(Color.Black, Color.White, Color.FromArgb(255, 255, 220), Color.DarkSlateBlue, 2, 12, "生存100秒", new Font("標楷體", 18), GlobalFormat.MiddleLeft);
            _UIGroup2.DrawObject = new DrawUITextFrame(Color.Black, Color.White, Color.WhiteSmoke, Color.DarkSlateBlue, 2, 12, "", new Font("標楷體", 18), GlobalFormat.MiddleLeft);

            _UIBack.Propertys.Add(new PropertyShadow(5, 4) { RFix = 1, GFix = 1 });
            _UIGroup1.Propertys.Add(new PropertyShadow(10, 8));
            _UIGroup2.Propertys.Add(new PropertyShadow(-10, 8));

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

            _UIGroup1.MoveObject.Target.SetOffsetByXY(-130, 0);
            _UIGroup2.MoveObject.Target.SetOffsetByXY(130, 0);
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

                        for (int i = 0; i < BackObjects.Count; i++)
                        {
                            BackObjects[i].MoveObject.Resistance = 1;
                        }
                        _BackBuildCounter.Limit = 50;
                        break;
                    case 1:
                        for (int i = 0; i < BackObjects.Count; i++)
                        {
                            BackObjects[i].MoveObject.Resistance = 10;
                        }

                        _UIGroup1.MoveObject.Target.SetOffsetByXY(-1000 + 130, 0);
                        _UIGroup2.MoveObject.Target.SetOffsetByXY(1000 - 130, 0);
                        _UIBack.DrawObject.Colors.Opacity = 0;
                        _UIBack.Visible = true;
                        _BackBuildCounter.Limit = 500;
                        break;
                }
            }
        }

        public override void Action()
        {
            BackObjects.AllAction();
            BackObjects.ClearAllDead();
            base.Action();
        }

        private CounterObject _BackBuildCounter = new CounterObject(100);
        private float _BackDrak = 0;
        protected override void OnAfterAction()
        {
            switch (_Mode)
            {
                case 1:
                    if (_UIBack.DrawObject.Colors.Opacity < 1)
                    {
                        _UIBack.DrawObject.Colors.Opacity += 0.05F;
                    }

                    if (_BackDrak < 1)
                    {
                        _BackDrak += 0.05F;

                        if (_BackDrak > 1) _BackDrak = 1;
                        DrawObject.Colors.RFix = DrawObject.Colors.GFix = DrawObject.Colors.BFix = -_BackDrak * 0.6F;
                        for (int i = 0; i < BackObjects.Count; i++)
                        {
                            BackObjects[i].DrawObject.Colors.RFix = BackObjects[i].DrawObject.Colors.GFix = _BackDrak * 1F;
                        }
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

                    if (_BackDrak > 0)
                    {
                        _BackDrak -= 0.05F;

                        if (_BackDrak < 0) _BackDrak = 0;
                        DrawObject.Colors.RFix = DrawObject.Colors.GFix = DrawObject.Colors.BFix = -_BackDrak * 0.6F;
                        for (int i = 0; i < BackObjects.Count; i++)
                        {
                            BackObjects[i].DrawObject.Colors.RFix = BackObjects[i].DrawObject.Colors.GFix = _BackDrak * 1F;
                        }
                    }
                    break;
            }

            if (_BackBuildCounter.IsFull)
            {
                int Size = Global.Rand.Next(2, 5);
                int top = Layout.Rectangle.Top + Global.Rand.Next(5, Layout.Rectangle.Height - 10);
                MoveStraight moveObject = new MoveStraight(null, 1, Size * Global.Rand.Next(80, 120), 1, 100, 0);
                ObjectSmoke newObject = new ObjectSmoke(-10, top, Size, Size, -1, 1, 1, Color.Gray, moveObject);
                newObject.DrawObject.Colors.RFix = newObject.DrawObject.Colors.GFix = _BackDrak * 1F;
                moveObject.Target.SetObject(newObject);
                moveObject.Target.SetOffsetByXY(1000, 0);
                moveObject.Resistance = Mode == 1 ? 10 : 1;
                BackObjects.Add(newObject);
                _BackBuildCounter.Value = 0;
            }
            else
            {
                _BackBuildCounter.Value += Scene.IntervalOfRound;
            }
            base.OnAfterAction();
        }

        public override void Draw(Graphics g)
        {
            if (Visible)
            {
                DrawObject.Draw(g, Layout.Rectangle);
                BackObjects.AllDrawSelf(g);
                UIObjects.AllDrawSelf(g);
            }
        }
    }
}
