using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    public partial class TestAngle : Form
    {
        bool check = false;
        Point point1 = new Point(200, 200);
        DrawPolygon draw1 = new DrawPolygon(Color.Blue, Color.Blue, 3, 1, 0);
        DrawPic draw2 = new DrawPic(Color.Black, Properties.Resources.Bomber, 0);
        double angle = 0;
        double angle2 = 0;

        public TestAngle()
        {
            InitializeComponent();
        }

        private void TestCollision_Paint(object sender, PaintEventArgs e)
        {
            PointF point2 = Function.GetOffsetPoint(point1.X, point1.Y, angle, 50);
            PointF point3 = Function.GetOffsetPoint(point1.X, point1.Y, angle2, 50);
            e.Graphics.DrawString(string.Format("{0:N0}", angle), Font, Brushes.Red, 5, 5);
            e.Graphics.DrawString(string.Format("{0:N0}", angle2), Font, Brushes.Blue, 5, 20);
            e.Graphics.DrawString(string.Format("{0:N0}", Function.GetRotateAngle(angle - angle2, 0)), Font, Brushes.BlueViolet, 5, 35);
            //e.Graphics.DrawString(Function.GetCross(point1, point2, point3).ToString(), Font, Brushes.BlueViolet, 5, 50);
            e.Graphics.DrawLine(Pens.Red, point1.X, point1.Y, point2.X, point2.Y);
            e.Graphics.DrawLine(Pens.Blue, point1.X, point1.Y, point3.X, point3.Y);
            draw1.Draw(e.Graphics, new Rectangle(50, 5, 40, 40));
            draw2.Draw(e.Graphics, new Rectangle(100, 5, 40, 40));
        }

        private void TestCollision_MouseMove(object sender, MouseEventArgs e)
        {
            Point point2 = e.Location;
            angle = Function.GetAngle(point1, point2);
            draw1.Angle = (int)angle;
            draw2.Angle = (int)angle;
            Invalidate();
        }

        private void TestCollision_MouseDown(object sender, MouseEventArgs e)
        {
            Point point2 = e.Location;
            angle2 = Function.GetAngle(point1, point2);
            Invalidate();
        }
    }
}
