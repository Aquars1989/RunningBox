using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 基礎活動物件
    /// </summary>
    public abstract class ObjectBase : IDisposable, ITargetability
    {
        #region ===== 事件 =====
        /// <summary>
        /// 發生於物件死亡
        /// </summary>
        public event ObjectDeadEventHandle Dead;

        /// <summary>
        /// 發生於回合動作結束
        /// </summary>
        public event EventHandler AfterAction;

        /// <summary>
        /// 發生於物件可見狀態變更
        /// </summary>
        public event EventHandler VisibleChanged;

        /// <summary>
        /// 發生於物件狀態變更
        /// </summary>
        public event ValueChangedEnentHandle<ObjectStatus> StatusChanged;

        /// <summary>
        /// 發生於繪圖物件變更
        /// </summary>
        public event ValueChangedEnentHandle<DrawBase> DrawObjectChanged;

        /// <summary>
        /// 發生於配置物件變更
        /// </summary>
        public event ValueChangedEnentHandle<LayoutSet> LayoutObjectChanged;

        /// <summary>
        /// 發生於移動物件變更
        /// </summary>
        public event ValueChangedEnentHandle<MoveBase> MoveObjectChanged;

        /// <summary>
        /// 發生於特性集合變更
        /// </summary>
        public event ValueChangedEnentHandle<PropertyCollection> PropertysChanged;

        /// <summary>
        /// 發生於所屬物件變更時(所屬物件可為集合>場景)
        /// </summary>
        public event EventHandler BindingChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於回合動作結束
        /// </summary>
        protected virtual void OnAfterAction()
        {
            if (AfterAction != null)
            {
                AfterAction(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於物件可見狀態變更
        /// </summary>
        protected virtual void OnVisibleChanged()
        {
            if (VisibleChanged != null)
            {
                VisibleChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於物件狀態變更
        /// </summary>
        protected virtual void OnStatusChanged(ObjectStatus oldValue, ObjectStatus newValue)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於繪圖物件變更
        /// </summary>
        protected virtual void OnDrawObjectChanged(DrawBase oldValue, DrawBase newValue)
        {
            if (oldValue != null)
            {
                oldValue.BindingUnlock();
                oldValue.Binding(Scene);
            }

            if (newValue != null)
            {
                newValue.Binding(this, true);
            }

            if (DrawObjectChanged != null)
            {
                DrawObjectChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於配置物件變更
        /// </summary>
        protected virtual void OnLayoutObjectChanged(LayoutSet oldValue, LayoutSet newValue)
        {
            if (oldValue != null)
            {
                oldValue.BindingUnlock();
                oldValue.Binding(Scene);
            }

            if (newValue != null)
            {
                newValue.Binding(this, true);
            }

            if (LayoutObjectChanged != null)
            {
                LayoutObjectChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於移動物件變更
        /// </summary>
        protected virtual void OnMoveObjectChanged(MoveBase oldValue, MoveBase newValue)
        {
            if (oldValue != null)
            {
                oldValue.Moving -= MoveObject_Moving;
                oldValue.BindingUnlock();
                oldValue.Binding(Scene);
            }

            if (newValue != null)
            {
                newValue.Moving += MoveObject_Moving;
                newValue.Binding(this, true);
            }

            if (MoveObjectChanged != null)
            {
                MoveObjectChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於特性集合變更
        /// </summary>
        protected virtual void OnPropertysChanged(PropertyCollection oldValue, PropertyCollection newValue)
        {
            if (oldValue != null)
            {
                oldValue.BindingUnlock();
                oldValue.Binding(Scene);
            }

            if (newValue != null)
            {
                newValue.Binding(this, true);
            }

            if (PropertysChanged != null)
            {
                PropertysChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於所屬物件變更時(所屬物件可為集合>場景)
        /// </summary>
        protected virtual void OnBindingChanged()
        {
            if (BindingChanged != null)
            {
                BindingChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生在物件死亡時
        /// </summary>
        /// <param name="sender">死亡物件</param>
        /// <param name="killer">殺手物件</param>
        protected virtual void OnDead(ObjectBase sender, ObjectBase killer, ObjectDeadType deadType)
        {
            if (Dead != null)
            {
                Dead(sender, killer, deadType);
            }
        }
        #endregion

        #region ===== 屬性 =====
        /// <summary>
        /// 是否鎖定綁定功能
        /// </summary>
        public bool BindingLock { get; private set; }

        private bool _Visible = true;
        /// <summary>
        /// 是否顯示物件
        /// </summary>
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (_Visible == value) return;
                _Visible = value;
                OnVisibleChanged();
            }
        }

        private ObjectCollection _Container;
        /// <summary>
        /// 取得物件歸屬集合
        /// </summary>
        public ObjectCollection Container
        {
            get { return _Container; }
            private set
            {
                if (_Container == value) return;
                _Container = value;
            }
        }

        private SceneBase _Scene;
        /// <summary>
        /// 取得歸屬場景
        /// </summary>
        public SceneBase Scene
        {
            get { return Container == null ? _Scene : Container.Scene; }
            private set
            {
                if (_Scene == value) return;
                _Scene = value;
            }
        }

        private DrawBase _DrawObject;
        /// <summary>
        /// 繪製物件(必要)
        /// </summary>
        public DrawBase DrawObject
        {
            get { return _DrawObject; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_DrawObject == value) return;

                DrawBase oldValue = _DrawObject;
                _DrawObject = value;
                OnDrawObjectChanged(oldValue, value);
            }
        }

        private MoveBase _MoveObject;
        /// <summary>
        /// 移動物件(必要)
        /// </summary>
        public MoveBase MoveObject
        {
            get { return _MoveObject; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_MoveObject == value) return;

                MoveBase oldValue = _MoveObject;
                _MoveObject = value;
                OnMoveObjectChanged(oldValue, value);
            }
        }

        private PropertyCollection _Propertys;
        /// <summary>
        /// 物件擁有的特性群組
        /// </summary>
        public PropertyCollection Propertys
        {
            get { return _Propertys; }
            set
            {
                if (_Propertys == value) return;
                PropertyCollection oldValue = _Propertys;
                _Propertys = value;
                OnPropertysChanged(oldValue, value);
            }
        }

        private LayoutSet _Layout;
        /// <summary>
        /// 物件配置方式(必要)
        /// </summary>
        public LayoutSet Layout
        {
            get { return _Layout; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_Layout == value) return;

                LayoutSet oldValue = _Layout;
                _Layout = value;
                OnLayoutObjectChanged(oldValue, value);
            }
        }

        private ObjectStatus _Status;
        /// <summary>
        /// 物件狀態
        /// </summary>
        public ObjectStatus Status
        {
            get { return _Status; }
            protected set
            {
                if (_Status == value) return;
                ObjectStatus oldValue = _Status;
                _Status = value;
                OnStatusChanged(oldValue, value);
            }
        }

        /// <summary>
        /// 物件所屬陣營,供技能或特性判定
        /// </summary>
        public LeagueType League { get; set; }

        /// <summary>
        /// 存活時間計數器(毫秒)
        /// </summary>
        public CounterObject Life { get; private set; }

        /// <summary>
        /// 給UI特性使用
        /// </summary>
        public int UIOffSetY { get; set; }
        #endregion

        /// <summary>
        /// 使用繪製物件和移動物件建立基本活動物件
        /// </summary>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectBase(DrawBase drawObject, MoveBase moveObject)
        {
            Layout = new LayoutSet();
            Status = ObjectStatus.Alive;
            League = LeagueType.None;
            Life = new CounterObject(-1);
            Propertys = new PropertyCollection();
            DrawObject = drawObject;
            MoveObject = moveObject;
        }

        #region ===== 方法 =====
        /// <summary>
        /// 綁定物件到場景
        /// </summary>
        /// <param name="scene">場景</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public void Binding(SceneBase scene, bool bindingLock = false)
        {
            if (_Scene == scene) return;
            if (BindingLock) throw new Exception("物件已被鎖定無法綁定");

            Container = null;
            Scene = scene;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定物件到集合(集合內綁定,除此之外勿使用此函數)
        /// </summary>
        /// <param name="collection">集合</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public void Binding(ObjectCollection collection, bool bindingLock = false)
        {
            if (_Container == collection) return;
            if (BindingLock) throw new Exception("物件已被鎖定無法綁定");
            if (collection != null && !collection.Contains(this))
            {
                throw new Exception("物件不在集合中");
            }

            Container = collection;
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

            Container = null;
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

        /// <summary>
        /// 殺死此物件
        /// </summary>
        /// <param name="killer">殺手物件</param>
        /// <param name="deadType">死亡類型</param>
        public virtual void Kill(ObjectBase killer, ObjectDeadType deadType)
        {
            if (Status == ObjectStatus.Alive)
            {
                Status = ObjectStatus.Dead;
                Propertys.AllDoAfterDead(killer, deadType);
                OnDead(this, killer, deadType);
            }
        }

        /// <summary>
        /// 物件在1回合內進行的活動
        /// </summary>
        public virtual void Action()
        {
            UIOffSetY = 0;
            Propertys.AllDoBeforeAction();
            Propertys.AllDoBeforeActionPlan();
            MoveObject.Plan();
            Propertys.AllDoBeforeActionMove();
            MoveObject.Move();
            Settlement();
            Propertys.AllSettlement();
            OnAfterAction();
            Propertys.ClearAllDisabled();
        }

        /// <summary>
        /// 物件移動中進行的活動
        /// </summary>
        public virtual void Moving()
        {
            Propertys.AllDoActionMoving();
        }

        /// <summary>
        /// 結算物件生命
        /// </summary>
        protected virtual void Settlement()
        {
            if (Life.IsFull)
            {
                Kill(null, ObjectDeadType.LifeEnd);
            }
            else
            {
                Life.Value += Scene.SceneIntervalOfRound;
            }
        }

        /// <summary>
        /// 繪製物件
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public virtual void Draw(Graphics g)
        {
            if (Visible)
            {
                Propertys.AllDoBeforeDraw(g);
                DrawObject.Draw(g, Layout.Rectangle);
                Propertys.AllDoAfterDraw(g);
            }
        }

        private void MoveObject_Moving(object sender, EventArgs e)
        {
            Moving();
        }

        /// <summary>
        /// 釋放物件時執行動作
        /// </summary>
        protected virtual void OnDispose()
        {
            if (DrawObject != null)
            {
                DrawObject.Dispose();
            }

            Propertys.Clear();
            MoveObject = MoveNull.Value;
        }
        #endregion

        #region ===== 實作ITargetability =====
        /// <summary>
        /// 使用特定的定位位置取得目標點X座標
        /// </summary>
        /// <param name="anchor">定位位置</param>
        /// <returns>目標點X座標</returns>
        public float GetTargetX(DirectionType anchor)
        {
            return Layout.GetTargetX(anchor);
        }

        /// <summary>
        /// 使用特定的定位位置取得目標點Y座標
        /// </summary>
        /// <param name="anchor">定位位置</param>
        /// <returns>目標點Y座標</returns>
        public float GetTargetY(DirectionType anchor)
        {
            return Layout.GetTargetY(anchor);
        }

        /// <summary>
        /// 使用特定的定位位置取得目標點
        /// </summary>
        /// <param name="anchor">定位位置</param>
        /// <returns>目標點</returns>
        public PointF GetTargetPoint(DirectionType anchor)
        {
            return Layout.GetTargetPoint(anchor);
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDispose();
                }
                disposedValue = true;
            }
        }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
