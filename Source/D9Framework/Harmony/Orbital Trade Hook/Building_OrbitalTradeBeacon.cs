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
        // DO NOT call base.TradeableCells() in an override here! Because of how the patch works, an infinite loop would result. This is bad, and you shouldn't do it.
        public new abstract IEnumerable<IntVec3> TradeableCells();
    }
}