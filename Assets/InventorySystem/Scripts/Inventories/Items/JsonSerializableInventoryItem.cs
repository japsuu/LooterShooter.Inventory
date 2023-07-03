using Newtonsoft.Json;

namespace InventorySystem.Inventories.Items
{
    /// <summary>
    /// Network safe read-only reference to the state of an <see cref="InventoryItem"/>.
    /// </summary>
    [System.Serializable]
    public readonly struct JsonSerializableInventoryItem
    {
        [JsonProperty("itemId")]
        public readonly int ItemDataId;
        
        [JsonProperty("inventoryPositionX")]
        public readonly int PositionX;
        
        [JsonProperty("inventoryPositionY")]
        public readonly int PositionY;
        
        [JsonProperty("inventoryRotation")]
        public readonly ItemRotation Rotation;

        
        public JsonSerializableInventoryItem(InventoryItem inventoryItem)
        {
            ItemDataId = inventoryItem.ItemDataReference.HashId;
            PositionX = inventoryItem.Bounds.Position.x;
            PositionY = inventoryItem.Bounds.Position.y;
            Rotation = inventoryItem.RotationInInventory;
        }


        public override string ToString()
        {
            return $"ItemSnapshot: {ItemDataId}@({PositionX},{PositionY})@{Rotation}";
        }
    }
}