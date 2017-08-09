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
                if (Targer != null)
                {
                    _X = Targer.X;
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
                if (Targer != null)
                {
                    _Y = Targer.Y;
                }
                return _Y;
            }
        }

        private ObjectBase _Targer = null;
        /// <summary>
        /// 做為目標的活動物件
        /// </summary>
        public ObjectBase Targer
        {
            get { return _Targer; }
            set
            {
                _Targer = value;
            }
        }

        /// <summary>
        /// 新增代表物件座標的目標物件
        /// </summary>
        /// <param name="objectBase">活動物件</param>
        public TargetObject(ObjectBase objectBase)
        {
            Targer = objectBase;
        }

        /// <summary>
        /// 取得物件的座標
        /// </summary>
        /// <returns>物件的座標</returns>
        public PointF GetPoint()
        {
            if (Targer != null)
            {
                _X = Targer.X;
                _Y = Targer.Y;
            }
            return new PointF(_X, _Y);
        }
    }
}
