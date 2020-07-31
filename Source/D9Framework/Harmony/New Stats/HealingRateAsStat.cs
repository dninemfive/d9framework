using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace D9Framework
{
    [ClassWithPatches("Calculate Bleed Rate Patch", "ApplyCalculateBleedRate", "D9FSettingsApplyCBR")]
    static class HealingRateAsStat
    {
        [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.HealthTick))]
        class HealingRateAsStatPatch
        {
            [HarmonyTranspiler]
            public static void HealthTickTranspiler(ref Pawn_HealthTracker __instance)
            {
                // multiply num3 by healing rate stat
            }
        }
    }
}
