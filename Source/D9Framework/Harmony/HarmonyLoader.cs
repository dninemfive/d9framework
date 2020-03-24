using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Verse;
using HarmonyLib;

namespace D9Framework
{
    [StaticConstructorOnStartup]
    class HarmonyLoader
    {
        static HarmonyLoader()
        {
            ULog.Message("Applying Harmony patches...");
            var harmony = new Harmony("com.dninemfive.D9Framework");
            if (D9FModSettings.ApplyCompFromStuff)
            {
                PatchAll(harmony, typeof(CompFromStuff));                
                ULog.DebugMessage("\tCompFromStuff enabled.", false);
            }
            if (D9FModSettings.ApplyDeconstructReturnFix)
            {
                PatchAll(harmony, typeof(DeconstructReturnFix));
                ULog.DebugMessage("\tDeconstruct Return Fix enabled.", false);
            }
            if (D9FModSettings.ApplyOrbitalTradeHook)
            {
                PatchAll(harmony, typeof(OrbitalTradeHook));
                ULog.DebugMessage("\tOrbital Trade Hook enabled.", false);
            }
            if (D9FModSettings.DEBUG) //even though they wouldn't *print* without this if-statement, I don't need to loop through patched methods otherwise
            {
                ULog.Message("The following patches were applied:");
                foreach (MethodBase mb in harmony.GetPatchedMethods()) ULog.DebugMessage("\t" + mb.Name, false);
            }            
        }

        // also thanks to lbmaian
        static void PatchAll(Harmony harmony, Type parentType)
        {
            foreach (var type in parentType.GetNestedTypes(AccessTools.all))
            {
                new PatchClassProcessor(harmony, type).Patch();
            }
        }

        /*
        // Thanks to lbmaian for this
        static void PatchAll(Harmony harmony, Type parentType)
        {
            foreach (var type in parentType.GetNestedTypes(AccessTools.all))
            {
                // Following copied from HarmonyInstance.PatchAll(Assembly).
                var harmonyMethods = type.GetHarmonyMethods();
                if (harmonyMethods != null && harmonyMethods.Count > 0)
                {
                    var attributes = HarmonyMethod.Merge(harmonyMethods);
                    var patchProcessor = new PatchProcessor(harmony, type, attributes);
                    patchProcessor.Patch();
                }
            }
        }
        */
    }
}
