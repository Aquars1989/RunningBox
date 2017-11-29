
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
        /// <summary>
        /// 內部集合物件
        /// </summary>
        private List<SkillBase> _Collection = new List<SkillBase>();

        #region ===== 事件 =====
        /// <summary>
        /// 發生於所屬物件變更時(所屬物件可為所有人>場景)
        /// </summary>
        public event EventHandler BindingChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於所屬物件變更時(所屬物件可為所有人>場景)
        /// </summary>
        protected virtual void OnBindingChanged()
        {
            if (BindingChanged != null)
            {
                BindingChanged(this, new EventArgs());
            }
        }
        #endregion

        #region ===== 屬性 =====
        /// <summary>
        /// 是否鎖定綁定功能
        /// </summary>
        public bool BindingLock { get; private set; }

        private ObjectActive _Owner;
        /// <summary>
        /// 取得歸屬的活動物件
        /// </summary>
        public ObjectActive Owner
        {
            get { return _Owner; }
            private set
            {
                if (_Owner == value) return;
                _Owner = value;
            }
        }

        private SceneBase _Scene;
        /// <summary>
        /// 取得歸屬的場景物件
        /// </summary>
        public SceneBase Scene
        {
            get { return Owner == null ? _Scene : Owner.Scene; }
            private set
            {
                if (_Scene == value) return;
                _Scene = value;
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

        #region ***** 建構式 *****
        /// <summary>
        /// 初始化技能物件集合
        /// </summary>
        public SkillCollection() { }

        /// <summary>
        /// 初始化技能物件集合,不指定所有者
        /// </summary>
        /// <param name="scene">所屬場景</param>
        public SkillCollection(SceneBase scene)
        {
            Binding(scene);
        }
        #endregion

        #region ===== 方法 =====
        /// <summary>
        /// 綁定技能集合到場景(所有人>場景)
        /// </summary>
        public void Binding(SceneBase scene, bool bindingLock = false)
        {
            if (_Scene == scene) return;
            if (BindingLock) throw new Exception("技能集合已被鎖定無法綁定");

            AllBreak();
            Owner = null;
            Scene = scene;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定技能集合到所有人物件(所有人>場景,由所有者綁定,除此之外勿使用此函數)
        /// </summary>
        public void Binding(ObjectActive owner, bool bindingLock = false)
        {
            if (_Owner == owner) return;
            if (BindingLock) throw new Exception("技能集合已被鎖定無法綁定");
            if (owner != null && owner.Skills != this) throw new Exception("所有者的技能集合物件不符");

            AllBreak();
            Owner = owner;
            Scene = null;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 清除綁定
        /// </summary>
        public void ClearBinding()
        {
            if (BindingLock) throw new Exception("技能集合已被鎖定無法綁定");

            AllBreak();
            Owner = null;
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

        #region ##### 集合項目調整 #####
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
            _Collection.Add(item);
            item.Binding(this, true);
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
                item.BindingUnlock();
                item.Binding(Scene);
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
                _Collection[i].BindingUnlock();
                _Collection[i].Binding(Scene);
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
        #endregion

        #region ##### 場景中動作(須有所有者) #####
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
        #endregion

        // 禁用Foreach避免新增時錯誤
        //public IEnumerator<SkillBase> GetEnumerator()
        //{
        //    return _Collection.GetEnumerator();
        //}
        #endregion
    }
}
