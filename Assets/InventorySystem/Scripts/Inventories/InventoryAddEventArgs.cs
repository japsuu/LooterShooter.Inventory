using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories
{
    public readonly struct InventoryAddEventArgs
    {
        public readonly ItemMetadataSnapshot AddedMetadata;


        public InventoryAddEventArgs(ItemMetadataSnapshot addedMetadata)
        {
            AddedMetadata = addedMetadata;
        }
    }
}