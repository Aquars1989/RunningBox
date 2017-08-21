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
        /// 存活時間計數(毫秒)
        /// </summary>
        public int LifeTicks { get; set; }

        /// <summary>
        /// 存活時間最大值(毫秒),小於0為永久
        /// </summary>
        public int LifeLimit { get; set; }

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
        /// <param name="life">存活時間(毫秒),小於0為永久</param>
        /// <param name="direction">方向</param>
        /// <param name="color">顏色</param>
        public ObjectScrap(float x, float y, int size, float speed, int life, double direction, Color color)
        {
            Status = ObjectStatus.Alive;
            X = x;
            Y = y;
            Size = size;
            Speed = speed;
            LifeLimit = life;
            Direction = direction;
            Color = color;
        }

        public override void Action()
        {
            if (LifeLimit >= 0 && LifeTicks >= LifeLimit)
            {
                Kill(null, ObjectDeadType.LifeEnd);
            }
            else
            {
                PointF move = GetMovePoint(Direction, Speed);
                X += move.X;
                Y += move.Y;
            }
            LifeTicks += Scene.SceneIntervalOfRound;
        }

        public override void Draw(Graphics g)
        {
            int alpha = (int)(255F / LifeLimit * (LifeLimit - LifeTicks));
            if (alpha < 0) alpha = 0;
            else if (alpha > 255) alpha = 255;

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, Color.R, Color.G, Color.B)))
            {
                g.FillEllipse(brush, Rectangle);
            }
        }
    }
}
