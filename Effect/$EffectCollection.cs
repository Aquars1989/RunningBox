﻿
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
        private List<IEffect> _Collection;
        private SceneBase _Scene;

        /// <summary>
        /// 集合物件數量
        /// </summary>
        public int Count
        {
            get { return _Collection.Count; }
        }

        /// <summary>
        /// 初始化特效物件集合
        /// </summary>
        /// <param name="scene">所屬場景物件</param>
        public EffectCollection(SceneBase scene)
        {
            _Scene = scene;
            _Collection = new List<IEffect>();
        }

        /// <summary>
        /// 取得指定之索引處的元素。
        /// </summary>
        /// <param name="index">要取得之項目的以零為起始的索引。</param>
        /// <returns>指定之索引處的項目。</returns>
        public IEffect this[int index]
        {
            get { return _Collection[index]; }
        }

        /// <summary>
        /// 增加特效物件到特效集合內
        /// </summary>
        /// <param name="item">特效物件</param>
        public void Add(IEffect item)
        {
            item.Scene = _Scene;
            _Collection.Add(item);
        }

        /// <summary>
        /// 從特效集合內移除指定特效物件
        /// </summary>
        /// <param name="item">特效物件</param>
        /// <returns>如果成功移除特效物件則為 true，否則為 false。</returns>
        public bool Remove(IEffect item)
        {
            bool result = _Collection.Remove(item);
            if (result)
            {
                item.Scene = null;
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
                IEffect item = _Collection[i];
                item.Scene = null;
            }
            _Collection.Clear();
        }

        /// <summary>
        /// 判斷指定特效物件是否存在集合內
        /// </summary>
        /// <param name="item">特效物件</param>
        /// <returns>如果特效物件在集合中則為 true，否則為 false。</returns>
        public bool Contains(IEffect item)
        {
            return _Collection.Contains(item);
        }

        /// <summary>
        /// 所有集合內特效物件執行DoBeforeRound方法
        /// </summary>
        public void AllDoBeforeRound()
        {
            for (int i = 0; i < _Collection.Count; i++)
            {
                IEffect item = _Collection[i];
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
                IEffect item = _Collection[i];
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
                IEffect item = _Collection[i];
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
                IEffect item = _Collection[i];
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
                IEffect item = _Collection[i];
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
                IEffect item = _Collection[i];
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
                IEffect item = _Collection[i];
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
                IEffect item = _Collection[i];
                item.Break();
            }
        }

        /// <summary>
        /// 清除集合內所有失效的特效物件
        /// </summary>
        public void ClearAllDisabled()
        {
            List<IEffect> disabledEffects = new List<IEffect>();
            for (int i = 0; i < _Collection.Count; i++)
            {
                IEffect item = _Collection[i];
                if (item.Status == EffectStatus.Disabled)
                {
                    disabledEffects.Add(item);
                }
            }

            if (disabledEffects.Count == 0) return;
            foreach (IEffect disabledEffect in disabledEffects)
            {
                _Collection.Remove(disabledEffect);
            }
        }

        //禁用Foreach避免新增時錯誤
        //public IEnumerator<IEffect> GetEnumerator()
        //{
        //    return _Collection.GetEnumerator();
        //}
    }
}