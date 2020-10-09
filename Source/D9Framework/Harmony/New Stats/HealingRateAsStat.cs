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
                List<CodeInstruction> instr = instructions.ToList();
                bool hasInjected = false;
                /* multiply num3 by healing rate stat just after the hediff factors*/                
                for(int i = 3; i < instr.Count; i++)
                {
                    // thank Wiri
                    if (!hasInjected
                        && instr[i].opcode == OpCodes.Ldloca_S && (byte)instr[i].operand == 10
                        && instr[i + 1].opcode == OpCodes.Call && instr[i + 1].operand == AccessTools.Method(typeof(List<Hediff>), nameof(MoveNext)))
                        {
                            yield return new CodeInstruction(OpCodes.Ldloc_0);
                        // Ldfld shit
                            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field().)
                            yield return new CodeInstruction(OpCodes.Mul);
                            hasInjected = true;
                        }
                }
                // just to get it to compile for now
                // return instructions;
            }
        }
    }
}
