using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 排行榜資料畫面
    /// </summary>
    public class ObjectUIHighScore : ObjectUIPanel
    {
        private static Font _ItemFont = new Font("微軟正黑體", 12, FontStyle.Bold);
        private static Font _ItemFont2 = new Font("微軟正黑體", 12, FontStyle.Bold);

        public event EventHandler Close;

        public void OnClose()
        {
            if (Close != null)
            {
                Close(this, new EventArgs());
            }
        }

        private DataTable _SourceData;
        private int _HoverLevel;
        private ISceneInfo _SceneInfo;

        private int _SelectedIndex;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (_SelectedIndex == value) return;
                _SelectedIndex = value;
                for (int i = 0; i < _SceneButtons.Length; i++)
                {
                    Color backColor = i == _SelectedIndex - 1 ? Color.FromArgb(200, 255, 220) : Color.FromArgb(175, 255, 255, 255);
                    _SceneButtons[i].DrawObject.Colors.SetColor("Back", backColor);
                }
            }
        }

        private DrawUIText _DrawCommandMessage;
        private ObjectUI _CommandClose;
        private ObjectUI _CommandMessage;
        private ObjectUI[] _SceneButtons;

        public ObjectUIHighScore(DirectionType anchor, int x, int y, MoveBase moveObject, ISceneInfo sceneInfo)
            : base(anchor, x, y, 360, 390, new DrawUIFrame(Color.AliceBlue, Color.CornflowerBlue, 2, 20), moveObject)
        {
            Propertys.Add(new PropertyShadow(0, 6, 0.95F, 1));

            DrawUIText drawCommandClose = new DrawUIText(Color.RoyalBlue, Color.Gray, Color.FromArgb(175, 220, 230, 255), Color.FromArgb(100, 140, 255), 1, 10, "關 閉", new Font("微軟正黑體", 14), GlobalFormat.MiddleCenter);
            DrawUIText drawCommandCloseHover = new DrawUIText(Color.RoyalBlue, Color.Gray, Color.FromArgb(220, 255, 250, 235), Color.FromArgb(100, 140, 255), 1, 10, "關 閉", new Font("微軟正黑體", 14), GlobalFormat.MiddleCenter);

            _CommandClose = new ObjectUI(275, 10, 75, 30, drawCommandClose) { DrawObjectHover = drawCommandCloseHover };
            _CommandClose.Propertys.Add(new PropertyShadow(2, 2) { RFix = -0.5F, GFix = -0.5F, BFix = -0.5F, Opacity = 0.2F });
            _CommandClose.Layout.Depend.Anchor = DirectionType.TopLeft;
            _CommandClose.Layout.Depend.SetObject(this);
            _CommandClose.Click += (s, e) => { OnClose(); };
            UIObjects.Add(_CommandClose);

            if (sceneInfo == null) return;
            _SceneInfo = sceneInfo;
            int itemWidth = 40;
            int itemHeight = 25;
            int left = 15;
            int top = 15;

            _SceneButtons = new ObjectUI[sceneInfo.MaxLevel];
            for (int i = 0; i < sceneInfo.MaxLevel; i++)
            {
                int idx = i + 1;
                DrawUIFrame drawObject = new DrawUIFrame(Color.FromArgb(175, 255, 255, 255), Color.FromArgb(100, 140, 255), 1, 8) { Scale = 0.8F };
                ObjectUI newObject = new ObjectUI(left, top, itemWidth, itemHeight, drawObject);
                newObject.Layout.Depend.SetObject(this);
                newObject.Layout.Depend.Anchor = DirectionType.TopLeft;
                newObject.Propertys.Add(new PropertyShadow(3, 3) { Opacity = 0.2F, ScaleX = 0.95F, ScaleY = 0.95F });
                UIObjects.Add(newObject);
                left += itemWidth;

                drawObject.AfterDraw += (s, g, r) =>
                {
                    g.DrawString(idx.ToString(), _ItemFont, Brushes.RoyalBlue, r, GlobalFormat.MiddleCenter);
                };

                newObject.GetFocus += (s, e) =>
                {
                    (s as ObjectUI).DrawObject.Colors.SetColor("Border", Color.Red);
                    _HoverLevel = idx;
                };

                newObject.LostFocus += (s, e) =>
                {
                    (s as ObjectUI).DrawObject.Colors.SetColor("Border", Color.FromArgb(100, 140, 255));
                    _HoverLevel = 0;
                };

                newObject.Click += (s, e) =>
                {
                    SelectedIndex = _HoverLevel;
                };

                _SceneButtons[i] = newObject;
            }
            SelectedIndex = 1;

            DrawUIFrame drawPanel = new DrawUIFrame(Color.FromArgb(180, 220, 240, 250), Color.Empty, 0, 0);
            ObjectUI panel = new ObjectUI(10, 50, Layout.Width - 20, Layout.Height - 60, drawPanel);
            panel.Propertys.Add(new PropertyShadow(-2, -2));
            panel.Layout.Depend.SetObject(this);
            panel.Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
            UIObjects.Add(panel);

            _DrawRects = new DrawUIText[3, 10];
            _DrawInfos = new string[3, 10];
            int rectLeft = 5, rectTop = 5;
            int rectHeight = 27;
            for (int i = 0; i < 10; i++)
            {
                _DrawRects[0, i] = new DrawUIText(Color.RoyalBlue, Color.Gray, Color.AliceBlue, Color.Empty, 0, 0, "", _ItemFont2, GlobalFormat.MiddleCenter);
                _DrawRects[1, i] = new DrawUIText(Color.RoyalBlue, Color.Gray, Color.AliceBlue, Color.Empty, 0, 0, "", _ItemFont2, GlobalFormat.MiddleCenter);
                _DrawRects[2, i] = new DrawUIText(Color.RoyalBlue, Color.Gray, Color.AliceBlue, Color.Empty, 0, 0, "", _ItemFont2, GlobalFormat.MiddleCenter);

                ObjectUI object1 = new ObjectUI(rectLeft, rectTop, 40, rectHeight, _DrawRects[0, i]);
                ObjectUI object2 = new ObjectUI(rectLeft + 45, rectTop, 205, rectHeight, _DrawRects[1, i]);
                ObjectUI object3 = new ObjectUI(rectLeft + 255, rectTop, 70, rectHeight, _DrawRects[2, i]);

                object1.Propertys.Add(new PropertyShadow(2, 2));
                object2.Propertys.Add(new PropertyShadow(2, 2));
                object3.Propertys.Add(new PropertyShadow(2, 2));
                object1.Layout.Depend.SetObject(panel);
                object2.Layout.Depend.SetObject(panel);
                object3.Layout.Depend.SetObject(panel);
                object1.Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
                object2.Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
                object3.Layout.Depend.Anchor = DirectionType.Left | DirectionType.Top;
                UIObjects.Add(object1);
                UIObjects.Add(object2);
                UIObjects.Add(object3);

                rectTop += rectHeight + 5;
            }

            _DrawCommandMessage = new DrawUIText(Color.Red, Color.Gray, Color.LightYellow, Color.Black, 2, 0, "連線中...", new Font("微軟正黑體", 20), GlobalFormat.MiddleCenter);
            _CommandMessage = new ObjectUI(DirectionType.Center, 100, 50, Layout.Width / 2, Layout.Height / 2, _DrawCommandMessage);
            _CommandMessage.Layout.Depend.SetObject(this);
            _CommandMessage.Layout.Depend.Anchor = DirectionType.Center;
            UIObjects.Add(_CommandMessage);
            LoadData();
        }

        private int _LoadStatus = 0;
        public int LoadStatus
        {
            get { return _LoadStatus; }
            set
            {
                if (_LoadStatus == value) return;
                _LoadStatus = value;

                switch (_LoadStatus)
                {
                    case 0:
                        _DrawCommandMessage.Text = "連線中...";
                        _CommandMessage.Visible = true;
                        break;
                    case 1:
                        _CommandMessage.Visible = false;
                        break;
                    case -1:
                        _DrawCommandMessage.Text = "連線失敗";
                        _CommandMessage.Visible = true;
                        break;
                }
            }

        }

        private async void LoadData()
        {
            Global.SQL.AddParameter("@SceneID", _SceneInfo.SceneID);
            _SourceData = await Global.SQL.RunAsync(@"select Level,PlayerName,Score,rank
                                                      from (  
	                                                      select Score_tmp.Level,Score_tmp.PlayerName,Score_tmp.Score,@rownum:=@rownum+1 ,  
		                                                         if(@Level=Score_tmp.Level,@rank:=@rank+1,@rank:=1) as rank,  
                                                                 @Level:=Score_tmp.Level  
                                                      from ( select Level,PlayerName,Score from Score where SceneID=@SceneID order by Level,Score desc ) Score_tmp ,
                                                      (select @rownum :=0 ,@rank:=0) a ) result   
                                                      WHERE rank<=10");
            _SourceData.DefaultView.Sort = "Score DESC";
            LoadStatus = _SourceData == null ? -1 : 1;
        }

        private void GetData(int level)
        {
            if (_SourceData == null) return;

            _SourceData.DefaultView.RowFilter = string.Format("Level={0}", level);
            for (int i = 0; i < 10; i++)
            {


            }
        }

        private static Font _TitleFont = new Font("微軟正黑體", 16);
        private static SolidBrush _BrushPanel = new SolidBrush(Color.FromArgb(180, 220, 240, 250));
        private DrawUIText[,] _DrawRects;
        private string[,] _DrawInfos;
    }
}
