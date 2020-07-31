using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D9Framework
{
    /// <summary>
    /// Used to make adding new Harmony patches internally easier. Not intended for other mods' use, but can likely be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class ClassWithPatchesAttribute : Attribute
    {
        public string PlainName, Key, DescKey;

        public ClassWithPatchesAttribute(string pn, string k, string dk)
        {
            PlainName = pn;
            Key = k;
            DescKey = dk;
        }
    }
}
