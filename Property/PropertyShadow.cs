using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 陰影特性,物件下方產生陰影
    /// </summary>
    class PropertyShadow : PropertyBase
    {
        private DrawBase DrawObject;

        /// <summary>
        /// 新增陰影特性,物件下方產生陰影
        /// </summary>
        public PropertyShadow()
            : base(TargetNull.Value) { }

        protected override void OnOwnerChanged()
        {
            if (DrawObject != null)
            {
                DrawObject.Dispose();
            }

            DrawObject = Owner.DrawObject.Copy();
            DrawObject.Colors.Opacity = 0.25F;
            DrawObject.Colors.RFix = -1;
            DrawObject.Colors.GFix = -1;
            DrawObject.Colors.BFix = -1;
            base.OnOwnerChanged();
        }

        public override void DoBeforeDraw(Graphics g)
        {
            Rectangle drawRect = new Rectangle(Owner.Layout.Rectangle.X + 2, Owner.Layout.Rectangle.Y + 3, Owner.Layout.Rectangle.Width, Owner.Layout.Rectangle.Height);
            DrawObject.Scene = Owner.Scene;
            DrawObject.Draw(g, drawRect);
            base.DoBeforeDraw(g);
        }
    }
}
