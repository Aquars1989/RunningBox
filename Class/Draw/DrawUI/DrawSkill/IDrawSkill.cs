using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能繪製介面
    /// </summary>
    public interface IDrawSkill : IDraw
    {
        /// <summary>
        /// 綁定技能物件
        /// </summary>
        SkillBase BindingSkill { get; set; }
    }
}
