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
        string optionsClassName;
        List<string> optionNames;
        PatchOperation match, nomatch;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            // TODO: handle non-static settings types
            Type type = AccessTools.TypeByName(optionsClassName);
            bool found = false;
            foreach (string name in optionNames)
            {
                if (SettingExistsAndIsEnabled(type, name))
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

        private bool SettingExistsAndIsEnabled(Type type, string name)
        {
            FieldInfo field = AccessTools.Field(type, name);
            if (field != null)
            {
                if (!(field.FieldType == typeof(bool))) return false;
                return false;
            }
            // get getter
            return true;
        }
    }
}
