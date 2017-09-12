using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 基礎介面物件
    /// </summary>
    public class ObjectUI : ObjectBase
    {
        /// <summary>
        /// 取得焦點
        /// </summary>
        public event EventHandler GetFocus;

        /// <summary>
        /// 失去焦點
        /// </summary>
        public event EventHandler LostFocus;

        /// <summary>
        /// 被點選
        /// </summary>
        public event MouseEventHandler Click;

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 是否綁定Click事件
        /// </summary>
        public bool HasClickEnevt { get { return Click != null; } }

        /// <summary>
        /// 使用指定的定位點和移動物件建立介面物件
        /// </summary>
        /// <param name="x">物件左上位置X</param>
        /// <param name="y">物件左上位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectUI(ContentAlignment anchor, int x, int y, int width, int height, DrawBase drawObject, MoveBase moveObject)
            : base(drawObject, moveObject)
        {
            Layout.CollisonShape = ShapeType.Rectangle;
            Layout.Anchor = anchor;
            Layout.X = x;
            Layout.Y = y;
            Layout.Width = width;
            Layout.Height = height;
            DrawObject = drawObject;
            Enabled = true;
        }

        /// <summary>
        /// 使用指定的定位點建立不可移動介面物件
        /// </summary>
        /// <param name="anchor">定位點位置X</param>
        /// <param name="x">物件左上位置X</param>
        /// <param name="y">物件左上位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="drawObject">繪製物件</param>
        public ObjectUI(ContentAlignment anchor, int x, int y, int width, int height, DrawBase drawObject)
            : this(ContentAlignment.TopLeft, x, y, width, height, drawObject, MoveNull.Value)
        {
            Layout.CollisonShape = ShapeType.Rectangle;
            Layout.Anchor = anchor;
            Layout.X = x;
            Layout.Y = y;
            Layout.Width = width;
            Layout.Height = height;
            DrawObject = drawObject;
        }

        /// <summary>
        /// 使用預設定位點(左上)建立不可移動介面物件
        /// </summary>
        /// <param name="x">物件左上位置X</param>
        /// <param name="y">物件左上位置Y</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectUI(int x, int y, int width, int height, DrawBase drawObject)
            : this(ContentAlignment.TopLeft, x, y, width, height, drawObject) { }

        /// <summary>
        /// 檢查座標落在哪個物件內
        /// </summary>
        /// <param name="point">檢查座標</param>
        public virtual ObjectUI InRectangle(Point point)
        {
            return point.X >= Layout.Rectangle.Left && point.X <= Layout.Rectangle.Left + Layout.Rectangle.Width &&
                   point.Y >= Layout.Rectangle.Top && point.Y <= Layout.Rectangle.Top + Layout.Rectangle.Height ? this : null;
        }

        public virtual void OnGetFocus()
        {
            if (Enabled && Visible)
            {
                if (GetFocus != null)
                {
                    GetFocus(this, new EventArgs());
                }
            }
        }

        public virtual void OnLostFocus()
        {
            if (Enabled && Visible)
            {
                if (LostFocus != null)
                {
                    LostFocus(this, new EventArgs());
                }
            }
        }

        public virtual void OnClick(MouseEventArgs e)
        {
            if (Enabled && Click != null)
            {
                Click(this, e);
            }
        }

        public override void Action()
        {
            if (MoveObject == MoveNull.Value)
            {
                base.Action();
            }
            else
            {
                ObjectUI inRect = InRectangle(Scene.TrackPoint);
                base.Action();
                if (inRect != InRectangle(Scene.TrackPoint))
                {
                    Scene.SearchFocusObjectUI();
                }
            }
        }

        protected override void OnVisibleChanged()
        {
            if (Scene == null) return;
            if (InRectangle(Scene.TrackPoint) != null)
            {
                Scene.SearchFocusObjectUI();
            }
            base.OnVisibleChanged();
        }
    }
}
