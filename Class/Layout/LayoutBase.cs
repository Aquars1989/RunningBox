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
    public abstract class LayoutBase
    {
        /// <summary>
        /// 是否需要重新產生實體位置
        /// </summary>
        protected bool RectangleBuild { get; set; }

        private float _X;
        /// <summary>
        /// 物件的位置X
        /// </summary>
        public float X
        {
            get { return _X; }
            set
            {
                _X = value;
                RectangleBuild = false;
            }
        }

        private float _Y;
        /// <summary>
        /// 物件的位置Y
        /// </summary>
        public float Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
                RectangleBuild = false;
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
                RectangleBuild = false;
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
                RectangleBuild = false;
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
                RectangleBuild = false;
            }
        }

        /// <summary>
        /// 取得物件中心點
        /// </summary>
        public abstract PointF Center { get; }

        /// <summary>
        /// 取得物件實體位置
        /// </summary>
        public abstract Rectangle Rectangle { get; }
    }
}
