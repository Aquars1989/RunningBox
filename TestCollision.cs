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
        Rectangle rectangel1 = new Rectangle(100, 100, 100, 50);
        Rectangle rectangel2 = new Rectangle(300, 100, 100, 50);
        Rectangle rectangel3 = new Rectangle(0, 0, 50, 50);
        public TestCollision()
        {
            InitializeComponent();
        }

        private void TestCollision_Paint(object sender, PaintEventArgs e)
        {
            bool collision1 = Function.IsCollison(ShapeType.Ellipse, rectangel3, ShapeType.Rectangle, rectangel1);
            bool collision2 = Function.IsCollison(ShapeType.Ellipse, rectangel3, ShapeType.Ellipse, rectangel2);
            e.Graphics.DrawRectangle(collision1 ? Pens.Red : Pens.Black, rectangel1);
            e.Graphics.DrawEllipse(collision2 ? Pens.Red : Pens.Black, rectangel2);
            e.Graphics.DrawEllipse(Pens.Blue, rectangel3);
        }

        private void TestCollision_MouseMove(object sender, MouseEventArgs e)
        {
            rectangel3.Location = new Point(e.X - rectangel3.Width / 2, e.Y - rectangel3.Height / 2);
            Invalidate();
        }
    }
}
