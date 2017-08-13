using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 圖片繪圖物件
    /// </summary>
    public class DrawImage : IDraw
    {
        private Color _Color;
        /// <summary>
        /// 繪製顏色(供技能/特效使用)
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// 繪製圖片
        /// </summary>
        public Image Image { get; set; }

        private ColorMatrix _Matrix;
        private float _Opacity;
        /// <summary>
        /// 不透明度0-1
        /// </summary>
        public float Opacity
        {
            get { return _Opacity; }
            set
            {
                if (_Opacity == value) return;
                _Opacity = value;
                if (_Opacity > 1) _Opacity = 1;
                else if (_Opacity < 0) _Opacity = 0;
                _Matrix.Matrix33 = _Opacity;
            }
        }

        /// <summary>
        /// 新增圖片繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色(供技能/特效使用)</param>
        /// <param name="opacity">透明度0-1</param>
        /// <param name="drawShape">繪製圖片</param>
        public DrawImage(Color color, Image image, float opacity = 1)
        {
            float[][] matrixArray ={ new float[] {1, 0, 0, 0, 0},
                                     new float[] {0, 1, 0, 0, 0},
                                     new float[] {0, 0, 1, 0, 0},
                                     new float[] {0, 0, 0, 1, 0},
                                     new float[] {0, 0, 0, 0, 1}};
            _Matrix = new ColorMatrix(matrixArray);

            Color = color;
            Image = image;
            Opacity = opacity;
        }

        /// <summary>
        /// 繪製到Graphics
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="rectangle">繪製區域</param>
        public void Draw(Graphics g, Rectangle rectangle)
        {
            if (Opacity == 1)
            {
                g.DrawImage(Image, rectangle);
            }
            else
            {
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(_Matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    g.DrawImage(Image, rectangle, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel, attributes);
                }
            }
        }

        /// <summary>
        /// 複製繪圖物件
        /// </summary>
        /// <returns>複製繪圖物件</returns>
        public IDraw Copy()
        {
            return new DrawImage(Color, Image, Opacity);
        }

        public void Dispose() { }
    }
}
