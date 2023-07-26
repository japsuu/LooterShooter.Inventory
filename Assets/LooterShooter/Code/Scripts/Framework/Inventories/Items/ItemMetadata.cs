using System.Collections.Generic;
using LooterShooter.Framework.Inventories.Serialization;
using LooterShooter.Framework.Saving;
using Newtonsoft.Json;

namespace LooterShooter.Framework.Inventories.Items
{
    /// <summary>
    /// Contains dynamic runtime-data of an item.
    /// Example: the durability of an item.
    /// </summary>
    [JsonConverter(typeof(ItemMetadataConverter))]
    public class ItemMetadata : MetadatableObject   //TODO: Test if using the generic MetadatableObject<ItemData> is better?
    {
        public readonly ItemData ItemData;


        public ItemMetadata(ItemData itemData, Dictionary<string, object> initialMetadata) : base(initialMetadata)
        {
            ItemData = itemData;
        }
    }
}