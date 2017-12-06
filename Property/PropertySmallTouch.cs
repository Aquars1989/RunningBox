using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 擁有此特性的物件十分接近的人時會增加分數
    /// </summary>
    class PropertySmallTouch : PropertyBase
    {
        private CounterObject _SkipTime;

        /// <summary>
        /// 新增碰撞特性,擁有此特性的物件十分接近的人時會增加分數
        /// </summary>
        /// <param name="skipTime">連續檢測間格時間</param>
        public PropertySmallTouch(int skipTime)
        {
            _SkipTime = new CounterObject(skipTime, skipTime);
        }

        public override void DoActionMoving()
        {
            SceneGaming scene = Scene as SceneGaming;
            if (scene == null) return;

            if (_SkipTime.IsFull)
            {
                Rectangle checkRect = new Rectangle(Owner.Layout.Rectangle.X - 2, Owner.Layout.Rectangle.Y - 2, Owner.Layout.Rectangle.Width + 4, Owner.Layout.Rectangle.Height + 4);

                if (Owner.Status == ObjectStatus.Alive && Status == PropertyStatus.Enabled &&
                   (Owner.Propertys.Affix & SpecialStatus.Ghost) != SpecialStatus.Ghost)
                {
                    for (int i = 0; i < Owner.Container.Count; i++)
                    {
                        ObjectBase objectActive = Owner.Container[i];
                        if (objectActive.Status != ObjectStatus.Alive || Function.IsFriendly(objectActive.League, Owner.League)) continue;

                        // 特殊狀態判定 具碰撞 非鬼魂
                        if ((objectActive.Propertys.Affix & SpecialStatus.Collision) != SpecialStatus.Collision ||
                            (objectActive.Propertys.Affix & SpecialStatus.Ghost) == SpecialStatus.Ghost)
                        {
                            continue;
                        }

                        // 碰撞判定
                        if (!Function.IsCollison(Owner.Layout.CollisonShape, checkRect, objectActive.Layout.CollisonShape, objectActive.Layout.Rectangle)) continue;

                        scene.AddScoreToPlayer("擦身而過", 1000);
                        _SkipTime.Value = 0;
                        break;
                    }
                }
            }
            else
            {
                _SkipTime.Value += Scene.IntervalOfRound;
            }
            base.DoActionMoving();
        }

        protected override void OnTargetObjectChanged(ITargetability oldValue, ITargetability newValue)
        {
            if (Target.TargetType != TargetType.GameObejct)
            {
                Target.ClearObject();
            }
            base.OnTargetObjectChanged(oldValue, newValue);
        }
    }
}
