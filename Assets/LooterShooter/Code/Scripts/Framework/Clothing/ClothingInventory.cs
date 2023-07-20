using System;
using JetBrains.Annotations;
using LooterShooter.Framework.Inventories;
using LooterShooter.Framework.Inventories.Items;
using Newtonsoft.Json;

namespace LooterShooter.Framework.Clothing
{
    /// <summary>
    /// Represents an clothing item that contains an inventory.
    /// </summary>
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