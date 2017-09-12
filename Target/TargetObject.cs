using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 代表物件座標的目標物件
    /// </summary>
    public class TargetObject : ITarget
    {
        private ContentAlignment _Anchor = ContentAlignment.MiddleCenter;
        /// <summary>
        /// 追蹤點位於目標的位置
        /// </summary>
        public ContentAlignment Anchor
        {
            get { return _Anchor; }
            set { _Anchor = value; }
        }

        private float _X = 0;
        /// <summary>
        /// 目標X座標
        /// </summary>
        public float X
        {
            get
            {
                if (Target != null)
                {
                    switch (Anchor)
                    {
                        case ContentAlignment.BottomLeft:
                        case ContentAlignment.MiddleLeft:
                        case ContentAlignment.TopLeft:
                            _X = Target.Layout.LeftTopX;
                            break;
                        case ContentAlignment.BottomCenter:
                        case ContentAlignment.MiddleCenter:
                        case ContentAlignment.TopCenter:
                            _X = Target.Layout.CenterX;
                            break;
                        case ContentAlignment.BottomRight:
                        case ContentAlignment.MiddleRight:
                        case ContentAlignment.TopRight:
                            _X = Target.Layout.RightBottomX;
                            break;
                    }
                }
                return _X;
            }
        }

        private float _Y = 0;
        /// <summary>
        /// 目標Y座標
        /// </summary>
        public float Y
        {
            get
            {
                if (Target != null)
                {
                    switch (Anchor)
                    {
                        
                       
                        case ContentAlignment.TopLeft:
                        case ContentAlignment.TopCenter:
                        case ContentAlignment.TopRight:
                            _Y = Target.Layout.LeftTopY;
                            break;
                        case ContentAlignment.MiddleLeft:
                        case ContentAlignment.MiddleCenter:
                        case ContentAlignment.MiddleRight:
                            _Y = Target.Layout.CenterY;
                            break;
                        case ContentAlignment.BottomLeft:
                        case ContentAlignment.BottomCenter:
                        case ContentAlignment.BottomRight:
                            _Y = Target.Layout.RightBottomY;
                            break;
                    }
                }
                return _Y;
            }
        }

        private ObjectBase _Target = null;
        /// <summary>
        /// 做為目標的活動物件
        /// </summary>
        public ObjectBase Target
        {
            get { return _Target; }
            set
            {
                _Target = value;
            }
        }

        /// <summary>
        /// 新增代表物件座標的目標物件
        /// </summary>
        /// <param name="objectBase">活動物件</param>
        public TargetObject(ObjectBase objectBase)
        {
            Target = objectBase;
        }

        /// <summary>
        /// 取得物件的座標
        /// </summary>
        /// <returns>物件的座標</returns>
        public PointF GetPoint()
        {
            if (Target != null)
            {
                _X = Target.Layout.CenterX;
                _Y = Target.Layout.CenterY;
            }
            return new PointF(_X, _Y);
        }
    }
}
