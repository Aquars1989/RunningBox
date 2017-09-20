
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 活動物件管理集合
    /// </summary>
    public class ObjectCollection
    {
        /// <summary>
        /// 內部集合物件
        /// </summary>
        private List<ObjectBase> _Collection = new List<ObjectBase>();

        #region ===== 事件 =====
        /// <summary>
        /// 發生於集合內物件死亡
        /// </summary>
        public ObjectDeadEventHandle ObjectDead;

        /// <summary>
        /// 發生於所屬物件變更時(所屬物件可為所有人>場景)
        /// </summary>
        public event EventHandler BindingChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於所屬物件變更時(所有人>場景)
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

        private ObjectUIPanel _Owner;
        /// <summary>
        /// 取得歸屬的活動物件(所有人>場景)
        /// </summary>
        public ObjectUIPanel Owner
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
        /// 集合物件數量
        /// </summary>
        public int Count
        {
            get { return _Collection.Count; }
        }
        #endregion 

        #region ***** 建構式 *****
        /// <summary>
        /// 初始化活動物件集合
        /// </summary>
        /// <param name="scene">所屬場景物件</param>
        public ObjectCollection(SceneBase scene)
        {
            Binding(scene);
        }

        /// <summary>
        /// 初始化活動物件集合,指定所有者
        /// </summary>
        /// <param name="owner">所屬活動物件</param>
        public ObjectCollection() { }
        #endregion

        #region ===== 方法 =====
        /// <summary>
        /// 綁定活動物件群組到場景(所有人>場景)
        /// </summary>
        /// <param name="scene">場景</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public void Binding(SceneBase scene, bool bindingLock = false)
        {
            if (_Scene == scene) return;
            if (BindingLock) throw new Exception("物件已被鎖定無法綁定");

            Owner = null;
            Scene = scene;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定活動物件群組到物件(所有人>場景,由所有者綁定,除此之外勿使用此函數)
        /// </summary>
        /// <param name="owner">所有人</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public void Binding(ObjectUIPanel owner, bool bindingLock = false)
        {
            if (_Owner == owner) return;
            if (BindingLock) throw new Exception("物件已被鎖定無法綁定");
            if (owner != null && owner.UIObjects != this) throw new Exception("所有者的子群組不符");

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
            if (BindingLock) throw new Exception("物件已被鎖定無法解除綁定");
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
        public ObjectBase this[int index]
        {
            get { return _Collection[index]; }
        }
        
        /// <summary>
        /// 增加活動物件到活動集合內
        /// </summary>
        /// <param name="item">活動物件</param>
        public void Add(ObjectBase item)
        {
            _Collection.Add(item);
            item.Binding(this,true);
            item.Dead += OnObjectDead;
        }

        /// <summary>
        /// 從活動集合內移除指定活動物件
        /// </summary>
        /// <param name="item">活動物件</param>
        /// <returns>如果成功移除活動物件則為 true，否則為 false。</returns>
        public bool Remove(ObjectBase item)
        {
            bool result = _Collection.Remove(item);
            if (result)
            {
                item.Dead -= OnObjectDead;
                item.Kill(null, ObjectDeadType.Clear);
                item.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 清空活動集合
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                _Collection[i].Dead -= OnObjectDead;
                _Collection[i].Kill(null, ObjectDeadType.Clear);
                _Collection[i].Dispose();
            }
            _Collection.Clear();
        }

        /// <summary>
        /// 判斷指定活動物件是否存在集合內
        /// </summary>
        /// <param name="item">活動物件</param>
        /// <returns>如果活動物件在集合中則為 true，否則為 false。</returns>
        public bool Contains(ObjectBase item)
        {
            return _Collection.Contains(item);
        }

        /// <summary>
        /// 清除集合內所有失效的活動物件
        /// </summary>
        public void ClearAllDead()
        {
            List<ObjectBase> deadObjects = new List<ObjectBase>();
            for (int i = 0; i < _Collection.Count; i++)
            {
                ObjectBase item = _Collection[i];
                if (item.Status == ObjectStatus.Dead)
                {
                    if ((item.Propertys.Affix & SpecialStatus.Remain) != SpecialStatus.Remain)
                    {
                        deadObjects.Add(item);
                    }
                }
            }

            if (deadObjects.Count == 0) return;
            foreach (ObjectBase deadObject in deadObjects)
            {
                deadObject.Dispose();
                _Collection.Remove(deadObject);
            }
        }

        /// <summary>
        /// 取得指定類型的物件
        /// </summary>
        /// <param name="item">活動物件</param>
        public List<T> GetObjectByType<T>() where T : ObjectBase
        {
            List<T> result = new List<T>();
            foreach (ObjectBase property in _Collection)
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

        #region ##### 場景中動作 #####
        /// <summary>
        /// 所有集合內活動物件執行Action方法
        /// </summary>
        public void AllAction()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                ObjectBase item = _Collection[i];
                item.Action();
            }
        }

        /// <summary>
        /// 所有集合內活動物件執行DrawSelf方法
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public void AllDrawSelf(Graphics g)
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                ObjectBase item = _Collection[i];
                item.Draw(g);
            }
        }       
        #endregion

        protected void OnObjectDead(ObjectBase sender, ObjectBase killer, ObjectDeadType deadType)
        {
            if (ObjectDead != null)
            {
                ObjectDead(sender, killer, deadType);
            }
        }
        #endregion
        //禁用Foreach避免新增時錯誤
        //public IEnumerator<ObjectBase> GetEnumerator()
        //{
        //    return _Collection.GetEnumerator();
        //}
    }
}
