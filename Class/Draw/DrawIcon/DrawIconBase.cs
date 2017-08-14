using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 畫筆繪圖物件
    /// </summary>
    public abstract class DrawIconBase : IDraw
    {
        /// <summary>
        /// 綁定技能物件
        /// </summary>
        public SkillBase BindingSkill { get; set; }

        private Pen _Pen;
        private SolidBrush _Brush;
        private GraphicsPath _BackRound;
        private Rectangle _BackRoundRectangle;

        private Color _Color;
        /// <summary>
        /// 繪製顏色
        /// </summary>
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color == value) return;
                _Color = value;
                BackPenAndBrush();
            }
        }

        /// <summary>
        /// 不透明度0-1,此處無用
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public void Draw(Graphics g, Rectangle rectangle)
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
                        using (LinearGradientBrush brush2 = new LinearGradientBrush(rectangle, Color.FromArgb(255,255,200), Color.FromArgb(255, 255, 240), 315))
                        {
                            g.FillRectangle(brush2, rectangle);
                        }
                        break;
                    case SkillStatus.Cooldown:
                        float cooldownSize = (float)(BindingSkill.CooldownRoundMax - BindingSkill.CooldownRound) / BindingSkill.CooldownRoundMax * rectangle.Height;
                        g.FillRectangle(Brushes.White, rectangle);
                        g.FillRectangle(Brushes.LightSlateGray, rectangle.X, rectangle.Y + rectangle.Height - cooldownSize, rectangle.Width, cooldownSize);
                        break;
                    case SkillStatus.Channeled:
                        float channeledSize = (float)(BindingSkill.ChanneledRoundMax - BindingSkill.ChanneledRound) / BindingSkill.ChanneledRoundMax * rectangle.Height;
                        g.FillRectangle(Brushes.White, rectangle);
                        g.FillRectangle(Brushes.LightGreen, rectangle.X, rectangle.Y + rectangle.Height - channeledSize, rectangle.Width, channeledSize);
                        break;
                }
            }
            g.DrawPath(pen, backRound);
            DrawIcon(pen, brush, g, rectangle);
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

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public abstract IDraw Copy();

        /// <summary>
        /// 取得畫筆物件
        /// </summary>
        /// <returns>畫筆物件</returns>
        public Pen GetPen()
        {
            if (_Pen == null)
            {
                _Pen = DrawPool.GetPen(Color);
            }
            return _Pen;
        }

        /// <summary>
        /// 取得筆刷物件
        /// </summary>
        /// <returns>筆刷物件</returns>
        public SolidBrush GetBrush()
        {
            if (_Brush == null)
            {
                _Brush = DrawPool.GetBrush(Color);
            }
            return _Brush;
        }

        /// <summary>
        /// 返還畫筆物件
        /// </summary>
        public void BackPenAndBrush()
        {
            if (_Pen != null)
            {
                DrawPool.BackPen(_Pen);
                _Pen = null;
            }

            if (_Brush != null)
            {
                DrawPool.BackBrush(_Brush);
                _Brush = null;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    BackPenAndBrush();
                }
                disposedValue = true;
            }
        }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
        }
        #endregion
    }
}
