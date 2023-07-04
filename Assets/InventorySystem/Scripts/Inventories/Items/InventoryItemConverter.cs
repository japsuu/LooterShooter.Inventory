using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace InventorySystem.Inventories.Items
{
    public class InventoryItemConverter : JsonConverter<InventoryItem>
    {
        private const string METADATA_PROPERTY_NAME = "metadata";
        private const string POSITION_PROPERTY_NAME = "position";
        private const string ROTATION_PROPERTY_NAME = "rotation";

        public override bool CanRead => true;
        public override bool CanWrite => true;
        
        
        public override void WriteJson(JsonWriter writer, InventoryItem value, JsonSerializer serializer)
        {
            JObject obj = new()
            {
                { METADATA_PROPERTY_NAME, JToken.FromObject(value.Metadata, serializer) },
                { POSITION_PROPERTY_NAME, JToken.FromObject(new { posX = value.Bounds.Position.x, posY = value.Bounds.Position.y }) },
                { ROTATION_PROPERTY_NAME, JToken.FromObject(value.RotationInInventory) }
            };

            obj.WriteTo(writer);
        }
        

        public override InventoryItem ReadJson(JsonReader reader, Type objectType, InventoryItem existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            if (obj == null)
                throw new InvalidDataException("Could not load JObject: reader is not valid JSON.");

            JToken metadataToken = obj.GetValue(METADATA_PROPERTY_NAME);
            JToken positionToken = obj.GetValue(POSITION_PROPERTY_NAME);
            JToken rotationToken = obj.GetValue(ROTATION_PROPERTY_NAME);

            if (metadataToken == null || positionToken == null || rotationToken == null)
                throw new InvalidDataException("Could not load JToken (metadata/position/rotation): PROPERTY_NAME was null.");
            
            ItemMetadata metadata = metadataToken.ToObject<ItemMetadata>(serializer);
            ItemRotation rotationInInventory = rotationToken.ToObject<ItemRotation>();

            JToken posXToken = positionToken["posX"];
            JToken posYToken = positionToken["posY"];
            
            if(posXToken == null || posYToken == null)
                throw new InvalidDataException("Could not load JToken (position: posX/posY): property not found.");
            
            int posX = posXToken.ToObject<int>();
            int posY = posYToken.ToObject<int>();
            
            int width = rotationInInventory.ShouldFlipWidthAndHeight() ? metadata.ItemData.InventorySizeY : metadata.ItemData.InventorySizeX;
            int height = rotationInInventory.ShouldFlipWidthAndHeight() ? metadata.ItemData.InventorySizeX : metadata.ItemData.InventorySizeY;
            
            InventoryBounds bounds = new(new Vector2Int(posX, posY), width, height);

            return new InventoryItem(metadata, bounds, rotationInInventory, null);
        }
    }
}