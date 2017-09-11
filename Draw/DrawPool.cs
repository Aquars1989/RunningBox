using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    class DrawPool
    {
        private static Dictionary<Color, DrawPoolBrush> _BrushPool = new Dictionary<Color, DrawPoolBrush>();
        private static Dictionary<Color, DrawPoolPen> _PenPool = new Dictionary<Color, DrawPoolPen>();

        public static int BrushCount
        {
            get { return _BrushPool.Count; }
        }

        public static int PenCount
        {
            get { return _PenPool.Count; }
        }

        public static SolidBrush GetBrush(Color color)
        {
            DrawPoolBrush drawPoolBrush;
            if (_BrushPool.TryGetValue(color, out drawPoolBrush))
            {
                drawPoolBrush.UseCount++;
                return drawPoolBrush.Brush;
            }
            else
            {
                drawPoolBrush = new DrawPoolBrush(new SolidBrush(color), 1);
                _BrushPool.Add(color, drawPoolBrush);
                return drawPoolBrush.Brush;
            }
        }

        public static Pen GetPen(Color color)
        {
            DrawPoolPen drawPoolPen;
            if (_PenPool.TryGetValue(color, out drawPoolPen))
            {
                drawPoolPen.UseCount++;
                return drawPoolPen.Pen;
            }
            else
            {
                drawPoolPen = new DrawPoolPen(new Pen(color), 1);
                _PenPool.Add(color, drawPoolPen);
                return drawPoolPen.Pen;
            }
        }

        public static void BackBrush(SolidBrush brush)
        {
            BackBrush(brush.Color);
        }

        public static void BackBrush(Color color)
        {
            DrawPoolBrush drawPoolBrush;
            if (_BrushPool.TryGetValue(color, out drawPoolBrush))
            {
                drawPoolBrush.UseCount--;
                if (drawPoolBrush.UseCount == 0)
                {
                    _BrushPool.Remove(color);
                    drawPoolBrush.Dispose();
                }
            }
        }

        public static void BackPen(Pen pen)
        {
            BackPen(pen.Color);

        }
        public static void BackPen(Color color)
        {
            DrawPoolPen drawPoolPen;
            if (_PenPool.TryGetValue(color, out drawPoolPen))
            {
                drawPoolPen.UseCount--;
                if (drawPoolPen.UseCount == 0)
                {
                    _PenPool.Remove(color);
                    drawPoolPen.Dispose();
                }
            }
        }
    }

    public class DrawPoolBrush : IDisposable
    {
        public SolidBrush Brush { get; private set; }
        public int UseCount { get; set; }

        public DrawPoolBrush(SolidBrush brush, int useCount)
        {
            Brush = brush;
            UseCount = useCount;
        }

        public void Dispose()
        {
            Brush.Dispose();
        }
    }

    public class DrawPoolPen : IDisposable
    {
        public Pen Pen { get; private set; }
        public int UseCount { get; set; }

        public DrawPoolPen(Pen pen, int useCount)
        {
            Pen = pen;
            UseCount = useCount;
        }

        public void Dispose()
        {
            Pen.Dispose();
        }
    }
}
