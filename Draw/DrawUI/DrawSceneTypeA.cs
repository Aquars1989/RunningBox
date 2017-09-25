using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 生存100秒群組繪製物件
    /// </summary>
    public class DrawSceneTypeA : DrawBase
    {
        private int _Quadrant = 2;
        private DrawBall _DrawPlayer;
        private List<DrawBall> _DrawEmemy = new List<DrawBall>();

        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Colors.GetColor("Main"); }
        }

        /// <summary>
        /// 動畫進度
        /// </summary>
        public int Animation { get; set; }

        /// <summary>
        /// 新增生存100秒群組繪製物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        public DrawSceneTypeA(DrawColors drawColor)
            : base(drawColor)
        {
        }

        /// <summary>
        /// 新增生存100秒群組繪製物件
        /// </summary>
        /// <param name="defaultColor">預設顏色</param>
        public DrawSceneTypeA(Color defaultColor)
        {
            Colors.SetColor("Player", defaultColor);
            Colors.SetColor("Ememy", defaultColor);
            Colors.SetColor("Shadow", Color.LightGray);
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            if (_DrawPlayer == null)
                _DrawPlayer = new DrawBall(Scene.Sec(0.2F));

            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            Pen penPlayer = Colors.GetPen("Player");
            Pen penShadow = Colors.GetPen("Shadow");
            SolidBrush brushEmemy = Colors.GetBrush("Ememy");
            SolidBrush brushShadow = Colors.GetBrush("Shadow");
            penPlayer.Width = penShadow.Width = drawRectangle.Width * 0.04F;
            _DrawPlayer.Animation.Value += Scene.IntervalOfRound;
            _DrawEmemy.RemoveAll((x) => { return x.Animation.IsFull; });
            foreach (DrawBall drawEmemy in _DrawEmemy)
            {
                drawEmemy.Animation.Value += Scene.IntervalOfRound;
            }


            if (_DrawPlayer.Animation.IsFull)
            {
                int oldQuadrant = _Quadrant;
                do
                {
                    _Quadrant = Global.Rand.Next(1, 4);
                } while (_Quadrant == oldQuadrant);

                _DrawPlayer.BaseX = _DrawPlayer.TargetX;
                _DrawPlayer.BaseY = _DrawPlayer.TargetY;
                _DrawPlayer.TargetX = (float)((_Quadrant == 1 || _Quadrant == 4 ? 0.7 : 0.3) + Global.Rand.NextDouble() * 0.2 - 0.1);
                _DrawPlayer.TargetY = (float)((_Quadrant == 1 || _Quadrant == 2 ? 0.3 : 0.7) + Global.Rand.NextDouble() * 0.2 - 0.1);
                _DrawPlayer.Animation.Value = 0;

                DrawBall newEmemy = new DrawBall(Scene.Sec(0.5F));
                bool enterDirection = Global.Rand.Next(2) == 1;
                switch (oldQuadrant + _Quadrant)
                {
                    case 5: //上下
                        newEmemy.BaseX = enterDirection ? -0.1F : 1.1F;
                        newEmemy.BaseY = _DrawPlayer.BaseY + (float)(Global.Rand.NextDouble() * 0.5 - 0.25);
                        break;
                    case 3://左右
                    case 7:
                        newEmemy.BaseX = _DrawPlayer.BaseX + (float)(Global.Rand.NextDouble() * 0.5 - 0.25);
                        newEmemy.BaseY = enterDirection ? -0.1F : 1.1F;
                        break;
                    case 4://右上左下
                        if (Global.Rand.Next(2) == 1)
                        {
                            newEmemy.BaseX = enterDirection ? (float)Global.Rand.NextDouble() / 2 : -0.1F;
                            newEmemy.BaseY = enterDirection ? -0.1F : (float)Global.Rand.NextDouble() / 2;
                        }
                        else
                        {
                            newEmemy.BaseX = enterDirection ? 0.5F + (float)Global.Rand.NextDouble() / 2 : 1.1F;
                            newEmemy.BaseY = enterDirection ? 1.1F : 0.5F + (float)Global.Rand.NextDouble() / 2;
                        }
                        break;
                    case 6://左上右下
                        if (Global.Rand.Next(2) == 1)
                        {
                            newEmemy.BaseX = enterDirection ? 0.5F + (float)Global.Rand.NextDouble() / 2 : 1.1F;
                            newEmemy.BaseY = enterDirection ? -0.1F : (float)Global.Rand.NextDouble() / 2;
                        }
                        else
                        {
                            newEmemy.BaseX = enterDirection ? (float)Global.Rand.NextDouble() / 2 : -0.1F;
                            newEmemy.BaseY = enterDirection ? 1.1F : 0.5F + (float)Global.Rand.NextDouble() / 2;
                        }
                        break;
                }
                double distanceMax = Math.Sqrt(2);
                double angle = Function.GetAngle(newEmemy.BaseX, newEmemy.BaseY, _DrawPlayer.BaseX, _DrawPlayer.BaseY);
                PointF offset2 = Function.GetOffsetPoint(0, 0, angle, distanceMax);
                newEmemy.TargetX = newEmemy.BaseX + offset2.X;
                newEmemy.TargetY = newEmemy.BaseY + offset2.Y;
                _DrawEmemy.Add(newEmemy);
            }

            float width = drawRectangle.Width;
            float hetight = drawRectangle.Height;
            float ballWidth = Math.Min(width, hetight) * 0.1F;
            float ballWidth2 = Math.Min(width, hetight) * 0.08F;
            float drawX = drawRectangle.Left + (_DrawPlayer.BaseX + (_DrawPlayer.TargetX - _DrawPlayer.BaseX) * _DrawPlayer.Animation.GetRatio()) * width - ballWidth / 2;
            float drawY = drawRectangle.Top + (_DrawPlayer.BaseY + (_DrawPlayer.TargetY - _DrawPlayer.BaseY) * _DrawPlayer.Animation.GetRatio()) * width - ballWidth / 2;
            g.DrawEllipse(penShadow, drawX + 3, drawY + 4, ballWidth, ballWidth);
            g.DrawEllipse(penPlayer, drawX, drawY, ballWidth, ballWidth);

            foreach (DrawBall drawEmemy in _DrawEmemy)
            {
                float drawX2 = drawRectangle.Left + (drawEmemy.BaseX + (drawEmemy.TargetX - drawEmemy.BaseX) * drawEmemy.Animation.GetRatio()) * width - ballWidth2 / 2;
                float drawY2 = drawRectangle.Top + (drawEmemy.BaseY + (drawEmemy.TargetY - drawEmemy.BaseY) * drawEmemy.Animation.GetRatio()) * hetight - ballWidth2 / 2;
                g.FillEllipse(brushShadow, drawX2 + 4, drawY2 + 6, ballWidth2, ballWidth2);
                g.FillEllipse(brushEmemy, drawX2, drawY2, ballWidth2, ballWidth2);
            }
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件(不包含內部物件,未綁定物件)
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawSceneTypeA(Colors.Copy())
            {
                Scale = this.Scale,
                Angle = this.Angle,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled
            };
        }

        private class DrawBall
        {
            public CounterObject Animation { get; set; }
            public float BaseX { get; set; }
            public float BaseY { get; set; }
            public float TargetX { get; set; }
            public float TargetY { get; set; }

            public DrawBall(int amimation)
            {
                Animation = new CounterObject(amimation);
            }
        }
    }
}
