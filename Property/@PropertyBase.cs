using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 特性基礎物件
    /// </summary>
    public abstract class PropertyBase
    {
        #region ===== 事件 =====
        /// <summary>
        /// 發生於目標內容變更
        /// </summary>
        public event ValueChangedEnentHandle TargetObjectChanged;

        /// <summary>
        /// 發生於附加於物件的特殊狀態變更
        /// </summary>
        public event ValueChangedEnentHandle AffixChanged;

        /// <summary>
        /// 發生於特性狀態變更
        /// </summary>
        public event ValueChangedEnentHandle StatusChanged;

        /// <summary>
        /// 發生於依附物件變更時(依附物件可為集合 場景 物件)
        /// </summary>
        public event EventHandler BindingChanged;

        /// <summary>
        /// 發生於特性結束時
        /// </summary>
        public event PropertyEndEnentHandle End;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於目標內容變更
        /// </summary>
        protected virtual void OnTargetObjectChanged(object oldValue, object newValue)
        {
            if (TargetObjectChanged != null)
            {
                TargetObjectChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於附加於物件的特殊狀態變更
        /// </summary>
        protected virtual void OnAffixChanged(object oldValue, object newValue)
        {
            if (AffixChanged != null)
            {
                AffixChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於特性狀態變更
        /// </summary>
        protected virtual void OnStatusChanged(object oldValue, object newValue)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於依附物件變更時(依附物件可為場景 物件)
        /// </summary>
        protected virtual void OnBindingChanged()
        {
            if (BindingChanged != null)
            {
                BindingChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於技能結束時
        /// </summary>
        /// <param name="endType">結束方式</param>
        protected virtual void OnEnd(PropertyEndType endType)
        {
            DoBeforeEnd(endType);
            Status = PropertyStatus.Disabled;

            if (End != null)
            {
                End(this, endType);
            }
        }
        #endregion

        #region ===== 屬性 =====
        private bool _BreakAfterDead = true;
        /// <summary>
        /// 死亡時是否中斷
        /// </summary>
        public bool BreakAfterDead
        {
            get { return _BreakAfterDead; }
            set
            {
                if (_Affix == value) return;
                object oldValue = _Affix;
                _Affix = value;
                OnAffixChanged(oldValue, value);
            }
        }

        private SpecialStatus _Affix = SpecialStatus.None;
        /// <summary>
        /// 附加特殊狀態
        /// </summary>
        public SpecialStatus Affix
        {
            get { return _Affix; }
            set
            {
                if (_Affix == value) return;
                object oldValue = _Affix;
                _Affix = value;
                OnAffixChanged(oldValue, value);
            }
        }

        private PropertyCollection _Container;
        /// <summary>
        /// 取得特性歸屬集合
        /// </summary>
        public PropertyCollection Container
        {
            get { return _Container; }
            private set
            {
                if (_Container == value) return;
                _Container = value;
            }
        }

        private ObjectBase _Owner;
        /// <summary>
        /// 取得特性所有人
        /// </summary>
        public ObjectBase Owner
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

        private PropertyStatus _Status = PropertyStatus.Enabled;
        /// <summary>
        /// 特性狀態
        /// </summary>
        public PropertyStatus Status
        {
            get { return _Status; }
            private set
            {
                if (_Status == value) return;
                object oldValue = _Status;
                _Status = value;
                OnStatusChanged(oldValue, value);
            }
        }

        /// <summary>
        /// 特性目標(必要)
        /// </summary>
        public TargetSet Target { get; private set; }

        /// <summary>
        /// 特性持續時間計時器(毫秒)
        /// </summary>
        public CounterObject DurationTime { get; private set; }
        #endregion

        #region ***** 建構式 *****
        /// <summary>
        /// 特性基礎物件初始化
        /// </summary>
        public PropertyBase()
        {
            Target = new TargetSet();
            DurationTime = new CounterObject(-1);

            Target.ObjectChanged += (s, o, n) => { OnTargetObjectChanged(o, n); };
        }
        #endregion

        #region ===== 方法 =====
        /// <summary>
        /// 綁定特性到場景
        /// </summary>
        public void Binding(SceneBase scene, bool skipCheck = false)
        {
            if (_Scene == scene) return;
            if (!skipCheck && Container != null && Container.Contains(this))
            {
                throw new Exception("特性已在集合內無法手動綁定");
            }

            Break();
            Container = null;
            Owner = null;
            Scene = Scene;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定特性到物件
        /// </summary>
        public void Binding(ObjectActive owner, bool skipCheck = false)
        {
            if (_Owner == owner) return;
            if (!skipCheck && Container != null && Container.Contains(this))
            {
                throw new Exception("特性已在集合內無法手動綁定");
            }

            Break();
            Container = null;
            Owner = owner;
            Scene = null;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定特性到集合(集合內綁定)
        /// </summary>
        public void Binding(PropertyCollection collection, bool skipCheck = false)
        {
            if (_Container == collection) return;
            if (!skipCheck && Container != null && Container.Contains(this))
            {
                throw new Exception("特性已在集合內無法手動綁定");
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
                throw new Exception("特性已在集合內無法手動綁定");
            }

            Break();
            Container = null;
            Owner = null;
            Scene = null;
            OnBindingChanged();
        }

        #region ##### 場景中動作(須有所有者) #####
        /// <summary>
        /// 取消特性效果
        /// </summary>
        public virtual void Break()
        {
            if (Status == PropertyStatus.Disabled) return;
            OnEnd(PropertyEndType.Break);
        }

        /// <summary>
        /// 在回合動作最後執行
        /// </summary>
        public virtual void Settlement()
        {
            if (Status == PropertyStatus.Enabled)
            {
                if (DurationTime.IsFull)
                {
                    OnEnd(PropertyEndType.Finish);
                }
                else
                {
                    DurationTime.Value += Owner.Scene.SceneIntervalOfRound;
                }
            }
        }

        /// <summary>
        /// 所有者死亡後執行動作(供上層呼叫),預設為取消特性
        /// </summary>
        public virtual void DoAfterDead(ObjectBase killer, ObjectDeadType deadType)
        {
            Break();
        }

        /// <summary>
        /// 物件活動前執行動作(供上層呼叫)
        /// </summary>
        public virtual void DoBeforeAction() { }

        /// <summary>
        /// 物件能量調整前執行動作(供上層呼叫)
        /// </summary>
        public virtual void DoBeforeActionEnergyGet() { }

        /// <summary>
        /// 物件規劃活動前執行動作(供上層呼叫)
        /// </summary>
        public virtual void DoBeforeActionPlan() { }

        /// <summary>
        /// 物件移動前執行動作(供上層呼叫)
        /// </summary>
        public virtual void DoBeforeActionMove() { }

        /// <summary>
        /// 物件移動中執行動作(供上層呼叫)
        /// </summary>
        public virtual void DoActionMoving() { }

        /// <summary>
        /// 物件活動後執行動作(供上層呼叫)
        /// </summary>
        public virtual void DoAfterAction() { }

        /// <summary>
        /// 繪製前執行動作(供上層呼叫)
        /// </summary>
        public virtual void DoBeforeDraw(Graphics g) { }

        /// <summary>
        /// 繪製後執行動作(供上層呼叫)
        /// </summary>
        public virtual void DoAfterDraw(Graphics g) { }

        /// <summary>
        /// 特性結束前執行(供上層呼叫)
        /// </summary>
        public virtual void DoBeforeEnd(PropertyEndType endType) { }
        #endregion
        #endregion
    }
}
