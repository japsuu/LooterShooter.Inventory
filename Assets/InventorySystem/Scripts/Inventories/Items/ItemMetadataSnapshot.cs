using UnityEngine;

namespace InventorySystem.Inventories.Items
{
    public readonly struct ItemMetadataSnapshot
    {
        public readonly ItemData ItemDataReference;
        public readonly InventoryX ContainingInventory;
        public readonly Vector2Int PositionInInventory;


        public ItemMetadataSnapshot(ItemData itemMetadata, InventoryX containingInventory, Vector2Int positionInInventory)
        {
            ItemDataReference = itemMetadata;
            ContainingInventory = containingInventory;
            PositionInInventory = positionInInventory;
        }
    }
}