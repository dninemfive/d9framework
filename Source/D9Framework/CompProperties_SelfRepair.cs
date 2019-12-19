﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace D9Framework
{
    class CompProperties_SelfRepair : CompProperties
    {
#pragma warning disable CS0649 //disable the warning that this field is never assigned to, as the game handles that
        public int TicksPerRepair;
        
        public CompProperties_SelfRepair()
        {
            base.compClass = typeof(CompSelfRepair);
        }
    }
}
