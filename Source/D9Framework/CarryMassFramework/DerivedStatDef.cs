using RimWorld;

namespace D9Framework
{
    // A DerivedStatDef can be used to define a StatDef which uses the final value
    // from another StatDef as it's base value. See the definition of the 
    // GearMass StatDef in CMF_Stats.xml for an example

    class DerivedStatDef : StatDef
    {
        public StatDef defaultBaseStat;

        public static string GetFromBaseStatString(StatDef stat)
        {
            string result = "";
            if (stat is DerivedStatDef)
            {
                DerivedStatDef derivedStat  = (stat as DerivedStatDef);
                if (derivedStat != null)
                {
                    StatDef baseStat = derivedStat.defaultBaseStat;
                    if (baseStat != null)
                    {
                        result = " (from " + baseStat.LabelCap +")";
                    }

                }
            }
            return result;
        }
    }

}
