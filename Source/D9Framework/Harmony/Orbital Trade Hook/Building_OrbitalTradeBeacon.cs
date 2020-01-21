using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using RimWorld;

namespace D9Framework
{
    /// <summary>
    /// Abstract class you can extend to make Orbital Trade Beacons using the orbital trade hook.
    /// </summary>
    public abstract class Building_CustomOrbitalTradeBeacon : Building_OrbitalTradeBeacon
    {
        public new abstract IEnumerable<IntVec3> TradeableCells();
    }
}