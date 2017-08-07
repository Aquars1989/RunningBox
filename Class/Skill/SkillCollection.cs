
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 技能物件管理集合
    /// </summary>
    public class SkillCollection
    {
        private List<ISkill> _Collection;

        /// <summary>
        /// 歸屬的活動物件
        /// </summary>
        public ObjectBase Owner { get; set; }

        /// <summary>
        /// 集合物件數量
        /// </summary>
        public int Count
        {
            get { return _Collection.Count; }
        }

        /// <summary>
        /// 初始化技能物件集合
        /// </summary>
        /// <param name="scene">所屬場景物件</param>
        public SkillCollection(ObjectBase owner)
        {
            Owner = owner;
            _Collection = new List<ISkill>();
        }

        /// <summary>
        /// 取得指定之索引處的元素。
        /// </summary>
        /// <param name="index">要取得之項目的以零為起始的索引。</param>
        /// <returns>指定之索引處的項目。</returns>
        public ISkill this[int index]
        {
            get { return _Collection[index]; }
        }

        /// <summary>
        /// 增加技能物件到活動集合內
        /// </summary>
        /// <param name="item">技能物件</param>
        public void Add(ISkill item)
        {
            item.Owner = Owner;
            _Collection.Add(item);
        }

        /// <summary>
        /// 從活動集合內移除指定技能物件
        /// </summary>
        /// <param name="item">技能物件</param>
        /// <returns>如果成功移除技能物件則為 true，否則為 false。</returns>
        public bool Remove(ISkill item)
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
            foreach (ISkill item in _Collection)
            {
                item.Owner = null;
            }
            _Collection.Clear();
        }

        /// <summary>
        /// 判斷指定技能物件是否存在集合內
        /// </summary>
        /// <param name="item">技能物件</param>
        /// <returns>如果技能物件在集合中則為 true，否則為 false。</returns>
        public bool Contains(ISkill item)
        {
            return _Collection.Contains(item);
        }

        /// <summary>
        /// 所有集合內技能物件執行DoBeforeRound方法
        /// </summary>
        public void AllDoBeforeAction()
        {
            foreach (ISkill item in _Collection)
            {
                item.DoBeforeAction();
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoBeforeActionEnergyGet方法
        /// </summary>
        public void AllDoBeforeActionEnergyGet()
        {
            foreach (ISkill item in _Collection)
            {
                item.DoBeforeActionEnergyGet();
            }

        }
        /// <summary>
        /// 所有集合內技能物件執行DoBeforeActionPlan方法
        /// </summary>
        public void AllDoBeforeActionPlan()
        {
            foreach (ISkill item in _Collection)
            {
                item.DoBeforeActionPlan();
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoBeforeActionMove方法
        /// </summary>
        public void AllDoBeforeActionMove()
        {
            foreach (ISkill item in _Collection)
            {
                item.DoBeforeActionMove();
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoAfterAction方法
        /// </summary>
        public void AllDoAfterAction()
        {
            foreach (ISkill item in _Collection)
            {
                item.DoAfterAction();
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoBeforeDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoBeforeDraw(Graphics g)
        {
            foreach (ISkill item in _Collection)
            {
                item.DoBeforeDraw(g);
            }
        }
        
        /// <summary>
        /// 所有集合內技能物件執行DoAfterDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoAfterDraw(Graphics g)
        {
            foreach (ISkill item in _Collection)
            {
                item.DoAfterDraw(g);
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoAfterDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoAfterDraw(ObjectActive killer)
        {
            foreach (ISkill item in _Collection)
            {
                item.DoAfterDead(killer);
            }
        }

        /// <summary>
        /// 中斷所有集合內技能
        /// </summary>
        public void AllBreak()
        {
            foreach (ISkill item in _Collection)
            {
                item.Break();
            }
        }

        public IEnumerator<ISkill> GetEnumerator()
        {
            return _Collection.GetEnumerator();
        }
    }
}
