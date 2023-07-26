using System;
using System.Collections.Generic;
using LooterShooter.Framework.Inventories.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LooterShooter.Framework.Inventories.Serialization
{
    public class ItemMetadataConverter : JsonConverter<ItemMetadata>
    {
        private const string ITEM_DATA_GUID_PROPERTY_NAME = "itemDataGuid";
        private const string METADATA_PROPERTY_NAME = "assignedMetadata";
        
        public override bool CanRead => true;
        public override bool CanWrite => true;
        
        
        public override void WriteJson(JsonWriter writer, ItemMetadata value, JsonSerializer serializer)
        {
            JObject obj = new()
            {
                { ITEM_DATA_GUID_PROPERTY_NAME, JToken.FromObject(value.ItemData.Guid) },
                { METADATA_PROPERTY_NAME, JToken.FromObject(value.AssignedMetadata) }
            };

            obj.WriteTo(writer);
        }

        public override ItemMetadata ReadJson(JsonReader reader, Type objectType, ItemMetadata existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            JToken dataGuidToken = obj.GetValue(ITEM_DATA_GUID_PROPERTY_NAME);
            JToken metadataToken = obj.GetValue(METADATA_PROPERTY_NAME);
            
            if(dataGuidToken == null)
            {
                Logger.Write(LogLevel.ERROR, nameof(ItemMetadataConverter), "Could not load JToken (itemDataId): property not found.");
                return null;
            }
            
            if(metadataToken == null)
            {
                Logger.Write(LogLevel.ERROR, nameof(ItemMetadataConverter), "Could not load JToken (metadata): property not found.");
                return null;
            }
            
            string guidString = dataGuidToken.ToObject<string>();
            Dictionary<string, object> metadata = metadataToken.ToObject<Dictionary<string, object>>();
            
            if(!Guid.TryParse(guidString, out Guid guid))
            {
                Logger.Write(LogLevel.ERROR, nameof(ItemMetadataConverter), "Could not parse itemMetadata: GUID is invalid and could not be parsed.");
                return null;
            }

            return ItemDatabase.Singleton.TryGetItemById(guid, out ItemData itemData) ? new ItemMetadata(itemData, metadata) : null;
        }
    }
}