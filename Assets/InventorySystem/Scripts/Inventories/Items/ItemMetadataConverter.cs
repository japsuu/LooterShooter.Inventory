using System;
using Newtonsoft.Json;

namespace InventorySystem.Inventories.Items
{
    public class ItemMetadataConverter : JsonConverter<ItemMetadata>
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;
        
        
        public override void WriteJson(JsonWriter writer, ItemMetadata value, JsonSerializer serializer)
        {
            writer.WriteValue();
        }


        public override ItemMetadata ReadJson(JsonReader reader, Type objectType, ItemMetadata existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            
        }
    }
}