
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

        private ObjectBase _Owner;
        /// <summary>
        /// 取得歸屬的活動物件(所有人>場景)
        /// </summary>
        public ObjectBase Owner
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
        /// 取得歸屬的場景物件(所有人>場景)
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
        /// 為true時須重新取得狀態值
        /// </summary>
        private bool _AffixChanged = true;

        private SpecialStatus _Affix;
        /// <summary>
        /// 附加於群組所有者上的特殊狀態(由群組內特性物件旗標組合)
        /// </summary>
        public SpecialStatus Affix
        {
            get
            {
                if (_AffixChanged)
                {
                    SpecialStatus result = RunningBox.SpecialStatus.None;
                    foreach (PropertyBase item in _Collection)
                    {
                        if (item.Status == PropertyStatus.Enabled)
                        {
                            result |= item.Affix;
                        }
                    }
                    _Affix = result;
                    _AffixChanged = false;
                }
                return _Affix;
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
        /// 初始化特性物件集合
        /// </summary>
        public PropertyCollection() { }

        /// <summary>
        /// 初始化特性物件集合,不指定所有者
        /// </summary>
        /// <param name="scene">所屬場景</param>
        public PropertyCollection(SceneBase scene)
        {
            Binding(scene);
        }
        #endregion

        #region ===== 方法 =====
        /// <summary>
        /// 綁定特性群組到場景(所有人>場景)
        /// </summary>
        /// <param name="scene">場景</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public void Binding(SceneBase scene, bool bindingLock = false)
        {
            if (_Scene == scene) return;
            if (BindingLock) throw new Exception("特性群組已被鎖定無法綁定");
            if (_Owner != null)
            {
                AllBreak();
            }
            Owner = null;
            Scene = scene;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定特性群組到所有人物件(所有人>場景,由所有者綁定,除此之外勿使用此函數)
        /// </summary>
        /// <param name="owner">所有人</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public void Binding(ObjectBase owner, bool bindingLock = false)
        {
            if (_Owner == owner) return;
            if (BindingLock) throw new Exception("特性群組已被鎖定無法綁定");
            if (owner != null && owner.Propertys != this) throw new Exception("所有者的特性群組物件不符");

            if (_Owner != null)
            {
                AllBreak();
            }
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
            if (BindingLock) throw new Exception("特性群組已被鎖定無法解除綁定");
            if (_Owner != null)
            {
                AllBreak();
            }
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
        /// 增加特性物件到活動集合內
        /// </summary>
        /// <param name="item">特性物件</param>
        public void Add(PropertyBase item)
        {
            item.StatusChanged += ItemStatusChanged;
            item.AffixChanged += ItemAffixChanged;
            if (item.Status == PropertyStatus.Enabled)
            {
                _Affix |= item.Affix;
            }
            _Collection.Add(item);
            item.Binding(this, true);
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
                    _AffixChanged = true;
                }
                item.StatusChanged -= ItemStatusChanged;
                item.AffixChanged -= ItemAffixChanged;
                item.BindingUnlock();
                item.Binding(Scene);
            }
            return result;
        }

        /// <summary>
        /// 清空特性集合
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].StatusChanged -= ItemStatusChanged;
                _Collection[i].AffixChanged -= ItemAffixChanged;
                _Collection[i].BindingUnlock();
                _Collection[i].Binding(Scene);
            }
            _Affix = RunningBox.SpecialStatus.None;
            _AffixChanged = false;
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
                disabledProperty.AffixChanged -= ItemAffixChanged;
                disabledProperty.BindingUnlock();
                disabledProperty.Binding(Scene, false);
                _Collection.Remove(disabledProperty);
            }
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
        #endregion

        #region ##### 場景中動作(須有所有者) #####
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
        public void AllDoAfterDead(ObjectBase killer, ObjectDeadType deadType)
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
                _Collection[i].Break();
            }
        }
        #endregion

        /// <summary>
        /// 集合內特性狀態變更動作
        /// </summary>
        private void ItemStatusChanged(object sender, PropertyStatus oldValue, PropertyStatus newValue)
        {
            PropertyBase item = sender as PropertyBase;
            if (item.Affix == RunningBox.SpecialStatus.None) return;
            if (item.Status == PropertyStatus.Enabled)
            {
                _Affix |= item.Affix;
            }
            else
            {
                _AffixChanged = true;
            }
        }

        /// <summary>
        /// 集合內特性狀態變更動作
        /// </summary>
        private void ItemAffixChanged(object sender, SpecialStatus oldValue, SpecialStatus newValue)
        {
            _AffixChanged = true;
        }
        #endregion

        // 禁用Foreach避免新增時錯誤
        //public IEnumerator<PropertyBase> GetEnumerator()
        //{
        //    return _Collection.GetEnumerator();
        //}
    }
}
