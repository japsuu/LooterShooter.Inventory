using System.Collections.Generic;
using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories
{
    public abstract class Inventory
    {
        public abstract IEnumerable<InventoryItem> GetItems();


        public abstract int ContainsItem(ItemData itemData);


        public abstract int TryAddItems(ItemData itemData, int count);


        public abstract int TryRemoveItems(ItemData item, int count);
    }
}