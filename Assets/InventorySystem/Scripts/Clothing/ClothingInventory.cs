using System;
using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace InventorySystem.Clothing
{
    [Serializable]
    public class ClothingInventory
    {
        [NotNull]
        [JsonProperty("clothingItemMetadata")]
        public ItemMetadata ClothingItem { get; private set; }
        
        [NotNull]
        [JsonProperty("inventory")]
        public SpatialInventory Inventory { get; private set; }
        
        
        public ClothingInventory([NotNull]ItemMetadata clothingItem, [NotNull]SpatialInventory inventory)
        {
            ClothingItem = clothingItem;
            Inventory = inventory;
        }
    }
}