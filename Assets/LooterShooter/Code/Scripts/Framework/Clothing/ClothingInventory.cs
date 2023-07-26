using System;
using JetBrains.Annotations;
using LooterShooter.Framework.Inventories;
using LooterShooter.Framework.Inventories.Items;
using LooterShooter.Framework.Inventories.Serialization;
using Newtonsoft.Json;

namespace LooterShooter.Framework.Clothing
{
    /// <summary>
    /// Represents an clothing item that contains an inventory.
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(ClothingInventoryConverter))]
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