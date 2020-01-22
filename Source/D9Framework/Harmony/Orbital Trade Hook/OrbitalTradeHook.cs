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
    [StaticConstructorOnStartup]
    public class OrbitalTradeHook
    {
        //static MethodInfo RWTradeBeaconAllPowered, PatchAllPowered, RWTradeUtilityAllLaunchable, PatchAllLaunchable;
        static OrbitalTradeHook()
        {
            //CacheMethods();
            var harmony = HarmonyInstance.Create("com.dninemfive.d9framework.oth");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            ULog.DebugMessage("Orbital Trade Hook loaded.");
        }

        /*public static IEnumerable<Building> AllPowered(Map map)
        {
            foreach (RimWorld.Building_OrbitalTradeBeacon b in RimWorld.Building_OrbitalTradeBeacon.AllPowered(map)) yield return b; 
            foreach(D9Framework.Building_OrbitalTradeBeacon b in map.listerBuildings.AllBuildingsColonistOfClass<D9Framework.Building_OrbitalTradeBeacon>())
            {                
                CompPowerTrader power = b.GetComp<CompPowerTrader>();
                CompRefuelable fuel = b.GetComp<CompRefuelable>();
                if ((power == null || (power != null && power.PowerOn)) && (fuel == null || (fuel != null && fuel.HasFuel))) yield return b;                
            }
        }*/
        // TODO: transpile Building_OrbitalTradeBeacon.AllPowered to do the standard (fuel?.HasFuel || power?.Powered) check

        // thanks, lbmaian!
        // from https://discordapp.com/channels/214523379766525963/215496692047413249/669060018645237800
        [HarmonyPatch(typeof(Building_OrbitalTradeBeacon), nameof(Building_OrbitalTradeBeacon.TradeableCells))]
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

        //TODO: patch DropCellFinder.TradeDropSpot() to find any cell near either base or this OrbitalTradeBeacon. Not super high priority since it always returns something, but would be nice
    }
    }
}
