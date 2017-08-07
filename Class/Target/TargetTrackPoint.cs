using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class TargetTrackPoint : ITarget
    {
        public float X
        {
            get { return Targer.TrackPoint.X; }
        }

        public float Y
        {
            get { return Targer.TrackPoint.Y; }
        }

        private SceneBase _Targer = null;
        public SceneBase Targer
        {
            get { return _Targer; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                _Targer = value;
            }
        }

        public TargetTrackPoint(SceneBase scene)
        {
            Targer = scene;
        }

        public PointF GetPoint()
        {
            return Targer.TrackPoint;
        }
    }
}
