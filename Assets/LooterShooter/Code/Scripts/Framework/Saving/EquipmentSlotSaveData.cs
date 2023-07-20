using System;
using JetBrains.Annotations;
using LooterShooter.Framework.Inventories.Items;
using Newtonsoft.Json;

namespace LooterShooter.Framework.Saving
{
    /// <summary>
    /// Used in <see cref="PlayerSaveData"/> to store the state (contents) of an EquipmentSlot.
    /// </summary>
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