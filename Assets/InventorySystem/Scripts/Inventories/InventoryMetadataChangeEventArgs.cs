using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories
{
    public readonly struct InventoryMetadataChangeEventArgs
    {
        public readonly ItemMetadataSnapshot NewMetadata;
        public readonly ItemMetadataSnapshot OldMetadata;


        public InventoryMetadataChangeEventArgs(ItemMetadataSnapshot oldMetadata, ItemMetadataSnapshot newMetadata)
        {
            OldMetadata = oldMetadata;
            NewMetadata = newMetadata;
        }
    }
}