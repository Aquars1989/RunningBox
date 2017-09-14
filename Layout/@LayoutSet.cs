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
    public class LayoutSet : ITargetability
    {
        private bool SizeChange = false;
        private bool LocationChange = false;

        /// <summary>
        /// 定位點位於寬度的位置
        /// </summary>
        private float _AnchorOfWidth;

        /// <summary>
        /// 定位點位於高度的位置
        /// </summary>
        private float _AnchorOfHeight;

        #region ===== 事件 =====
        /// <summary>
        /// 發生於依附目標變更
        /// </summary>
        public event ValueChangedEnentHandle DependObjectChanged;

        /// <summary>
        /// 發生於碰撞形狀變更
        /// </summary>
        public event ValueChangedEnentHandle CollisonShapeChanged;

        /// <summary>
        /// 發生於物件的定位位置變更時
        /// </summary>
        public event ValueChangedEnentHandle AnchorChanged;

        /// <summary>
        /// 發生於尺寸變化時
        /// </summary>
        public event ValueChangedEnentHandle SizeChanged;

        /// <summary>
        /// 發生於定位點變化時
        /// </summary>
        public event ValueChangedEnentHandle LocationChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於依附目標變更
        /// </summary>
        protected virtual void OnDependObjectChanged(object oldValue, object newValue)
        {
            if (DependObjectChanged != null)
            {
                DependObjectChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於碰撞形狀變更
        /// </summary>
        protected virtual void OnCollisonShapeChanged(object oldValue, object newValue)
        {
            if (CollisonShapeChanged != null)
            {
                CollisonShapeChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於物件的定位位置變更時
        /// </summary>
        protected virtual void OnAnchorChanged(object oldValue, object newValue)
        {
            if (AnchorChanged != null)
            {
                AnchorChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於尺寸變化時
        /// </summary>
        protected virtual void OnSizeChanged(int oldWidth, int oldHeight, int newWidth, int newHeight)
        {
            SizeChange = true;
            if (SizeChanged != null)
            {
                SizeChanged(this, new Size(oldWidth, oldHeight), new Size(newWidth, newHeight));
            }
        }

        /// <summary>
        /// 發生於定位點變化時
        /// </summary>
        protected virtual void OnLocationChanged(float oldX, float oldY, float newX, float newY)
        {
            LocationChange = true;
            if (LocationChanged != null)
            {
                LocationChanged(this, new PointF(oldX, oldY), new PointF(newX, newY));
            }
        }
        #endregion

        #region ===== 屬性 =====
        private ShapeType _CollisonShape = ShapeType.Ellipse;
        /// <summary>
        /// 碰撞形狀
        /// </summary>
        public ShapeType CollisonShape
        {
            get { return _CollisonShape; }
            set
            {
                if (_CollisonShape == value) return;
                object oldValue = _CollisonShape;
                _CollisonShape = value;
                OnCollisonShapeChanged(oldValue, value);
            }
        }

        /// <summary>
        /// 依附目標(必要),有值時會以此目標為原點而非(0,0)
        /// </summary>
        public TargetSet Depend { get; private set; }

        private float _LeftTopX;
        /// <summary>
        /// 左上角座標X
        /// </summary>
        public float LeftTopX
        {
            get { return _LeftTopX + Depend.X; }
            set
            {
                value -= Depend.X;
                if (_LeftTopX == value) return;

                float fix = value - _LeftTopX;
                float oldValue = _X;
                _X += fix;
                _CenterX += fix;
                _RightBottomX = value;
                _LeftTopX = value;
                OnLocationChanged(oldValue, _Y, _X, _Y);
            }
        }

        private float _LeftTopY;
        /// <summary>
        /// 左上角座標Y
        /// </summary>
        public float LeftTopY
        {
            get { return _LeftTopY + Depend.Y; }
            set
            {
                value -= Depend.Y;
                if (_LeftTopY == value) return;

                float fix = value - _LeftTopY;
                float oldValue = _Y;
                _Y += fix;
                _CenterY += fix;
                _RightBottomY += fix;
                _LeftTopY = value;
                OnLocationChanged(_X, oldValue, _X, _Y);
            }
        }

        private float _RightBottomX;
        /// <summary>
        /// 右下角座標X
        /// </summary>
        public float RightBottomX
        {
            get { return _RightBottomX + Depend.X; }
            set
            {
                value -= Depend.X;
                if (_RightBottomX == value) return;

                float fix = value - _RightBottomX;
                float oldValue = _X;
                _X += fix;
                _CenterX += fix;
                _LeftTopX += fix;
                _RightBottomX = value;
                OnLocationChanged(oldValue, _Y, _X, _Y);
            }
        }

        private float _RightBottomY;
        /// <summary>
        /// 右下角座標Y
        /// </summary>
        public float RightBottomY
        {
            get { return _RightBottomY + Depend.Y; }
            set
            {
                value -= Depend.Y;
                if (_RightBottomY == value) return;

                float fix = value - _RightBottomY;
                float oldValue = _Y;
                _Y += fix;
                _CenterY += fix;
                _LeftTopY += fix;
                _RightBottomY = value;
                OnLocationChanged(_X, oldValue, _X, _Y);
            }
        }

        private float _CenterX;
        /// <summary>
        /// 取得物件中心點X座標
        /// </summary>
        public float CenterX
        {
            get { return _CenterX + Depend.X; }
            set
            {
                value -= Depend.X;
                if (_CenterX == value) return;

                float fix = value - _CenterX;
                float oldValue = _X;
                _X += fix;
                _LeftTopX += fix;
                _RightBottomX += fix;
                _CenterX = value;
                OnLocationChanged(oldValue, _Y, _X, _Y);
            }
        }

        private float _CenterY;
        /// <summary>
        /// 取得物件中心點Y座標
        /// </summary>
        public float CenterY
        {
            get { return _CenterY + Depend.Y; }
            set
            {
                value -= Depend.Y;
                if (_CenterY == value) return;

                float fix = value - _CenterY;
                float oldValue = _Y;
                _Y += fix;
                _LeftTopY += fix;
                _RightBottomY += fix;
                _CenterY = value;
                OnLocationChanged(_X, oldValue, _X, _Y);
            }
        }

        private DirectionType _Anchor = DirectionType.Center;
        /// <summary>
        /// 設定物件定位位置
        /// </summary>
        public DirectionType Anchor
        {
            get { return _Anchor; }
            set
            {
                if (_Anchor == value) return;

                object oldValue = _Anchor;
                _Anchor = value;

                if ((_Anchor & DirectionType.Left) == DirectionType.Left)
                {
                    _AnchorOfWidth = 0;
                }
                else if ((_Anchor & DirectionType.Right) == DirectionType.Right)
                {
                    _AnchorOfWidth = 1;
                }
                else
                {
                    _AnchorOfWidth = 0.5F;
                }

                if ((_Anchor & DirectionType.Top) == DirectionType.Top)
                {
                    _AnchorOfHeight = 0;
                }
                else if ((_Anchor & DirectionType.Bottom) == DirectionType.Bottom)
                {
                    _AnchorOfHeight = 1;
                }
                else
                {
                    _AnchorOfHeight = 0.5F;
                }

                _X = LeftTopX + RectWidth * _AnchorOfWidth;
                _Y = LeftTopY + RectHeight * _AnchorOfHeight;

                OnAnchorChanged(oldValue, value);
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
                float oldValue = _X;
                _LeftTopX += fix;
                _RightBottomX += fix;
                _CenterX += fix;
                _X = value;
                OnLocationChanged(oldValue, _Y, _X, _Y);
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
                float oldValue = _Y;
                _LeftTopY += fix;
                _RightBottomY += fix;
                _CenterY += fix;
                _Y = value;
                OnLocationChanged(_X, oldValue, _X, _Y);
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
                if (_RectWidth == value) return;
                int oldValue = _RectWidth;
                _RectWidth = value;
                _LeftTopX = _X - _RectWidth * _AnchorOfWidth;
                _RightBottomX = _LeftTopX + _RectWidth;
                _CenterX = _LeftTopX + _RectWidth * 0.5F;
                OnSizeChanged(oldValue, _RectHeight, value, _RectHeight);
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
                if (_RectHeight == value) return;
                int oldValue = _RectHeight;
                _RectHeight = value;
                _LeftTopY = _Y - _RectHeight * _AnchorOfHeight;
                _RightBottomY = _LeftTopY + _RectHeight;
                _CenterY = _LeftTopY + _RectHeight * 0.5F;
                OnSizeChanged(_RectWidth, oldValue, _RectWidth, value);
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

        private float _Scale = 1;
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
                if (Depend != null)
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
        #endregion

        public LayoutSet()
        {
            Depend = new TargetSet();
            Depend.ObjectChanged += (s, o, n) => { OnDependObjectChanged(o, n); };
        }

        #region ===== 實作ITargetability =====
        /// <summary>
        /// 使用特定的定位位置取得目標點X座標
        /// </summary>
        /// <param name="anchor">定位位置</param>
        /// <returns>目標點X座標</returns>
        public float GetTargetX(DirectionType anchor)
        {
            if ((anchor & DirectionType.Left) == DirectionType.Left)
            {
                return LeftTopX;
            }
            else if ((anchor & DirectionType.Right) == DirectionType.Right)
            {
                return RightBottomX;
            }
            else
            {
                return CenterX;
            }
        }

        /// <summary>
        /// 使用特定的定位位置取得目標點Y座標
        /// </summary>
        /// <param name="anchor">定位位置</param>
        /// <returns>目標點Y座標</returns>
        public float GetTargetY(DirectionType anchor)
        {
            if ((anchor & DirectionType.Top) == DirectionType.Top)
            {
                return LeftTopY;
            }
            else if ((anchor & DirectionType.Bottom) == DirectionType.Bottom)
            {
                return RightBottomY;
            }
            else
            {
                return CenterY;
            }
        }

        /// <summary>
        /// 使用特定的定位位置取得目標點
        /// </summary>
        /// <param name="anchor">定位位置</param>
        /// <returns>目標點</returns>
        public PointF GetTargetPoint(DirectionType anchor)
        {
            return new PointF(GetTargetX(anchor), GetTargetY(anchor));
        }
        #endregion
    }
}
