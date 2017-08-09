using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 虛擬物件,朝特定方向移動並淡出
    /// </summary>
    public class ObjectScrap : ObjectBase
    {
        /// <summary>
        /// 生命計時器
        /// </summary>
        public int LifeTick { get; set; }

        /// <summary>
        /// 生命計時器最大值
        /// </summary>
        public int LifeTickMax { get; set; }

        /// <summary>
        /// 移動速度
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public double Direction { get; set; }

        private Color _Color;
        /// <summary>
        /// 顏色
        /// </summary>
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color == value) return;
                _Color = value;
            }
        }

        /// <summary>
        /// 建立虛擬物件,朝特定方向移動並淡出
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="size">物件大小</param>
        /// <param name="speed">速度</param>
        /// <param name="life">持續回合數</param>
        /// <param name="direction">方向</param>
        /// <param name="color">顏色</param>
        public ObjectScrap(float x, float y, int size, float speed, int life, double direction, Color color)
        {
            Status = ObjectStatus.Dying;
            X = x;
            Y = y;
            Size = size;
            Speed = speed;
            LifeTickMax = life;
            Direction = direction;
            Color = color;
        }

        public override void Action()
        {
            if (LifeTick >= LifeTickMax)
            {
                Kill(null);
            }
            else
            {
                float moveX = (float)Math.Cos(Direction / 180 * Math.PI) * (Speed / 100F);
                float moveY = (float)Math.Sin(Direction / 180 * Math.PI) * (Speed / 100F);
                X += moveX;
                Y += moveY;
            }
            LifeTick++;
        }

        public override void Draw(Graphics g)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb((int)(255F / LifeTickMax * (LifeTickMax - LifeTick)), Color.R, Color.G, Color.B)))
            {
                g.FillEllipse(brush, Rectangle);
            }
        }
    }
}
