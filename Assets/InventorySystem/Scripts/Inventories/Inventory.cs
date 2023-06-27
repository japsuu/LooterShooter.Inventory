using System.Collections.Generic;
using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories
{
    public abstract class Inventory
    {
        public abstract IEnumerable<InventoryData> GetItems();


        public abstract int ContainsItem(ItemData itemData);


        public abstract bool TryAddItem(ItemData itemData);


        public abstract int TryRemoveItems(ItemData item, int count);
    }
}