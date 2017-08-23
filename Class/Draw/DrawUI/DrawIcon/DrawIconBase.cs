using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 圖示繪圖基本物件
    /// </summary>
    public abstract class DrawIconBase : DrawUI
    {
        private SolidBrush _BrushChanneled = new SolidBrush(Color.FromArgb(200, 230, 140));
        private GraphicsPath _BackFrame;
        private Rectangle _BackFrameRectangle;
        private int _Animation;
        /// <summary>
        /// 綁定技能物件
        /// </summary>
        public SkillBase BindingSkill { get; set; }

        /// <summary>
        /// 是否顯示熱鍵圖示
        /// </summary>
        public EnumSkillButton DrawButton { get; set; }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public override void Draw(Graphics g, Rectangle rectangle)
        {
            Pen pen = GetPen();
            SolidBrush brush = GetBrush();
            GraphicsPath backFrame = GetBackFrame(rectangle);

            pen.Width = 2;
            if (BindingSkill != null)
            {
                switch (BindingSkill.Status)
                {
                    case SkillStatus.Disabled:
                        if (BindingSkill.Owner != null && BindingSkill.Owner.Energy < BindingSkill.CostEnergy)
                        {
                            g.FillRectangle(Brushes.LightPink, rectangle);
                        }
                        else
                        {
                            using (LinearGradientBrush brush2 = new LinearGradientBrush(rectangle, Color.FromArgb(255, 255, 200), Color.FromArgb(255, 255, 240), 315))
                            {
                                g.FillRectangle(brush2, rectangle);
                            }
                        }
                        break;
                    case SkillStatus.Cooldown:
                        float cooldownSize = (float)(BindingSkill.CooldownLimit - BindingSkill.CooldownTicks) / BindingSkill.CooldownLimit * rectangle.Height;
                        g.FillRectangle(Brushes.AliceBlue, rectangle);
                        g.FillRectangle(Brushes.LightSlateGray, rectangle.X, rectangle.Y + rectangle.Height - cooldownSize, rectangle.Width, cooldownSize);
                        break;
                    case SkillStatus.Channeled:
                        if (BindingSkill.ChanneledLimit < 0)
                        {
                            if (_Animation > 20)
                            {
                                _Animation %= 20;
                            }
                            int angle = _Animation * 18;
                            using (LinearGradientBrush brush2 = new LinearGradientBrush(rectangle, Color.FromArgb(140, 200, 255), Color.FromArgb(245, 255, 240), angle))
                            {
                                g.FillRectangle(brush2, rectangle);
                            }
                            _Animation++;
                        }
                        else
                        {
                            float channeledSize = (float)(BindingSkill.ChanneledLimit - BindingSkill.ChanneledTicks) / BindingSkill.ChanneledLimit * rectangle.Height;
                            g.FillRectangle(Brushes.White, rectangle);
                            g.FillRectangle(_BrushChanneled, rectangle.X, rectangle.Y + rectangle.Height - channeledSize, rectangle.Width, channeledSize);
                        }
                        break;
                }
            }
            g.DrawPath(pen, backFrame);
            DrawIcon(pen, brush, g, rectangle);

            if (DrawButton != RunningBox.EnumSkillButton.None)
            {
                pen.Width = 2;
                Rectangle keyRectangle = new Rectangle(rectangle.Left + rectangle.Width - 15, rectangle.Top + rectangle.Height - 15, 20, 25);
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
                g.DrawEllipse(pen, keyRectangle);
                pen.Width = 1;
                g.DrawLine(pen, keyRectangle.Left, keyRectangle.Top + keyRectangle.Height / 2, keyRectangle.Left + keyRectangle.Width, keyRectangle.Top + keyRectangle.Height / 2);
                g.DrawLine(pen, keyRectangle.Left + keyRectangle.Width / 2, keyRectangle.Top, keyRectangle.Left + keyRectangle.Width / 2, keyRectangle.Top + keyRectangle.Height / 2);
            }
        }

        /// <summary>
        /// 繪製圖示內容
        /// </summary>
        /// <param name="pen">畫筆物件</param>
        /// <param name="brush">筆刷物件</param>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected abstract void DrawIcon(Pen pen, SolidBrush brush, Graphics g, Rectangle rectangle);

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
    }
}
