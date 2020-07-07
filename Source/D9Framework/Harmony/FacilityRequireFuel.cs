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
    public class FacilityRequireFuel : ClassWithPatches
    {
        public override string PlainName => "Facility Require Fuel";

        [HarmonyPatch(typeof(CompFacility), nameof(CompFacility.CanBeActive), MethodType.Getter)]
        class FacilityRequireFuelPatch
        {
            [HarmonyPostfix]
            public static void CanBeActivePostfix(ref bool __result, ref CompFacility __instance)
            {
                CompRefuelable fuel = __instance.parent.TryGetComp<CompRefuelable>();
                __result &= (fuel == null || fuel.HasFuel);
            }
        }
    }
}