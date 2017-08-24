using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 物件實體位置配置
    /// </summary>
    public class Layout
    {
        private bool SizeChange = false;
        private bool LocationChange = false;

        /// <summary>
        /// 碰撞形狀
        /// </summary>
        public ShapeType CollisonShape { get; set; }

        /// <summary>
        /// 定位點位於寬度的位置
        /// </summary>
        private float _AnchorOfWidth;

        /// <summary>
        /// 定位點位於高度的位置
        /// </summary>
        private float _AnchorOfHeight;

        private ITarget _DependTarget;
        /// <summary>
        /// 依附目標,有值時會以此目標為原點而非(0,0)
        /// </summary>
        public ITarget DependTarget { get; set; }

        private float _LeftTopX;
        /// <summary>
        /// 左上角座標X
        /// </summary>
        public float LeftTopX
        {
            get { return DependTarget == null ? _LeftTopX : _LeftTopX + DependTarget.X; }

            set
            {
                if (DependTarget != null)
                    value -= DependTarget.X;

                if (_LeftTopX == value) return;

                float fix = value - _LeftTopX;
                _X += fix;
                _CenterX += fix;
                _LeftTopX = value;
                OnLocationChanged();
            }
        }

        private float _LeftTopY;
        /// <summary>
        /// 左上角座標Y
        /// </summary>
        public float LeftTopY
        {
            get { return DependTarget == null ? _LeftTopY : _LeftTopY + DependTarget.Y; }
            set
            {
                if (DependTarget != null)
                    value -= DependTarget.Y;

                if (_LeftTopY == value) return;

                float fix = value - _LeftTopY;
                _Y += fix;
                _CenterY += fix;
                _LeftTopY = value;
                OnLocationChanged();
            }
        }

        private float _CenterX;
        /// <summary>
        /// 取得物件中心點X座標
        /// </summary>
        public float CenterX
        {
            get { return DependTarget == null ? _CenterX : _CenterX + DependTarget.X; }
            set
            {
                if (DependTarget != null)
                    value -= DependTarget.X;

                if (_CenterX == value) return;

                float fix = value - _CenterX;
                _X += fix;
                _LeftTopX += fix;
                _CenterX = value;
                OnLocationChanged();
            }
        }

        private float _CenterY;
        /// <summary>
        /// 取得物件中心點Y座標
        /// </summary>
        public float CenterY
        {
            get { return DependTarget == null ? _CenterY : _CenterY + DependTarget.Y; }
            set
            {
                if (DependTarget != null)
                    value -= DependTarget.Y;

                if (_CenterY == value) return;

                float fix = value - _CenterY;
                _Y += fix;
                _LeftTopY += fix;
                _CenterY = value;
                OnLocationChanged();
            }
        }

        private ContentAlignment _Anchor = ContentAlignment.TopLeft;
        /// <summary>
        /// 設定物件定位位置
        /// </summary>
        public ContentAlignment Anchor
        {
            get { return _Anchor; }
            set
            {
                if (_Anchor == value) return;

                _Anchor = value;
                switch (_Anchor)
                {
                    case ContentAlignment.TopCenter:
                        _AnchorOfWidth = 0.5F;
                        _AnchorOfHeight = 0;
                        break;
                    case ContentAlignment.TopLeft:
                        _AnchorOfWidth = 0;
                        _AnchorOfHeight = 0;
                        break;
                    case ContentAlignment.TopRight:
                        _AnchorOfWidth = 1;
                        _AnchorOfHeight = 0;
                        break;
                    case ContentAlignment.MiddleCenter:
                        _AnchorOfWidth = 0.5F;
                        _AnchorOfHeight = 0.5F;
                        break;
                    case ContentAlignment.MiddleLeft:
                        _AnchorOfWidth = 0;
                        _AnchorOfHeight = 0.5F;
                        break;
                    case ContentAlignment.MiddleRight:
                        _AnchorOfWidth = 1;
                        _AnchorOfHeight = 0.5F;
                        break;
                    case ContentAlignment.BottomCenter:
                        _AnchorOfWidth = 0.5F;
                        _AnchorOfHeight = 1;
                        break;
                    case ContentAlignment.BottomLeft:
                        _AnchorOfWidth = 0;
                        _AnchorOfHeight = 1;
                        break;
                    case ContentAlignment.BottomRight:
                        _AnchorOfWidth = 1;
                        _AnchorOfHeight = 1;
                        break;
                }
                _X = LeftTopX + RectWidth * _AnchorOfWidth;
                _Y = LeftTopY + RectHeight * _AnchorOfHeight;
            }
        }

        private float _X;
        /// <summary>
        /// 物件的定位點座標X
        /// </summary>
        public float X
        {
            get { return _X; }
            set
            {
                if (_X == value) return;

                float fix = value - _X;
                _LeftTopX += fix;
                _CenterX += fix;
                _X = value;
                OnLocationChanged();
            }
        }

        private float _Y;
        /// <summary>
        /// 物件的定位點座標Y
        /// </summary>
        public float Y
        {
            get { return _Y; }
            set
            {
                if (_Y == value) return;

                float fix = value - _Y;
                _LeftTopY += fix;
                _CenterY += fix;
                _Y = value;
                OnLocationChanged();
            }
        }

        private int _RectWidth;
        /// <summary>
        /// 取得物件的實際寬度
        /// </summary>
        public int RectWidth
        {
            get { return _RectWidth; }
            private set
            {
                //value = Math.Max(value, 1);
                if (_RectWidth == value) return;

                _RectWidth = value;
                _LeftTopX = _X - _RectWidth * _AnchorOfWidth;
                _CenterX = _LeftTopX + _RectWidth * 0.5F;
                OnSizeChanged();
            }
        }

        private int _RectHeight;
        /// <summary>
        /// 取得物件的實際高度
        /// </summary>
        public int RectHeight
        {
            get { return _RectHeight; }
            private set
            {
                //value = Math.Max(value, 1);
                if (_RectHeight == value) return;

                _RectHeight = value;
                _LeftTopY = _Y - _RectHeight * _AnchorOfHeight;
                _CenterY = _LeftTopY + _RectHeight * 0.5F;
                OnSizeChanged();
            }
        }

        private int _Width;
        /// <summary>
        /// 取得或設定物件的設定寬度
        /// </summary>
        public int Width
        {
            get { return _Width; }
            set
            {
                if (_Width == value) return;

                _Width = value;
                RectWidth = (int)(_Width * _Scale);
            }
        }

        private int _Height;
        /// <summary>
        /// 取得或設定物件的設定高度
        /// </summary>
        public int Height
        {
            get { return _Height; }
            set
            {
                if (_Height == value) return;

                _Height = value;
                RectHeight = (int)(_Height * _Scale);
            }
        }

        private float _Scale;
        /// <summary>
        /// 取得或設定物件的縮放值
        /// </summary>
        public float Scale
        {
            get { return _Scale; }
            set
            {
                if (_Scale == value) return;

                _Scale = value;
                RectWidth = (int)(_Width * _Scale);
                RectHeight = (int)(_Height * _Scale);
                OnSizeChanged();
            }
        }


        private Rectangle _Rectangle;
        /// <summary>
        /// 取得物件實體位置
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                if (DependTarget != null)
                {
                    LocationChange = true;
                }

                if (SizeChange)
                {
                    _Rectangle.Size = new Size(RectWidth, RectHeight);
                    _Rectangle.Location = new Point((int)(LeftTopX + 0.5F), (int)(LeftTopY + 0.5F));
                    SizeChange = false;
                    LocationChange = false;
                }

                if (LocationChange)
                {
                    _Rectangle.Location = new Point((int)(LeftTopX + 0.5F), (int)(LeftTopY + 0.5F));
                    LocationChange = false;
                }
                return _Rectangle;
            }
        }

        public Layout()
        {
            Anchor = ContentAlignment.TopLeft;
            Scale = 1;
        }

        /// <summary>
        /// 發生於尺寸變化時
        /// </summary>
        protected void OnSizeChanged()
        {
            SizeChange = true;
        }

        /// <summary>
        /// 發生於定位點變化時
        /// </summary>
        protected void OnLocationChanged()
        {
            LocationChange = true;
        }
    }
}
