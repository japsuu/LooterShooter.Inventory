using InventorySystem.Inventories.Items;
using Newtonsoft.Json;

namespace InventorySystem.Inventories
{
    [System.Serializable]
    public readonly struct JsonSerializableInventory
    {
        [JsonProperty("inventoryName")]
        public readonly string InventoryName;
        
        [JsonProperty("inventoryWidthCells")]
        public readonly int WidthCells;
        
        [JsonProperty("inventoryHeightCells")]
        public readonly int HeightCells;
        
        [JsonProperty("containedItems")]
        public readonly JsonSerializableInventoryItem[] Contents;


        public JsonSerializableInventory(string inventoryName, int widthCells, int heightCells, JsonSerializableInventoryItem[] contents)
        {
            if (string.IsNullOrEmpty(inventoryName))
            {
                inventoryName = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
                Logger.Log(LogLevel.ERROR, "Tried to create an inventorySnapshot with empty/null inventoryName!");
            }

            if (widthCells < 1)
            {
                widthCells = 1;
                Logger.Log(LogLevel.ERROR, "Tried to create an inventorySnapshot with width of 0!");
            }

            if (heightCells < 1)
            {
                heightCells = 1;
                Logger.Log(LogLevel.ERROR, "Tried to create an inventorySnapshot with height of 0!");
            }
            
            InventoryName = inventoryName;
            WidthCells = widthCells;
            HeightCells = heightCells;

            if (contents == null)
            {
                Contents = System.Array.Empty<JsonSerializableInventoryItem>();
                return;
            }
            
            Contents = contents;
        }
    }
}