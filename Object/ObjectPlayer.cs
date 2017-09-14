using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 基本玩家物件範本,不包含任何技能,特性
    /// </summary>
    public class ObjectPlayer : ObjectActive
    {
        /// <summary>
        /// 建立一個基本玩家物件範本
        /// </summary>
        /// <param name="x">物件中心位置X</param>
        /// <param name="y">物件中心位置Y</param>
        /// <param name="maxMoves">最大調整值紀錄數量</param>
        /// <param name="width">物件寬度</param>
        /// <param name="height">物件高度</param>
        /// <param name="speed">基本速度</param>
        /// <param name="leage">物件所屬陣營,供技能或特性判定</param>
        /// <param name="drawObject">繪製物件</param>
        /// <param name="moveObject">移動物件</param>
        public ObjectPlayer(float x, float y, int maxMoves, int width, int height, float speed, LeagueType leage, DrawBase drawObject, MovePlayer moveObject) :
            base(x, y, width, height, -1, leage, ShapeType.Ellipse, drawObject, moveObject) { }

        public override void Kill(ObjectBase killer, ObjectDeadType deadType)
        {
            if (Status != ObjectStatus.Alive) return;

            if (deadType == ObjectDeadType.Collision)
            {
                Scene.EffectObjects.Add(new EffectShark(Scene.Sec(0.6F), 10) { CanBreak = false });
            }
            base.Kill(killer, deadType);
        }
    }
}
