using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 計數器物件
    /// </summary>
    public class CounterObject
    {
        #region ===== 事件 =====
        /// <summary>
        /// 發生於計數值變更
        /// </summary>
        public event ValueChangedEnentHandle<int> ValueChanged;

        /// <summary>
        /// 發生於上限值變更
        /// </summary>
        public event ValueChangedEnentHandle<int> LimitChanged;

        /// <summary>
        /// 發生於是否可超出最大值變更
        /// </summary>
        public event ValueChangedEnentHandle<bool> OverloadChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於計數值變更
        /// </summary>
        protected void OnValueChanged(int oldValue, int newValue)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於上限值變更
        /// </summary>
        protected void OnLimitChanged(int oldValue, int newValue)
        {
            if (Limit >= 0 && _Value > Limit && !Overload) _Value = Limit;
            if (LimitChanged != null)
            {
                LimitChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於是否可超出最大值變更
        /// </summary>
        protected void OnOverloadChanged(bool oldValue, bool newValue)
        {
            if (Limit >= 0 && _Value > Limit && !Overload) _Value = Limit;
            if (OverloadChanged != null)
            {
                OverloadChanged(this, oldValue, newValue);
            }
        }
        #endregion

        #region ===== 屬性 =====
        /// <summary>
        /// 是否已滿
        /// </summary>
        public bool IsFull { get { return Limit >= 0 && Value >= Limit; } }

        private int _Limit;
        /// <summary>
        /// 上限值,負數為不限制
        /// </summary>
        public int Limit
        {
            get { return _Limit; }
            set
            {
                if (_Limit == value) return;
                int oldValue = _Limit;
                _Limit = value;
                OnLimitChanged(oldValue, value);
            }
        }

        private int _Value;
        /// <summary>
        /// 目前值
        /// </summary>
        public int Value
        {
            get { return _Value; }
            set
            {
                if (value < 0) value = 0;
                else if (Limit >= 0 && value > Limit && !Overload) value = Limit;

                if (_Value == value) return;
                int oldValue = _Value;
                _Value = value;
                OnValueChanged(oldValue, value);
            }
        }

        private bool _Overload;
        /// <summary>
        /// 計數值是否可超出最大值
        /// </summary>
        public bool Overload
        {
            get { return _Overload; }
            set
            {
                if (_Overload == value) return;
                bool oldValue = _Overload;
                _Overload = value;
                OnOverloadChanged(oldValue, value);
            }
        }
        #endregion

        /// <summary>
        /// 新增計數器物件
        /// </summary>
        /// <param name="limit">計數器最大值</param>
        /// <param name="value">計數器目前數值</param>
        /// <param name="overload">計數值是否可超出最大值</param>
        public CounterObject(int limit, int value = 0, bool overload = true)
        {
            _Limit = limit;
            _Overload = overload;
            Value = value;
        }


        /// <summary>
        /// 取得目前比例0~1(無上限時為0)
        /// </summary>
        /// <returns></returns>
        public float GetRatio()
        {
            return _Limit < 1 ? 0 : _Value / (float)_Limit;
        }
    }
}
