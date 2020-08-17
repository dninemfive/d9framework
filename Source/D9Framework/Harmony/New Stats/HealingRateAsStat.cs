using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace D9Framework
{
    [ClassWithPatches("Heal Rate Patch", "ApplyHealRatePatch", "D9FSettingsApplyHRP")]
    static class HealingRateAsStat
    {
        [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.HealthTick))]
        class HealingRateAsStatPatch
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> HealthTickTranspiler(IEnumerable<CodeInstruction> instructions)
            {
                /*// multiply num3 by healing rate stat just before the hediff factors
                List<CodeInstruction> instr = instructions.ToList();
                for(int i = 3; i < instr.Count; i++)
                {
                    if (instr[i - 3].Branches(out Label? label);
                }*/
                // just to get it to compile for now
                return instructions;
            }
        }
    }
}
