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
                    _X = Target.Layout.CenterX;
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
                    _Y = Target.Layout.CenterY;
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
