
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 特性物件管理集合
    /// </summary>
    public class PropertyCollection
    {
        /// <summary>
        /// 內部集合物件
        /// </summary>
        private List<PropertyBase> _Collection = new List<PropertyBase>();

        /// <summary>
        /// 發生於特性群組所有者變更
        /// </summary>
        public event EventHandler OwnerChanged;

        /// <summary>
        /// 發生於特性群組所有者變更
        /// </summary>
        public void OnOwnerChanged()
        {
            foreach (PropertyBase item in _Collection)
            {
                item.Owner = Owner;
            }

            if (OwnerChanged != null)
            {
                OwnerChanged(this, new EventArgs());
            }
        }

        #region ===== 屬性 =====
        private ObjectActive _Owner;
        /// <summary>
        /// 依附的活動物件,即為群組所有者(必要)
        /// </summary>
        public ObjectActive Owner
        {
            get { return _Owner; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (value == null) return;
                _Owner = value;
                OnOwnerChanged();
            }
        }

        /// <summary>
        /// 為true時須重新取得狀態值
        /// </summary>
        private bool _SpecialStatusChanged = true;

        private SpecialStatus _SpecialStatus;
        /// <summary>
        /// 附加於群組所有者上的特殊狀態(由群組內特性物件旗標組合)
        /// </summary>
        public SpecialStatus Affix
        {
            get
            {
                if (_SpecialStatusChanged)
                {
                    SpecialStatus result = RunningBox.SpecialStatus.None;
                    foreach (PropertyBase item in _Collection)
                    {
                        if (item.Status == PropertyStatus.Enabled)
                        {
                            result |= item.Affix;
                        }
                    }
                    _SpecialStatus = result;
                    _SpecialStatusChanged = false;
                }
                return _SpecialStatus;
            }
        }

        /// <summary>
        /// 集合物件數量
        /// </summary>
        public int Count
        {
            get { return _Collection.Count; }
        }
        #endregion

        /// <summary>
        /// 初始化特性物件集合
        /// </summary>
        /// <param name="scene">所屬活動物件</param>
        public PropertyCollection(ObjectActive owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// 取得指定之索引處的元素。
        /// </summary>
        /// <param name="index">要取得之項目的以零為起始的索引。</param>
        /// <returns>指定之索引處的項目。</returns>
        public PropertyBase this[int index]
        {
            get { return _Collection[index]; }
        }

        #region ===== 集合項目調整 =====
        /// <summary>
        /// 增加特性物件到活動集合內
        /// </summary>
        /// <param name="item">特性物件</param>
        public void Add(PropertyBase item)
        {
            item.Owner = Owner;
            item.StatusChanged += ItemStatusChanged;
            item.SpecialStatusChanged += ItemSpecialStatusChanged;
            if (item.Status == PropertyStatus.Enabled)
            {
                _SpecialStatus |= item.Affix;
            }
            _Collection.Add(item);
        }

        /// <summary>
        /// 從活動集合內移除指定特性物件
        /// </summary>
        /// <param name="item">特性物件</param>
        /// <returns>如果成功移除特性物件則為 true，否則為 false。</returns>
        public bool Remove(PropertyBase item)
        {
            bool result = _Collection.Remove(item);
            if (result)
            {
                if (item.Status == PropertyStatus.Enabled)
                {
                    _SpecialStatusChanged = true;
                }
                item.StatusChanged -= ItemStatusChanged;
                item.SpecialStatusChanged -= ItemSpecialStatusChanged;
                item.End(PropertyEndType.Break);
            }
            return result;
        }

        /// <summary>
        /// 清空技能集合
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].StatusChanged -= ItemStatusChanged;
                _Collection[i].SpecialStatusChanged -= ItemSpecialStatusChanged;
                _Collection[i].End(PropertyEndType.Break);
            }
            _SpecialStatus = RunningBox.SpecialStatus.None;
            _SpecialStatusChanged = false;
            _Collection.Clear();
        }

        /// <summary>
        /// 清除集合內所有失效的特性物件
        /// </summary>
        public void ClearAllDisabled()
        {
            List<PropertyBase> disabledPropertys = new List<PropertyBase>();
            for (int i = 0; i < _Collection.Count; i++)
            {
                PropertyBase item = _Collection[i];
                if (item.Status == PropertyStatus.Disabled)
                {
                    disabledPropertys.Add(item);
                }
            }

            if (disabledPropertys.Count == 0) return;
            foreach (PropertyBase disabledProperty in disabledPropertys)
            {
                disabledProperty.StatusChanged -= ItemStatusChanged;
                disabledProperty.SpecialStatusChanged -= ItemSpecialStatusChanged;
                _Collection.Remove(disabledProperty);
            }
        }
        #endregion

        #region ===== 集合項目動作 =====
        /// <summary>
        /// 所有集合內特性物件執行DoBeforeRound方法
        /// </summary>
        public void AllDoBeforeAction()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].DoBeforeAction();
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoBeforeActionEnergyGet方法
        /// </summary>
        public void AllDoBeforeActionEnergyGet()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].DoBeforeActionEnergyGet();
            }

        }

        /// <summary>
        /// 所有集合內特性物件執行DoBeforeActionPlan方法
        /// </summary>
        public void AllDoBeforeActionPlan()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].DoBeforeActionPlan();
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoBeforeActionMove方法
        /// </summary>
        public void AllDoBeforeActionMove()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].DoBeforeActionMove();
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoActionMoving方法
        /// </summary>
        public void AllDoActionMoving()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].DoActionMoving();
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoAfterAction方法
        /// </summary>
        public void AllDoAfterAction()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].DoAfterAction();
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoBeforeDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoBeforeDraw(Graphics g)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].DoBeforeDraw(g);
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoAfterDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoAfterDraw(Graphics g)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].DoAfterDraw(g);
            }
        }

        /// <summary>
        /// 所有特性進入回合結算
        /// </summary>
        public void AllSettlement()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].Settlement();
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoAfterDead方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoAfterDead(ObjectActive killer, ObjectDeadType deadType)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].DoAfterDead(killer, deadType);
            }
        }

        /// <summary>
        /// 中斷所有特性
        /// </summary>
        public void AllBreak()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].End(PropertyEndType.Break);
            }
        }
        #endregion

        /// <summary>
        /// 判斷指定特性物件是否存在集合內
        /// </summary>
        /// <param name="item">特性物件</param>
        /// <returns>如果特性物件在集合中則為 true，否則為 false。</returns>
        public bool Contains(PropertyBase item)
        {
            return _Collection.Contains(item);
        }

        /// <summary>
        /// 取得指定類型的特性
        /// </summary>
        /// <param name="item">特性物件</param>
        public List<T> GetPropertyByType<T>() where T : PropertyBase
        {
            List<T> result = new List<T>();
            foreach (PropertyBase property in _Collection)
            {
                T converted = property as T;
                if (converted != null)
                {
                    result.Add(converted);
                }
            }
            return result;
        }

        /// <summary>
        /// 集合內特性狀態變更動作
        /// </summary>
        private void ItemStatusChanged(object sender, EventArgs e)
        {
            PropertyBase item = sender as PropertyBase;
            if (item.Affix == RunningBox.SpecialStatus.None) return;
            if (item.Status == PropertyStatus.Enabled)
            {
                _SpecialStatus |= item.Affix;
            }
            else
            {
                _SpecialStatusChanged = true;
            }
        }

        /// <summary>
        /// 集合內特性狀態變更動作
        /// </summary>
        private void ItemSpecialStatusChanged(object sender, EventArgs e)
        {
            _SpecialStatusChanged = true;
        }

        //禁用Foreach避免新增時錯誤
        //public IEnumerator<PropertyBase> GetEnumerator()
        //{
        //    return _Collection.GetEnumerator();
        //}
    }
}
