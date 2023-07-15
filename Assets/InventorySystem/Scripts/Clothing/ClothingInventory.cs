using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using JetBrains.Annotations;

namespace InventorySystem.Clothing
{
    public class ClothingInventory
    {
        [NotNull]
        public ItemMetadata ClothingItem { get; private set; }
        
        [NotNull]
        public SpatialInventory Inventory { get; private set; }
        
        
        public ClothingInventory([NotNull]ItemMetadata clothingItem, [NotNull]SpatialInventory inventory)
        {
            ClothingItem = clothingItem;
            Inventory = inventory;
        }
    }
}