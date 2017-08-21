using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 使用中心點及尺寸建立實體位置
    /// </summary>
    public class LayoutCentral : LayoutBase
    {

        public override PointF Center
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private Rectangle _Rectangle;
        /// <summary>
        /// 取得物件實體位置
        /// </summary>
        public override Rectangle Rectangle
        {
            get
            {
                if (!RectangleBuild)
                {
                    _Rectangle = new Rectangle();
                    RectangleBuild = true;
                }
                return _Rectangle;
            }
        }
    }
}
