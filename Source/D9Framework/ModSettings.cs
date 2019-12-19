using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace D9Framework
{
    public class D9FModSettings : ModSettings
    {
        public static bool DEBUG = true; //for release set false by default
        //TODO: add options to disable each individual Harmony patch iff DEBUG is true
        //make it so the patch isn't even added to the game, game must be reloaded to take effect

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DEBUG, "debug", false);
        }
    }
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
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "D9FSettingsCategory".Translate();
        }
    }
}
