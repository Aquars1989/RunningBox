using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能:衝刺繪圖物件
    /// </summary>
    public class DrawSkillSleep : DrawSkillBase
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
        public DrawSkillSleep(DrawColors drawColor, SkillBase bindingSkill = null)
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
        public DrawSkillSleep(Color iconColor, SkillBase bindingSkill = null)
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
            Rectangle drawRectangle = GetScaleRectangle(rectangle);

            int aniMax = 360;
            if (Animation >= aniMax)
            {
                Animation %= aniMax;
            }

            int ani = Animation;
            Pen penIcon = Colors.GetPen("Icon");
            penIcon.Width = 2;

            float allRatio = 0;
            for (int i = 0; i < 3; i++)
            {
                float ratio = (0.1F + 0.07F * i);
                float size = drawRectangle.Width * ratio; //原始大小
                float drawX = drawRectangle.Left + (drawRectangle.Width * (0.2F + allRatio + (0.1F * i))) - size / 2; //原始位置
                float drawY = drawRectangle.Top + (drawRectangle.Height * (0.8F - allRatio - (0.1F * i))) - size / 2; //原始位置

                PointF offsetPoint = Function.GetOffsetPoint(drawX, drawY, (ani - i * 30), drawRectangle.Width * (0.02F * (i + 1)));
                drawX = offsetPoint.X;
                drawY = offsetPoint.Y;
                PointF[] drawPots = { new PointF(drawX , drawY ), 
                                      new PointF(drawX + size, drawY ),
                                      new PointF(drawX , drawY + size),
                                      new PointF(drawX + size, drawY + size)};
                g.DrawLines(penIcon, drawPots);
                allRatio += ratio;
            }
            Animation += 15;
        }

        /// <summary>
        /// 複製繪圖物件,未綁定物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawSkillSprint(Colors.Copy(), BindingSkill)
            {
                Scale = this.Scale,
                Angle = this.Angle,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled
            };
        }
    }
}
