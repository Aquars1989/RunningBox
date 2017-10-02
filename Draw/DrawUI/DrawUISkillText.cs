using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 文字繪圖物件
    /// </summary>
    public class DrawUISkillText : DrawUIFrame
    {
        private static Font _MainFont = new Font("微軟正黑體", 11);
        private static Font _InfoFont = new Font("微軟正黑體", 9);

        /// <summary>
        /// 發生在綁定技能物件變更時
        /// </summary>
        public event EventHandler BindingSkillChanged;

        /// <summary>
        /// 發生在綁定技能物件變更時
        /// </summary>
        protected virtual void OnBindingSkillChanged()
        {
            if (BindingSkillChanged != null)
            {
                BindingSkillChanged(this, new EventArgs());
            }
        }

        private SkillBase _BindingSkill;
        /// <summary>
        /// 綁定技能物件
        /// </summary>
        public SkillBase BindingSkill
        {
            get { return _BindingSkill; }
            set
            {
                if (_BindingSkill == value) return;
                _BindingSkill = value;
                OnBindingSkillChanged();
            }
        }

        /// <summary>
        /// 文字內部間距
        /// </summary>
        public Padding TextPadding { get; set; }

        /// <summary>
        /// 使用繪圖工具管理物件新增文字繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="readius">圓角大小</param>
        /// <param name="font">字型</param>
        public DrawUISkillText(DrawColors drawColor, int borderWidtrh, int readius)
            : base(drawColor, borderWidtrh, readius)
        {
            TextPadding = new Padding(5, 5, 5, 5);
        }

        /// <summary>
        /// 新增文字繪圖物件
        /// </summary>
        /// <param name="textColor">文字顏色</param>
        /// <param name="shadowColor">文字顏色(底部)</param>
        /// <param name="backColor">背景色</param>
        /// <param name="borderColor">框線顏色</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="readius">圓角大小</param>
        /// <param name="text">文字</param>
        /// <param name="font">字型</param>
        public DrawUISkillText(Color textColor, Color shadowColor, Color backColor, Color borderColor, int borderWidtrh, int readius)
            : base(backColor, borderColor, borderWidtrh, readius)
        {
            Colors.SetColor("Text", textColor);
            Colors.SetColor("Shadow", shadowColor);
            Colors.SetColor("Info1", Color.Chocolate);
            Colors.SetColor("Info2", Color.SteelBlue);
            Colors.SetColor("Info3", Color.DarkGreen);
            TextPadding = new Padding(5, 5, 5, 5);
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

            GetBackFrame(drawRectangle);
            g.FillPath(brushBack, _BackFrame);

            g.Clip = _BackRegion;
            if (DrawObjectInside != DrawNull.Value)
            {
                DrawObjectInside.Draw(g, drawRectangle);
            }

            Rectangle insideRectangle = GetScaleRectangle(new Rectangle(rectangle.X + TextPadding.Left, rectangle.Y + TextPadding.Top, rectangle.Width - TextPadding.Horizontal, rectangle.Height - TextPadding.Vertical));
            int insideLeft = insideRectangle.Left;
            int insideTop = insideRectangle.Top;
            int insideWidth = insideRectangle.Width;
            int insideHeight = insideRectangle.Height;
            Rectangle textRectangle = new Rectangle(insideLeft, insideTop, insideWidth, (int)(insideHeight * 0.7F));

            string mainText = BindingSkill.Info;
            SolidBrush brushText = Colors.GetBrush("Text");
            SolidBrush brushShadow = Colors.GetBrush("Shadow");
            SolidBrush brushInfo1 = Colors.GetBrush("Info1");
            SolidBrush brushInfo2 = Colors.GetBrush("Info2");
            SolidBrush brushInfo3 = Colors.GetBrush("Info3");

            textRectangle.Offset(-1, -1);
            g.DrawString(mainText, _MainFont, brushShadow, textRectangle, GlobalFormat.MiddleLeft);
            textRectangle.Offset(1, 1);
            g.DrawString(mainText, _MainFont, brushText, textRectangle, GlobalFormat.MiddleLeft);

            int infoLeft = insideLeft;
            int infoTop = insideTop + (int)(insideHeight * 0.7F);

            g.DrawString("能量", _InfoFont, brushInfo1, infoLeft, infoTop);
            infoLeft += (int)g.MeasureString("能量", _InfoFont).Width;
            string energyText = "";
            if (BindingSkill.CostEnergy > 0)
            {
                energyText = (BindingSkill.CostEnergy / 100).ToString();
            }
            if (BindingSkill.CostEnergyPerSec > 0)
            {
                energyText = (string.IsNullOrWhiteSpace(energyText) ? "" : energyText + "+") + (BindingSkill.CostEnergyPerSec / 100).ToString() + "/s";
            }

            if (string.IsNullOrWhiteSpace(energyText)) energyText = "無";
            g.DrawString(energyText, _InfoFont, brushInfo2, infoLeft, infoTop);

            infoLeft = insideLeft + insideWidth / 2 + 10;
            g.DrawString("冷卻", _InfoFont, brushInfo1, infoLeft, infoTop);
            infoLeft += (int)g.MeasureString("冷卻", _InfoFont).Width;
            string cooldownText = string.Format("{0:N1}秒", BindingSkill.Cooldown.Limit / 1000F);
            g.DrawString(cooldownText, _InfoFont, brushInfo3, infoLeft, infoTop);
            g.ResetClip();

            if (BorderWidth > 0)
            {
                g.DrawPath(Colors.GetPen("Border", BorderWidth), _BackFrame);
            }
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件(不包含內部物件)
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawUISkillText(Colors.Copy(), BorderWidth, Readius)
            {
                Scale = this.Scale,
                TextPadding = this.TextPadding,
                Angle = this.Angle,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled,
                BindingSkill = this.BindingSkill
            };
        }
    }
}
