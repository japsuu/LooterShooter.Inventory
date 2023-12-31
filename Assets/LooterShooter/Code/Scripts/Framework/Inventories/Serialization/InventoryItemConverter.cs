﻿using System;
using System.IO;
using LooterShooter.Framework.Inventories.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LooterShooter.Framework.Inventories.Serialization
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
            JToken metadataToken = obj.GetValue(METADATA_PROPERTY_NAME);
            JToken positionToken = obj.GetValue(POSITION_PROPERTY_NAME);
            JToken rotationToken = obj.GetValue(ROTATION_PROPERTY_NAME);

            if (metadataToken == null || positionToken == null || rotationToken == null)
            {
                Logger.Write(LogLevel.ERROR, "Could not load JToken (metadata/position/rotation): PROPERTY_NAME was null.");
                return null;
            }
            
            ItemMetadata metadata = metadataToken.ToObject<ItemMetadata>(serializer);
            InventoryItemRotation rotationInInventory = rotationToken.ToObject<InventoryItemRotation>();

            JToken posXToken = positionToken["posX"];
            JToken posYToken = positionToken["posY"];
            
            if(posXToken == null || posYToken == null)
            {
                Logger.Write(LogLevel.ERROR, "Could not load JToken (position: posX/posY): property not found.");
                return null;
            }

            if (metadata == null)
            {
                Logger.Write(LogLevel.ERROR, "Could not load inventory item because loaded metadata was null!");
                return null;
            }
            
            int posX = posXToken.ToObject<int>();
            int posY = posYToken.ToObject<int>();
            
            InventoryBounds bounds = new(metadata.ItemData, new Vector2Int(posX, posY), rotationInInventory);

            return new InventoryItem(metadata, bounds, rotationInInventory, null);
        }
    }
}