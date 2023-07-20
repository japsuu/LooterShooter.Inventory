using LooterShooter.Framework.Inventories.Items;
using UnityEngine;

namespace LooterShooter.Framework.Inventories
{
    public interface IInventory
    {
        public string Name { get; } //TODO: Get rid of this property? It's required mostly for debugging.
        
        
        //TODO: Get rid of this method. This is not good design :D.
        /// <returns>New <see cref="InventoryItem"/> for this Inventory with the given parameters.</returns>
        public bool TryCreateNewInventoryItem(ItemMetadata metadata, Vector2Int position, InventoryItemRotation rotation, InventoryBounds? boundsToIgnore, out InventoryItem createdInventoryItem);


        public void RemoveItem(Vector2Int itemPosition);

        
        public void AddItem(InventoryItem item);
    }
}