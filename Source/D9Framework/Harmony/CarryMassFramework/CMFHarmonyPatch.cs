using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace D9Framework
{
    internal static class CMFHarmonyPatch
    {
        static public void DoPatch(Harmony harmonyCMF)
        {
            harmonyCMF.Patch(original: AccessTools.Method(typeof(StatWorker), "GetBaseValueFor"), prefix: null, postfix: null,
                transpiler: new HarmonyMethod(typeof(CMFHarmonyPatch), nameof(CMFHarmonyPatch.StatWorker_GetBaseValueForTranspiler)));

            harmonyCMF.Patch(original: AccessTools.Method(typeof(StatWorker), "GetExplanationUnfinalized"), prefix: null, postfix: null,
                transpiler: new HarmonyMethod(typeof(CMFHarmonyPatch), nameof(CMFHarmonyPatch.StatWorker_GetExplanationUnfinalizedTranspiler)));

            harmonyCMF.Patch(original: AccessTools.Method(typeof(MassUtility), nameof(MassUtility.Capacity)), prefix: null, postfix: null,
                transpiler: new HarmonyMethod(typeof(CMFHarmonyPatch), nameof(MassUtility_CapacityTranspiler)));

            harmonyCMF.Patch(original: AccessTools.Method(typeof(MassUtility), nameof(MassUtility.GearMass)), prefix: null, postfix: null,
                transpiler: new HarmonyMethod(typeof(CMFHarmonyPatch), nameof(MassUtility_GearMassTranspiler)));

            harmonyCMF.Patch(original: AccessTools.Method(typeof(MassUtility), nameof(MassUtility.InventoryMass)), prefix: null, postfix: null,
                transpiler: new HarmonyMethod(typeof(CMFHarmonyPatch), nameof(MassUtility_InventoryMassTranspiler)));

            harmonyCMF.Patch(original: AccessTools.Method(typeof(MassUtility), nameof(MassUtility.CountToPickUpUntilOverEncumbered)), prefix: null, postfix: null,
                transpiler: new HarmonyMethod(typeof(CMFHarmonyPatch), nameof(MassUtility_CountToPickUpUntilOverEncumberedTranspiler)));

            harmonyCMF.Patch(original: AccessTools.Method(typeof(MassUtility), nameof(MassUtility.WillBeOverEncumberedAfterPickingUp)), prefix: null, postfix: null,
                transpiler: new HarmonyMethod(typeof(CMFHarmonyPatch), nameof(MassUtility_WillBeOverEncumberedAfterPickingUpTranspiler)));           

            harmonyCMF.Patch(original: AccessTools.Method(typeof(ITab_Pawn_Gear), "DrawThingRow"), prefix: null, postfix: null,
                transpiler: new HarmonyMethod(typeof(CMFHarmonyPatch), nameof(ITab_Pawn_Gear_DrawThingRowTranspiler)));
        }

        static public float DoGetBaseValueFor(StatDef stat, StatRequest request)
        // Helper function called by the transpiled StatWorker.GetBaseValueFor
        // returns the final value of the specified defaultBaseStat if 
        // the StatDef is a DerivedStatDef
        {
            float result = stat.defaultBaseValue;
            if (stat is DerivedStatDef)
            {
                DerivedStatDef derivedStat = (stat as DerivedStatDef);
                if (stat != null)
                {
                    StatDef baseStat = derivedStat.defaultBaseStat;
                    if (baseStat != null)
                    {
                        Thing thing = request.Thing;
                        if (thing != null)
                        {
                            result = thing.GetStatValue(baseStat, true);
                        }
                    }
                }
            }
            return result;
        }

        public static float DoCapacity(Pawn p)
        // Helper function called by the transpiled StatWorker.Capacity
        // returns the final value of the CarryMass stat
            => p.GetStatValue(CMFStatDefOf.CarryMass);

        public delegate void CaravanThingsTabUtility_DrawMass_Float(float mass, Rect rect);
        // delegate defintion that matches CaravanThingsTabUtility.DrawMass(float)

        public static readonly CaravanThingsTabUtility_DrawMass_Float delegateTo_CaravanThingsTabUtility_DrawMass_Float =
        // a delegate to call the non-public CaravanThingsTabUtility.DrawMass(float)
            (CaravanThingsTabUtility_DrawMass_Float)AccessTools.Method(typeof(CaravanThingsTabUtility), nameof(CaravanThingsTabUtility.DrawMass), new Type[] { typeof(float), typeof(Rect) })
            .CreateDelegate(typeof(CaravanThingsTabUtility_DrawMass_Float));

        public static readonly MethodInfo MethodInfo_CaravanThingsTabUtility_DrawMass_Thing =
        // a MethodInfo for the non-public CaravanThingsTabUtility.DrawMass(Thing)
            AccessTools.Method(typeof(CaravanThingsTabUtility), nameof(CaravanThingsTabUtility.DrawMass), new Type[] { typeof(Thing), typeof(Rect) });
            

        public static void Do_CaravanThingsTabUtility_DrawMass_Thing(Thing thing, Rect rect, bool inventory, Pawn p)
        // Helper method to be used instead of the non-public CaravanThingsTabUtility.DrawMass(Thing)
        // it's a helper method which gets called by transpiling the caller of CaravanThingsTabUtility.DrawMass(Thing)
        // instead of transpiling CaravanThingsTabUtility.DrawMass(Thing) itself, because it needs an additional parameter 
        // to function.
        //
        // This implementation will correctly apply the Pawns InventoryMassReduction stat for inventory items,         
        // and use the GearMass stat instead of the Mass stat for worn/equipped items.
        {
            float mass = (float)thing.stackCount;
            if (inventory)
            {
                float inventoryMassFactor = 1;
                if (p != null)
                {
                    inventoryMassFactor -= p.GetStatValue(CMFStatDefOf.InventoryMassReduction, true);
                }
                mass *= thing.GetStatValue(StatDefOf.Mass, true) * inventoryMassFactor;
            }
            else
            {
                mass *= thing.GetStatValue(CMFStatDefOf.GearMass, true);
            }
            if (mass != 0) { 
                delegateTo_CaravanThingsTabUtility_DrawMass_Float(mass, rect);
            }               
        }

        public static float AdjustInventoryMass(float mass, Pawn p)
        // Helper function called by the transpiled MassUtility.InventoryMass
        //
        // It adjusts the inventory mass by reducing it by the inventory mass multiplied with the
        // InventoryMassReduction stat of the carrying Pawn 
        {
            float inventoryMassFactor = 1 - p.GetStatValue(CMFStatDefOf.InventoryMassReduction, true);
            return mass * inventoryMassFactor;
        }

        public static IEnumerable<CodeInstruction> StatWorker_GetBaseValueForTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        // This is a transpiler for StatWorker.GetBaseValueFor
        //
        // Instead of just using defaultBaseValue as base value,
        // it calls DoGetBaseValueFor, which checks if the 
        // StatDef is a DerivedStatDef, and if yes, uses the final 
        // value from the stat referred to by defaultBaseStat as 
        // the base value.
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            int LdfldCount = 0;
            bool Done = false;

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];

                if (!Done)
                {

                    if (instruction.opcode == OpCodes.Ldfld)
                    {
                        if (LdfldCount == 1)
                        {
                            yield return new CodeInstruction(opcode: OpCodes.Ldarg_1);
                            yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(CMFHarmonyPatch), nameof(CMFHarmonyPatch.DoGetBaseValueFor)));
                            Done = true;
                            continue;
                        }
                        LdfldCount++;
                    }
                }

                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> StatWorker_GetExplanationUnfinalizedTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        // This is a transpiler for StatWorker.GetExplanationUnfinalized
        //
        // It changes the "Base Value: x" line to "BaseValue (from Stat): x" if the 
        // StatDef is a DerivedStatDef
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            int LdstrCount = 0;
            bool Done = false;

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];

                if (!Done)
                {
                    if (instruction.opcode == OpCodes.Ldstr)
                    {
                        if (LdstrCount == 1)
                        {
                            yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                            yield return new CodeInstruction(opcode: OpCodes.Ldfld, operand: AccessTools.Field(typeof(StatWorker), "stat"));
                            yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(DerivedStatDef), nameof(DerivedStatDef.GetFromBaseStatString)));
                            yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(TaggedString), "op_Addition", new Type[] { typeof(TaggedString), typeof(string) }));

                            Done = true;
                        }
                        LdstrCount++;
                    }
                }

                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> MassUtility_CapacityTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        // This is a transpiler for StatWorker.Capacity
        //
        // Instead of doing a hardcoded calulcation of 35 * BodySize, 
        // it returns the value of the CarryMass stat of the Pawn
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            bool Done = false;
            bool Skip = false;

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];

                if (!Done)
                {

                    if (instruction.opcode == OpCodes.Callvirt)
                    {
                        yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(CMFHarmonyPatch), nameof(CMFHarmonyPatch.DoCapacity)));
                        Skip = true;
                    }
                    if (Skip & instruction.opcode == OpCodes.Stloc_0)
                    {
                        Skip = false;
                        Done = true;
                    }
                }

                if (!Skip)
                {
                    yield return instruction;
                }

            }
        }

        public static IEnumerable<CodeInstruction> MassUtility_GearMassTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        // This is a transpiler for MassUtility.GearMass
        //
        // When summing up the mass of worn and equipped items, it uses the GearMass stat instead of the Mass stat
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];

                if (instruction.opcode == OpCodes.Ldsfld)
                {
                    instruction.operand = AccessTools.Field(typeof(CMFStatDefOf), nameof(CMFStatDefOf.GearMass));
                }

                yield return instruction;

            }
        }

        public static IEnumerable<CodeInstruction> MassUtility_InventoryMassTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        // This is a transpiler for MassUtility.InventoryMass
        //
        // After summing up the mass of items carried as inventory, AdjustInventoryMass is called,
        // which reduces the inventory mass by the summed up mass multiplied with the InventoryMassReduction
        // stat of the Pawn
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];

                if (instruction.opcode == OpCodes.Ret)
                {
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(CMFHarmonyPatch), nameof(CMFHarmonyPatch.AdjustInventoryMass)));
                }

                yield return instruction;

            }
        }

        public static IEnumerable<CodeInstruction> MassUtility_CountToPickUpUntilOverEncumberedTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        // This is a transpiler for MassUtility.CountToPickUpUntilOverEncumbered
        //
        // Calls AdjustInventoryMass on the Mass stat before dividing by it
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];

                if (instruction.opcode == OpCodes.Div)
                {
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(CMFHarmonyPatch), nameof(CMFHarmonyPatch.AdjustInventoryMass)));
                }

                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> MassUtility_WillBeOverEncumberedAfterPickingUpTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        // This is a transpiler for MassUtility.WillBeOverEncumberedAfterPickingUp
        //
        // Calls AdjustInventoryMass on the Mass stat before multiplying by it
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];

                if (instruction.opcode == OpCodes.Mul)
                {
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(CMFHarmonyPatch), nameof(CMFHarmonyPatch.AdjustInventoryMass)));
                }

                yield return instruction;
            }
        }

        

        public static IEnumerable<CodeInstruction> ITab_Pawn_Gear_DrawThingRowTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        // This is a transpiler for ITab_Pawn_Gear.DrawThingRow
        //
        // it changes the call to CaravanThingsTabUtility.DrawMass to a call of
        // Do_CaravanThingsTabUtility_DrawMass_Thing instead so that worn/equipped gear
        // will show GearMass and items carried in inventory will show their reduced mass
        // because of the Pawns InventoryMassReduction
        {
            List<CodeInstruction> instructionList = instructions.ToList();


            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];

                if (instruction.opcode == OpCodes.Call & MethodInfo_CaravanThingsTabUtility_DrawMass_Thing.Equals(instruction.operand))
                {
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 4);
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(ITab_Pawn_Gear), "get_SelPawnForGear"));
                    instruction.operand = AccessTools.Method(typeof(CMFHarmonyPatch), nameof(CMFHarmonyPatch.Do_CaravanThingsTabUtility_DrawMass_Thing));
                }

                yield return instruction;

            }
        }
    }

}
