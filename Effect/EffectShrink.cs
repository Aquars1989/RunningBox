using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 逐漸限縮場地範圍的特效
    /// </summary>
    class EffectShrink : EffectBase
    {
        private Padding _Shrinked;// 實際已縮小值

        /// <summary>
        /// 各邊界縮小值
        /// </summary>
        public Padding ShrinkValue { get; private set; }

        /// <summary>
        /// 新增逐漸限縮場地範圍的特效
        /// </summary>
        /// <param name="shrinkValue">各邊界縮小值</param>
        /// <param name="durationTime">縮小持續的時間(毫秒),小於0為永久</param>
        /// <param name="enablingTime">用來縮小的時間(毫秒)</param>
        /// <param name="disablingTime">用來恢復的時間(毫秒)</param>
        public EffectShrink(Padding shrinkValue, int enablingTime, int durationTime, int disablingTime)
            : base(enablingTime, durationTime, disablingTime)
        {
            ShrinkValue = shrinkValue;
        }

        public override void DoAfterRound()
        {
            switch (Status)
            {
                case EffectStatus.Enabling: // 縮小階段
                    if (!EnablingTime.IsFull)
                    {
                        // 計算應該修改的比例
                        float ratio = Scene.SceneIntervalOfRound / (float)(EnablingTime.Limit - EnablingTime.Value + Scene.SceneIntervalOfRound);
                        int left = (int)((ShrinkValue.Left - _Shrinked.Left) * ratio);
                        int right = (int)((ShrinkValue.Right - _Shrinked.Right) * ratio);
                        int top = (int)((ShrinkValue.Top - _Shrinked.Top) * ratio);
                        int bottom = (int)((ShrinkValue.Bottom - _Shrinked.Bottom) * ratio);

                        Scene.MainRectangle = new Rectangle
                        (
                            Scene.MainRectangle.Left + left,
                            Scene.MainRectangle.Top + top,
                            Scene.MainRectangle.Width - left - right,
                            Scene.MainRectangle.Height - top - bottom
                        );
                        _Shrinked = new Padding(_Shrinked.Left + left, _Shrinked.Top + top, _Shrinked.Right + right, _Shrinked.Bottom + bottom);
                    }
                    else
                    {
                        // 縮小結束時直接將值設為最終結果
                        Scene.MainRectangle = new Rectangle
                        (
                            Scene.MainRectangle.Left - _Shrinked.Left + ShrinkValue.Left,
                            Scene.MainRectangle.Top - _Shrinked.Top + ShrinkValue.Top,
                            Scene.MainRectangle.Width + _Shrinked.Horizontal - ShrinkValue.Horizontal,
                            Scene.MainRectangle.Height + _Shrinked.Vertical - ShrinkValue.Vertical
                        );
                        _Shrinked = ShrinkValue;
                    }
                    break;
                case EffectStatus.Disabling: // 恢復階段
                    if (!DisablingTime.IsFull)
                    {
                        // 計算應該修改的比例
                        float ratio = Scene.SceneIntervalOfRound / (float)(DisablingTime.Limit - DisablingTime.Value + Scene.SceneIntervalOfRound);
                        int left = (int)(_Shrinked.Left * ratio);
                        int right = (int)(_Shrinked.Right * ratio);
                        int top = (int)(_Shrinked.Top * ratio);
                        int botton = (int)(_Shrinked.Bottom * ratio);

                        Scene.MainRectangle = new Rectangle
                        (
                            Scene.MainRectangle.Left - left,
                            Scene.MainRectangle.Top - top,
                            Scene.MainRectangle.Width + left + right,
                            Scene.MainRectangle.Height + top + botton
                        );
                        _Shrinked = new Padding(_Shrinked.Left - left, _Shrinked.Top - top, _Shrinked.Right - right, _Shrinked.Bottom - botton);
                    }
                    else
                    {
                        // 恢復結束時直接將值設為最終結果
                        Scene.MainRectangle = new Rectangle
                        (
                            Scene.MainRectangle.Left - _Shrinked.Left,
                            Scene.MainRectangle.Top - _Shrinked.Top,
                            Scene.MainRectangle.Width + _Shrinked.Horizontal,
                            Scene.MainRectangle.Height + _Shrinked.Vertical
                        );
                        _Shrinked = Padding.Empty;
                        break;
                    }
                    break;
            }

            base.DoAfterRound();
        }
    }
}
