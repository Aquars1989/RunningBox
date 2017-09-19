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
    public class DrawUISkillFrame : DrawUIFrame
    {
        private int _Animation;

        /// <summary>
        /// 是否忽略技能狀態
        /// </summary>
        public bool StaticMode { get; set; }

        /// <summary>
        /// 是否顯示熱鍵圖示
        /// </summary>
        public SkillKeyType DrawButton { get; set; }

        /// <summary>
        /// 使用繪圖工具管理物件新增技能框架繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="readius">圓角大小</param>
        /// <param name="drawButton">繪製技能熱鍵</param>
        /// <param name="iconDrawObject">技能繪製物件</param>
        public DrawUISkillFrame(DrawColors drawColor, int borderWidtrh, int readius, SkillKeyType drawButton, DrawBase iconDrawObject)
            : base(drawColor, borderWidtrh, readius, iconDrawObject)
        {
            DrawButton = drawButton;
            Colors.SetColor("Button", Color.Black);
            Colors.SetColor("Channel", GlobalColors.Channeled);
        }

        /// <summary>
        /// 使用繪圖工具管理物件新增技能框架繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="readius">圓角大小</param>
        /// <param name="drawButton">繪製技能熱鍵</param>
        public DrawUISkillFrame(DrawColors drawColor, int borderWidtrh, int readius, SkillKeyType drawButton)
            : base(drawColor, borderWidtrh, readius)
        {
            DrawButton = drawButton;
            Colors.SetColor("Button", Color.Black);
            Colors.SetColor("Channel", GlobalColors.Channeled);
        }

        /// <summary>
        /// 使用繪圖工具管理物件新增技能框架繪圖物件
        /// </summary>
        /// <param name="borderColor">框線顏色</param>
        /// <param name="backColor">背景色</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="readius">圓角大小</param>
        /// <param name="drawButton">繪製技能熱鍵</param>
        /// <param name="iconDrawObject">技能繪製物件</param>
        public DrawUISkillFrame(Color backColor, Color borderColor, int borderWidtrh, int readius, SkillKeyType drawButton, DrawBase iconDrawObject)
            : base(backColor, borderColor, borderWidtrh, readius, iconDrawObject)
        {
            DrawButton = drawButton;
            Colors.SetColor("Button", Color.Black);
            Colors.SetColor("Channel", GlobalColors.Channeled);
        }

        /// <summary>
        /// 新增技能框架繪圖物件
        /// </summary>
        /// <param name="borderColor">框線顏色</param>
        /// <param name="backColor">背景色</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="readius">圓角大小</param>
        /// <param name="drawButton">繪製技能熱鍵</param>
        /// <param name="iconDrawObject">技能繪製物件</param>
        public DrawUISkillFrame(Color backColor, Color borderColor, int borderWidtrh, int readius, SkillKeyType drawButton)
            : base(backColor, borderColor, borderWidtrh, readius)
        {
            DrawButton = drawButton;
            Colors.SetColor("Button", Color.Black);
            Colors.SetColor("Channel", GlobalColors.Channeled);
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = GetScaleRectangle(rectangle);

            SolidBrush brushBack = Colors.GetBrush("Back");
            Pen penBorder = Colors.GetPen("Border");
            penBorder.Width = BorderWidth;

            GetBackFrame(drawRectangle);
            DrawSkillBase drawSkillBase = DrawObjectInside as DrawSkillBase;
            SkillBase bindingSkill = drawSkillBase == null ? null : drawSkillBase.BindingSkill; //取得綁定技能

            g.FillPath(brushBack, _BackFrame);
            g.Clip = _BackRegion;
            if (bindingSkill != null && !StaticMode)
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
                            using (LinearGradientBrush brush2 = new LinearGradientBrush(drawRectangle, GlobalColors.Channeled, Color.FromArgb(245, 255, 240), angle))
                            {
                                g.FillRectangle(brush2, drawRectangle);
                            }
                            _Animation++;
                        }
                        else
                        {
                            float channeledSize = (1F - bindingSkill.Channeled.GetRatio()) * drawRectangle.Height;
                            SolidBrush brushChannel = Colors.GetBrush("Channel");
                            g.FillRectangle(brushChannel, drawRectangle.X, drawRectangle.Y + drawRectangle.Height - channeledSize, drawRectangle.Width, channeledSize);
                        }
                        break;
                }
            }

            if (DrawObjectInside != DrawNull.Value)
            {
                DrawObjectInside.Draw(g, drawRectangle);
            }
            g.ResetClip();
            g.DrawPath(penBorder, _BackFrame);

            if (DrawButton != RunningBox.SkillKeyType.None)
            {
                Pen penBlack = Colors.GetPen("Button");
                penBlack.Width = 2;
                Rectangle keyRectangle = new Rectangle(drawRectangle.Left + drawRectangle.Width - 15, drawRectangle.Top + drawRectangle.Height - 15, 20, 25);
                g.FillEllipse(Brushes.White, keyRectangle);

                switch (DrawButton)
                {
                    case RunningBox.SkillKeyType.MouseButtonLeft:
                        g.FillPie(Brushes.SkyBlue, keyRectangle, 180, 90);

                        break;
                    case RunningBox.SkillKeyType.MouseButtonRight:
                        g.FillPie(Brushes.SkyBlue, keyRectangle, 270, 90);

                        break;
                }
                g.DrawEllipse(penBlack, keyRectangle);
                penBlack.Width = 1;
                g.DrawLine(penBlack, keyRectangle.Left, keyRectangle.Top + keyRectangle.Height / 2, keyRectangle.Left + keyRectangle.Width, keyRectangle.Top + keyRectangle.Height / 2);
                g.DrawLine(penBlack, keyRectangle.Left + keyRectangle.Width / 2, keyRectangle.Top, keyRectangle.Left + keyRectangle.Width / 2, keyRectangle.Top + keyRectangle.Height / 2);
            }
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件(不包含內部物件,未綁定物件)
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawUISkillFrame(Colors.Copy(), BorderWidth, Readius, DrawButton)
            {
                Scale = this.Scale
            };
        }
    }
}
