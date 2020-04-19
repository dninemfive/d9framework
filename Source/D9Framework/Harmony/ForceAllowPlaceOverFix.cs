using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using RimWorld;

namespace D9Framework
{
    /// <summary>
    /// Makes BuildableDef.ForceAllowPlaceOver actually cause the appropriate things to ignore the defs in question
    /// </summary>
    static class ForceAllowPlaceOverFix
    {
        /// <summary>
        /// Inserts the equivalent of "if newDef.ForceAllowPlaceOver(oldDef) return true;" into GenConstruct.CanPlaceBlueprintOver
        /// </summary>
        [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.CanPlaceBlueprintOver), new Type[] { typeof(BuildableDef), typeof(ThingDef) })]
        class GenConstructPlaceOver
        {            
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> CanPlaceBlueprintOverTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                List<CodeInstruction> instrList = instructions.ToList();
                Label returnTrue = generator.DefineLabel();
                for (int i = 0; i < instrList.Count; i++)
                {
                    // Looking for the first half of (oldDef == ThingDefOf.SteamGeyser && !newDef.ForceAllowPlaceOver(oldDef))
                    if (instrList[i + 1].opcode == OpCodes.Ldsfld                                                         // IL 0075: ldsfld class Verse.ThingDef RimWorld.ThingDefOf::SteamGeyser
                        && instrList[i + 1].operand as FieldInfo == AccessTools.Field(typeof(ThingDefOf), nameof(ThingDefOf.SteamGeyser)))
                    {
                        // if newDef.ForceAllowPlaceOver(oldDef) return true;
                        yield return new CodeInstruction(OpCodes.Ldarg_0); // load newDef onto the stack
                        yield return new CodeInstruction(OpCodes.Ldarg_1); // load oldDef onto the stack
                        // call newDef.ForceAllowPlaceOver
                        yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(BuildableDef), nameof(BuildableDef.ForceAllowPlaceOver)));
                        // if the above call leaves 1 on the stack, jump to my "return true" statement
                        yield return new CodeInstruction(OpCodes.Brtrue, returnTrue);
                    }
                    yield return instrList[i];
                }
                // append a "return true" at the end with my label
                CodeInstruction loadTrue = new CodeInstruction(OpCodes.Ldc_I4_1);
                loadTrue.labels.Add(returnTrue);
                yield return loadTrue;
                yield return new CodeInstruction(OpCodes.Ret);
            }
        }

        /// <summary>
        /// Inserts the equivalent of "if newDef.ForceAllowPlaceOver(oldDef) return true;" into GenConstruct.BlocksConstruction
        /// </summary>
        [HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.BlocksConstruction), new Type[] { typeof(Thing), typeof(Thing) })]
        class GenConstructBlocksConstruction
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> CanPlaceBlueprintOverTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                List<CodeInstruction> instrList = instructions.ToList();
                Label returnTrue = generator.DefineLabel();
                for (int i = 0; i < instructions.Count(); i++)
                {
                    // Looking for the first half of (t.def == ThingDefOf.SteamGeyser && thingDef.entityDefToBuild.ForceAllowPlaceOver(t.def))
                    if (instrList[i+2].opcode == OpCodes.Ldsfld                                                     // IL 00D7: ldsfld class Verse.ThingDef RimWorld.ThingDefOf::SteamGeyser
                        && instrList[i+2].operand as FieldInfo == AccessTools.Field(typeof(ThingDefOf), nameof(ThingDefOf.SteamGeyser)))
                    {
                        // if newDef.ForceAllowPlaceOver(oldDef) return true;
                        yield return new CodeInstruction(OpCodes.Ldarg_0); // load newDef onto the stack
                        yield return new CodeInstruction(OpCodes.Ldarg_1); // load oldDef onto the stack
                        // call newDef.ForceAllowPlaceOver
                        yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(BuildableDef), nameof(BuildableDef.ForceAllowPlaceOver)));
                        // if the above call leaves 1 on the stack, jump to my "return true" statement
                        yield return new CodeInstruction(OpCodes.Brtrue, returnTrue);
                    }
                    yield return instrList[i];
                }
                // append a "return true" at the end with my label
                CodeInstruction loadTrue = new CodeInstruction(OpCodes.Ldc_I4_1);
                loadTrue.labels.Add(returnTrue);
                yield return loadTrue;
                yield return new CodeInstruction(OpCodes.Ret);
            }
        }
    }
}
