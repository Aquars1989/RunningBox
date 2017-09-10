using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    public abstract class MoveBase
    {
        private float _NextOffsetFixX = 0;
        private float _NextOffsetFixY = 0;

        #region ===== 事件 =====
        /// <summary>
        /// 發生於移動時
        /// </summary>
        public event EventHandler Moving;

        /// <summary>
        /// 發生於所有者變更
        /// </summary>
        public event EventHandler OwnerChanged;

        /// <summary>
        /// 發生於目標變更
        /// </summary>
        public event EventHandler TargetChanged;

        /// <summary>
        /// 發生於移動調整值最大數量變更
        /// </summary>
        public event EventHandler OffsetsLimitChanged;

        /// <summary>
        /// 發生於移動調整值列表內容變更
        /// </summary>
        public event EventHandler OffsetsChanged;

        /// <summary>
        /// 發生於移動速度變更
        /// </summary>
        public event EventHandler SpeedChanged;

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
        /// 發生於所有者變更
        /// </summary>
        protected void OnOwnerChanged()
        {
            if (OwnerChanged != null)
            {
                OwnerChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於目標變更
        /// </summary>
        protected void OnTargetChanged()
        {
            if (TargetChanged != null)
            {
                TargetChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於移動調整值最大數量變更
        /// </summary>
        protected void OnOffsetsLimitChanged()
        {
            CheckOffset();
            if (OffsetsLimitChanged != null)
            {
                OffsetsLimitChanged(this, new EventArgs());
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
        protected void OnSpeedChanged()
        {
            if (SpeedChanged != null)
            {
                SpeedChanged(this, new EventArgs());
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
        /// 移動調整值列表
        /// </summary>
        private List<PointF> Offsets { get; set; }

        private ObjectBase _Owner;
        /// <summary>
        /// 所有者(必要,由上層設定)
        /// </summary>
        public ObjectBase Owner
        {
            get { return _Owner; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_Owner == value) return;
                _Owner = value;
                OnOwnerChanged();
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
                _OffsetsLimit = value;
                _SpeedPerOffsets = _Speed / _OffsetsLimit;
                OnOffsetsLimitChanged();
            }
        }

        private float _Speed;
        /// <summary>
        /// 總體移動速度最大值
        /// </summary>
        public float Speed
        {
            get { return _Speed; }
            set
            {
                if (_Speed == value) return;
                _Speed = value;
                _SpeedPerOffsets = _Speed / _OffsetsLimit;
                OnSpeedChanged();
            }
        }

        private float _SpeedPerOffsets;
        /// <summary>
        /// 移動速度,決定每個移動調整值的最大距離
        /// </summary>
        public float SpeedPerOffsets
        {
            get { return _SpeedPerOffsets; }
            set
            {
                if (_SpeedPerOffsets == value) return;
                _SpeedPerOffsets = value;
                _Speed = _SpeedPerOffsets * _OffsetsLimit;
                OnSpeedChanged();
            }
        }

        private ITarget _Target;
        /// <summary>
        /// 追蹤目標(必要)
        /// </summary>
        public ITarget Target
        {
            get { return _Target; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_Target == value) return;
                _Target = value;

                OnTargetChanged();
            }
        }
        #endregion

        /// <summary>
        /// 基本移動物件建構式
        /// </summary>
        /// <param name="Target">追蹤目標(必要)</param>
        /// <param name="offsetsLimit">移動調整值列表最大數量</param>
        public MoveBase(ITarget target, float speed, int offsetsLimit)
        {
            Offsets = new List<PointF>();
            OffsetsLimit = offsetsLimit;
            Speed = speed;
            Target = target;
        }

        #region ===== 方法 =====
        /// <summary>
        /// 規劃移動調整值
        /// </summary>
        public abstract void Plan();

        /// <summary>
        /// 移動所有者
        /// </summary>
        public virtual void Move()
        {
            if (MoveX != 0 || MoveY != 0)
            {
                ObjectActive ownerActive = Owner as ObjectActive;
                if (ownerActive != null && (ownerActive.Propertys.Affix & SpecialStatus.Movesplit) == SpecialStatus.Movesplit)
                {
                    //移動距離大時分割為多部分移動
                    double distance = Function.GetDistance(0, 0, MoveX, MoveY) / Owner.Scene.SceneRoundPerSec;
                    int partCount = (int)(distance / Math.Min(Owner.Layout.Width, Owner.Layout.Height)) + 1;
                    float partX = MoveX / Owner.Scene.SceneRoundPerSec / partCount;
                    float partY = MoveY / Owner.Scene.SceneRoundPerSec / partCount;
                    for (int i = 0; i < partCount; i++)
                    {
                        Owner.Layout.X += partX;
                        Owner.Layout.Y += partY;
                        OnMoving();
                    }
                }
                else
                {
                    Owner.Layout.X += MoveX / Owner.Scene.SceneRoundPerSec;
                    Owner.Layout.Y += MoveY / Owner.Scene.SceneRoundPerSec;
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
            return Function.GetOffsetPoint(0, 0, angle, speed / _OffsetsLimit);
        }

        /// <summary>
        /// 使用角度和速度取得位移值
        /// </summary>
        /// <param name="angle">角度</param>
        /// <param name="speed">移動速度</param>
        /// <returns>位移值</returns>
        public PointF GetOffsetByAngle(double angle, float speed)
        {
            return Function.GetOffsetPoint(0, 0, angle, speed / _OffsetsLimit);
        }
        #endregion
    }
}
