using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能:衝擊波繪圖物件
    /// </summary>
    public class DrawSkillShockwave : DrawSkillBase
    {
        /// <summary>
        /// 主要繪製顏色(供碎片物件使用)
        /// </summary>
        public override Color MainColor
        {
            get { return Colors.GetColor("Icon"); }
        }

        /// <summary>
        /// 動畫進度
        /// </summary>
        public int Animation { get; set; }

        /// <summary>
        /// 由繪圖工具管理物件新增技能:衝刺繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="bindingSkill">綁定技能</param>
        public DrawSkillShockwave(DrawColors drawColor, SkillBase bindingSkill = null)
            : base(drawColor)
        {
            Animation = 0;
            BindingSkill = bindingSkill;
        }

        /// <summary>
        /// 新增技能:衝刺繪圖物件
        /// </summary>
        /// <param name="iconColor">繪製顏色</param>
        /// <param name="bindingSkill">綁定技能</param>
        public DrawSkillShockwave(Color iconColor, SkillBase bindingSkill = null)
        {
            Colors.SetColor("Icon", iconColor);
            Animation = 0;
            BindingSkill = bindingSkill;
        }
        
        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            int aniMax = BindingSkill != null && BindingSkill.Status == SkillStatus.Channeled ? 4 : 24;
            int criMax = 24;
            if (Animation >= aniMax)
            {
                Animation %= aniMax;
            }

            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            int left = drawRectangle.Left;
            int top = drawRectangle.Top;
            int width = drawRectangle.Width;
            int height = drawRectangle.Height;
            int centerWidth = (int)(width * 0.25F + 0.5F);
            int centerHeight = (int)(height * 0.25F + 0.5F);
            Rectangle centerRect = new Rectangle(left + (width - centerWidth) / 2, top + (height - centerHeight) / 2, centerWidth, centerHeight);

            SolidBrush brushIcon = Colors.GetBrush("Icon");
            Pen penIcon = Colors.GetPen("Icon");
            penIcon.Width = 1;
            int ani = Animation;
            float size = drawRectangle.Width * 0.3F; //原始大小
            float maxWidth = width * 1.5F;

            using (Region clip = new Region(drawRectangle))
            {
                g.Clip = clip;
                while (true)
                {
                    float ratio = ani / (float)criMax;
                    size += ratio * maxWidth;
                    if (size < maxWidth)
                    {
                        g.DrawEllipse(penIcon, drawRectangle.Left + (drawRectangle.Width - size) / 2, drawRectangle.Top + (drawRectangle.Height - size) / 2, size, size);
                    }
                    else break;
                    ani = aniMax;
                }
                g.ResetClip();
            }

            g.FillEllipse(brushIcon, centerRect);
            Animation++;
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawSkillSprint(Colors.Copy(), BindingSkill)
            {
                Scene = this.Scene,
                Owner = this.Owner,
                Scale = this.Scale
            };
        }
    }
}
