using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Inventories
{
    public interface IInventory
    {
        public string Name { get; }
        
        
        public void RequestMoveItem(Vector2Int oldPosition, Vector2Int newPosition, ItemRotation newRotation, IInventory targetInventory);

        
        public bool IsPositionInsideInventory(Vector2Int position);

        
        public bool TryGetItemAtPosition(Vector2Int position, out InventoryItem item);

        
        public bool IsItemBoundsValid(InventoryBounds itemBounds, InventoryBounds? existingBoundsToIgnore = null);


        public InventoryItem ReceiveExistingInventoryItem(InventoryItem existingItem, InventoryBounds bounds, ItemRotation rotation);
    }
}