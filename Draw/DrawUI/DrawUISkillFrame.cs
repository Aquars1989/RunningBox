using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能框架繪圖物件
    /// </summary>
    public class DrawUISkillFrame : DrawBase
    {
        public event EventHandler IconDrawObjectChanged;

        private static Pen _PenDrawButton = new Pen(Color.Black);
        private static SolidBrush _BrushChanneled = new SolidBrush(Colors.Channeled);
        private GraphicsPath _BackFrame;
        private Rectangle _BackFrameRectangle;
        private int _Animation;
        private Pen _Pen;

        private DrawSkillBase _IconDrawObject;
        /// <summary>
        /// 內部的圖示繪圖物件
        /// </summary>
        public DrawSkillBase IconDrawObject
        {
            get { return _IconDrawObject; }
            set
            {
                if (_IconDrawObject == value) return;
                _IconDrawObject = value;
                OnIconDrawObjectChanged();
            }
        }

        /// <summary>
        /// 是否顯示熱鍵圖示
        /// </summary>
        public EnumSkillButton DrawButton { get; set; }

        /// <summary>
        /// 新增技能框架繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <param name="drawButton">繪製技能熱鍵</param>
        /// <param name="iconDrawObject">技能繪製物件</param>
        public DrawUISkillFrame(Color color, EnumSkillButton drawButton, DrawSkillBase iconDrawObject = null)
        {
            Color = color;
            DrawButton = drawButton;
            IconDrawObject = iconDrawObject;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public override void Draw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            GetPen(ref _Pen, Color, Opacity, RFix, GFix, BFix);
            _Pen.Width = 2;

            GraphicsPath backFrame = GetBackFrame(drawRectangle);
            SkillBase bindingSkill = IconDrawObject == null ? null : IconDrawObject.BindingSkill;
            if (bindingSkill != null)
            {
                switch (bindingSkill.Status)
                {
                    case SkillStatus.Disabled:
                        if (bindingSkill.Owner != null && bindingSkill.Owner.Energy.Value < bindingSkill.CostEnergy)
                        {
                            g.FillRectangle(Brushes.LightPink, drawRectangle);
                        }
                        else
                        {
                            using (LinearGradientBrush brush2 = new LinearGradientBrush(drawRectangle, Color.FromArgb(255, 255, 200), Color.FromArgb(255, 255, 240), 315))
                            {
                                g.FillRectangle(brush2, drawRectangle);
                            }
                        }
                        break;
                    case SkillStatus.Cooldown:
                        float cooldownSize = (1F - bindingSkill.Cooldown.GetRatio()) * drawRectangle.Height;
                        g.FillRectangle(Brushes.AliceBlue, drawRectangle);
                        g.FillRectangle(Brushes.LightSlateGray, drawRectangle.X, drawRectangle.Y + drawRectangle.Height - cooldownSize, drawRectangle.Width, cooldownSize);
                        break;
                    case SkillStatus.Channeled:
                        if (bindingSkill.Channeled.Limit < 0)
                        {
                            if (_Animation > 20)
                            {
                                _Animation %= 20;
                            }
                            int angle = _Animation * 18;
                            using (LinearGradientBrush brush2 = new LinearGradientBrush(drawRectangle, Colors.Channeled, Color.FromArgb(245, 255, 240), angle))
                            {
                                g.FillRectangle(brush2, drawRectangle);
                            }
                            _Animation++;
                        }
                        else
                        {
                            float channeledSize = (1F - bindingSkill.Channeled.GetRatio()) * drawRectangle.Height;
                            g.FillRectangle(Brushes.White, drawRectangle);
                            g.FillRectangle(_BrushChanneled, drawRectangle.X, drawRectangle.Y + drawRectangle.Height - channeledSize, drawRectangle.Width, channeledSize);
                        }
                        break;
                }
            }
            g.DrawPath(_Pen, backFrame);
            if (IconDrawObject != null)
            {
                IconDrawObject.Draw(g, drawRectangle);
            }
            if (DrawButton != RunningBox.EnumSkillButton.None)
            {
                _PenDrawButton.Width = 2;
                Rectangle keyRectangle = new Rectangle(drawRectangle.Left + drawRectangle.Width - 15, drawRectangle.Top + drawRectangle.Height - 15, 20, 25);
                g.FillEllipse(Brushes.White, keyRectangle);

                switch (DrawButton)
                {
                    case RunningBox.EnumSkillButton.MouseButtonLeft:
                        g.FillPie(Brushes.SkyBlue, keyRectangle, 180, 90);

                        break;
                    case RunningBox.EnumSkillButton.MouseButtonRight:
                        g.FillPie(Brushes.SkyBlue, keyRectangle, 270, 90);

                        break;
                }
                g.DrawEllipse(_PenDrawButton, keyRectangle);
                _PenDrawButton.Width = 1;
                g.DrawLine(_PenDrawButton, keyRectangle.Left, keyRectangle.Top + keyRectangle.Height / 2, keyRectangle.Left + keyRectangle.Width, keyRectangle.Top + keyRectangle.Height / 2);
                g.DrawLine(_PenDrawButton, keyRectangle.Left + keyRectangle.Width / 2, keyRectangle.Top, keyRectangle.Left + keyRectangle.Width / 2, keyRectangle.Top + keyRectangle.Height / 2);
            }
        }

        /// <summary>
        /// 產生圓角區域
        /// </summary>
        private GraphicsPath GetBackFrame(Rectangle rectangle)
        {
            if (_BackFrameRectangle != rectangle && _BackFrame != null)
            {
                _BackFrame.Dispose();
                _BackFrame = null;
            }

            if (_BackFrame == null)
            {
                int width = rectangle.Width;
                int height = rectangle.Height;

                _BackFrameRectangle = rectangle;
                _BackFrame = new GraphicsPath();

                int radius = 8;
                //頂端
                _BackFrame.AddLine(rectangle.Left + radius, rectangle.Top, rectangle.Right - radius, rectangle.Top);
                //右上角
                _BackFrame.AddArc(rectangle.Right - radius, rectangle.Top, radius, radius, 270, 90);
                //右邊
                _BackFrame.AddLine(rectangle.Right, rectangle.Top + radius, rectangle.Right, rectangle.Bottom - radius);
                //右下角
                _BackFrame.AddArc(rectangle.Right - radius, rectangle.Bottom - radius, radius, radius, 0, 90);
                //底邊
                _BackFrame.AddLine(rectangle.Right - radius, rectangle.Bottom, rectangle.Left + radius, rectangle.Bottom);
                //左下角
                _BackFrame.AddArc(rectangle.Left, rectangle.Bottom - radius, radius, radius, 90, 90);
                //左邊
                _BackFrame.AddLine(rectangle.Left, rectangle.Bottom - radius, rectangle.Left, rectangle.Top + radius);
                //左上角
                _BackFrame.AddArc(rectangle.Left, rectangle.Top, radius, radius, 180, 90);
                _BackFrame.CloseAllFigures();
            }
            return _BackFrame;
        }

        public override DrawBase Copy()
        {
            return new DrawUISkillFrame(Color, DrawButton, IconDrawObject);
        }

        protected virtual void OnIconDrawObjectChanged()
        {
            if (_IconDrawObject != null)
            {
                _IconDrawObject.Scene = this.Scene;
                _IconDrawObject.Owner = this.Owner;
                _IconDrawObject.Scale = this.Scale;
            }

            if (IconDrawObjectChanged != null)
            {
                IconDrawObjectChanged(this, new EventArgs());
            }
        }

        protected override void OnColorChanged()
        {
            BackPen(ref _Pen);
            base.OnColorChanged();
        }

        protected override void OnColorFixChanged()
        {
            BackPen(ref _Pen);
            base.OnColorFixChanged();
        }

        protected override void OnOwnerChanged()
        {
            if (_IconDrawObject != null)
            {
                _IconDrawObject.Owner = this.Owner;
            }
            base.OnOwnerChanged();
        }

        protected override void OnSceneChanged()
        {
            if (_IconDrawObject != null)
            {
                _IconDrawObject.Scene = this.Scene;
            }
            base.OnSceneChanged();
        }

        protected override void OnScaleChanged()
        {
            if (_IconDrawObject != null)
            {
                _IconDrawObject.Scale = this.Scale;
            }
            base.OnScaleChanged();
        }

        protected override void OnDispose()
        {
            BackPen(ref _Pen);
        }
    }
}
