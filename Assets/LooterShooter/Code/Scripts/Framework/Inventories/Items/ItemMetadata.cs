using LooterShooter.Framework.Inventories.Serialization;
using Newtonsoft.Json;

namespace LooterShooter.Framework.Inventories.Items
{
    /// <summary>
    /// Contains dynamic runtime-data of an item.
    /// Example: the durability of an item.
    /// </summary>
    [JsonConverter(typeof(ItemMetadataConverter))]
    public class ItemMetadata
    {
        public readonly ItemData ItemData;


        public ItemMetadata(ItemData itemData)
        {
            ItemData = itemData;
        }
    }
}