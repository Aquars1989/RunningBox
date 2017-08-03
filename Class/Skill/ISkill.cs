
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RunningBox
{
    interface ISkill
    {
        int CostEnargy { get; set; }
        int CostEnargyPerAction { get; set; }

        void DoWhenAction();
        void DoWhenDrawing();
        void Use();
        void Use(ObjectPlayer playerObject);
        void Reset();
    }
}
