using System;
using LooterShooter.Framework.Clothing;
using LooterShooter.Framework.Inventories.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LooterShooter.Framework.Inventories.Serialization
{
    public class ClothingInventoryConverter : JsonConverter<ClothingInventory>
    {
        private const string CLOTHING_ITEM_PROPERTY_NAME = "clothingItem";
        private const string INVENTORY_PROPERTY_NAME = "inventory";
        
        public override void WriteJson(JsonWriter writer, ClothingInventory value, JsonSerializer serializer)
        {
            JObject obj = new()
            {
                { CLOTHING_ITEM_PROPERTY_NAME, JToken.FromObject(value.ClothingItem) },
                { INVENTORY_PROPERTY_NAME, JToken.FromObject(value.Inventory) }
            };

            obj.WriteTo(writer);
        }


        public override ClothingInventory ReadJson(JsonReader reader, Type objectType, ClothingInventory existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            JToken clothingItemToken = obj.GetValue(CLOTHING_ITEM_PROPERTY_NAME);
            JToken inventoryToken = obj.GetValue(INVENTORY_PROPERTY_NAME);
            
            if(clothingItemToken == null)
            {
                Logger.Write(LogLevel.ERROR, nameof(ClothingInventoryConverter), "Could not load JToken (clothingItem): property not found.");
                return null;
            }
            
            if(inventoryToken == null)
            {
                Logger.Write(LogLevel.ERROR, nameof(ClothingInventoryConverter), "Could not load JToken (inventory): property not found.");
                return null;
            }
            
            ItemMetadata clothingItem = clothingItemToken.ToObject<ItemMetadata>();
            SpatialInventory inventory = inventoryToken.ToObject<SpatialInventory>();

            return new ClothingInventory(clothingItem, inventory);
        }
    }
}