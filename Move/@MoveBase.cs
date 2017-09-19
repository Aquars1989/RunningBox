using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public abstract class MoveBase
    {
        //記錄作用於下次移動的偏移植
        private float _NextOffsetFixX = 0;
        private float _NextOffsetFixY = 0;

        #region ===== 事件 =====
        /// <summary>
        /// 發生於移動時
        /// </summary>
        public event EventHandler Moving;

        /// <summary>
        /// 發生於所屬物件變更時(所屬物件可為所有人>場景)
        /// </summary>
        public event EventHandler BindingChanged;

        /// <summary>
        /// 發生於目標變更
        /// </summary>
        public event ValueChangedEnentHandle<ITargetability> TargetObjectChanged;

        /// <summary>
        /// 發生於移動調整值最大數量變更
        /// </summary>
        public event ValueChangedEnentHandle<int> OffsetsLimitChanged;

        /// <summary>
        /// 發生於移動速度變更
        /// </summary>
        public event ValueChangedEnentHandle<float> SpeedChanged;

        /// <summary>
        /// 發生於移動調整值列表內容變更
        /// </summary>
        public event EventHandler OffsetsChanged;

        /// <summary>
        /// 發生於移動規劃後
        /// </summary>
        public event EventHandler AfterPlan;

        /// <summary>
        /// 發生於移動所有者後
        /// </summary>
        public event EventHandler AfterMove;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於移動時
        /// </summary>
        protected void OnMoving()
        {
            if (Moving != null)
            {
                Moving(this, new EventArgs());
            }
        }

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

        /// <summary>
        /// 發生於目標變更
        /// </summary>
        protected void OnTargetObjectChanged(ITargetability oldValue, ITargetability newValue)
        {
            if (TargetObjectChanged != null)
            {
                TargetObjectChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於移動調整值最大數量變更
        /// </summary>
        protected void OnOffsetsLimitChanged(int oldValue, int newValue)
        {
            CheckOffset();
            if (OffsetsLimitChanged != null)
            {
                OffsetsLimitChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於移動調整值列表內容變更
        /// </summary>
        protected void OnOffsetsChanged()
        {
            if (OffsetsChanged != null)
            {
                OffsetsChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於移動速度變更
        /// </summary>
        protected void OnSpeedChanged(float oldValue, float newValue)
        {
            if (SpeedChanged != null)
            {
                SpeedChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於移動規劃後
        /// </summary>
        protected void OnAfterPlan()
        {
            if (AfterPlan != null)
            {
                AfterPlan(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於移動所有者後
        /// </summary>
        protected void OnAfterMove()
        {
            if (AfterMove != null)
            {
                AfterMove(this, new EventArgs());
            }
        }
        #endregion

        #region ===== 屬性 =====
        /// <summary>
        /// 是否鎖定綁定功能
        /// </summary>
        public bool BindingLock { get; private set; }

        /// <summary>
        /// 移動調整值列表
        /// </summary>
        private List<PointF> Offsets { get; set; }

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
        /// 回合中移動的X總值
        /// </summary>
        public float MoveX { get; private set; }

        /// <summary>
        /// 回合中移動的Y總值
        /// </summary>
        public float MoveY { get; private set; }

        private int _OffsetsLimit;
        /// <summary>
        /// 移動調整值列表最大數量
        /// </summary>
        public int OffsetsLimit
        {
            get { return _OffsetsLimit; }
            set
            {
                if (_OffsetsLimit == value) return;
                int oldValue = _OffsetsLimit;
                _OffsetsLimit = value;
                _SpeedPerOffsets = _Speed / _OffsetsLimit;
                OnOffsetsLimitChanged(oldValue, value);
            }
        }

        /// <summary>
        /// 重量,最終移動速度會受到此值影響(finalSpeed = speeed/Weight)
        /// </summary>
        public float Weight;

        private float _Speed;
        /// <summary>
        /// 總體移動速度最大值(排除重量影響)
        /// </summary>
        public float Speed
        {
            get { return _Speed; }
            set
            {
                if (_Speed == value) return;
                float oldValue = _Speed;
                _Speed = value;
                _SpeedPerOffsets = _Speed / _OffsetsLimit;
                OnSpeedChanged(oldValue, value);
            }
        }

        private float _SpeedPerOffsets;
        /// <summary>
        /// 移動速度,決定每個移動調整值的最大距離(排除重量影響)
        /// </summary>
        public float SpeedPerOffsets
        {
            get { return _SpeedPerOffsets; }
            set
            {
                if (_SpeedPerOffsets == value) return;
                float oldValue = _Speed;
                _SpeedPerOffsets = value;
                _Speed = _SpeedPerOffsets * _OffsetsLimit;
                OnSpeedChanged(oldValue, _Speed);
            }
        }

        /// <summary>
        /// 追蹤目標(必要)
        /// </summary>
        public TargetSet Target { get; private set; }
        #endregion

        /// <summary>
        /// 基本移動物件建構式
        /// </summary>
        /// <param name="Target">追蹤目標</param>
        /// <param name="weight">重量,最終移動速度會受到此值影響(finalSpeed = speeed/Weight)</param>
        /// <param name="speed">總體移動速度最大值(排除重量影響)</param>
        /// <param name="offsetsLimit">移動調整值列表最大數量</param>
        public MoveBase(ITargetability target, float weight, float speed, int offsetsLimit)
        {
            Offsets = new List<PointF>();
            OffsetsLimit = offsetsLimit;
            Weight = weight;
            Speed = speed;
            Target = new TargetSet(target);
            Target.ObjectChanged += (s, o, n) => { OnTargetObjectChanged(o, n); };
        }

        #region ===== 方法 =====
        /// <summary>
        /// 綁定移動物件到場景(所有人>場景)
        /// </summary>
        /// <param name="scene">場景</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public virtual void Binding(SceneBase scene, bool bindingLock = false)
        {
            if (_Scene == scene) return;
            if (BindingLock) throw new Exception("移動物件已被鎖定無法綁定");

            Owner = null;
            Scene = scene;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 綁定移動物件到所有人物件(所有人>場景,由所有者綁定,除此之外勿使用此函數)
        /// </summary>
        /// <param name="owner">所有人物件</param>
        /// <param name="bindingLock">綁定後是否鎖定綁定功能</param>
        public virtual void Binding(ObjectBase owner, bool bindingLock = false)
        {
            if (_Owner == owner) return;
            if (BindingLock) throw new Exception("移動物件已被鎖定無法綁定");
            if (owner != null && owner.MoveObject != this) throw new Exception("所有者的移動物件不符");
            Owner = owner;
            Scene = null;
            BindingLock = bindingLock;
            OnBindingChanged();
        }

        /// <summary>
        /// 清除綁定
        /// </summary>
        public virtual void ClearBinding()
        {
            if (BindingLock) throw new Exception("移動物件已被鎖定無法解除綁定");
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

        /// <summary>
        /// 規劃移動調整值
        /// </summary>
        public abstract void Plan();

        /// <summary>
        /// 移動所有者
        /// </summary>
        public virtual void Move()
        {
            float moveX = MoveX / Scene.SceneRoundPerSec / Weight;
            float moveY = MoveY / Scene.SceneRoundPerSec / Weight;
            if (MoveX != 0 || MoveY != 0)
            {
                ObjectActive ownerActive = Owner as ObjectActive;
                if (ownerActive != null && (ownerActive.Propertys.Affix & SpecialStatus.Movesplit) == SpecialStatus.Movesplit)
                {
                    //移動距離大時分成多次移動,供碰撞用
                    int partCount = (int)(Math.Max(Math.Abs(moveX / Owner.Layout.Width), Math.Abs(moveY / Owner.Layout.Height))) + 1;
                    float partX = moveX / partCount;
                    float partY = moveY / partCount;
                    for (int i = 0; i < partCount; i++)
                    {
                        Owner.Layout.X += partX;
                        Owner.Layout.Y += partY;
                        OnMoving();
                    }
                }
                else
                {
                    Owner.Layout.X += moveX;
                    Owner.Layout.Y += moveY;
                    OnMoving();
                }
            }
            OnAfterMove();
        }

        /// <summary>
        /// 加入移動調整值到下一次移動調整值中,因加入時物件可能已移動,所以加在下次移動調整內
        /// </summary>
        /// <param name="stepOffset">移動調整值</param>
        public void AddToNextOffset(PointF stepOffset)
        {
            _NextOffsetFixX += stepOffset.X;
            _NextOffsetFixY += stepOffset.Y;
        }

        /// <summary>
        /// 加入移動調整值並檢查移動調整值數量,超出上限時移除多餘項目
        /// </summary>
        /// <param name="stepOffset">移動調整值</param>
        public void AddOffset(PointF stepOffset)
        {
            if (_NextOffsetFixX != 0 || _NextOffsetFixY != 0)
            {
                stepOffset = new PointF(stepOffset.X + _NextOffsetFixX, stepOffset.Y + _NextOffsetFixY);
                _NextOffsetFixX = 0;
                _NextOffsetFixY = 0;
            }

            MoveX += stepOffset.X;
            MoveY += stepOffset.Y;

            Offsets.Add(stepOffset);
            if (Offsets.Count > OffsetsLimit)
            {
                for (int i = 0; i < Offsets.Count - OffsetsLimit; i++)
                {
                    MoveX -= Offsets[i].X;
                    MoveY -= Offsets[i].Y;
                }
                Offsets.RemoveRange(0, Offsets.Count - OffsetsLimit);
            }
            OnOffsetsChanged();
        }

        /// <summary>
        /// 檢查移動調整值數量,超出上限時移除多餘項目
        /// </summary>
        /// <param name="stepOffset">移動調整值</param>
        public void CheckOffset()
        {
            if (Offsets.Count > OffsetsLimit)
            {
                for (int i = 0; i < Offsets.Count - OffsetsLimit; i++)
                {
                    MoveX -= Offsets[i].X;
                    MoveY -= Offsets[i].Y;
                }
                Offsets.RemoveRange(0, Offsets.Count - OffsetsLimit);
                OnOffsetsChanged();
            }
        }

        /// <summary>
        /// 清除移動調整值
        /// </summary>
        public void ClearOffset()
        {
            Offsets.Clear();
            MoveX = 0;
            MoveY = 0;
            OnOffsetsChanged();
        }

        /// <summary>
        /// 使用目標座標和速度取得位移值
        /// </summary>
        /// <param name="x">目標X座標</param>
        /// <param name="y">目標Y座標</param>
        /// <param name="speed">移動速度</param>
        /// <returns>位移值</returns>
        public PointF GetOffsetByXY(float x, float y, float speed)
        {
            double angle = Function.GetAngle(Owner.Layout.CenterX, Owner.Layout.CenterY, x, y);
            return Function.GetOffsetPoint(0, 0, angle, speed / _OffsetsLimit / Weight);
        }

        /// <summary>
        /// 使用角度和速度取得位移值
        /// </summary>
        /// <param name="angle">角度</param>
        /// <param name="speed">移動速度</param>
        /// <returns>位移值</returns>
        public PointF GetOffsetByAngle(double angle, float speed)
        {
            return Function.GetOffsetPoint(0, 0, angle, speed / _OffsetsLimit / Weight);
        }
        #endregion
    }
}
