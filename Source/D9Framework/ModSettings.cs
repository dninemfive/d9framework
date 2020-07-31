using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace D9Framework
{
    /// <summary>
    /// <c>ModSettings</c> class for D9 Framework. Mainly handles which Harmony patches should be applied and saves all specified settings.
    /// </summary>
    /// <remarks>Static for convenience.</remarks>
    public class D9FModSettings : ModSettings
    {        
        public static bool DEBUG = false; //for release set false by default

        public static bool ApplyCompFromStuff => !DEBUG || applyCFS;
        public static bool ApplyOrbitalTradeHook => !DEBUG || applyOTH;
        public static bool ApplyDeconstructReturnFix => !DEBUG || applyDRF;
        public static bool ApplyCarryMassFramework => !DEBUG || applyCMF;
        public static bool ApplyNegativeFertilityPatch => !DEBUG || applyNFP;
        public static bool PrintPatchedMethods => DEBUG && printPatchedMethods;
        // despite the public flag, don't reference these; they're only public for the purposes of the mod settings screen below. Reference the above variables instead.
        public static bool applyCFS = true, applyOTH = true, applyDRF = true, applyCMF = true, applyNFP = true;
        public static bool printPatchedMethods = false;

        public static Dictionary<string, bool> PatchApplicationSettings;
        public static Dictionary<string, (string labelKey, string descKey)> SettingsUIKeys;

        public override void ExposeData()
        {
            base.ExposeData();
            PatchApplicationSettings = new Dictionary<string, bool>();
            // backwards compatibility
            if(Scribe.mode != LoadSaveMode.Saving)
            {
                bool cur = false;
                string[] keysToLook = { "ApplyCompFromStuff", "ApplyOrbitalTradeHook", "ApplyDeconstructReturnFix", "ApplyCarryMassFramework", "ApplyNegativeFertilityPatch" };
                foreach(string key in keysToLook)
                {
                    Scribe_Values.Look(ref cur, key);
                    PatchApplicationSettings.Add(key, cur);
                }
            }
            Scribe_Values.Look(ref DEBUG, "debug", false);
            Scribe_Collections.Look(ref PatchApplicationSettings, "Patches");
        }

        public static bool ShouldPatch(string patchkey)
        {
            if (!DEBUG) return true;
            return PatchApplicationSettings[patchkey];
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
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("D9FSettingsDebug".Translate(), ref D9FModSettings.DEBUG, "D9FSettingsDebugTooltip".Translate());
            if (D9FModSettings.DEBUG)
            {
                listing.CheckboxLabeled("D9FSettingsPPM".Translate(), ref D9FModSettings.printPatchedMethods, "D9FSettingsPPMTooltip".Translate());
                listing.Label("D9FSettingsApplyAtOwnRisk".Translate());
                listing.Label("D9FSettingsRestartToApply".Translate());
                listing.Label("D9FSettingsDebugModeRequired".Translate());
                foreach(string key in D9FModSettings.PatchApplicationSettings.Keys)
                {
                    // TODO: figure out how to do this extendably. Guessing you can make a List of boolean references to pass. The cur workaround used above (probably) won't work here.
                    listing.CheckBoxLabeled(D9FModSettings.SettingsUIKeys[key].labelKey.Translate(), ref D9FModSettings.PatchApplicationSettings[key], D9FModSettings.SettingsUIKeys[key].descKey.Translate());
                }
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
