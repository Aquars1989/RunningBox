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
    /// 捲動文字繪圖物件
    /// </summary>
    public class DrawUITextScroll : DrawUIText
    {
        /// <summary>
        /// 舊文字駐列
        /// </summary>
        private Queue<string> _LastTexts = new Queue<string>();

        /// <summary>
        /// 顯示中舊文字
        /// </summary>
        private string _LastText = null;

        /// <summary>
        /// 顯示中文字
        /// </summary>
        private string _DisplayText = null;

        private string _Text = "";
        /// <summary>
        /// 繪製文字
        /// </summary>
        public new string Text
        {
            get { return _Text; }
            set
            {
                string oldValue = _Text;
                _Text = (value ?? "").Trim();

                if (IgnoreSameText && oldValue == _Text) return;
                if (_DisplayText == null)
                {
                    _DisplayText = _Text;
                    Animation.Value = Animation.Limit;
                }
                else
                {
                    if (IncompleteShow)
                    {
                        _LastTexts.Clear();
                    }
                    _LastTexts.Enqueue(_Text);
                }
            }
        }

        /// <summary>
        /// 如果佇列中有多筆文字則只顯示最後一筆
        /// </summary>
        public bool IncompleteShow { get; set; }

        /// <summary>
        /// 是否忽略相同文字而不產生捲動效果
        /// </summary>
        public bool IgnoreSameText { get; set; }

        /// <summary>
        /// 切換時間(毫秒)
        /// </summary>
        public CounterObject Animation { get; set; }

        /// <summary>
        /// 使用繪圖工具管理物件新增文字繪圖物件
        /// </summary>
        /// <param name="drawColor">繪圖工具管理物件</param>
        /// <param name="borderWidtrh">框線粗細</param>
        /// <param name="readius">圓角大小</param>
        /// <param name="text">文字</param>
        /// <param name="font">字型</param>
        /// <param name="drawFormat">繪製設定</param>
        /// <param name="animation">切換時間</param>
        public DrawUITextScroll(DrawColors drawColor, int borderWidtrh, int readius, string text, Font font, StringFormat drawFormat, int animation)
            : base(drawColor, borderWidtrh, readius, "", font, drawFormat)
        {
            Animation = new CounterObject(animation);
            Text = text;
            IncompleteShow = true;
            IgnoreSameText = false;
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
        public DrawUITextScroll(Color textColor, Color shadowColor, Color backColor, Color borderColor, int borderWidtrh, int readius, string text, Font font, StringFormat drawFormat, int animation)
            : base(textColor, shadowColor, backColor, borderColor, borderWidtrh, readius, text, font, drawFormat)
        {
            Animation = new CounterObject(animation);
            Text = text;
            IncompleteShow = true;
            IgnoreSameText = false;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        protected override void OnDraw(Graphics g, Rectangle rectangle)
        {
            Rectangle drawRectangle = GetScaleRectangle(rectangle);
            Brush brushBack = Colors.GetBrush("Back");

            GetBackFrame(drawRectangle);
            g.FillPath(brushBack, _BackFrame);

            g.Clip = _BackRegion;
            if (DrawObjectInside != DrawNull.Value)
            {
                DrawObjectInside.Draw(g, drawRectangle);
            }

            if (Animation.IsFull && _LastTexts.Count > 0)
            {
                _LastText = _DisplayText;
                _DisplayText = _LastTexts.Dequeue();
                Animation.Value = 0;
            }
            Animation.Value += Scene.IntervalOfRound;

            Brush brushText = Colors.GetBrush("Text");
            Brush brushShadow = Colors.GetBrush("Shadow");

            Rectangle textRectangle = GetScaleRectangle(new Rectangle(rectangle.X + TextPadding.Left, rectangle.Y + TextPadding.Top, rectangle.Width - TextPadding.Horizontal, rectangle.Height - TextPadding.Vertical));
            if (Animation.IsFull)
            {
                textRectangle.Offset(1, 1);
                g.DrawString(_DisplayText, Font, brushShadow, textRectangle, DrawFormat);
                textRectangle.Offset(-1, -1);
                g.DrawString(_DisplayText, Font, brushText, textRectangle, DrawFormat);
            }
            else
            {
                int moveMaxY = textRectangle.Height + rectangle.Height / 2;
                textRectangle.Offset(1, (int)(1 - Animation.GetRatio() * moveMaxY));
                g.DrawString(_LastText, Font, brushShadow, textRectangle, DrawFormat);
                textRectangle.Offset(-1, -1);
                g.DrawString(_LastText, Font, brushText, textRectangle, DrawFormat);

                textRectangle.Offset(1, moveMaxY);
                g.DrawString(_DisplayText, Font, brushShadow, textRectangle, DrawFormat);
                textRectangle.Offset(-1, -1);
                g.DrawString(_DisplayText, Font, brushText, textRectangle, DrawFormat);
            }

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
            return new DrawUITextScroll(Colors.Copy(), BorderWidth, Readius, Text, Font, DrawFormat, Animation.Limit)
            {
                Scale = this.Scale,
                TextPadding = this.TextPadding,
                Angle = this.Angle,
                Resistance = this.Resistance,
                RotateEnabled = this.RotateEnabled,
                IncompleteShow = this.IncompleteShow,
                IgnoreSameText = this.IgnoreSameText
            };
        }
    }
}
