using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace D9Framework
{
    /// <summary>
    /// Applies a patch if any specified boolean fields in a particular specified type are true.
    /// </summary>
    /*
     * TODO:
     *      Checks for the four cases:
     *      1. static class, field
     *      2. static class, property (-> getter)
     *      3. non-static class, field
     *      4. non-static class, property
     *      (actually, might have to handle static/non-static fields but I don't think so)
     */
    class PatchOperationModOptional : PatchOperation
    {
        string modClassName;
        List<string> optionNames;
        PatchOperation match, nomatch;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            // TODO: handle non-static settings types
            Mod targetMod = null;
            foreach (Mod m in LoadedModManager.ModHandles) if (nameof(m) == modClassName)
                {
                    targetMod = m;
                    break;
                }
            if (targetMod == null) return false;
            bool found = false;
            foreach (string name in optionNames)
            {
                if (SettingExistsAndIsEnabled(targetMod, name))
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                if (match != null) return match.Apply(xml);
            }
            else
            {
                if (nomatch != null) return nomatch.Apply(xml);
            }
            return true;
        }

        private bool SettingExistsAndIsEnabled(Mod mod, string name)
        {
            Type modType = mod.GetType();
            // case 1: field
            FieldInfo settingsField = AccessTools.Field(modType, "modSettings");
            var settings = settingsField.GetValue(mod);
            Type settingsType = settings.GetType();
            FieldInfo field = AccessTools.Field(settingsType, name);
            if (field != null)
            {
                bool? result = field.GetValue(settings) as bool?;
                if (result.HasValue) return result.Value;
            }
            // case 2: getter
            return true;
        }
    }
}
