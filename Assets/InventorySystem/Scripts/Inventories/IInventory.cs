using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Inventories
{
    public interface IInventory
    {
        public string Name { get; }
        
        
        /// <returns>New <see cref="InventoryItem"/> for this Inventory with the given parameters.</returns>
        public bool TryCreateNewInventoryItem(ItemMetadata metadata, Vector2Int position, ItemRotation rotation, InventoryBounds? boundsToIgnore, out InventoryItem createdInventoryItem);


        public void RemoveItem(Vector2Int itemPosition);

        
        public void AddItem(InventoryItem item);


        //public bool IsPositionInsideInventory(Vector2Int position);

        
        //public bool TryGetItemAtPosition(Vector2Int position, out InventoryItem item);

        
        //public bool IsItemBoundsValid(InventoryBounds itemBounds, InventoryBounds? existingBoundsToIgnore = null);


        //public InventoryItem ReceiveExistingInventoryItem(InventoryItem existingItem, InventoryBounds bounds, ItemRotation rotation);
    }
}