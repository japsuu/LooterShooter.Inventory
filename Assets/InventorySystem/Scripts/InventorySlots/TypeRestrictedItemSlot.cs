using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;

namespace InventorySystem.InventorySlots
{
    public abstract class TypeRestrictedItemSlot : ItemSlot
    {


        public override bool CanDropDraggableItem(DraggableItem draggableItem)
        {
            if (ItemTypeRestrictions == null || ItemTypeRestrictions.Length == 0)
                return false;
            
            foreach (ItemType restriction in ItemTypeRestrictions)
            {
                if (restriction == draggableItem.InventoryItem.Metadata.ItemData.ItemType)
                    return true;
            }

            return false;
        }
    }
}