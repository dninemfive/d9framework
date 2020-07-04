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
    [StaticConstructorOnStartup]
    static class NegativeFertilityPatch
    {
        public static float EffectiveMaxFertility;

        static NegativeFertilityPatch()
        {
            IEnumerable<TerrainDef> terrainsByFertility = (from td in DefDatabase<TerrainDef>.AllDefsListForReading
                                                           orderby td.fertility descending
                                                           select td);
            if (terrainsByFertility.EnumerableNullOrEmpty())
            {
                ULog.Error("Negative Fertility Patch: terrainsByFertility was empty. Setting EffectiveMaxFertility to 1.");
                EffectiveMaxFertility = 1f;
            }
            else
            {
                EffectiveMaxFertility = terrainsByFertility.ElementAt(0).fertility;
            }
        }

        [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRateFactor_Fertility), MethodType.Getter)]
        class NegativeFertilityPostfix
        {
            [HarmonyPostfix]
            public static void GrowthRateFactor_FertilityPostfix(ref float __result, ref Plant __instance)
            {
                UseNegativeFertility me;
                if ((me = __instance.def.GetModExtension<UseNegativeFertility>()) != null)
                {
                    __result = Mathf.Clamp((EffectiveMaxFertility - __instance.Map.fertilityGrid.FertilityAt(__instance.Position)) * __instance.def.plant.fertilitySensitivity + (1f - __instance.def.plant.fertilitySensitivity),
                               me.minFertility, 
                               me.maxFertility);
                }
            }
        }
    }
    public class UseNegativeFertility : DefModExtension
    {
        public float minFertility = 0.05f, maxFertility = 1.5f;
    }
}