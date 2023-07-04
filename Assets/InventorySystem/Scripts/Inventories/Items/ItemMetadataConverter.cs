using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InventorySystem.Inventories.Items
{
    public class ItemMetadataConverter : JsonConverter<ItemMetadata>
    {
        private const string ITEM_DATA_ID_PROPERTY_NAME = "itemDataId";
        
        public override bool CanRead => true;
        public override bool CanWrite => true;
        
        
        public override void WriteJson(JsonWriter writer, ItemMetadata value, JsonSerializer serializer)
        {
            JObject obj = new()
            {
                { ITEM_DATA_ID_PROPERTY_NAME, JToken.FromObject(value.ItemData.HashId) }
            };

            obj.WriteTo(writer);
        }

        public override ItemMetadata ReadJson(JsonReader reader, Type objectType, ItemMetadata existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            JToken dataIdToken = obj.GetValue(ITEM_DATA_ID_PROPERTY_NAME);
            
            if(dataIdToken == null)
                throw new InvalidDataException("Could not load JToken (itemDataId): property not found.");
            
            int itemDataId = dataIdToken.ToObject<int>();
            
            return ItemDatabase.Singleton.TryGetItemById(itemDataId, out ItemData itemData) ? new ItemMetadata(itemData) : null;
        }
    }
}