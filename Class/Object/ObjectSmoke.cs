using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class ObjectSmoke : ObjectBase
    {
        public int FadeTick { get; set; }
        public int FadeTickMax { get; set; }

        private SolidBrush _Brush;
        private Color _Color;
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color == value) return;

                if (_Brush != null)
                {
                    DrawPool.BackBrush(_Color);
                }

                _Color = value;
                _Brush = DrawPool.GetBrush(_Color);
            }
        }

        public ObjectSmoke(float x, float y, int size, Color color, int fadeTick)
        {
            Status = ObjectStatus.Dying;
            X = x;
            Y = y;
            Size = size;
            //Moves = new List<PointF>();
            FadeTickMax = fadeTick;
            FadeTick = fadeTick;
            Color = color;
        }

        ~ObjectSmoke()
        {
            DrawPool.BackBrush(_Color);
        }

        public override void Action()
        {
            FadeTick--;
            if (FadeTick == 0)
            {
                Size--;
                if (Size == 0)
                {
                    Kill();
                }
                else
                {
                    FadeTick = FadeTickMax;
                }
            }
        }

        public override void DrawSelf(Graphics g)
        {
            g.FillEllipse(_Brush, Rectangle);
        }
    }
}
