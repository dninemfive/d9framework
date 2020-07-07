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
    public class NegativeFertilityPatch : ClassWithPatches
    {
        public override string PlainName => "Negative Fertility Patch";

        public static float MaxNaturalFertility;

        public NegativeFertilityPatch()
        {
            HashSet<TerrainDef> allPossibleNaturalTerrains = new HashSet<TerrainDef>();
            foreach (BiomeDef bd in DefDatabase<BiomeDef>.AllDefsListForReading)
            {
                foreach (TerrainThreshold tt in bd.terrainsByFertility) allPossibleNaturalTerrains.Add(tt.terrain);
                foreach (TerrainPatchMaker tpm in bd.terrainPatchMakers)
                    foreach (TerrainThreshold tt in tpm.thresholds) allPossibleNaturalTerrains.Add(tt.terrain);
            }
            IEnumerable<TerrainDef> terrainsByFertility = (from td in DefDatabase<TerrainDef>.AllDefsListForReading
                                                           where allPossibleNaturalTerrains.Contains(td)
                                                           orderby td.fertility descending
                                                           select td);
            if (terrainsByFertility.EnumerableNullOrEmpty())
            {
                ULog.Error("Negative Fertility Patch: terrainsByFertility was empty. Setting EffectiveMaxFertility to 1.");
                MaxNaturalFertility = 1f;
            }
            else
            {
                MaxNaturalFertility = terrainsByFertility.First().fertility;
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
                    __result = Mathf.Clamp((MaxNaturalFertility - __instance.Map.fertilityGrid.FertilityAt(__instance.Position)) * __instance.def.plant.fertilitySensitivity + (1f - __instance.def.plant.fertilitySensitivity),
                               me.minFertility, 
                               me.maxFertility);
                }
            }
        }
    }
    public class UseNegativeFertility : DefModExtension
    {
        public float minFertility = 0.05f, maxFertility = 1.4f;
    }
}