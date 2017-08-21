
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
        private List<ObjectBase> _Collection;

        /// <summary>
        /// 發生於集合內物件死亡
        /// </summary>
        public ObjectDeadEventHandle ObjectDead;

        /// <summary>
        /// 歸屬的場景物件
        /// </summary>
        public SceneBase Scene { get; set; }

        /// <summary>
        /// 集合物件數量
        /// </summary>
        public int Count
        {
            get { return _Collection.Count; }
        }

        /// <summary>
        /// 初始化活動物件集合
        /// </summary>
        /// <param name="scene">所屬場景物件</param>
        public ObjectCollection(SceneBase scene)
        {
            Scene = scene;
            _Collection = new List<ObjectBase>();
        }

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
            item.Container = this;
            item.Dead += OnObjectDead;
            _Collection.Add(item);
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
                item.Container = null;
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
                ObjectBase item = _Collection[i];
                item.Container = null;
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
                    deadObjects.Add(item);
                }
            }

            if (deadObjects.Count == 0) return;
            foreach (ObjectBase deadObject in deadObjects)
            {
                deadObject.Dispose();
                _Collection.Remove(deadObject);
            }
        }

        protected void OnObjectDead(ObjectBase sender, ObjectBase killer, ObjectDeadType deadType)
        {
            if (ObjectDead != null)
            {
                ObjectDead(sender, killer, deadType);
            }
        }

        //禁用Foreach避免新增時錯誤
        //public IEnumerator<ObjectBase> GetEnumerator()
        //{
        //    return _Collection.GetEnumerator();
        //}
    }
}
