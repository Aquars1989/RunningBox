
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace RunningBox
{
    /// <summary>
    /// 技能基礎物件
    /// </summary>
    public abstract class SkillBase
    {
        #region ===== 事件 =====
        /// <summary>
        /// 發生於自動施放物件變更時
        /// </summary>
        public event ValueChangedEnentHandle AutoCastObjectChanged;

        /// <summary>
        /// 發生於依附物件變更時(依附物件可為集合 場景 物件)
        /// </summary>
        public event EventHandler BindingChanged;

        /// <summary>
        /// 發生於技能狀態變更時
        /// </summary>
        public event ValueChangedEnentHandle StatusChanged;

        /// <summary>
        /// 發生於技能施放時
        /// </summary>
        public event EventHandler Start;

        /// <summary>
        /// 發生於技能結束時
        /// </summary>
        public event SkillEndEnentHandle End;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於自動施放物件變更時
        /// </summary>
        protected virtual void OnAutoCastObjectChanged(object oldValue, object newValue)
        {
            if (AutoCastObjectChanged != null)
            {
                AutoCastObjectChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於依附物件變更時(依附物件可為集合 場景 物件)
        /// </summary>
        protected virtual void OnBindingChanged()
        {
            if (BindingChanged != null)
            {
                BindingChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於技能狀態變更時
        /// </summary>
        protected virtual void OnStatusChanged(object oldValue, object newValue)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於技能施放時
        /// </summary>
        /// <param name="target">技能目標</param>
        protected virtual void OnStart(ITargetability target)
        {
            Status = SkillStatus.Enabled;
            Target.SetObject(target);

            if (Start != null)
            {
                Start(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於技能結束時
        /// </summary>
        /// <param name="endType">結束方式</param>
        protected virtual void OnEnd(SkillEndType endType)
        {
            DoAfterEnd(endType);

            if (End != null)
            {
                End(this, endType);
            }
        }
        #endregion

        #region ===== 屬性 =====
        //說明文字用
        private static Font _InfoFont = new Font("微軟正黑體", 12);

        /// <summary>
        /// 說明文字
        /// </summary>
        public abstract string Info { get; }

        private AutoCastBase _AutoCastObject;
        /// <summary>
        /// 自動施放物件
        /// </summary>
        public AutoCastBase AutoCastObject
        {
            get { return _AutoCastObject; }
            set
            {
                if (_AutoCastObject == value) return;
                object oldValue = _AutoCastObject;
                _AutoCastObject = value;
                OnAutoCastObjectChanged(oldValue, value);
            }
        }

        /// <summary>
        /// 技能目標(必要)
        /// </summary>
        public TargetSet Target { get; private set; }

        private SkillCollection _Container;
        /// <summary>
        /// 取得技能歸屬集合
        /// </summary>
        public SkillCollection Container
        {
            get { return _Container; }
            private set
            {
                if (_Container == value) return;
                _Container = value;
            }
        }

        private ObjectActive _Owner;
        /// <summary>
        /// 取得技能所有人
        /// </summary>
        public ObjectActive Owner
        {
            get { return Container == null ? _Owner : Container.Owner; }
            private set
            {
                if (_Owner == value) return;
                _Owner = value;
            }
        }

        private SceneBase _Scene;
        /// <summary>
        /// 取得歸屬場景
        /// </summary>
        public SceneBase Scene
        {
            get { return Container == null ? Owner == null ? _Scene : Owner.Scene : Container.Scene; }
            private set
            {
                if (_Scene == value) return;
                _Scene = value;
            }
        }

        /// <summary>
        /// 技能冷卻時間計數器(毫秒)
        /// </summary>
        public CounterObject Cooldown { get; protected set; }

        /// <summary>
        /// 引導型技能引導時間計數器(毫秒)
        /// </summary>
        public CounterObject Channeled { get; protected set; }

        /// <summary>
        /// 技能耗費能量
        /// </summary>
        public int CostEnergy { get; set; }

        /// <summary>
        /// 引導型技能每秒耗費能量
        /// </summary>
        public int CostEnergyPerSec { get; set; }

        private SkillStatus _Status;
        /// <summary>
        /// 技能狀態
        /// </summary>
        public SkillStatus Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value) return;
                object oldValue = _Status;
                _Status = value;
                switch (_Status)
                {
                    case SkillStatus.Cooldown:
                        Cooldown.Value = 0;
                        break;
                    case SkillStatus.Channeled:
                        Channeled.Value = 0;
                        break;
                }
                OnStatusChanged(oldValue, value);
            }
        }
        #endregion

        #region ***** 建構式 *****
        public SkillBase()
        {
            Target = new TargetSet();
        }
        #endregion

        #region ===== 方法 =====
        /// <summary>
        /// 綁定技能到場景
        /// </summary>
        public void Binding(SceneBase scene, bool skipCheck = false)
        {
            if (_Scene == scene) return;
            if (!skipCheck && Container != null && Container.Contains(this))
            {
                throw new Exception("技能已在集合內無法手動綁定");
            }

            Break();
            Container = null;
            Owner = null;
            Scene = Scene;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定技能到物件
        /// </summary>
        public void Binding(ObjectActive owner, bool skipCheck = false)
        {
            if (_Owner == owner) return;
            if (!skipCheck && Container != null && Container.Contains(this))
            {
                throw new Exception("技能已在集合內無法手動綁定");
            }

            Break();
            Container = null;
            Owner = owner;
            Scene = null;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定技能到集合(集合內綁定)
        /// </summary>
        public void Binding(SkillCollection collection, bool skipCheck = false)
        {
            if (_Container == collection) return;
            if (!skipCheck && Container != null && Container.Contains(this))
            {
                throw new Exception("技能已在集合內無法手動綁定");
            }

            Break();
            Container = collection;
            Owner = null;
            Scene = null;
            OnBindingChanged();
        }

        /// <summary>
        /// 清除綁定
        /// </summary>
        public void ClearBinding(bool skipCheck = false)
        {
            if (!skipCheck && Container != null && Container.Contains(this))
            {
                throw new Exception("技能已在集合內無法手動綁定");
            }

            Break();
            Container = null;
            Owner = null;
            Scene = null;
            OnBindingChanged();
        }

        #region ##### 場景中動作(須有所有者) #####
        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="target">技能目標</param>
        public virtual void Cast(ITargetability target)
        {
            if (Owner == null) return;

            switch (Status)
            {
                case SkillStatus.Channeled:
                    DoUseWhenEfficacy(target);
                    break;
                case SkillStatus.Disabled:
                    if (Owner.Status == ObjectStatus.Alive && Owner.Energy.Value > CostEnergy)
                    {
                        Owner.Energy.Value -= CostEnergy;
                        OnStart(target);
                    }
                    break;
            }
        }

        /// <summary>
        /// 中斷技能
        /// </summary>
        public virtual void Break()
        {
            switch (Status)
            {
                case SkillStatus.Enabled:
                    Owner.Energy.Value += CostEnergy;
                    Status = SkillStatus.Disabled;
                    OnEnd(SkillEndType.CastBreak);
                    break;
                case SkillStatus.Channeled:
                    Status = SkillStatus.Cooldown;
                    OnEnd(SkillEndType.ChanneledBreak);
                    break;
            }
        }

        /// <summary>
        /// 在回合動作最後執行
        /// </summary>
        public virtual void Settlement()
        {
            switch (Status)
            {
                case SkillStatus.Channeled:
                    if (Channeled.IsFull)
                    {
                        Status = SkillStatus.Cooldown;
                        OnEnd(SkillEndType.Finish);
                        goto case SkillStatus.Cooldown;
                    }
                    else
                    {
                        Channeled.Value += Owner.Scene.SceneIntervalOfRound;
                    }

                    int costEnergy = (int)(CostEnergyPerSec / Owner.Scene.SceneRoundPerSec + 0.5F);
                    if (Owner.Energy.Value >= costEnergy)
                    {
                        Owner.Energy.Value -= costEnergy;
                    }
                    else
                    {
                        Status = SkillStatus.Cooldown;
                        OnEnd(SkillEndType.ChanneledBreak);
                        goto case SkillStatus.Cooldown;
                    }
                    break;
                case SkillStatus.Cooldown:
                    if (Cooldown.IsFull)
                    {
                        Status = SkillStatus.Disabled;
                    }
                    else
                    {
                        Cooldown.Value += Owner.Scene.SceneIntervalOfRound;
                    }
                    break;
            }
        }

        /// <summary>
        /// 重設技能
        /// </summary>
        public virtual void Reset()
        {
            Break();
            Status = SkillStatus.Disabled;
        }

        /// <summary>
        /// 當技能生效時又使用技能時的動作
        /// </summary>
        public virtual void DoUseWhenEfficacy(ITargetability target) { }

        /// <summary>
        /// 物件活動前執行動作
        /// </summary>
        public virtual void DoBeforeAction() { }

        /// <summary>
        /// 物件能量調整前執行動作
        /// </summary>
        public virtual void DoBeforeActionEnergyGet() { }

        /// <summary>
        /// 物件規劃活動前執行動作
        /// </summary>
        public virtual void DoBeforeActionPlan() { }

        /// <summary>
        /// 物件移動前執行動作
        /// </summary>
        public virtual void DoBeforeActionMove() { }

        /// <summary>
        /// 物件活動後執行動作
        /// </summary>
        public virtual void DoAfterAction() { }

        /// <summary>
        /// 繪製前執行動作
        /// </summary>
        public virtual void DoBeforeDraw(Graphics g) { }

        /// <summary>
        /// 繪製後執行動作
        /// </summary>
        public virtual void DoAfterDraw(Graphics g) { }

        /// <summary>
        /// 死亡後執行動作
        /// </summary>
        public virtual void DoAfterDead(ObjectBase killer, ObjectDeadType deadType)
        {
            Break();
        }

        /// <summary>
        /// 技能結束卻後(包含中斷)執行
        /// </summary>
        public virtual void DoAfterEnd(SkillEndType endType) { }
        #endregion

        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <returns>繪圖物件</returns>
        public abstract DrawSkillBase GetDrawObject(Color color);

        /// <summary>
        /// 取得繪圖物件
        /// </summary>
        /// <param name="color">繪製顏色</param>
        /// <returns>繪圖物件</returns>
        public virtual DrawUITextFrame GetInfoObject(Color color, Color backColor, Color borderColor)
        {
            DrawUITextFrame result = new DrawUITextFrame(color, Color.WhiteSmoke, backColor, borderColor, 2, 10, Info, _InfoFont, GlobalFormat.TopLeft);
            return result;
        }
        #endregion
    }
}
