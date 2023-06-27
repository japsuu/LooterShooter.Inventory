using System.Collections.Generic;
using System.Linq;
using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories
{
    public class SimpleInventory : Inventory
    {
        private List<InventoryData> _contents;

        
        public override IEnumerable<InventoryData> GetItems() => _contents.AsEnumerable();


        public override int ContainsItem(ItemData itemData) => _contents.Count(i => i.Item == itemData);


        public override bool TryAddItem(ItemData itemData)
        {
            if (_contents.Count >= _contents.Capacity)
                return false;
            
            _contents.Add(new InventoryData(itemData));

            return true;
        }


        public override int TryRemoveItems(ItemData item, int count)
        {
            if(item == null)
                return 0;
            
            int removedCount = 0;
            for (int i = _contents.Count - 1; i >= 0; i--)
            {
                InventoryData inventoryData = _contents[i];
                if (inventoryData.Item != item)
                    continue;

                _contents.Remove(inventoryData);
                removedCount++;

                if (removedCount == count)
                    return removedCount;
            }

            return removedCount;
        }
    }
}