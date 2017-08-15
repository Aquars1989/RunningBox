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
        /// <summary>
        /// 綁定技能物件
        /// </summary>
        public SkillBase BindingSkill { get; set; }

        private GraphicsPath _BackRound;
        private Rectangle _BackRoundRectangle;

        /// <summary>
        /// 是否顯示熱鍵圖示
        /// </summary>
        public SkillButton DrawButton { get; set; }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public override void Draw(Graphics g, Rectangle rectangle)
        {
            Pen pen = GetPen();
            SolidBrush brush = GetBrush();
            GraphicsPath backRound = GetBackRound(rectangle);

            pen.Width = 2;
            if (BindingSkill != null)
            {
                switch (BindingSkill.Status)
                {
                    case SkillStatus.Disabled:
                        if (BindingSkill.Owner != null && BindingSkill.Owner.Energy < BindingSkill.CostEnargy)
                        {
                            g.FillRectangle(Brushes.AliceBlue, rectangle);
                        }
                        else
                        {
                            using (LinearGradientBrush brush2 = new LinearGradientBrush(rectangle, Color.LightYellow, Color.FromArgb(255, 255, 240), 315))
                            {
                                g.FillRectangle(brush2, rectangle);
                            }
                        }
                        break;
                    case SkillStatus.Cooldown:
                        float cooldownSize = (float)(BindingSkill.CooldownRoundMax - BindingSkill.CooldownRound) / BindingSkill.CooldownRoundMax * rectangle.Height;
                        g.FillRectangle(Brushes.AliceBlue, rectangle);
                        g.FillRectangle(Brushes.LightSlateGray, rectangle.X, rectangle.Y + rectangle.Height - cooldownSize, rectangle.Width, cooldownSize);
                        break;
                    case SkillStatus.Channeled:
                        if (BindingSkill.ChanneledRoundMax < 0)
                        {
                            using (LinearGradientBrush brush2 = new LinearGradientBrush(rectangle, Color.AliceBlue, Color.FromArgb(240, 255, 240), 315))
                            {
                                g.FillRectangle(brush2, rectangle);
                            }
                        }
                        else
                        {
                            float channeledSize = (float)(BindingSkill.ChanneledRoundMax - BindingSkill.ChanneledRound) / BindingSkill.ChanneledRoundMax * rectangle.Height;
                            g.FillRectangle(Brushes.White, rectangle);
                            g.FillRectangle(Brushes.LightSkyBlue, rectangle.X, rectangle.Y + rectangle.Height - channeledSize, rectangle.Width, channeledSize);
                        }
                        break;
                }
            }
            g.DrawPath(pen, backRound);
            DrawIcon(pen, brush, g, rectangle);

            if (DrawButton != RunningBox.SkillButton.None)
            {
                pen.Width = 2;
                Rectangle keyRectangle = new Rectangle(rectangle.Left + rectangle.Width - 15, rectangle.Top + rectangle.Height - 15, 20, 25);
                g.FillEllipse(Brushes.White, keyRectangle);

                switch (DrawButton)
                {
                    case RunningBox.SkillButton.MouseButtonLeft:
                        g.FillPie(Brushes.SkyBlue, keyRectangle, 180, 90);

                        break;
                    case RunningBox.SkillButton.MouseButtonRight:
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
        private GraphicsPath GetBackRound(Rectangle rectangle)
        {
            if (_BackRoundRectangle != rectangle && _BackRound != null)
            {
                _BackRound.Dispose();
                _BackRound = null;
            }

            if (_BackRound == null)
            {
                int width = rectangle.Width;
                int height = rectangle.Height;

                _BackRoundRectangle = rectangle;
                _BackRound = new GraphicsPath();

                int matrixRound = 8;
                //頂端
                _BackRound.AddLine(rectangle.Left + (matrixRound / 2), rectangle.Top, rectangle.Right - matrixRound, rectangle.Top);
                //roundRect.AddLine(rect.Left + radius - 1, rect.Top - 1, rect.Right - radius, rect.Top - 1);
                //右上角
                _BackRound.AddArc(rectangle.Right - matrixRound, rectangle.Top, matrixRound, matrixRound, 270, 90);
                //右邊
                _BackRound.AddLine(rectangle.Right, rectangle.Top + matrixRound, rectangle.Right, rectangle.Bottom - matrixRound);
                //右下角
                _BackRound.AddArc(rectangle.Right - matrixRound, rectangle.Bottom - matrixRound, matrixRound, matrixRound, 0, 90);
                //底邊
                _BackRound.AddLine(rectangle.Right - matrixRound, rectangle.Bottom, rectangle.Left + matrixRound, rectangle.Bottom);
                //左下角
                _BackRound.AddArc(rectangle.Left, rectangle.Bottom - matrixRound, matrixRound, matrixRound, 90, 90);
                //左邊
                _BackRound.AddLine(rectangle.Left, rectangle.Bottom - matrixRound, rectangle.Left, rectangle.Top + matrixRound);
                //左上角
                _BackRound.AddArc(rectangle.Left, rectangle.Top, matrixRound, matrixRound, 180, 90);
            }

            return _BackRound;
        }
    }
}
