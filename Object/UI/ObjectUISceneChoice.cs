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
        private float _PaddingX = 20;
        private float _PaddingY = 60;
        private float _ItemPaddingX = 20;
        private float _ItemPaddingY = 20;

        /// <summary>
        /// 發生於場景被選取時
        /// </summary>
        public event SceneInfoEnentHandle SceneChoice;

        /// <summary>
        /// 發生於背景集合變更
        /// </summary>
        public event ValueChangedEnentHandle<ObjectCollection> BackObjectsChanged;

        /// <summary>
        /// 發生於場景被選取時
        /// </summary>
        /// <param name="sceneInfo">場景資訊</param>
        /// <param name="level">關卡等級</param>
        protected virtual void OnSceneChoice(ISceneInfo sceneInfo, int level)
        {
            if (SceneChoice != null)
            {
                SceneChoice(this, sceneInfo, level);
            }
        }

        /// <summary>
        /// 發生於背景集合變更
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

        private DrawUITextFrame _DrawGroup1;
        private DrawUITextFrame _DrawGroup2;

        private ObjectUI _CommandBack;
        private ObjectUI _UIGroup1;
        private ObjectUI _UIGroup2;

        private ObjectUISceneInfo[] _UIScenes;
        private PointF[] _UIScenesLocation;

        public ObjectUISceneChoice(int x, int y, int width, int height)
            : base(x, y, width, height, new DrawBrush(Color.White, ShapeType.Rectangle))
        {
            BackObjects = new ObjectCollection();

            var ScenesItems = GlobalScenes.Scenes.GetItems();
            int group1Idx = 0;
            int group1Cot = ScenesItems.Count;
            _UIScenes = new ObjectUISceneInfo[group1Cot];
            _UIScenesLocation = new PointF[group1Cot];

            foreach (ISceneInfo sceneInfo in ScenesItems)
            {
                _UIScenes[group1Idx] = new ObjectUISceneInfo(DirectionType.Left | DirectionType.Top, 0, 0, new MoveStraight(this, 1, 3000, 1, 100, 1F));
                _UIScenes[group1Idx].MoveObject.Target.Anchor = DirectionType.Left | DirectionType.Top;
                _UIScenes[group1Idx].MoveObject.Anchor = DirectionType.Left | DirectionType.Top;
                _UIScenes[group1Idx].Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
                _UIScenes[group1Idx].Layout.Depend.SetObject(this);
                _UIScenes[group1Idx].BindingScene = sceneInfo;
                _UIScenes[group1Idx].SceneChoice += (s, i, l) => { OnSceneChoice(i, l); };
                UIObjects.Add(_UIScenes[group1Idx]);
                group1Idx++;
            }

            if (group1Cot > 0)
            {
                int maxH = (int)((width - _PaddingX * 2 + _ItemPaddingX) / (_UIScenes[0].Layout.RectWidth + _ItemPaddingX)); //橫向最多項目數

                int cotX = group1Cot > maxH ? maxH : group1Cot; //橫向數量
                int cotY = (group1Cot - 1) / maxH + 1;          //縱向數量

                float baseX = (width - (_UIScenes[0].Layout.RectWidth * cotX) - (_ItemPaddingX * (cotX - 1))) / 2;
                float baseY = _PaddingY + (height - (_UIScenes[0].Layout.RectWidth * cotY) - (_ItemPaddingY * (cotY - 1))) / 2;
                float originX = baseX;

                for (int i = 0; i < _UIScenes.Length; i++)
                {
                    _UIScenesLocation[i] = new PointF(baseX, baseY);
                    _UIScenes[i].Layout.X = baseX;
                    _UIScenes[i].Layout.Y = baseY + height;
                    _UIScenes[i].MoveObject.Target.SetOffsetByXY(baseX, baseY + height);

                    if (i > 0 && i % cotX == 0)
                    {
                        baseX = originX;
                        baseY += _UIScenes[0].Layout.RectHeight + _ItemPaddingY;
                    }
                    else
                    {
                        baseX += _UIScenes[i].Layout.RectWidth + _ItemPaddingX;
                    }
                }
            }

            _DrawGroup1 = new DrawUITextFrame(Color.DarkSlateBlue, Color.White, Color.AliceBlue, Color.DarkSlateBlue, 2, 12, "生存100秒", new Font("標楷體", 18, FontStyle.Bold), GlobalFormat.MiddleBottom);
            _DrawGroup1.DrawObjectInside = new DrawSceneTypeA(Color.LightSteelBlue);
            _UIGroup1 = new ObjectUI(DirectionType.Center, -200, height / 2, 200, 200, _DrawGroup1, new MoveStraight(this, 1, 3000, 1, 100, 1F));
            _UIGroup1.Propertys.Add(new PropertyShadow(3, 4));
            _UIGroup1.Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
            _UIGroup1.Layout.Depend.SetObject(this);
            _UIGroup1.GetFocus += (s, e) =>
            {
                _DrawGroup1.Colors.SetColor("Back", Color.FromArgb(255, 255, 220));
                _DrawGroup1.DrawObjectInside.Colors.SetColor("Player", Color.Black);
                _DrawGroup1.DrawObjectInside.Colors.SetColor("Ememy", Color.Red);
            };

            _UIGroup1.LostFocus += (s, e) =>
            {
                _DrawGroup1.Colors.SetColor("Back", Color.AliceBlue);
                _DrawGroup1.DrawObjectInside.Colors.SetColor("Player", Color.LightSteelBlue);
                _DrawGroup1.DrawObjectInside.Colors.SetColor("Ememy", Color.LightSteelBlue);
            };

            _UIGroup1.Click += (s, e) =>
            {
                Mode = 1;
            };


            _DrawGroup2 = new DrawUITextFrame(Color.DarkSlateBlue, Color.White, Color.WhiteSmoke, Color.DarkSlateBlue, 2, 12, "", new Font("標楷體", 18), GlobalFormat.MiddleBottom);
            _UIGroup2 = new ObjectUI(DirectionType.Center, width + 200, height / 2, 200, 200, _DrawGroup2, new MoveStraight(this, 1, 3000, 1, 100, 1F));
            _UIGroup2.Propertys.Add(new PropertyShadow(-3, 4));
            _UIGroup2.Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
            _UIGroup2.Layout.Depend.SetObject(this);

            _CommandBack = new ObjectUI(20, 20, 80, 40, new DrawUITextFrame(Color.Black, Color.Gray, Color.LightYellow, Color.Black, 1, 8, "返回", new Font("微軟正黑體", 18), GlobalFormat.MiddleCenter));
            _CommandBack.Propertys.Add(new PropertyShadow(5, 4) { RFix = 0.5F, GFix = 0.5F });
            _CommandBack.Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
            _CommandBack.Layout.Depend.SetObject(this);
            _CommandBack.Visible = false;
            _CommandBack.Click += (s, e) =>
                {
                    Mode = 0;
                };



            UIObjects.Add(_CommandBack);
            UIObjects.Add(_UIGroup1);
            UIObjects.Add(_UIGroup2);

            Mode = 0;
        }


        private int _Mode = -1;
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

                        if (_UIScenes.Length > 0)
                        {
                            for (int i = 0; i < _UIScenes.Length; i++)
                            {
                                _UIScenes[i].MoveObject.Target.SetOffsetByXY(_UIScenesLocation[i].X, _UIScenesLocation[i].Y + Layout.RectHeight);
                            }
                        }

                        _BackBuildCounter.Limit = 50;
                        break;
                    case 1:
                        for (int i = 0; i < BackObjects.Count; i++)
                        {
                            BackObjects[i].MoveObject.Resistance = 10;
                        }

                        if (_UIScenes.Length > 0)
                        {
                            for (int i = 0; i < _UIScenes.Length; i++)
                            {
                                _UIScenes[i].MoveObject.Target.SetOffsetByXY(_UIScenesLocation[i].X, _UIScenesLocation[i].Y);
                            }
                        }

                        _UIGroup1.MoveObject.Target.SetOffsetByXY(-1000 + 130, 0);
                        _UIGroup2.MoveObject.Target.SetOffsetByXY(1000 - 130, 0);
                        _CommandBack.DrawObject.Colors.Opacity = 0;
                        _CommandBack.Visible = true;
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
                    if (_CommandBack.DrawObject.Colors.Opacity < 1)
                    {
                        _CommandBack.DrawObject.Colors.Opacity += 0.05F;
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
                    if (_CommandBack.DrawObject.Colors.Opacity > 0)
                    {
                        _CommandBack.DrawObject.Colors.Opacity -= 0.1F;
                        if (_CommandBack.DrawObject.Colors.Opacity <= 0)
                        {
                            _CommandBack.Visible = false;
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
                int top = Layout.Rectangle.Top + Global.Rand.Next(5, Layout.RectHeight - 10);
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
                Propertys.AllDoBeforeDraw(g);
                DrawObject.Draw(g, Layout.Rectangle);
                BackObjects.AllDrawSelf(g);
                UIObjects.AllDrawSelf(g);
                Propertys.AllDoAfterDraw(g);
            }
        }
    }
}
