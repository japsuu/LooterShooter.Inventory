namespace InventorySystem.Inventories.Items
{
    /// <summary>
    /// Contains dynamic runtime-data of an item.
    /// Example: the durability of an item.
    /// </summary>
    [System.Serializable]
    public class ItemMetadata
    {
        public readonly ItemData ItemData;


        public ItemMetadata(ItemData itemData)
        {
            ItemData = itemData;
        }
    }
}