using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 代表場景追蹤點的目標物件
    /// </summary>
    public class TargetTrackPoint : ITarget
    {
        /// <summary>
        /// 目標X座標
        /// </summary>
        public float X
        {
            get { return Targer.TrackPoint.X; }
        }

        /// <summary>
        /// 目標Y座標
        /// </summary>
        public float Y
        {
            get { return Targer.TrackPoint.Y; }
        }

        private SceneBase _Targer = null;
        /// <summary>
        /// 指定場景
        /// </summary>
        public SceneBase Targer
        {
            get { return _Targer; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                _Targer = value;
            }
        }

        /// <summary>
        /// 新增代表場景追蹤點的目標物件
        /// </summary>
        /// <param name="scene">場景物件</param>
        public TargetTrackPoint(SceneBase scene)
        {
            Targer = scene;
        }

        /// <summary>
        /// 取得場景追蹤點
        /// </summary>
        /// <returns>場景追蹤點</returns>
        public PointF GetPoint()
        {
            return Targer.TrackPoint;
        }
    }
}
