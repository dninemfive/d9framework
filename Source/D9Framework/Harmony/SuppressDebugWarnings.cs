using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;

namespace D9Framework
{
    static class SuppressDebugWarnings
    {
        [HarmonyPatch(typeof(ModLister), nameof(ModLister.ShouldLogIssues), MethodType.Getter)]
        class ModListerShouldLogIssues
        {
            [HarmonyPostfix]
            static void Postfix(ref bool __result)
            {
                __result = false;
            }
        }
    }
}
