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
        private Padding _Shrinked;//實際已縮小值

        /// <summary>
        /// 特效是否可被中斷
        /// </summary>
        public bool CanBreak { get; set; }

        /// <summary>
        /// 作用場景物件
        /// </summary>
        public SceneBase Scene { get; set; }

        /// <summary>
        /// 特效狀態
        /// </summary>
        public EffectStatus Status { get; private set; }

        /// <summary>
        /// 縮小持續的時間計時器(毫秒)
        /// </summary>
        public CounterObject DurationTime { get; private set; }

        /// <summary>
        /// 用來縮小的時間計時器(毫秒)
        /// </summary>
        public CounterObject EnablingTime { get; private set; }

        /// <summary>
        /// 用來還原的時間計時器(毫秒)
        /// </summary>
        public CounterObject DisablingTime { get; private set; }

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
        public EffectShrink(Padding shrinkValue, int durationTime, int enablingTime, int disablingTime)
        {
            CanBreak = true;
            Status = EffectStatus.Enabling;
            ShrinkValue = shrinkValue;
            DurationTime = new CounterObject(durationTime);
            EnablingTime = new CounterObject(enablingTime);
            DisablingTime = new CounterObject(disablingTime);
        }

        public void DoAfterRound()
        {
            switch (Status)
            {
                case EffectStatus.Enabling: //縮小階段
                    if (!EnablingTime.IsFull)
                    {
                        //計算應該修改的比例
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
                        EnablingTime.Value += Scene.SceneIntervalOfRound;
                    }
                    else
                    {
                        //縮小結束時直接將值設為最終結果
                        Scene.MainRectangle = new Rectangle
                        (
                            Scene.MainRectangle.Left - _Shrinked.Left + ShrinkValue.Left,
                            Scene.MainRectangle.Top - _Shrinked.Top + ShrinkValue.Top,
                            Scene.MainRectangle.Width + _Shrinked.Horizontal - ShrinkValue.Horizontal,
                            Scene.MainRectangle.Height + _Shrinked.Vertical - ShrinkValue.Vertical
                        );
                        _Shrinked = ShrinkValue;

                        Status = EffectStatus.Enabled;
                        goto case EffectStatus.Enabled;
                    }
                    break;
                case EffectStatus.Enabled: //維持階段
                    if (DurationTime.IsFull)
                    {
                        Status = EffectStatus.Disabling;
                        goto case EffectStatus.Disabling;
                    }
                    else
                    {
                        DurationTime.Value += Scene.SceneIntervalOfRound;
                    }
                    break;
                case EffectStatus.Disabling: //恢復階段
                    if (!DisablingTime.IsFull)
                    {
                        //計算應該修改的比例
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
                        DisablingTime.Value += Scene.SceneIntervalOfRound;
                    }
                    else
                    {
                        //恢復結束時直接將值設為最終結果
                        Scene.MainRectangle = new Rectangle
                        (
                            Scene.MainRectangle.Left - _Shrinked.Left,
                            Scene.MainRectangle.Top - _Shrinked.Top,
                            Scene.MainRectangle.Width + _Shrinked.Horizontal,
                            Scene.MainRectangle.Height + _Shrinked.Vertical
                        );
                        _Shrinked = Padding.Empty;
                        Status = EffectStatus.Disabled;
                        break;
                    }
                    break;
            }
        }

        public void Break()
        {
            if (CanBreak)
            {
                Status = EffectStatus.Disabling;
            }
        }

        public void DoBeforeDraw(Graphics g) { }
        public void DoBeforeDrawObject(Graphics g) { }
        public void DoBeforeDrawFloor(Graphics g) { }
        public void DoAfterDraw(Graphics g) { }
        public void DoBeforeRound() { }
        public void DoBeforeDrawUI(Graphics g) { }
    }
}
