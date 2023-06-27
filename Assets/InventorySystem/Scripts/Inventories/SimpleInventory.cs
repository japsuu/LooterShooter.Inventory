using System.Collections.Generic;
using System.Linq;
using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories
{
    public class SimpleInventory : Inventory
    {
        private List<InventoryItem<>> _contents;

        
        public override IEnumerable<InventoryItem<>> GetItems() => _contents.AsEnumerable();


        public override int ContainsItem(ItemData itemData) => _contents.Count(i => i.Item == itemData);


        public override int TryAddItems(ItemData itemData, int count)
        {
            int addCount = 0;

            for (int i = 0; i < count; i++)
            {
                if (_contents.Count >= _contents.Capacity)
                    return addCount;
            
                _contents.Add(new InventoryItem<>(itemData, this));
                addCount++;
            }

            return addCount;
        }


        public override int TryRemoveItems(ItemData item, int count)
        {
            if(item == null)
                return 0;
            
            int removedCount = 0;
            for (int i = _contents.Count - 1; i >= 0; i--)
            {
                InventoryItem<> inventoryItem = _contents[i];
                if (inventoryItem.Item != item)
                    continue;

                _contents.Remove(inventoryItem);
                removedCount++;

                if (removedCount == count)
                    return removedCount;
            }

            return removedCount;
        }
    }
}