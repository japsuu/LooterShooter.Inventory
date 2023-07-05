using System;
using System.IO;
using InventorySystem.Inventories.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InventorySystem.Inventories.Serialization
{
    public class ItemMetadataConverter : JsonConverter<ItemMetadata>
    {
        private const string ITEM_DATA_GUID_PROPERTY_NAME = "itemDataGuid";
        
        public override bool CanRead => true;
        public override bool CanWrite => true;
        
        
        public override void WriteJson(JsonWriter writer, ItemMetadata value, JsonSerializer serializer)
        {
            JObject obj = new()
            {
                { ITEM_DATA_GUID_PROPERTY_NAME, JToken.FromObject(value.ItemData.Guid) }
            };

            obj.WriteTo(writer);
        }

        public override ItemMetadata ReadJson(JsonReader reader, Type objectType, ItemMetadata existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            JToken dataGuidToken = obj.GetValue(ITEM_DATA_GUID_PROPERTY_NAME);
            
            if(dataGuidToken == null)
                throw new InvalidDataException("Could not load JToken (itemDataId): property not found.");
            
            string guidString = dataGuidToken.ToObject<string>();
            
            if(!Guid.TryParse(guidString, out Guid guid))
                throw new InvalidDataException("Could not parse itemMetadata: GUID is invalid and could not be parsed.");
            
            return ItemDatabase.Singleton.TryGetItemById(guid, out ItemData itemData) ? new ItemMetadata(itemData) : null;
        }
    }
}