using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 物件實體位置配置介面
    /// </summary>
    public class LayoutBase
    {
        /// <summary>
        /// 左上角座標X
        /// </summary>
        private float _LeftTopX;
        
        /// <summary>
        /// 左上角座標Y
        /// </summary>
        private float _LeftTopY;

        /// <summary>
        /// 定位點位於寬度的位置
        /// </summary>
        private float _AnchorOfWidth;

        /// <summary>
        /// 定位點位於高度的位置
        /// </summary>
        private float _AnchorOfHeight;

        private ContentAlignment _Anchor;
        /// <summary>
        /// 設定物件定位位置
        /// </summary>
        public ContentAlignment Anchor
        {
            get { return _Anchor; }
            set
            {
                if (_Anchor == value) return;

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
                _X = _LeftTopX + Width * _AnchorOfWidth;
                _Y = _LeftTopY + Height * _AnchorOfHeight;
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
                _X = value;
                _LeftTopX = _X - Width * _AnchorOfWidth;
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
                _Y = value;
                _LeftTopY = _Y - Height * _AnchorOfHeight;
                OnLocationChanged();
            }
        }

        private int _Width;
        /// <summary>
        /// 物件的寬度
        /// </summary>
        public int Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
                OnSizeChanged();
            }
        }

        private int _Height;
        /// <summary>
        /// 物件的高度
        /// </summary>
        public int Height
        {
            get { return _Height; }
            set
            {
                _Height = value;
                OnSizeChanged();
            }
        }

        private float _Scale;
        /// <summary>
        /// 物件的縮放值
        /// </summary>
        public float Scale
        {
            get { return _Scale; }
            set
            {
                _Scale = value;
                OnSizeChanged();
            }
        }

        /// <summary>
        /// 取得物件中心點X座標
        /// </summary>
        public abstract float CenterX { get; }

        /// <summary>
        /// 取得物件中心點Y座標
        /// </summary>
        public abstract float CenterY { get; }

        /// <summary>
        /// 取得物件實體位置
        /// </summary>
        public abstract Rectangle Rectangle { get; }

        /// <summary>
        /// 發生於尺寸變化時
        /// </summary>
        protected abstract void OnSizeChanged();

        /// <summary>
        /// 發生於定位點變化時
        /// </summary>
        protected abstract void OnLocationChanged();
    }
}
