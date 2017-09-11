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
        private Timer _TimerOfEnter = new Timer() { Interval = 1000 };

        private static string[] _StringPoint =
        {
            " 0 000  0       000      0    00",
            "  0   0  0      0  0      0  0  ",
            "  0   0  0      0   0      00   ",
            "  0000   0      000000     0    ",
            "  0      0   0  0    0     0    ",
            " 0      00000  0      00  0     ",
            "0                        0      "
        };


        public SceneWelcome()
        {
            InitializeComponent();

            _TimerOfEnter.Tick += (x, e) =>
            {
                _TimerOfEnter.Enabled = false;
                OnGoScene(new SceneSkill());
            };
        }

        protected override void OnLoadComplete()
        {
            base.OnLoadComplete();

            int drawWidth = 400;
            int drawHeight = 150;
            int drawLeft = (Width - drawWidth) / 2;
            int drawTop = (Height - drawHeight) / 2;
            int partCountX = _StringPoint[0].Length;
            int partCountY = _StringPoint.Length;
            int partWidth = drawWidth / partCountX;
            int partHeight = drawHeight / partCountY;
            float delay = 0;
            int[,] map = new int[partCountX, partCountY];
            //建立關連圖
            for (int x = 0; x < partCountX; x++)
            {
                for (int y = 0; y < partCountY; y++)
                {
                    if (_StringPoint[y][x] == '0')
                    {
                        bool checkRight = x + 1 < partCountX;
                        bool checkBottom = y + 1 < partCountY;
                        bool checkLeft = x - 1 >= 0;

                        if (checkRight)
                        {
                            map[x + 1, y] |= 1; //右
                        }

                        if (checkBottom)
                        {
                            map[x, y + 1] |= 2; //下
                        }

                        if (checkRight && checkBottom)
                        {
                            map[x + 1, y + 1] |= 4; //右下
                        }

                        if (checkLeft && checkBottom)
                        {
                            map[x - 1, y + 1] |= 8; //左下
                        }
                    }
                    else
                    {
                        map[x, y] = -1;
                    }
                }
            }

            byte[] linkIdx = { 0, 1, 2, 4, 8 };
            int[] linkOffsetX = { 0, -1, 0, -1, 1 };
            int[] linkOffsetY = { 0, 0, -1, -1, -1 };
            for (int x = 0; x < partCountX; x++)
            {
                for (int y = 0; y < partCountY; y++)
                {
                    int mapValue = map[x, y];
                    if (mapValue >= 0)
                    {
                        Point enterPoint = GetEnterPoint();
                        int size = Global.Rand.Next(6, 8);
                        int speed = Global.Rand.Next(1800, 2200);
                        PointF targetPoint = new PointF(drawLeft + x * partWidth, drawTop + y * partHeight);
                        TargetPoint target = new TargetPoint(targetPoint);
                        MoveStraight moveObject = new MoveStraight(target, 1, speed, 1, 100, 1F);
                        ObjectActive bigBall = new ObjectActive(enterPoint.X, enterPoint.Y, size, size, -1, LeagueType.None, ShapeType.Ellipse, new DrawPen(Color.FromArgb(150, 0, 0, 0), ShapeType.Ellipse, 4), moveObject);
                        bigBall.Propertys.Add(new PropertyFreeze(Sec(delay)));
                        GameObjects.Add(bigBall);

                        for (int i = 0; i < linkIdx.Length; i++)
                        {
                            if ((mapValue & linkIdx[i]) == linkIdx[i])
                            {
                                int smallballCount = Global.Rand.Next(2, 3);
                                for (int n = 0; n < smallballCount; n++)
                                {
                                    Point enterPoint2 = GetEnterPoint();
                                    int size2 = Global.Rand.Next(3, 8);
                                    int speed2 = Global.Rand.Next(1400, 1600);
                                    float fx = 1 - size2 * 0.02F;
                                    int targetFixX = (partWidth * linkOffsetX[i] / 2) + (int)((0.5F - Global.Rand.NextDouble()) * partWidth * fx + 0.5F);
                                    int targetFixY = (partHeight * linkOffsetY[i] / 2) + (int)((0.5F - Global.Rand.NextDouble()) * partHeight * fx + 0.5F);
                                    TargetPoint target2 = new TargetPoint((int)(targetPoint.X) + targetFixX, (int)(targetPoint.Y) + targetFixY);
                                    MoveStraight moveObject2 = new MoveStraight(target2,1, speed2, 1, 100, 1F);
                                    Color color2 = Color.FromArgb(Global.Rand.Next(200), Global.Rand.Next(200), Global.Rand.Next(200));
                                    ObjectActive smallBall = new ObjectActive(enterPoint2.X, enterPoint2.Y, size2, size2, -1, LeagueType.None, ShapeType.Ellipse, new DrawBrush(color2, ShapeType.Ellipse), moveObject2);
                                    smallBall.Propertys.Add(new PropertyFreeze(Sec(0.75F + (float)Global.Rand.NextDouble() * 0.5F)));
                                    GameObjects.Add(smallBall);
                                }
                            }
                        }

                    }
                    delay += 0.003F;
                }
            }
        }


        private void SceneWelcome_MouseDown(object sender, MouseEventArgs e)
        {

            EffectObjects.Add(new EffectShark(Sec(0.8F), 10));
            int ex = e.X;
            int ey = e.Y;
            for (int i = 0; i < GameObjects.Count; i++)
            {
                ObjectActive objectActive = GameObjects[i] as ObjectActive;
                if (objectActive != null)
                {
                    double distance = Function.GetDistance(e.X, e.Y, objectActive.Layout.CenterX, objectActive.Layout.CenterY);
                    if (distance < 100)
                    {
                        //objectActive.Life.Value = 0;
                        //objectActive.Life.Limit = Sec(0.05F);
                        objectActive.Propertys.Add(new PropertyDeadBroken(5, 2, 2, ObjectDeadType.Collision, 360, 200, 400, Sec(0.8F), Sec(1.4F)));
                        objectActive.Kill(null, ObjectDeadType.Collision);
                    }
                    else
                    {
                        double angle = Function.GetAngle(e.X, e.Y, objectActive.Layout.CenterX, objectActive.Layout.CenterY);
                        PointF targetPoint = Function.GetOffsetPoint(objectActive.Layout.CenterX, objectActive.Layout.CenterY, angle, Width + Height);
                        objectActive.MoveObject.Target = new TargetPoint(targetPoint);
                        objectActive.Propertys.Add(new PropertyFreeze(Sec(0.1F + (float)Global.Rand.NextDouble() * 0.2F)));
                    }
                    //objectActive.Speed *= 2;

                }
            }
            _TimerOfEnter.Enabled = true;
        }
    }
}
