using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 代表指定目標物件偏移位置的目標物件
    /// </summary>
    public class TargetOffset : ITarget
    {
        private ITarget _Target;
        /// <summary>
        /// 作為偏移原點的目標物件(必要)
        /// </summary>
        public ITarget Target
        {
            get { return _Target; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                _Target = value;
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

        /// <summary>
        /// 目標物件偏移後的X座標
        /// </summary>
        public float X
        {
            get { return Target.X + OffsetX; }
        }

        /// <summary>
        /// 目標物件偏移後的Y座標
        /// </summary>
        public float Y
        {
            get { return Target.X + OffsetY; }
        }

        /// <summary>
        /// 新增代表指定目標物件偏移位置的目標物件
        /// </summary>
        /// <param name="target">作為原點的目標物件</param>
        /// <param name="offsetPoint">目標偏移位置</param>
        public TargetOffset(ITarget target,PointF offsetPoint)
        {
            Target = target;
            SetOffsetByXY(offsetPoint.X, offsetPoint.Y);
        }

        /// <summary>
        /// 新增代表指定目標物件偏移位置的目標物件
        /// </summary>
        /// <param name="target">作為原點的目標物件</param>
        /// <param name="offsetAngle">偏移角度</param>
        /// <param name="offsetDistance">偏移距離</param>
        public TargetOffset(ITarget target, double offsetAngle, double offsetDistance)
        {
            Target = target;
            SetOffsetByAngle(offsetAngle, offsetDistance);
        }

        /// <summary>
        /// 取得目標物件偏移後的座標
        /// </summary>
        /// <returns>目標物件偏移後的座標</returns>
        public PointF GetPoint()
        {
            return new PointF(Target.X + OffsetX, Target.X + OffsetY);
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
        }
    }
}
