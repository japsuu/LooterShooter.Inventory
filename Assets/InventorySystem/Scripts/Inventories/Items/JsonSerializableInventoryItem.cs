using Newtonsoft.Json;

namespace InventorySystem.Inventories.Items
{
    /*/// <summary>
    /// Network safe read-only reference to the state of an <see cref="InventoryItem"/>.
    /// </summary>
    [System.Serializable]
    public readonly struct JsonSerializableInventoryItem
    {
        [JsonProperty("itemMetadata")]
        [JsonConverter(typeof(ItemMetadataConverter))]
        public readonly ItemMetadata ItemMetadata;
        
        [JsonProperty("inventoryPositionX")]
        public readonly int PositionX;
        
        [JsonProperty("inventoryPositionY")]
        public readonly int PositionY;
        
        [JsonProperty("inventoryRotation")]
        public readonly ItemRotation Rotation;

        
        public JsonSerializableInventoryItem(InventoryItem inventoryItem)
        {
            ItemMetadata = inventoryItem.Metadata.ItemData.HashId;
            PositionX = inventoryItem.Bounds.Position.x;
            PositionY = inventoryItem.Bounds.Position.y;
            Rotation = inventoryItem.RotationInInventory;
        }


        public override string ToString()
        {
            return $"ItemSnapshot: {ItemMetadata}@({PositionX},{PositionY})@{Rotation}";
        }
        
        
        public bool TryDeserialize(IInventory containingSpatialInventory, out InventoryItem inventoryItem)
        {
            bool wasSerializedDataValid = ItemDatabase.Singleton.TryGetItemById(ItemMetadata, out ItemData data);

            UnityEngine.Vector2Int position = new(PositionX, PositionY);
            int widthCells = Rotation.ShouldFlipWidthAndHeight() ? data.InventorySizeY : data.InventorySizeX;
            int heightCells = Rotation.ShouldFlipWidthAndHeight() ? data.InventorySizeX : data.InventorySizeY;
            
            InventoryBounds bounds = new(position, widthCells, heightCells);

            if (wasSerializedDataValid)
            {
                inventoryItem = new InventoryItem(containingSpatialInventory, data, bounds, Rotation);
            }
            else
            {
                inventoryItem = null;
                Logger.Log(LogLevel.WARN, $"Could not deserialize an item with ID '{ItemMetadata}' in Inventory '{containingSpatialInventory}'");
            }

            return wasSerializedDataValid;
        }
    }*/
}