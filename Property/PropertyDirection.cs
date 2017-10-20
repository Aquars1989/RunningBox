using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 顯示移動方向
    /// </summary>
    class PropertyShowMoveAngle : PropertyBase
    {
        /// <summary>
        /// 繪製顏色
        /// </summary>
        private Color _MainColor;

        /// <summary>
        /// 繪製物件
        /// </summary>
        private DrawColors _DrawColor;

        /// <summary>
        /// 新增顯示移動方向特性
        /// </summary>
        /// <param name="duration">持續時間(毫秒),小於0為永久</param>
        public PropertyShowMoveAngle(int duration)
        {
            DurationTime.Limit = duration;
            _DrawColor = new DrawColors() { Opacity = 0.2F };
        }

        public override void DoBeforeDraw(Graphics g)
        {
            GetDrawObject();
            SolidBrush brush = _DrawColor.GetBrush("Main");

            double angle = Function.GetAngle(0, 0, Owner.MoveObject.MoveX, Owner.MoveObject.MoveY);

            PointF pot1 = Function.GetOffsetPoint(Owner.Layout.CenterX, Owner.Layout.CenterY, angle, Owner.Layout.Width + 6);
            PointF pot2 = Function.GetOffsetPoint(pot1.X, pot1.Y, angle - 140, 8);
            PointF pot3 = Function.GetOffsetPoint(pot1.X, pot1.Y, angle + 140, 8);

            g.FillPolygon(brush, new PointF[] { pot2, pot1, pot3 });
            base.DoBeforeDraw(g);
        }

        private void GetDrawObject()
        {
            if (_MainColor != Owner.DrawObject.MainColor)
            {
                _DrawColor.SetColor("Main", Owner.DrawObject.MainColor);
                _MainColor = Owner.DrawObject.MainColor;
            }
        }

        public override void DoBeforeEnd(PropertyEndType endType)
        {
            _DrawColor.Dispose();
            base.DoBeforeEnd(endType);
        }
    }
}
