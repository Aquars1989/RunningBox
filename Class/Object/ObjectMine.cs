using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public class ObjectMine : ObjectBase
    {
        public ITarget Target { get; set; }
        public int LifeTick { get; set; }
        public int LifeTickMax { get; set; }

        private Image _Image;
        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                _Image = value;
            }
        }

        public ObjectMine(float x, float y, int maxMoves, int size, float speed, int life, Image image, ITarget target)
        {
            Status = ObjectStatus.Alive;
            MaxMoves = maxMoves;
            X = x;
            Y = y;
            Size = size;
            Speed = speed;
            LifeTickMax = life;
            LifeTick = life;
            Moves = new List<PointF>();
            Image = image;
            Target = target;
        }

        public override void Action()
        {
            LifeTick--;
            switch (Status)
            {
                case ObjectStatus.Alive:
                    ObjectBase playObject = Scene.PlayerObject;
                    if (playObject != null && playObject.Rectangle.IntersectsWith(Rectangle))
                    {
                        this.Kill(null);
                        playObject.Kill(this);
                        return;
                    }

                    if (Target != null)
                    {
                        if (Moves.Count >= MaxMoves)
                        {
                            Moves.RemoveAt(0);
                        }

                        double direction = Function.PointRotation(X, Y, Target.X, Target.Y);
                        float moveX = (float)Math.Cos(direction / 180 * Math.PI) * (Speed / 100F);
                        float moveY = (float)Math.Sin(direction / 180 * Math.PI) * (Speed / 100F);
                        Moves.Add(new PointF((float)moveX, (float)moveY));
                    }

                    float moveTotalX = 0;
                    float moveTotalY = 0;
                    foreach (PointF pt in Moves)
                    {
                        moveTotalX += pt.X;
                        moveTotalY += pt.Y;
                    }

                    X += moveTotalX * Scene.WorldSpeed;
                    Y += moveTotalY * Scene.WorldSpeed;

                    if (playObject != null && playObject.Rectangle.IntersectsWith(Rectangle))
                    {
                        this.Kill(null);
                        playObject.Kill(this);
                        return;
                    }

                    if (LifeTick <= 0)
                    {
                        Kill(null);
                    }
                    break;
            }
        }

        public override void DrawSelf(Graphics g)
        {
            switch (Status)
            {
                case ObjectStatus.Alive:
                    Rectangle rectDraw;
                    if (LifeTick < 20)
                    {
                        int sizeFix = LifeTick / 2 % 5;
                        rectDraw = new Rectangle(Rectangle.Left - sizeFix, Rectangle.Top - sizeFix, Rectangle.Width + sizeFix * 2, Rectangle.Height + sizeFix * 2);
                    }
                    else
                    {
                        rectDraw = Rectangle;
                    }
                    g.DrawImage(Image, Rectangle);
                    break;
            }
        }

        public override void Kill(ObjectBase killer)
        {
            if (Status != ObjectStatus.Alive) return;

            float moveTotalX = 0;
            float moveTotalY = 0;
            foreach (PointF pt in Moves)
            {
                moveTotalX += pt.X;
                moveTotalY += pt.Y;
            }

            double direction = Function.PointRotation(0, 0, moveTotalX, moveTotalY);
            for (int i = 0; i < 15; i++)
            {
                int speed = Global.Rand.Next(300, 900);
                int life = Global.Rand.Next(25, 40);
                int size = Global.Rand.Next(1, 4) / 2;
                double scrapDirection = direction + (Global.Rand.NextDouble() - 0.5) * 20;
                Scene.GameObjects.Add(new ObjectScrap(X, Y, 1, speed, life, scrapDirection, Color));
            }
            Scene.EffectObjects.Add(new EffectShark(20, 10) { CanBreak = false });

            base.Kill(killer);
        }
    }
}
