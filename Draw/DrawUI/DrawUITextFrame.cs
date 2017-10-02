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
    public class DrawUITextFrame : DrawUIFrame
    {
        /// <summary>
        /// 繪製文字
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 繪製字型
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// 繪製設定
        /// </summary>
        public StringFormat DrawFormat { get; set; }

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
        /// <param name="text">文字</param>
        /// <param name="font">字型</param>
        /// <param name="drawFormat">繪製設定</param>
        public DrawUITextFrame(DrawColors drawColor, int borderWidtrh, int readius, string text, Font font, StringFormat drawFormat)
            : base(drawColor, borderWidtrh, readius)
        {
            Text = text;
            Font = font;
            DrawFormat = drawFormat;
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
        /// <param name="drawFormat">繪製設定</param>
        public DrawUITextFrame(Color textColor, Color shadowColor, Color backColor, Color borderColor, int borderWidtrh, int readius, string text, Font font, StringFormat drawFormat)
            : base(backColor, borderColor, borderWidtrh, readius)
        {
            Colors.SetColor("Text", textColor);
            Colors.SetColor("Shadow", shadowColor);
            Text = text;
            Font = font;
            DrawFormat = drawFormat;
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
            SolidBrush brushText = Colors.GetBrush("Text");
            SolidBrush brushShadow = Colors.GetBrush("Shadow");

            Rectangle textRectangle = GetScaleRectangle(new Rectangle(rectangle.X + TextPadding.Left, rectangle.Y + TextPadding.Top, rectangle.Width - TextPadding.Horizontal, rectangle.Height - TextPadding.Vertical));
            g.DrawString(Text, Font, brushShadow, new Rectangle(textRectangle.X + 1, textRectangle.Y + 1, textRectangle.Width, textRectangle.Height), DrawFormat);
            g.DrawString(Text, Font, brushText, textRectangle, DrawFormat);
            g.ResetClip();

            if (BorderWidth > 0)
            {
                g.DrawPath(Colors.GetPen("Border", BorderWidth), _BackFrame);
            }
        }

        /// <summary>
        /// 複製繪圖物件及內部的繪圖工具管理物件(不包含內部物件,未綁定物件)
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public override DrawBase Copy()
        {
            return new DrawUITextFrame(Colors.Copy(), BorderWidth, Readius, Text, Font, DrawFormat)
            {
                Scale = this.Scale,
                TextPadding = this.TextPadding,
                Angle = this.Angle,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled
            };
        }
    }
}
