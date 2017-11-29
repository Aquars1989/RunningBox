
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 輪替畫面顏色的特效
    /// </summary>
    public class EffectDyeingRotate : EffectBase
    {
        private int _ColorIndex = 0;        // 渲染顏色索引
        private Color _LastColor;           // 上一個渲染顏色
        private Color _NowColor;            // 目前渲染顏色
        private CounterObject _RotateTime;  // 輪替計時器
        private CounterObject _ConvertTime; // 轉換計時器

        /// <summary>
        /// 渲染色彩
        /// </summary>
        public Color[] Colors { get; private set; }

        /// <summary>
        /// 新增逐漸輪替畫面顏色的特效
        /// </summary>
        /// <param name="colors">要繪製的顏色陣列</param>
        /// <param name="durationTime">持續時間(毫秒),小於0為永久</param>
        /// <param name="rotateTime">輪替時間(毫秒)</param>
        /// <param name="convertTime">轉換時間(毫秒)</param>
        public EffectDyeingRotate(Color[] colors, int durationTime, int rotateTime, int convertTime)
            : base(rotateTime, durationTime, rotateTime)
        {
            Status = EffectStatus.Enabling;
            Colors = colors;
            _RotateTime = new CounterObject(rotateTime);
            _ConvertTime = new CounterObject(convertTime);
        }

        public override void DoBeforeDrawFloor(Graphics g)
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingTime.Value > 0)
                    {
                        int alpha = (int)(EnablingTime.GetRatio() * Colors[_ColorIndex].A + 0.5F);
                        if (alpha < 0) alpha = 0;
                        else if (alpha > 255) alpha = 255;

                        _LastColor = Color.FromArgb(alpha, Colors[_ColorIndex]);
                        using (Brush brush = new SolidBrush(_LastColor))
                        {
                            g.FillRectangle(brush, Scene.ClientRectangle);
                        }
                    }
                    break;
                case EffectStatus.Enabled:
                    using (Brush brush = new SolidBrush(_NowColor))
                    {
                        g.FillRectangle(brush, Scene.ClientRectangle);
                    }
                    break;
                case EffectStatus.Disabling:
                    if (DisablingTime.Value > 0)
                    {
                        int alpha = (int)((1 - DisablingTime.GetRatio()) * _NowColor.A + 0.5F);

                        if (alpha < 0) alpha = 0;
                        else if (alpha > 255) alpha = 255;
                        using (Brush brush = new SolidBrush(Color.FromArgb(alpha, _NowColor)))
                        {
                            g.FillRectangle(brush, Scene.ClientRectangle);
                        }
                    }
                    break;
            }
            base.DoBeforeDrawFloor(g);
        }

        /// <summary>
        /// 在回合動作最後執行
        /// </summary>
        public override void Settlement()
        {
            base.Settlement();
            switch (Status)
            {
                case EffectStatus.Enabled:
                    if (_RotateTime.IsFull)
                    {
                        _ColorIndex++;
                        if (_ColorIndex >= Colors.Length)
                        {
                            _ColorIndex = 0;
                        }

                        _LastColor = _NowColor;
                        _RotateTime.Value = 0;
                        _ConvertTime.Value = 0;
                    }
                    else
                    {
                        if (_ConvertTime.IsFull)
                        {
                            _NowColor = Colors[_ColorIndex];
                        }
                        else
                        {
                            double ratio = _ConvertTime.GetRatio();
                            int fxA = (int)((Colors[_ColorIndex].A - _LastColor.A) * ratio + 0.5F);
                            int fxR = (int)((Colors[_ColorIndex].R - _LastColor.R) * ratio + 0.5F);
                            int fxG = (int)((Colors[_ColorIndex].G - _LastColor.G) * ratio + 0.5F);
                            int fxB = (int)((Colors[_ColorIndex].B - _LastColor.B) * ratio + 0.5F);
                            _NowColor = Color.FromArgb(_LastColor.A + fxA, _LastColor.R + fxR, _LastColor.G + fxG, _LastColor.B + fxB);
                            _ConvertTime.Value += Scene.SceneIntervalOfRound;
                        }
                    }
                    _RotateTime.Value += Scene.SceneIntervalOfRound;
                    break;
            }
        }
    }
}
