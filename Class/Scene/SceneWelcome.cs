using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace RunningBox
{
    public partial class SceneWelcome : SceneBase
    {
        private string[] _StringPoint =
        {
            " 0 000   0      000      0    00",
            "  0   0   0     0  0      0  0  ",
            "  0   0   0     0   0      00   ",
            "  0000    0     000000     0    ",
            "  0      0   0  0    0     0    ",
            " 0      00000  0      00  0     "
        };


        public SceneWelcome()
        {
            InitializeComponent();
        }

        private void SceneWelcome_Load(object sender, EventArgs e)
        {
            int drawWidth = 400;
            int drawHeight = 150;
            int drawLeft = (Width - drawWidth) / 2;
            int drawTop = (Height - drawHeight) / 2;
            int partCountX = _StringPoint[0].Length;
            int partCountY = _StringPoint.Length;
            int partWidth = drawWidth / partCountX;
            int partHeight = drawHeight / partCountY;

            for (int x = 0; x < partCountX; x++)
            {
                for (int y = 0; y < partCountY; y++)
                {
                    if (_StringPoint[y][x] == '0')
                    {
                        Point enterPoint = GetEnterPoint();
                        int size = Global.Rand.Next(6, 8);
                        int speed = Global.Rand.Next(100, 300);
                        TargetPoint targetPoint = new TargetPoint(drawLeft + x * partWidth, drawTop + y * partHeight);
                        GameObjects.Add(new ObjectActive(enterPoint.X, enterPoint.Y, 1, size, size, speed, -1, League.None, new DrawPen(Color.FromArgb(150, 0, 0, 0), ShapeType.Ellipse, 4), targetPoint));

                        int smallballCount = Global.Rand.Next(4, 8);
                        for (int i = 0; i < smallballCount; i++)
                        {
                            Point enterPoint2 = GetEnterPoint();
                            int size2 = Global.Rand.Next(4, 6);
                            int speed2 = Global.Rand.Next(100, 300);
                            int targetFixX = (int)((0.5F - Global.Rand.NextDouble()) * partWidth * 0.9F + 0.5F);
                            int targetFixY = (int)((0.5F - Global.Rand.NextDouble()) * partHeight * 0.9F + 0.5F);
                            TargetPoint targetPoint2 = new TargetPoint((int)(targetPoint.X) + targetFixX, (int)(targetPoint.Y) + targetFixY);
                            Color color2 = Color.FromArgb(Global.Rand.Next(200), Global.Rand.Next(200), Global.Rand.Next(200));
                            GameObjects.Add(new ObjectActive(enterPoint2.X, enterPoint2.Y, 1, size2, size2, speed2, -1, League.None, new DrawBrush(color2, ShapeType.Ellipse), targetPoint2));

                        }
                    }
                }
            }
        }
    }
}
