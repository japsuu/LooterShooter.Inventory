using InventorySystem.Inventories.Items;

namespace InventorySystem.InventorySlots
{
    public class ClothingSlot : TypeRestrictedItemSlot
    {
        protected override ItemType[] ItemTypeRestrictions => new[]
        {
            ItemType.CLOTHING
        };
    }
}