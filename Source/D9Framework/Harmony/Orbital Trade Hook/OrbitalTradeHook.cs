using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace D9Framework
{
    /// <summary>
    /// Hook allowing modders to easily create custom trade beacons by extending <c>D9Framework.Building_OrbitalTradeBeacon</c>
    /// </summary>
    public class OrbitalTradeHook
    {
        static OrbitalTradeHook()
        {            
        }
        // TODO: transpile Building_OrbitalTradeBeacon.AllPowered to do the standard (fuel?.HasFuel || power?.Powered) check

        // thanks, lbmaian!
        // from https://discordapp.com/channels/214523379766525963/215496692047413249/669060018645237800
        [HarmonyPatch(typeof(Building_OrbitalTradeBeacon), nameof(Building_OrbitalTradeBeacon.TradeableCellsAround), new Type[] { typeof(IntVec3), typeof(Map) })]
        static class TradeableCellsPatch
        {
            [ThreadStatic]
            static bool withinOverrideCall = false; //will hopefully prevent infinite loops if a CustomTradeBeacon calls base.TradeableCells

            [HarmonyPrefix]
            static bool Prefix(Building_OrbitalTradeBeacon __instance, ref IEnumerable<IntVec3> __result)
            {
                if (!withinOverrideCall && __instance is Building_CustomTradeBeacon inst)
                {
                    try
                    {
                        withinOverrideCall = true;
                        __result = inst.TradeableCells();
                    }
                    finally
                    {
                        withinOverrideCall = false;
                    }
                    return false;
                }
                return true;
            }
        }
    }
}
