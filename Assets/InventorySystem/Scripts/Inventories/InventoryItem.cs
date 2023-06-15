using InventorySystem.Items;

namespace InventorySystem.Inventories
{
    public class InventoryItem
    {
        public InventoryBounds Bounds { get; private set; }
        public ItemData Item { get; }

        
        public InventoryItem(ItemData item, InventoryBounds bounds)
        {
            Item = item;
            Bounds = bounds;
        }


        public void UpdateBounds(InventoryBounds bounds) => Bounds = bounds;
    }
}