using System;
using System.IO;
using InventorySystem.Inventories.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InventorySystem.Inventories.Serialization
{
    public class SpatialInventoryConverter : JsonConverter<SpatialInventory>
    {
        private const string INVENTORY_NAME_PROPERTY_NAME = "inventoryName";
        private const string INVENTORY_SIZE_PROPERTY_NAME = "inventorySize";
        private const string INVENTORY_CONTENTS_PROPERTY_NAME = "contents";
        
        public override bool CanRead => true;
        public override bool CanWrite => true;
        
        
        public override void WriteJson(JsonWriter writer, SpatialInventory value, JsonSerializer serializer)
        {
            JObject obj = new()
            {
                { INVENTORY_NAME_PROPERTY_NAME, value.Name },
                { INVENTORY_SIZE_PROPERTY_NAME, JToken.FromObject(new { widthCells = value.Bounds.Width, heightCells = value.Bounds.Height }) },
            };

            JArray contentsArray = new();
            
            foreach (InventoryItem item in value.GetAllItems())
            {
                contentsArray.Add(JToken.FromObject(item, serializer));
            }
            
            obj.Add(INVENTORY_CONTENTS_PROPERTY_NAME, contentsArray);

            obj.WriteTo(writer);
        }

        
        public override SpatialInventory ReadJson(JsonReader reader, Type objectType, SpatialInventory existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            JToken nameToken = obj.GetValue(INVENTORY_NAME_PROPERTY_NAME);
            JToken sizeToken = obj.GetValue(INVENTORY_SIZE_PROPERTY_NAME);
            JToken contentsToken = obj.GetValue(INVENTORY_CONTENTS_PROPERTY_NAME);
            JArray contentsArray = (JArray)contentsToken;

            if (nameToken == null || sizeToken == null || contentsToken == null)
                throw new InvalidDataException("Could not load JToken (name/size/contents): PROPERTY_NAME was null.");
            
            string name = nameToken.ToObject<string>();
            
            JToken widthToken = sizeToken["widthCells"];
            JToken heightToken = sizeToken["heightCells"];
            
            if(widthToken == null || heightToken == null)
                throw new InvalidDataException("Could not load JToken (size: widthCells/heightCells): property not found.");
            
            int widthCells = widthToken.ToObject<int>();
            int heightCells = heightToken.ToObject<int>();

            // Construct the inventory.
            SpatialInventory spatialInventory = new(name, widthCells, heightCells);

            // Construct all contents.
            InventoryItem[] contents = new InventoryItem[contentsArray.Count];
            for (int i = 0; i < contentsArray.Count; i++)
            {
                contents[i] = contentsArray[i].ToObject<InventoryItem>(serializer);
                contents[i].OverwriteContainingInventory(spatialInventory);
            }
            
            // Override inventory contents with loaded contents.
            spatialInventory.AddItems(contents);

            return spatialInventory;
        }
    }
}