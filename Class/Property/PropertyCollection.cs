
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
        private List<PropertyBase> _Collection;

        /// <summary>
        /// 歸屬的活動物件
        /// </summary>
        public ObjectActive Owner { get; set; }

        /// <summary>
        /// 集合物件數量
        /// </summary>
        public int Count
        {
            get { return _Collection.Count; }
        }

        /// <summary>
        /// 初始化特性物件集合
        /// </summary>
        /// <param name="scene">所屬活動物件</param>
        public PropertyCollection(ObjectActive owner)
        {
            Owner = owner;
            _Collection = new List<PropertyBase>();
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

        /// <summary>
        /// 增加特性物件到活動集合內
        /// </summary>
        /// <param name="item">特性物件</param>
        public void Add(PropertyBase item)
        {
            item.Owner = Owner;
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
                item.Owner = null;
            }
            return result;
        }

        /// <summary>
        /// 清空技能集合
        /// </summary>
        public void Clear()
        {
            foreach (PropertyBase item in _Collection)
            {
                item.Owner = null;
            }
            _Collection.Clear();
        }

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
        /// 所有集合內特性物件執行DoBeforeRound方法
        /// </summary>
        public void AllDoBeforeAction()
        {
            foreach (PropertyBase item in _Collection)
            {
                item.DoBeforeAction();
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoBeforeActionEnergyGet方法
        /// </summary>
        public void AllDoBeforeActionEnergyGet()
        {
            foreach (PropertyBase item in _Collection)
            {
                item.DoBeforeActionEnergyGet();
            }

        }
        /// <summary>
        /// 所有集合內特性物件執行DoBeforeActionPlan方法
        /// </summary>
        public void AllDoBeforeActionPlan()
        {
            foreach (PropertyBase item in _Collection)
            {
                item.DoBeforeActionPlan();
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoBeforeActionMove方法
        /// </summary>
        public void AllDoBeforeActionMove()
        {
            foreach (PropertyBase item in _Collection)
            {
                item.DoBeforeActionMove();
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoAfterAction方法
        /// </summary>
        public void AllDoAfterAction()
        {
            foreach (PropertyBase item in _Collection)
            {
                item.DoAfterAction();
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoBeforeDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoBeforeDraw(Graphics g)
        {
            foreach (PropertyBase item in _Collection)
            {
                item.DoBeforeDraw(g);
            }
        }
        
        /// <summary>
        /// 所有集合內特性物件執行DoAfterDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoAfterDraw(Graphics g)
        {
            foreach (PropertyBase item in _Collection)
            {
                item.DoAfterDraw(g);
            }
        }

        /// <summary>
        /// 所有集合內特性物件執行DoAfterDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoAfterDraw(ObjectActive killer)
        {
            foreach (PropertyBase item in _Collection)
            {
                item.DoAfterDead(killer);
            }
        }

        /// <summary>
        /// 所有特性進入回合結算
        /// </summary>
        public void AllSettlement()
        {
            foreach (PropertyBase item in _Collection)
            {
                item.Settlement();
            }
        }

        /// <summary>
        /// 中斷所有特性
        /// </summary>
        public void AllBreak()
        {
            foreach (PropertyBase item in _Collection)
            {
                item.Break();
            }
        }

        /// <summary>
        /// 清除集合內所有失效的特性物件
        /// </summary>
        public void ClearAllDisabled()
        {
            List<PropertyBase> disabledPropertys = new List<PropertyBase>();
            foreach (PropertyBase item in _Collection)
            {
                if (item.Status ==  PropertyStatus.Disabled)
                {
                    disabledPropertys.Add(item);
                }
            }

            if (disabledPropertys.Count == 0) return;
            foreach (PropertyBase disabledEffect in disabledPropertys)
            {
                _Collection.Remove(disabledEffect);
            }
        }

        public IEnumerator<PropertyBase> GetEnumerator()
        {
            return _Collection.GetEnumerator();
        }
    }
}
