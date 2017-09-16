
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
        /// 發生於依附物件變更時(依附物件可為場景 物件)
        /// </summary>
        public event EventHandler BindingChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於依附物件變更時(依附物件可為場景 物件)
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
        private ObjectUIPanel _Owner;
        /// <summary>
        /// 取得歸屬的活動物件
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
        /// 綁定活動物件群組到場景
        /// </summary>
        public void Binding(SceneBase scene)
        {
            if (_Scene == scene) return;
            if (_Owner != null && _Owner.UIObjects == this) throw new Exception("活動物件群組已被綁定");
 
            Owner = null;
            Scene = scene;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定活動物件群組到物件(由所有者綁定,除此之外勿使用此函數)
        /// </summary>
        public void Binding(ObjectUIPanel owner)
        {
            if (_Owner == owner) return;
            if (_Owner != null && _Owner.UIObjects == this) throw new Exception("活動物件群組已被綁定");
            if (owner != null && owner.UIObjects != this) throw new Exception("所有者的子群組不符");

            Owner = owner;
            Scene = null;
            OnBindingChanged();
        }

        /// <summary>
        /// 清除綁定
        /// </summary>
        public void ClearBinding()
        {
            if (_Owner != null && _Owner.UIObjects == this) throw new Exception("活動物件群組已被綁定");
            Owner = null;
            Scene = null;
            OnBindingChanged();
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
            item.Binding(this);
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
                    ObjectActive itemA = item as ObjectActive;
                    if (itemA == null || (itemA.Propertys.Affix & SpecialStatus.Remain) != SpecialStatus.Remain)
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
