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
        /// 發生於所有者變更
        /// </summary>
        public event ValueChangedEnentHandle OwnerChanged;
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
        /// 發生所有者變更
        /// </summary>
        protected virtual void OnOwnerChanged(object oldValue, object newValue)
        {
            if (OwnerChanged != null)
            {
                OwnerChanged(this, oldValue, newValue);
            }
        }
        #endregion

        #region ===== 屬性 =====
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

        private ObjectBase _Owner;
        /// <summary>
        /// 特性所有者(必要,上層設定)
        /// </summary>
        public ObjectBase Owner
        {
            get { return _Owner; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_Owner == value) return;
                object oldValue = _Owner;
                _Owner = value;
                OnOwnerChanged(oldValue, value);
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

        /// <summary>
        /// 特性基礎物件初始化
        /// </summary>
        public PropertyBase()
        {
            Target = new TargetSet();
            DurationTime = new CounterObject(-1);

            Target.ObjectChanged += (s, o, n) => { OnTargetObjectChanged(o, n); };
        }

        #region ===== 方法 =====
        /// <summary>
        /// 取消特性效果
        /// </summary>
        public virtual void End(PropertyEndType endType)
        {
            if (Status == PropertyStatus.Disabled) return;

            DoBeforeEnd(endType);
            Status = PropertyStatus.Disabled;
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
                    End(PropertyEndType.Finish);
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
            End(PropertyEndType.Break);
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
    }
}
