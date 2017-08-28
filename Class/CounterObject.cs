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
                _Limit = value;
                if (Limit >= 0 && _Value > Limit && !Overload) _Value = Limit;
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
                _Value = Math.Min(value, int.MaxValue);

                if (_Value < 0) _Value = 0;
                else if (Limit >= 0 && _Value > Limit && !Overload) _Value = Limit;
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
                _Overload = value;
                if (Limit >= 0 && _Value > Limit && !Overload) _Value = Limit;
            }
        }

        /// <summary>
        /// 新增技數器物件
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
        /// 取得目前比例
        /// </summary>
        /// <returns></returns>
        public float GetRatio()
        {
            return _Value / (float)_Limit;
        }
    }
}
