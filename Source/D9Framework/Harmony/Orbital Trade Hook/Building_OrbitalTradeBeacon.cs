using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;

namespace D9Framework
{
    public abstract class Building_OrbitalTradeBeacon : Building
    {
        public abstract IEnumerable<IntVec3> TradeableCells();
    }
}