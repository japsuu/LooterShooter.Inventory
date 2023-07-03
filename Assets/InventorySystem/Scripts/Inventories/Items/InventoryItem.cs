using UnityEngine;

namespace InventorySystem.Inventories.Items
{
    public class InventoryItem
    {
        public readonly Inventory ContainingInventory;
        public readonly ItemData ItemDataReference;
        public readonly InventoryBounds Bounds;
        public readonly ItemRotation RotationInInventory;


        /// <returns>Serializes this inventoryItem to a ReadOnly-struct.</returns>
        public JsonSerializableInventoryItem Serialize() => new(this);


        public InventoryItem(Inventory containingInventory, ItemData itemDataReference, InventoryBounds bounds, ItemRotation rotationInInventory)
        {
            ContainingInventory = containingInventory;
            ItemDataReference = itemDataReference;
            Bounds = bounds;
            RotationInInventory = rotationInInventory;
        }


        public void RequestMove(Inventory newInventory, Vector2Int newPos, ItemRotation newRotation)
        {
            ContainingInventory.RequestMoveItem(Bounds.Position, newPos, newRotation, newInventory);
        }
    }
}