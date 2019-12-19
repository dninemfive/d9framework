using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace D9Framework
{
    /// <summary>
    /// ModExtension for use with CompsFromStuff. Should be self-explanatory; if not, read https://rimworldwiki.com/wiki/Modding_Tutorials/DefModExtension.
    /// </summary>
    class CompsToAddWhenStuff : DefModExtension
    {
# pragma warning disable CS0649 //disable the warning that this field is never assigned to, as the game handles that
        public List<CompProperties> comps;
    }
}
