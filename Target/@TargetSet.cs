
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 定義目標物件
    /// </summary>
    public class TargetSet
    {
        #region ===== 事件 =====
        /// <summary>
        /// 發生於關聯物件變更
        /// </summary>
        public event ValueChangedEnentHandle<ITargetability> ObjectChanged;

        /// <summary>
        /// 發生於偏移值變更
        /// </summary>
        public event EventHandler OffsetChanged;

        /// <summary>
        /// 發生於物件的定位位置變更時
        /// </summary>
        public event ValueChangedEnentHandle<DirectionType> AnchorChanged;
        #endregion

        #region ===== 引發事件 =====
        /// <summary>
        /// 發生於關聯物件變更
        /// </summary>
        protected void OnObjectChanged(ITargetability oldValue, ITargetability newValue)
        {
            if (ObjectChanged != null)
            {
                ObjectChanged(this, oldValue, newValue);
            }
        }

        /// <summary>
        /// 發生於偏移值變更
        /// </summary>
        protected void OnOffsetChanged()
        {
            if (OffsetChanged != null)
            {
                OffsetChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 發生於物件的定位位置變更時
        /// </summary>
        protected virtual void OnAnchorChanged(DirectionType oldValue, DirectionType newValue)
        {
            if (AnchorChanged != null)
            {
                AnchorChanged(this, oldValue, newValue);
            }
        }
        #endregion

        #region ===== 屬性 =====
        private ITargetability _Object;
        /// <summary>
        /// 取得關聯的物件
        /// </summary>
        public ITargetability Object
        {
            get { return _Object; }
            private set
            {
                if (_Object == value) return;
                ITargetability oldValue = _Object;
                _Object = value;
                OnObjectChanged(oldValue, value);
            }
        }

        /// <summary>
        /// 目標類型
        /// </summary>
        public TargetType TargetType { get; private set; }

        private DirectionType _Anchor = DirectionType.Center;
        /// <summary>
        /// 目標為單位時的定位點(預設為中心)
        /// </summary>
        public DirectionType Anchor
        {
            get { return _Anchor; }
            set
            {
                if (_Anchor == value) return;
                DirectionType oldValue = _Anchor;
                _Anchor = value;
                OnAnchorChanged(oldValue, value);
            }
        }

        /// <summary>
        /// 取得目標X軸偏移位置
        /// </summary>
        public float OffsetX { get; private set; }

        /// <summary>
        /// 取得目標Y軸偏移位置
        /// </summary>
        public float OffsetY { get; private set; }

        /// <summary>
        /// 取得目標偏移角度
        /// </summary>
        public double OffsetAngle { get; private set; }

        /// <summary>
        /// 取得目標偏移距離
        /// </summary>
        public double OffsetDistance { get; private set; }

        private float _BaseX;
        // <summary>
        /// 取得目標尚未偏移的X座標
        /// </summary>
        public float BaseX
        {
            get
            {
                if (_Object != null)
                {
                    _BaseX = _Object.GetTargetX(Anchor);
                }
                return _BaseX;
            }
        }

        private float _BaseY;
        /// <summary>
        /// 取得目標尚未偏移的Y座標
        /// </summary>
        public float BaseY
        {
            get
            {
                if (_Object != null)
                {
                    _BaseY = _Object.GetTargetY(Anchor);
                }
                return _BaseY;
            }
        }

        /// <summary>
        /// 取得目標物件偏移後的X座標
        /// </summary>
        public float X { get { return BaseX + OffsetX; } }

        /// <summary>
        /// 取得目標物件偏移後的Y座標
        /// </summary>
        public float Y { get { return BaseY + OffsetY; } }
        #endregion

        #region ===== 建構式 =====
        /// <summary>
        /// 新增無目標的目標管理物件
        /// </summary>
        public TargetSet()
        {
            ClearObject();
        }

        /// <summary>
        /// 新增指定目標的目標管理物件
        /// </summary>
        /// <param name="targetObject">目標物件</param>
        public TargetSet(ITargetability targetObject)
        {
            SetObject(targetObject);
        }

        /// <summary>
        /// 新增指定場景追蹤點的目標管理物件
        /// </summary>
        /// <param name="scene">指定場景</param>
        public TargetSet(SceneBase scene)
        {
            SetObject(scene);
        }

        /// <summary>
        /// 新增指定物件的目標管理物件
        /// </summary>
        /// <param name="objectBase">物件</param>
        public TargetSet(ObjectBase objectBase)
        {
            SetObject(objectBase);
        }

        /// <summary>
        /// 新增指定位置配置物件的目標管理物件
        /// </summary>
        /// <param name="layout">位置配置物件</param>
        public TargetSet(LayoutSet layout)
        {
            SetObject(layout);
        }

        /// <summary>
        /// 新增指定座標物件的目標管理物件
        /// </summary>
        /// <param name="point">座標物件</param>
        public TargetSet(PointObject point)
        {
            SetObject(point);
        }
        #endregion

        #region ===== 方法 =====
        /// <summary>
        /// 設定追蹤物件為指定場景追蹤點
        /// </summary>
        /// <param name="scene">指定場景</param>
        public void SetObject(SceneBase scene)
        {
            if (scene == null)
            {
                TargetType = TargetType.None;
                Object = null;
            }
            else
            {
                TargetType = TargetType.Scene;
                Object = scene;
            }
        }

        /// <summary>
        /// 設定追蹤物件為指定物件
        /// </summary>
        /// <param name="objectBase">物件</param>
        public void SetObject(ObjectBase objectBase)
        {
            if (objectBase == null)
            {
                TargetType = TargetType.None;
                Object = null;
            }
            else
            {
                TargetType = TargetType.GameObejct;
                Object = objectBase;
            }
        }

        /// <summary>
        /// 設定追蹤物件為指定位置配置物件
        /// </summary>
        /// <param name="layout">位置配置物件</param>
        public void SetObject(LayoutSet layout)
        {
            if (layout == null)
            {
                TargetType = TargetType.None;
                Object = null;
            }
            else
            {
                TargetType = TargetType.Layout;
                Object = layout;
            }
        }

        /// <summary>
        /// 設定追蹤物件為指定座標物件
        /// </summary>
        /// <param name="point">座標物件</param>
        public void SetObject(PointObject point)
        {
            if (point == null)
            {
                TargetType = TargetType.None;
                Object = null;
            }
            else
            {
                TargetType = TargetType.Point;
                Object = point;
            }
        }

        /// <summary>
        /// 設定追蹤物件為指定物件
        /// </summary>
        /// <param name="point">指定物件</param>
        public void SetObject(ITargetability target)
        {
            if (target is SceneBase) TargetType = TargetType.Scene;
            else if (target is ObjectBase) TargetType = TargetType.GameObejct;
            else if (target is LayoutSet) TargetType = TargetType.Layout;
            else if (target is PointObject) TargetType = TargetType.Point;
            else TargetType = TargetType.None;
            Object = target;
        }

        /// <summary>
        /// 清除追蹤物件
        /// </summary>
        public void ClearObject()
        {
            TargetType = TargetType.None;
            Object = null;
        }

        /// <summary>
        /// 取得目標點
        /// </summary>
        /// <returns></returns>
        public PointF GetPoint()
        {
            return new PointF(X, Y);
        }

        /// <summary>
        /// 使用座標定義偏移位置
        /// </summary>
        /// <param name="offsetX">X軸偏移位置</param>
        /// <param name="offsetY">Y軸偏移位置</param>
        public void SetOffsetByXY(float offsetX, float offsetY)
        {
            if (OffsetX == offsetX && OffsetY == offsetY) return;
            OffsetX = offsetX;
            OffsetY = offsetY;
            OffsetAngle = Function.GetAngle(0, 0, offsetX, offsetY);
            OffsetDistance = Function.GetDistance(0, 0, offsetX, offsetY);
            OnOffsetChanged();
        }

        /// <summary>
        /// 使用角度距離定義偏移位置
        /// </summary>
        /// <param name="offsetAngle">目標偏移角度</param>
        /// <param name="offsetDistance">目標偏移距離</param>
        public void SetOffsetByAngle(double offsetAngle, double offsetDistance)
        {
            if (OffsetAngle == offsetAngle && OffsetDistance == offsetDistance) return;
            OffsetAngle = offsetAngle;
            OffsetDistance = offsetDistance;
            PointF offsetPoint = Function.GetOffsetPoint(0, 0, offsetAngle, offsetDistance);
            OffsetX = offsetPoint.X;
            OffsetY = offsetPoint.Y;
            OnOffsetChanged();
        }

        /// <summary>
        /// 清除偏移值
        /// </summary>
        public void ClearOffset()
        {
            OffsetX = 0;
            OffsetY = 0;
            OffsetAngle = 0;
            OffsetDistance = 0;
            OnOffsetChanged();
        }
        #endregion
    }
}
