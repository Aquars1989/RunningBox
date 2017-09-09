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
        /// 發生於目標變更
        /// </summary>
        public event EventHandler TargetChanged;

        /// <summary>
        /// 發生於附加於物件的特殊狀態變更
        /// </summary>
        public event EventHandler SpecialStatusChanged;

        /// <summary>
        /// 發生於特性狀態變更
        /// </summary>
        public event EventHandler StatusChanged;

        /// <summary>
        /// 發生於所有者變更
        /// </summary>
        public event EventHandler OwnerChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於目標變更
        /// </summary>
        protected virtual void OnTargetChanged()
        {
            if (TargetChanged != null)
            {
                TargetChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於附加於物件的特殊狀態變更
        /// </summary>
        protected virtual void OnSpecialStatusChanged()
        {
            if (SpecialStatusChanged != null)
            {
                SpecialStatusChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於特性狀態變更
        /// </summary>
        protected virtual void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生所有者變更
        /// </summary>
        protected virtual void OnOwnerChanged()
        {
            if (OwnerChanged != null)
            {
                OwnerChanged(this, new EventArgs());
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
                _Affix = value;
                OnSpecialStatusChanged();
            }
        }

        private ITarget _Target;
        /// <summary>
        /// 特性目標(必要)
        /// </summary>
        public ITarget Target
        {
            get { return _Target; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_Target == value) return;
                _Target = value;
                OnTargetChanged();
            }
        }

        private ObjectActive _Owner;
        /// <summary>
        /// 特性所有者(必要,上層設定)
        /// </summary>
        public ObjectActive Owner
        {
            get { return _Owner; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_Owner == value) return;
                _Owner = value;
                OnOwnerChanged();
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
                _Status = value;
                OnStatusChanged();
            }
        }

        /// <summary>
        /// 特性持續時間計時器(毫秒)
        /// </summary>
        public CounterObject DurationTime { get; private set; }
        #endregion

        /// <summary>
        /// 特性基礎物件初始化
        /// </summary>
        public PropertyBase(ITarget target)
        {
            Target = target;
            DurationTime = new CounterObject(-1);
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
        public virtual void DoAfterDead(ObjectActive killer, ObjectDeadType deadType)
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
