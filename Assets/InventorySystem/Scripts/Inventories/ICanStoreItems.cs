using InventorySystem.Inventories.Items;
using UnityEngine;

namespace InventorySystem.Inventories
{
    public interface ICanStoreItems
    {
        public void HandleAddItem(Vector2Int position, ItemRotation rotation);
        public void HandleRemoveItem(Vector2Int position);
    }
}