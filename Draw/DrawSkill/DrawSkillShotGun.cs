using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能:散彈繪圖物件
    /// </summary>
    public class DrawSkillShotGun : DrawSkillBase
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
        public DrawSkillShotGun(DrawColors drawColor, SkillBase bindingSkill = null)
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
        public DrawSkillShotGun(Color iconColor, SkillBase bindingSkill = null)
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

            int aniMax = 16;
            int aniHalf = aniMax / 2;
            if (Animation >= aniMax)
            {
                Animation %= aniMax;
            }

            int ani = Animation;
            float drawX = drawRectangle.Left + (drawRectangle.Width * 0.7F); // 原始位置
            float drawY = drawRectangle.Top + (drawRectangle.Height * 0.7F); // 原始位置
            float fixX = 0, fixY = 0;
            float size = drawRectangle.Width * 0.3F;
            double maxDis = Function.GetDistance(0, 0, drawRectangle.Width, drawRectangle.Height);

            if (Animation < aniHalf)
            {
                int anyHHalf = aniHalf / 3;
                int move = anyHHalf - Math.Abs(anyHHalf - Animation);
                PointF movePoint = Function.GetOffsetPoint(0, 0, 45, maxDis / 75 * move);
                fixX = movePoint.X;
                fixY = movePoint.Y;
            }
            Brush brushIcon = Colors.GetBrush("Icon");
            g.FillEllipse(brushIcon, drawX + fixX - size / 2, drawY + fixY - size / 2, size, size);

            size /= 2;
            float ratio = ani / (float)aniMax * 1.3F;
            if (ratio < 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    int angle = 180 + 30 * i;

                    double distance = ratio * maxDis;
                    PointF drawPoint = Function.GetOffsetPoint(drawX, drawY, angle, distance);
                    g.FillEllipse(brushIcon, drawPoint.X - size / 2, drawPoint.Y - size / 2, size, size);
                }
            }

            Animation++;
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
