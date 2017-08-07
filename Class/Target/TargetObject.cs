using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class TargetObject : ITarget
    {
        public float X
        {
            get { return Targer.X; }
        }

        public float Y
        {
            get { return Targer.Y; }
        }

        private ObjectBase _Targer = null;
        public ObjectBase Targer
        {
            get { return _Targer; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                _Targer = value;
            }
        }

        public TargetObject(ObjectBase objectBase)
        {
            Targer = objectBase;
        }

        public PointF GetPoint()
        {
            return new PointF(Targer.X, Targer.Y);
        }
    }
}
