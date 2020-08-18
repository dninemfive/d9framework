using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace D9Framework
{
    /// <summary>
    /// <c>ModSettings</c> class for D9 Framework. Mainly handles which Harmony patches should be applied and saves all specified settings.
    /// </summary>
    /// <remarks>Static for convenience.</remarks>
    public class D9FModSettings : ModSettings
    {        
        public static bool DEBUG = false; //for release set false by default
        public static bool PrintPatchedMethods => DEBUG && printPatchedMethods;
        public static bool printPatchedMethods = false;

        // I don't want to touch CMF because it has custom patching, so retain old settings setup.
        public static bool ApplyCarryMassFramework => !DEBUG || applyCMF;
        public static bool applyCMF = true;

        // despite being public, please don't fuck with these. Access patch application settings with ShouldPatch.
        // They're only public so I can use them in the mod settings screen.
        public static Dictionary<string, PatchInfo> Patches = new Dictionary<string, PatchInfo>();

        public class PatchInfo
        {
            public bool apply;
            public string labelKey, descKey;

            public PatchInfo(bool a, string l, string d)
            {
                apply = a;
                labelKey = l;
                descKey = d;
            }
        } 

        public override void ExposeData()
        {
            base.ExposeData();
            // backwards compatibility
            if(Scribe.mode != LoadSaveMode.Saving)
            {
                bool cur = false;
                string[] keysToLook = { "ApplyCompFromStuff", "ApplyOrbitalTradeHook", "ApplyDeconstructReturnFix", "ApplyNegativeFertilityPatch" };
                foreach(string key in keysToLook)
                {
                    Scribe_Values.Look(ref cur, key);
                    // discarding the data because I don't have the info to initialize and idrc if a couple settings flip on the 1.1 -> 1.2 update
                    // Just looking to avoid errors
                }
            }
            Scribe_Values.Look(ref applyCMF, "ApplyCarryMassFramework", true);
            Scribe_Values.Look(ref DEBUG, "debug", false);
            Scribe_Collections.Look(ref Patches, "Patches", keyLookMode: LookMode.Value, valueLookMode: LookMode.Deep);
        }

        public static bool ShouldPatch(string patchkey)
        {
            if (!DEBUG) return true;
            if (!Patches.ContainsKey(patchkey))
            {
                ULog.Warning("ShouldPatch called for non-initialized patchkey.");
                return true;
            }
            return Patches[patchkey].apply;
        }
    }
    /// <summary>
    /// <c>Mod</c> class for D9 Framework. Mainly handles the settings screen.
    /// </summary>
    public class D9FrameworkMod : Mod
    {
        D9FModSettings settings;
        public D9FrameworkMod(ModContentPack con) : base(con)
        {
            this.settings = GetSettings<D9FModSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Log.Message("0");
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("D9FSettingsDebug".Translate(), ref D9FModSettings.DEBUG, "D9FSettingsDebugTooltip".Translate());
            Log.Message("1");
            if (D9FModSettings.DEBUG)
            {
                Log.Message("2");
                listing.CheckboxLabeled("D9FSettingsPPM".Translate(), ref D9FModSettings.printPatchedMethods, "D9FSettingsPPMTooltip".Translate());
                listing.Label("D9FSettingsApplyAtOwnRisk".Translate());
                listing.Label("D9FSettingsRestartToApply".Translate());
                listing.Label("D9FSettingsDebugModeRequired".Translate());
                Log.Message("3");
                int ct = 1;
                foreach(string key in D9FModSettings.Patches.Keys.ToList())
                {
                    Log.Message("\t3." + ct + ": " + key);
                    bool cur = D9FModSettings.Patches[key].apply;
                    listing.CheckboxLabeled(D9FModSettings.Patches[key].labelKey.Translate(), ref cur, D9FModSettings.Patches[key].descKey.Translate());
                    D9FModSettings.Patches[key].apply = cur;
                }
                Log.Message("4");
                listing.CheckboxLabeled("D9FSettingsApplyCMF".Translate(), ref D9FModSettings.applyCMF, "D9FSettingsApplyCMFTooltip".Translate());
            }
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "D9FSettingsCategory".Translate();
        }
    }
}
