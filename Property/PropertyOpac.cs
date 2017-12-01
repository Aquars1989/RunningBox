using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 不透明度調整屬性,調整到指定不透明度後中斷該特性
    /// </summary>
    class PropertyOpacityFix : PropertyBase
    {
        /// <summary>
        /// 目標不透明度
        /// </summary>
        public float Opacity { get; set; }

        private float _FixPerSec;
        /// <summary>
        /// 每秒不透明度調整值
        /// </summary>
        public float FixPerSec
        {
            get { return _FixPerSec; }
            set { _FixPerSec = Math.Abs(value); }
        }

        /// <summary>
        /// 不透明度調整屬性,調整到指定不透明度後中斷該特性
        /// </summary>
        /// <param name="opacity">目標不透明度</param>
        /// <param name="fixPerSec">每秒不透明度調整值</param>
        /// <param name="breakOld">是否先中斷目標身上其他透明度調整</param>
        public PropertyOpacityFix(float opacity, float fixPerSec, bool breakOld)
        {
            Opacity = opacity;
            FixPerSec = fixPerSec;

            if (breakOld)
            {
                for (int i = 0; i < Owner.Propertys.Count; i++)
                {
                    if (Owner.Propertys[i] is PropertyOpacityFix)
                    {
                        Owner.Propertys[i].Break();
                    }
                }
            }
        }

        public override void DoAfterAction()
        {
            if (Status == PropertyStatus.Enabled)
            {
                float fix = FixPerSec / Scene.RoundPerSec;
                float ned = Math.Abs(Owner.DrawObject.Colors.Opacity - Opacity);
                if (ned <= fix)
                {
                    Owner.DrawObject.Colors.Opacity = Opacity;
                    Break();
                }
                else
                {
                    Owner.DrawObject.Colors.Opacity += (Owner.DrawObject.Colors.Opacity > Opacity ? -fix : fix);
                }
            }
            base.DoAfterAction();
        }
    }
}
