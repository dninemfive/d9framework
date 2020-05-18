/*using System;
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
     *
     * TODO:
     *      1. Make user specify the mod settings type in addition to the class name, or if not specified append "Settings" to modClassName
     *      Optional: Make optionNames into pairs of names and values, compare
     *
    class PatchOperationModOptional : PatchOperation
    {
#pragma warning disable CS0649
        string modClassName;
        List<string> optionNames;
        PatchOperation match, nomatch;
        private const string SettingsFieldName = "modSettings";
#pragma warning restore CS0649

        protected override bool ApplyWorker(XmlDocument xml)
        {
            Mod targetMod = null;
            foreach (Mod m in LoadedModManager.ModHandles) if (nameof(m) == modClassName)
                {
                    targetMod = m;
                    break;
                }
            if (targetMod == null) return false;
            // cache settings stuff
            var settings = AccessTools.Field(targetMod.GetType(), SettingsFieldName).GetValue(targetMod);
            Type settingsType = settings.GetType();
            bool found = false;
            foreach (string name in optionNames)
            {
                if (SettingExistsAndIsEnabled(settings, settingsType, name))
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

        private bool SettingExistsAndIsEnabled(object settings, Type settingsType, string name)
        {            
            // case 1: field            
            FieldInfo field = AccessTools.Field(settingsType, name);
            bool? result = field?.GetValue(settings) as bool?;
            if (result.HasValue) return result.Value;
            // case 2: getter
            MethodInfo getter = AccessTools.PropertyGetter(settingsType, name);
            result = getter.Invoke(settingsType, null) as bool?;
            if (result.HasValue) return result.Value;
            return false;
        }
    }
}*/