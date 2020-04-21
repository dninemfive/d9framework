using RimWorld;

namespace D9Framework
{
    // This static class provides easy access to the new StatDefs that
    // are integral to the Carry Mass Framework
    [DefOf]
    public static class CMFStatDefOf
    {
        // *** Pawn stats ***
        
        public static StatDef CarryMass;
        // How much mass the Pawn can carry, replaces the hardcoded 35*BodySize
        // Can be modified by worn items with the CarryMassOffset stat

        public static StatDef InventoryMassReduction;
        // Defines a mass reduction factor for items carried as inventory
        // Can be modified by worn items with the InventoryMassReductionOffset stat


        // *** Item stats ***

        public static StatDef GearMass;
        // Mass of the item when being worn/equipped (instead of carried as inventory).
        // Derives from Mass stat when not explicitly set

        public static StatDef CarryMassOffset;
        // Modifies the CarryMass stat of the Pawn wearing this item

        public static StatDef InventoryMassReductionOffset;
        // Modifies the InventoryMassReduction stat of the Pawn wearing this item
    }



}
