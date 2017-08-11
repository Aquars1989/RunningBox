﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    /// <summary>
    /// 基礎可互動活動物件
    /// </summary>
    public class ObjectActive : ObjectBase
    {
        /// <summary>
        /// 存活回合計數
        /// </summary>
        public int LifeRound { get; set; }

        /// <summary>
        /// 存活回合計數最大值,小於0為永久
        /// </summary>
        public int LifeRoundMax { get; set; }

        /// <summary>
        /// 物件所屬陣營,供技能或特性判定
        /// </summary>
        public League League { get; set; }

        /// <summary>
        /// 追尋目標
        /// </summary>
        public ITarget Target { get; set; }

        /// <summary>
        /// 移動調整值紀錄
        /// </summary>
        public List<PointF> Moves { get; set; }

        /// <summary>
        /// 最大調整值紀錄數量
        /// </summary>
        public int MaxMoves { get; set; }

        /// <summary>
        /// 移動速度,決定每個移動調整值的距離
        /// </summary>
        public float Speed { get; set; }

        private int _Energy;
        /// <summary>
        /// 能量,使用技能用
        /// </summary>
        public int Energy
        {
            get { return _Energy; }
            set
            {
                _Energy = value;
                if (_Energy < 0) _Energy = 0;
                else if (_Energy > EnergyMax) _Energy = EnergyMax;
            }
        }

        /// <summary>
        /// 能量上限,能量不可高於此數值
        /// </summary>
        public int EnergyMax { get; set; }

        /// <summary>
        /// 每回合自動恢復的能量數
        /// </summary>
        public int EnergyGetPerRound { get; set; }


        /// <summary>
        /// 活動物件擁有的技能群組
        /// </summary>
        public SkillCollection Skills { get; set; }

        /// <summary>
        /// 活動物件擁有的特性群組
        /// </summary>
        public PropertyCollection Propertys { get; set; }

        /// <summary>
        /// 建立一個互動性活動物件
        /// </summary>
        /// <param name="x">物件位置X</param>
        /// <param name="y">物件位置Y</param>
        /// <param name="maxMoves">最大調整值紀錄數量</param>
        /// <param name="size">物件大小</param>
        /// <param name="speed">速度</param>
        /// <param name="life">存活時間,小於0為永久</param>
        /// <param name="leage">物件所屬陣營,供技能或特性判定</param>
        /// <param name="target">追蹤目標</param>
        public ObjectActive(float x, float y, int maxMoves, int size, float speed, int life, League leage, TargetObject target)
            : this()
        {
            Status = ObjectStatus.Alive;
            MaxMoves = maxMoves;
            X = x;
            Y = y;
            Size = size;
            Speed = speed;
            LifeRoundMax = life;
            Target = target;
            League = leage;
        }

        /// <summary>
        /// 建立一個互動性活動物件
        /// </summary>
        public ObjectActive()
        {
            Skills = new SkillCollection(this);
            Propertys = new PropertyCollection(this);
            Moves = new List<PointF>();
            EnergyMax = 1000;
            Energy = 1000;
            EnergyGetPerRound = 5;
        }

        /// <summary>
        /// 活動物件每回合動作
        /// </summary>
        public override void Action()
        {
            Skills.AllDoAutoCast();

            Skills.AllDoBeforeAction();
            Propertys.AllDoBeforeAction();

            Skills.AllDoBeforeActionEnergyGet();
            Propertys.AllDoBeforeActionEnergyGet();

            ActionEnergyGet();

            Skills.AllDoBeforeActionPlan();
            Propertys.AllDoBeforeActionPlan();

            ActionPlan();

            Skills.AllDoBeforeActionMove();
            Propertys.AllDoBeforeActionMove();

            ActionMove();

            Skills.AllDoAfterAction();
            Propertys.AllDoAfterAction();

            Skills.AllSettlement();
            Propertys.AllSettlement();

            Propertys.ClearAllDisabled();
        }

        /// <summary>
        /// 物件能量調整
        /// </summary>
        protected virtual void ActionEnergyGet()
        {
            Energy += EnergyGetPerRound;
        }

        /// <summary>
        /// 物件在回合內進行的規劃活動
        /// </summary>
        protected virtual void ActionPlan()
        {
            if (Target != null)
            {
                double direction = Function.PointRotation(X, Y, Target.X, Target.Y);
                float moveX = (float)Math.Cos(direction / 180 * Math.PI) * (Speed / 100F);
                float moveY = (float)Math.Sin(direction / 180 * Math.PI) * (Speed / 100F);
                Moves.Add(new PointF(moveX, moveY));
            }
        }

        /// <summary>
        /// 物件在回合內進行的移動活動
        /// </summary>
        protected virtual void ActionMove()
        {
            if (Moves.Count > MaxMoves)
            {
                Moves.RemoveRange(0, Moves.Count - MaxMoves);
            }

            float moveTotalX = 0;
            float moveTotalY = 0;
            foreach (PointF pt in Moves)
            {
                moveTotalX += pt.X;
                moveTotalY += pt.Y;
            }

            X += moveTotalX * Scene.WorldSpeed;
            Y += moveTotalY * Scene.WorldSpeed;

            LifeRound++;
            if (LifeRoundMax >= 0 && LifeRound >= LifeRoundMax)
            {
                Kill(null, ObjectDeadType.LifeEnd);
            }

        }

        /// <summary>
        /// 殺死此物件
        /// </summary>
        /// <param name="killer">殺手物件</param>
        public override void Kill(ObjectActive killer, ObjectDeadType deadType)
        {
            if (Status == ObjectStatus.Alive)
            {
                base.Kill(killer, deadType);
                Skills.AllDoAfterDead(killer, deadType);
                Propertys.AllDoAfterDead(killer, deadType);
            }
        }

        /// <summary>
        /// 繪製物件
        /// </summary>
        /// <param name="g">Graphics物件</param>
        public override void Draw(Graphics g)
        {
            Skills.AllDoBeforeDraw(g);
            base.Draw(g);
            Skills.AllDoAfterDraw(g);
        }
    }

    /// <summary>
    /// 活動物件所屬陣營
    /// </summary>
    public enum League
    {
        Player,
        Ememy
    }
}