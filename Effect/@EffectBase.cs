using System;
using System.Drawing;

namespace RunningBox
{
    /// <summary>
    /// 特效介面
    /// </summary>
    public abstract class EffectBase
    {
        #region ===== 事件 =====
        /// <summary>
        /// 發生於特效狀態變更
        /// </summary>
        public event ValueChangedEnentHandle<EffectStatus> StatusChanged;

        /// <summary>
        /// 發生於特性結束時
        /// </summary>
        public event EffectEndEnentHandle End;

        /// <summary>
        /// 發生於所屬物件變更時(所屬物件可為集合>場景)
        /// </summary>
        public event EventHandler BindingChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於附加於物件的特殊狀態變更
        /// </summary>
        protected virtual void OnStatusChanged(EffectStatus oldValue, EffectStatus newValue)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於所屬物件變更時(所屬物件可為集合>場景)
        /// </summary>
        protected virtual void OnBindingChanged()
        {
            if (BindingChanged != null)
            {
                BindingChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生特效結束時
        /// </summary>
        /// <param name="endType">結束方式</param>
        protected virtual void OnEnd(EffectEndType endType)
        {
            DoBeforeEnd(endType);
            Status = DisablingTime.Limit == 0 ? EffectStatus.Disabled : EffectStatus.Disabling;

            if (End != null)
            {
                End(this, endType);
            }
        }
        #endregion

        #region ===== 屬性 =====
        /// <summary>
        /// 是否鎖定綁定功能
        /// </summary>
        public bool BindingLock { get; private set; }

        private EffectCollection _Container;
        /// <summary>
        /// 取得特效歸屬集合(集合>場景)
        /// </summary>
        public EffectCollection Container
        {
            get { return _Container; }
            private set
            {
                if (_Container == value) return;
                _Container = value;
            }
        }

        private SceneBase _Scene;
        /// <summary>
        /// 取得歸屬場景(集合>場景)
        /// </summary>
        public SceneBase Scene
        {
            get { return Container == null ? _Scene : Container.Scene; }
            private set
            {
                if (_Scene == value) return;
                _Scene = value;
            }
        }

        /// <summary>
        /// 是否可被中斷
        /// </summary>
        public bool CanBreak { get; set; }

        private EffectStatus _Status;
        /// <summary>
        /// 特性狀態
        /// </summary>
        public EffectStatus Status
        {
            get { return _Status; }
            protected set
            {
                if (_Status == value) return;
                EffectStatus oldValue = _Status;
                _Status = value;
                OnStatusChanged(oldValue, value);
            }
        }

        /// <summary>
        /// 生效時間計時器(毫秒)
        /// </summary>
        public CounterObject DurationTime { get; private set; }

        /// <summary>
        /// 持續時間計時器(毫秒)
        /// </summary>
        public CounterObject EnablingTime { get; private set; }

        /// <summary>
        /// 消退時間計時器(毫秒)
        /// </summary>
        public CounterObject DisablingTime { get; private set; }
        #endregion

        #region ***** 建構式 *****
        /// <summary>
        /// 基本特效建構式
        /// </summary>
        /// <param name="enablingTime">生效時間</param>
        /// <param name="durationTime">持續時間</param>
        /// <param name="disablingTime">消退時間</param>
        public EffectBase(int enablingTime, int durationTime, int disablingTime)
        {
            EnablingTime = new CounterObject(enablingTime);
            DurationTime = new CounterObject(durationTime);
            DisablingTime = new CounterObject(disablingTime);
            _Status = enablingTime == 0 ? EffectStatus.Enabled : EffectStatus.Enabling;
            CanBreak = true;
        }
        #endregion

        #region ===== 方法 =====
        /// <summary>
        /// 綁定特性到場景(集合>場景)
        /// </summary>
        /// <param name="scene">場景</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public void Binding(SceneBase scene, bool bindingLock = false)
        {
            if (_Scene == scene) return;
            if (BindingLock) throw new Exception("特性已被鎖定無法綁定");

            Container = null;
            Scene = scene;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定特性到集合(集合>場景,集合內綁定,除此之外勿使用此函數)
        /// </summary>
        /// <param name="collection">集合</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public void Binding(EffectCollection collection, bool bindingLock = false)
        {
            if (_Container == collection) return;
            if (BindingLock) throw new Exception("特效已被鎖定無法綁定");
            if (collection != null && !collection.Contains(this))
            {
                throw new Exception("特效不在集合中");
            }

            Container = collection;
            Scene = null;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 清除綁定
        /// </summary>
        public void ClearBinding()
        {
            if (BindingLock) throw new Exception("特效已被鎖定無法解除綁定");

            Container = null;
            Scene = null;
            OnBindingChanged();
        }

        /// <summary>
        /// 解除綁定鎖定
        /// </summary>
        public void BindingUnlock()
        {
            BindingLock = false;
        }

        #region ##### 場景中動作(須有所有者) #####
        /// <summary>
        /// 中斷特效
        /// </summary>
        public virtual void Break()
        {
            if (!CanBreak || Status == EffectStatus.Disabled || Status == EffectStatus.Disabling) return;
            OnEnd(EffectEndType.Break);
        }


        /// <summary>
        /// 物件活動前執行動作
        /// </summary>
        public virtual void DoBeforeRound() { }

        /// <summary>
        /// 物件活動後執行動作
        /// </summary>
        public virtual void DoAfterRound() { }

        /// <summary>
        /// 背景繪製前執行動作
        /// </summary>
        public virtual void DoBeforeDraw(Graphics g) { }

        /// <summary>
        /// 繪製背景前執行動作
        /// </summary>
        public virtual void DoBeforeDrawFloor(Graphics g) { }

        /// <summary>
        /// 背景繪製後，物件繪製前執行動作
        /// </summary>
        public virtual void DoBeforeDrawObject(Graphics g) { }

        /// <summary>
        /// 繪製UI前執行動作
        /// </summary>
        public virtual void DoBeforeDrawUI(Graphics g) { }

        /// <summary>
        /// 繪製後執行動作
        /// </summary>
        public virtual void DoAfterDraw(Graphics g) { }

        /// <summary>
        /// 特效結束前執行
        /// </summary>
        public virtual void DoBeforeEnd(EffectEndType endType) { }

        /// <summary>
        /// 在回合動作最後執行
        /// </summary>
        public virtual void Settlement()
        {
            switch (Status)
            {
                case EffectStatus.Enabling:
                    if (EnablingTime.IsFull)
                    {
                        Status = EffectStatus.Enabled;
                        goto case EffectStatus.Enabled;
                    }
                    else
                    {
                        EnablingTime.Value += Scene.SceneIntervalOfRound;
                    }
                    break;
                case EffectStatus.Enabled:
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
                case EffectStatus.Disabling:
                    if (DisablingTime.IsFull)
                    {
                        Status = EffectStatus.Disabled;
                    }
                    else
                    {
                        DisablingTime.Value += Scene.SceneIntervalOfRound;
                    }
                    break;
            }
        }
        #endregion
        #endregion
    }
}
