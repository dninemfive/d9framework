using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace D9Framework
{
    class CompsToAddWhenStuff : DefModExtension
    {
# pragma warning disable CS0649 //disable the warning that this field is never assigned to, as the game handles that
        public List<CompProperties> comps;
    }
}
