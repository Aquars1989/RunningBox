using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 繪圖物件介面
    /// </summary>
    public interface IDraw : IDisposable
    {
        /// <summary>
        /// 繪製顏色
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// 不透明度(0~1)
        /// </summary>
        float Opacity { get; set; }

        /// <summary>
        /// 紅色值調整(-1~1)
        /// </summary>
        float RFix { get; set; }

        /// <summary>
        /// 綠色值調整(-1~1)
        /// </summary>
        float GFix { get; set; }

        /// <summary>
        /// 藍色值調整(-1~1)
        /// </summary>
        float BFix { get; set; }

        /// <summary>
        /// 縮放比例調整
        /// </summary>
        float Scale { get; set; }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        void Draw(Graphics g, Rectangle rectangle);

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        IDraw Copy();
    }

    /// <summary>
    /// 繪製型狀
    /// </summary>
    public enum DrawShape
    {
        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle = 0,

        /// <summary>
        /// 圓形
        /// </summary>
        Ellipse = 1
    }
}
