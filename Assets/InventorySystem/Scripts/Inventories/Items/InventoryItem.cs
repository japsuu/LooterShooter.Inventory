using Newtonsoft.Json;
using UnityEngine;

namespace InventorySystem.Inventories.Items
{
    [JsonConverter(typeof(InventoryItemConverter))]
    public class InventoryItem
    {
        public IInventory ContainingInventory;
        public readonly ItemMetadata Metadata;
        public readonly InventoryBounds Bounds;
        public readonly ItemRotation RotationInInventory;


        public InventoryItem(ItemMetadata itemMetadata, InventoryBounds bounds, ItemRotation rotationInInventory, IInventory containingInventory)
        {
            Metadata = itemMetadata;
            Bounds = bounds;
            RotationInInventory = rotationInInventory;
            ContainingInventory = containingInventory;
        }


        public void RequestMove(IInventory newInventory, Vector2Int newPos, ItemRotation newRotation)
        {
            ContainingInventory.RequestMoveItem(Bounds.Position, newPos, newRotation, newInventory);
        }


        public void OverwriteContainingInventory(IInventory containingInventory)
        {
            ContainingInventory = containingInventory;
        }
    }
}