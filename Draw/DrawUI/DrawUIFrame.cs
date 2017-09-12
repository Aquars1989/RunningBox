using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 框架繪圖物件
    /// </summary>
    public class DrawUIFrame : DrawBase
    {
        /// <summary>
        /// 圓角框架暫存
        /// </summary>
        protected GraphicsPath _BackFrame;

        /// <summary>
        /// 繪置區預暫存
        /// </summary>
        protected Region _BackRegion;

        /// <summary>
        /// 為置配置暫存
        /// </summary>
        protected Rectangle _BackFrameRectangle;

        #region ===== 事件 =====
        /// <summary>
        /// 發生於內部繪置物件變更
        /// </summary>
        public event EventHandler IconDrawObjectChanged;

        /// <summary>
        /// 發生於圓角大小變更
        /// </summary>
        public event EventHandler ReadiusChanged;

        /// <summary>
        /// 發生於框線粗細變更
        /// </summary>
        public event EventHandler BorderWidtrhChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於內部繪置物件變更
        /// </summary>
        protected virtual void OnDrawObjectInsideChanged()
        {
            if (DrawObjectInside != null)
            {
                DrawObjectInside.Scene = this.Scene;
                DrawObjectInside.Owner = this.Owner;
            }

            if (IconDrawObjectChanged != null)
            {
                IconDrawObjectChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於圓角大小變更
        /// </summary>
        protected virtual void OnReadiusChanged()
        {
            if (_BackFrame != null)
            {
                _BackFrame.Dispose();
                _BackFrame = null;
            }
            if (_BackRegion != null)
            {
                _BackRegion.Dispose();
                _BackRegion = null;
            }

            if (ReadiusChanged != null)
            {
                ReadiusChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於框線粗細變更
        /// </summary>
        protected virtual void OnBorderWidtrhChanged()
        {
            if (BorderWidtrhChanged != null)
            {
                BorderWidtrhChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於物件釋放
        /// </summary>
        protected override void OnDispose()
        {
            if (_BackFrame != null)
            {
                _BackFrame.Dispose();
                _BackFrame = null;
            }
            if (_BackRegion != null)
            {
                _BackRegion.Dispose();
                _BackRegion = null;
            }
            base.OnDispose();
        }
        #endregion

        #region ===== 屬性 =====
        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Colors.GetColor("Border"); }
        }

        private int _BorderWidtrh;
        /// <summary>
        /// 框線粗細
        /// </summary>
        public int BorderWidtrh
        {
            get { return _BorderWidtrh; }
            set
            {
                if (_BorderWidtrh == value) return;
                _BorderWidtrh = value;
                OnBorderWidtrhChanged();
            }
        }

        private int _Readius;
        /// <summary>
        /// 圓角大小
        /// </summary>
        public int Readius
        {
            get { return _Readius; }
            set
            {
                if (_Readius == value) return;
                _Readius = value;
                OnReadiusChanged();
            }
        }

        private DrawBase _DrawObjectInside;
        /// <summary>
        /// 內部的圖示繪圖物件(必要)
        /// </summary>
        public DrawBase DrawObjectInside
        {
            get { return _DrawObjectInside; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_DrawObjectInside == value) return;
                _DrawObjectInside = value;
                OnDrawObjectInsideChanged();
            }
        }
        #endregion

        /// <summary>
        /// 使用繪圖工具管理物件新增框架繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="readius">圓角大小</param>
        /// <param name="iconDrawObject">內部繪圖物件</param>
        public DrawUIFrame(DrawColors drawColor, int borderWidtrh, int readius, DrawBase iconDrawObject)
            : base(drawColor)
        {
            BorderWidtrh = borderWidtrh;
            Readius = readius;
            DrawObjectInside = iconDrawObject;
        }

        /// <summary>
        /// 使用繪圖工具管理物件新增框架繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="readius">圓角大小</param>
        public DrawUIFrame(DrawColors drawColor, int borderWidtrh, int readius)
            : this(drawColor, borderWidtrh, readius, DrawNull.Value) { }

        /// <summary>
        /// 新增框架繪圖物件
        /// </summary>
        /// <param name="borderColor">框線顏色</param>
        /// <param name="backColor">背景色</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="readius">圓角大小</param>
        /// <param name="iconDrawObject">內部繪圖物件</param>
        public DrawUIFrame(Color backColor, Color borderColor, int borderWidtrh, int readius, DrawBase iconDrawObject)
        {
            Colors.SetColor("Border", borderColor);
            Colors.SetColor("Back", backColor);
            BorderWidtrh = borderWidtrh;
            Readius = readius;
            DrawObjectInside = iconDrawObject;
        }

        /// <summary>
        /// 新增框架繪圖物件
        /// </summary>
        /// <param name="borderColor">框線顏色</param>
        /// <param name="param">背景色</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="borderWidtrh">框線粗細</param>
        public DrawUIFrame(Color backColor, Color borderColor, int borderWidtrh, int readius) :
            this(backColor, borderColor, borderWidtrh, readius, DrawNull.Value) { }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = GetScaleRectangle(rectangle);

            SolidBrush brushBack = Colors.GetBrush("Back");
            Pen penBorder = Colors.GetPen("Border");
            penBorder.Width = BorderWidtrh;

            GetBackFrame(drawRectangle);
            g.FillPath(brushBack, _BackFrame);
            if (DrawObjectInside != DrawNull.Value)
            {
                g.Clip = _BackRegion;
                DrawObjectInside.Draw(g, drawRectangle);
                g.ResetClip();
            }
            g.DrawPath(penBorder, _BackFrame);
        }

        /// <summary>
        /// 產生圓角區域
        /// </summary>
        protected void GetBackFrame(Rectangle rectangle)
        {
            if (_BackFrameRectangle != rectangle && _BackFrame != null)
            {
                _BackFrame.Dispose();
                _BackRegion.Dispose();
                _BackFrame = null;
                _BackRegion = null;
            }

            if (_BackFrame == null)
            {
                _BackFrameRectangle = rectangle;
                _BackFrame = Function.GetRadiusFrame(rectangle, _Readius);
                _BackRegion = new Region(_BackFrame);
            }
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件(不包含內部物件)
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawUIFrame(Colors.Copy(), BorderWidtrh, Readius)
            {
                Scene = this.Scene,
                Owner = this.Owner,
                Scale = this.Scale
            };
        }
    }
}
