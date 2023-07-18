using System;
using InventorySystem.Inventories.Items;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace InventorySystem.Saving
{
    [Serializable]
    public class EquipmentSlotSaveData
    {
        [NotNull]
        [JsonRequired]
        [JsonProperty("identifier")]
        public string Identifier { get; private set; }
        
        [CanBeNull]
        [JsonProperty("containedItem")]
        public ItemMetadata ContainedItem { get; private set; }


        public EquipmentSlotSaveData([NotNull] string identifier, [CanBeNull] ItemMetadata containedItem)
        {
            Identifier = identifier;
            ContainedItem = containedItem;
        }
    }
}