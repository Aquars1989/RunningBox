using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 顏色調整處理
    /// </summary>
    public static class ColorFix
    {
        private static ColorMatrix _Matrix;
        private static ImageAttributes _Attributes;
        static ColorFix()
        {
            float[][] matrixArray ={ new float[] {1, 0, 0, 0, 0},
                                     new float[] {0, 1, 0, 0, 0},
                                     new float[] {0, 0, 1, 0, 0},
                                     new float[] {0, 0, 0, 1, 0},
                                     new float[] {0, 0, 0, 0, 1}};
            _Matrix = new ColorMatrix(matrixArray);
            _Attributes = new ImageAttributes();
        }

        /// <summary>
        /// 使用指定的色彩調整繪製影像
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="image">要繪製的影像</param>
        /// <param name="rectangle">繪製區域(延伸)</param>
        /// <param name="a">不透明度倍數(0~1)</param>
        /// <param name="r">紅色調整值(-1~1)</param>
        /// <param name="g">綠色調整值(-1~1)</param>
        /// <param name="b">藍色調整值(-1~1)</param>
        public static void DrawImage(Graphics gp, Image image, Rectangle rectangle, float a, float r, float g, float b)
        {
            if (a == 1 && r == 0 && g == 0 && b == 0)
            {
                gp.DrawImage(image, rectangle);
            }
            else
            {
                _Matrix.Matrix33 = a;
                _Matrix.Matrix40 = r;
                _Matrix.Matrix41 = g;
                _Matrix.Matrix42 = b;

                _Attributes.ClearColorMatrix();
                _Attributes.SetColorMatrix(_Matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                gp.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, _Attributes);
            }
        }

        /// <summary>
        /// 取得調整後的色彩
        /// </summary>
        /// <param name="color">原始色彩</param>
        /// <param name="a">不透明度倍數(0~1)</param>
        /// <param name="r">紅色調整值(-1~1)</param>
        /// <param name="g">綠色調整值(-1~1)</param>
        /// <param name="b">藍色調整值(-1~1)</param>
        public static Color GetColor(Color color, float a, float r, float g, float b)
        {
            if (a == 1 && r == 0 && g == 0 && b == 0) return color;

            int newR = (int)(color.R + r * 255);
            int newG = (int)(color.G + g * 255);
            int newB = (int)(color.B + b * 255);
            int newA = (int)(color.A * a);

            if (newA > 255) newA = 255;
            else if (newA < 0) newA = 0;

            if (newR > 255) newR = 255;
            else if (newR < 0) newR = 0;

            if (newG > 255) newG = 255;
            else if (newG < 0) newG = 0;

            if (newB > 255) newB = 255;
            else if (newB < 0) newB = 0;
            return Color.FromArgb(newA, newR, newG, newB);
        }
    }
}
