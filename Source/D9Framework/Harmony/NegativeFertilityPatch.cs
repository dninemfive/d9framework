using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using Verse;
using RimWorld;

namespace D9Framework
{
    static class NegativeFertilityPatch
    {
        [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRateFactor_Fertility), new Type[] { })]
        class NegativeFertilityPostfix
        {
            [HarmonyPostfix]
            public static void GrowthRateFactor_FertilityPostfix(ref float __result, ref Plant __instance)
            {
                if (__instance.def.HasModExtension<UseNegativeFertility>())
                {
                    __result = Mathf.Clamp01(1f - __instance.Map.fertilityGrid.FertilityAt(__instance.Position)) * __instance.def.plant.fertilitySensitivity + (1f - __instance.def.plant.fertilitySensitivity);
                }
            }
        }
    }
    public class UseNegativeFertility : DefModExtension { }
}
