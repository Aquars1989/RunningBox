using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能繪製基本物件
    /// </summary>
    public abstract class DrawSkillBase : DrawBase
    {
        /// <summary>
        /// 發生在綁定技能物件變更時
        /// </summary>
        public event EventHandler BindingSkillChanged;

        /// <summary>
        /// 發生在綁定技能物件變更時
        /// </summary>
        protected virtual void OnBindingSkillChanged()
        {
            if (BindingSkillChanged != null)
            {
                BindingSkillChanged(this, new EventArgs());
            }
        }

        private SkillBase _BindingSkill;
        /// <summary>
        /// 綁定技能物件
        /// </summary>
        public SkillBase BindingSkill
        {
            get { return _BindingSkill; }
            set
            {
                if (_BindingSkill == value) return;
                _BindingSkill = value;
                OnBindingSkillChanged();
            }
        }

        public DrawSkillBase(DrawColors drawColor) : base(drawColor) { }
        public DrawSkillBase() : base() { }
    }
}
