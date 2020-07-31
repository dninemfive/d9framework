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
    static class BleedRateAsStat
    {
        [HarmonyPatch(typeof(HediffSet), "CalculateBleedRate")]
        class BleedRateAsStatPatch
        {
            [HarmonyPostfix]
            public static void CalculateBleedRatePostfix(ref float __result, ref HediffSet __instance)
            {
                __result *= __instance.pawn.GetStatValue(D9FrameworkDefOf.BleedRateFactor);
            }
        }
    }
}
