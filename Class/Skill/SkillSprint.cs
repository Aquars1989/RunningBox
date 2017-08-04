using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RunningBox
{
    class SkillSprint : ISkill
    {
        public int CostEnargy { get; set; }
        public int CostEnargyPerRound { get; set; }

        public SkillSprint()
        {
            CostEnargy = 300;
        }

        public void DoWhenRound(){}
        public void DoWhenDrawing() { }

        public void Use() { }
        public void Use(ObjectPlayer playerObject)
        {
            if (playerObject == null || playerObject.Energy < CostEnargy ||playerObject.Moves.Count==0) return;
            playerObject.Energy -= CostEnargy;

            int lastIndex =playerObject.Moves.Count-1;
           playerObject.Moves[lastIndex]= new PointF( playerObject.Moves[lastIndex].X*10, playerObject.Moves[lastIndex].Y*10);
        }

        public void Reset()
        {

        }

    }
}
