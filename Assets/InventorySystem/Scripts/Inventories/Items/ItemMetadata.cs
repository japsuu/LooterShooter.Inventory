using System;

namespace InventorySystem.Inventories.Items
{
    [Serializable]
    public class ItemMetadata
    {
        public readonly ItemData ItemDataReference;
        public InventoryBounds Bounds { get; private set; }
        public ItemRotation Rotation { get; private set; }


        public ItemMetadata(ItemData itemData, InventoryBounds bounds, ItemRotation rotation)
        {
            ItemDataReference = itemData;
            
            UpdateBounds(bounds, rotation);
        }


        public void UpdateBounds(InventoryBounds newBounds, ItemRotation newRotation)
        {
            Bounds = newBounds;
            Rotation = newRotation;
        }
    }
}