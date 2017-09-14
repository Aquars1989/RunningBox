
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
        private List<SkillBase> _Collection;

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
        /// 初始化技能物件集合
        /// </summary>
        /// <param name="scene">所屬活動物件</param>
        public SkillCollection(ObjectActive owner)
        {
            Owner = owner;
            _Collection = new List<SkillBase>();
        }

        /// <summary>
        /// 取得指定之索引處的元素。
        /// </summary>
        /// <param name="index">要取得之項目的以零為起始的索引。</param>
        /// <returns>指定之索引處的項目。</returns>
        public SkillBase this[int index]
        {
            get { return _Collection[index]; }
        }

        /// <summary>
        /// 增加技能物件到活動集合內
        /// </summary>
        /// <param name="item">技能物件</param>
        public void Add(SkillBase item)
        {
            item.Owner = Owner;
            _Collection.Add(item);
        }

        /// <summary>
        /// 從活動集合內移除指定技能物件
        /// </summary>
        /// <param name="item">技能物件</param>
        /// <returns>如果成功移除技能物件則為 true，否則為 false。</returns>
        public bool Remove(SkillBase item)
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
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                item.Owner = null;
            }
            _Collection.Clear();
        }

        /// <summary>
        /// 判斷指定技能物件是否存在集合內
        /// </summary>
        /// <param name="item">技能物件</param>
        /// <returns>如果技能物件在集合中則為 true，否則為 false。</returns>
        public bool Contains(SkillBase item)
        {
            return _Collection.Contains(item);
        }

        /// <summary>
        /// 所有集合內技能物件檢查自動施放方法
        /// </summary>
        public void AllDoAutoCast()
        {
            if (_Collection.Find(x => { return x.Status == SkillStatus.Enabled || x.Status == SkillStatus.Channeled; }) != null) return;
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                if (item.AutoCastObject == null) return;
                if (item.AutoCastObject.Check(item)) return;
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoBeforeRound方法
        /// </summary>
        public void AllDoBeforeAction()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                item.DoBeforeAction();
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoBeforeActionEnergyGet方法
        /// </summary>
        public void AllDoBeforeActionEnergyGet()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                item.DoBeforeActionEnergyGet();
            }

        }
        /// <summary>
        /// 所有集合內技能物件執行DoBeforeActionPlan方法
        /// </summary>
        public void AllDoBeforeActionPlan()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                item.DoBeforeActionPlan();
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoBeforeActionMove方法
        /// </summary>
        public void AllDoBeforeActionMove()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                item.DoBeforeActionMove();
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoAfterAction方法
        /// </summary>
        public void AllDoAfterAction()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                item.DoAfterAction();
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoBeforeDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoBeforeDraw(Graphics g)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                item.DoBeforeDraw(g);
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoAfterDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoAfterDraw(Graphics g)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                item.DoAfterDraw(g);
            }
        }

        /// <summary>
        /// 所有技能進入回合結算
        /// </summary>
        public void AllSettlement()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                item.Settlement();
            }
        }

        /// <summary>
        /// 所有集合內技能物件執行DoAfterAction方法
        /// </summary>
        public void AllDoAfterDead(ObjectBase killer, ObjectDeadType deadType)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                item.DoAfterDead(killer, deadType);
            }
        }

        /// <summary>
        /// 中斷所有集合內技能
        /// </summary>
        public void AllBreak()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                SkillBase item = _Collection[i];
                item.Break();
            }
        }

        //禁用Foreach避免新增時錯誤
        //public IEnumerator<SkillBase> GetEnumerator()
        //{
        //    return _Collection.GetEnumerator();
        //}
    }
}
