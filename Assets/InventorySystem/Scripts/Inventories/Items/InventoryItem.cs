using UnityEngine;

namespace InventorySystem.Inventories.Items
{
    public class InventoryItem
    {
        public IInventory ContainingInventory;
        public readonly ItemMetadata Metadata;
        public readonly InventoryBounds Bounds;
        public readonly ItemRotation RotationInInventory;


        /// <returns>Serializes this inventoryItem to a ReadOnly-struct.</returns>
        //public JsonSerializableInventoryItem Serialize() => new(this);


        public InventoryItem(ItemMetadata itemMetadata, InventoryBounds bounds, ItemRotation rotationInInventory)
        {
            Metadata = itemMetadata;
            Bounds = bounds;
            RotationInInventory = rotationInInventory;
        }


        public void RequestMove(IInventory newInventory, Vector2Int newPos, ItemRotation newRotation)
        {
            ContainingInventory.RequestMoveItem(Bounds.Position, newPos, newRotation, newInventory);
        }


        public void AssignInventory(IInventory containingInventory)
        {
            ContainingInventory = containingInventory;
        }
    }
}