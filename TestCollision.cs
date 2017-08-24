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
    public partial class TestCollision : Form
    {
        bool check = false;
        Rectangle rectangel1 = new Rectangle(100, 100, 100, 50);
        Rectangle rectangel2 = new Rectangle(300, 100, 100, 50);
        Rectangle rectangel3 = new Rectangle(0, 0, 50, 100);
        public TestCollision()
        {
            InitializeComponent();
        }

        private void TestCollision_Paint(object sender, PaintEventArgs e)
        {
            bool collision1 = !check ? false : Function.IsCollison(ShapeType.Ellipse, rectangel3, ShapeType.Rectangle, rectangel1);
            bool collision2 = !check ? false : Function.IsCollison(ShapeType.Ellipse, rectangel3, ShapeType.Ellipse, rectangel2);
            e.Graphics.FillRectangle(collision1 ? Brushes.Red : Brushes.Black, rectangel1);
            e.Graphics.FillEllipse(collision2 ? Brushes.Red : Brushes.Black, rectangel2);
            e.Graphics.FillEllipse(check ? Brushes.Blue: Brushes.AliceBlue, rectangel3);
        }

        private void TestCollision_MouseMove(object sender, MouseEventArgs e)
        {
            rectangel3.Location = new Point(e.X - rectangel3.Width / 2, e.Y - rectangel3.Height / 2);
            Invalidate();
        }

        private void TestCollision_MouseDown(object sender, MouseEventArgs e)
        {
            check = e.Button == MouseButtons.Left;
        }
    }
}
