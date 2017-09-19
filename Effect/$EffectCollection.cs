
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 特效管理集合
    /// </summary>
    public class EffectCollection
    {
        private List<EffectBase> _Collection;

        /// <summary>
        /// 發生於所屬物件變更時(所屬物件可為場景)
        /// </summary>
        public event EventHandler BindingChanged;

        /// <summary>
        /// 發生於所屬物件變更時(所屬物件可為場景)
        /// </summary>
        protected virtual void OnBindingChanged()
        {
            if (BindingChanged != null)
            {
                BindingChanged(this, new EventArgs());
            }
        }

        #region ===== 屬性 =====
        /// <summary>
        /// 是否鎖定綁定功能
        /// </summary>
        public bool BindingLock { get; private set; }

        private SceneBase _Scene;
        /// <summary>
        /// 取得歸屬場景(集合>場景)
        /// </summary>
        public SceneBase Scene
        {
            get { return _Scene; }
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

        /// <summary>
        /// 初始化特效物件集合
        /// </summary>
        /// <param name="scene">所屬場景物件</param>
        public EffectCollection(SceneBase scene)
        {
            _Scene = scene;
            _Collection = new List<EffectBase>();
        }

        #region ===== 方法 =====
        /// <summary>
        /// 綁定特性到場景(集合>場景)
        /// </summary>
        /// <param name="scene">場景</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public void Binding(SceneBase scene, bool bindingLock = false)
        {
            if (_Scene == scene) return;
            if (BindingLock) throw new Exception("特效集合已被鎖定無法綁定");

            if (_Scene != null)
            {
                AllBreak();
            }
            Scene = scene;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 清除綁定
        /// </summary>
        public void ClearBinding()
        {
            if (BindingLock) throw new Exception("特效集合已被鎖定無法解除綁定");

            if (_Scene != null)
            {
                AllBreak();
            }
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
        public EffectBase this[int index]
        {
            get { return _Collection[index]; }
        }

        /// <summary>
        /// 增加特效物件到特效集合內
        /// </summary>
        /// <param name="item">特效物件</param>
        public void Add(EffectBase item)
        {
            _Collection.Add(item);
            item.Binding(this, true);
        }

        /// <summary>
        /// 從特效集合內移除指定特效物件
        /// </summary>
        /// <param name="item">特效物件</param>
        /// <returns>如果成功移除特效物件則為 true，否則為 false。</returns>
        public bool Remove(EffectBase item)
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
        /// 清空特效集合
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
        /// 判斷指定特效物件是否存在集合內
        /// </summary>
        /// <param name="item">特效物件</param>
        /// <returns>如果特效物件在集合中則為 true，否則為 false。</returns>
        public bool Contains(EffectBase item)
        {
            return _Collection.Contains(item);
        }

        /// <summary>
        /// 清除集合內所有失效的特效物件
        /// </summary>
        public void ClearAllDisabled()
        {
            List<EffectBase> disabledEffects = new List<EffectBase>();
            for (int i = 0; i < _Collection.Count; i++)
            {
                EffectBase item = _Collection[i];
                if (item.Status == EffectStatus.Disabled)
                {
                    disabledEffects.Add(item);
                }
            }

            if (disabledEffects.Count == 0) return;
            foreach (EffectBase disabledEffect in disabledEffects)
            {
                disabledEffect.BindingUnlock();
                disabledEffect.Binding(Scene);
                _Collection.Remove(disabledEffect);
            }
        }
        #endregion

        #region ##### 場景中動作 #####
        /// <summary>
        /// 所有集合內特效物件執行DoBeforeRound方法
        /// </summary>
        public void AllDoBeforeRound()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                EffectBase item = _Collection[i];
                item.DoBeforeRound();
            }
        }

        /// <summary>
        /// 所有集合內特效物件執行DoAfterRound方法
        /// </summary>
        public void AllDoAfterRound()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                EffectBase item = _Collection[i];
                item.DoAfterRound();
            }
        }

        /// <summary>
        /// 所有集合內特效物件執行DoBeforeDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoBeforeDraw(Graphics g)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                EffectBase item = _Collection[i];
                item.DoBeforeDraw(g);
            }
        }

        /// <summary>
        /// 所有集合內特效物件執行DoBeforeDrawFloor方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoBeforeDrawFloor(Graphics g)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                EffectBase item = _Collection[i];
                item.DoBeforeDrawFloor(g);
            }
        }

        /// <summary>
        /// 所有集合內特效物件執行DoBeforeDrawObject方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoBeforeDrawObject(Graphics g)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                EffectBase item = _Collection[i];
                item.DoBeforeDrawObject(g);
            }
        }

        /// <summary>
        /// 所有集合內特效物件執行DoAfterDraw方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoAfterDraw(Graphics g)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                EffectBase item = _Collection[i];
                item.DoAfterDraw(g);
            }
        }

        /// <summary>
        /// 所有集合內特效物件執行DoBeforeDrawUI方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDoBeforeDrawUI(Graphics g)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                EffectBase item = _Collection[i];
                item.DoBeforeDrawUI(g);
            }
        }

        /// <summary>
        /// 中斷所有集合內特效
        /// </summary>
        public void AllBreak()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                EffectBase item = _Collection[i];
                item.Break();
            }
        }
        #endregion
        #endregion

        //禁用Foreach避免新增時錯誤
        //public IEnumerator<IEffect> GetEnumerator()
        //{
        //    return _Collection.GetEnumerator();
        //}
    }
}
