using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories
{
    public readonly struct InventoryRemoveEventArgs
    {
        public readonly ItemMetadataSnapshot RemovedMetadata;


        public InventoryRemoveEventArgs(ItemMetadataSnapshot removedMetadata)
        {
            RemovedMetadata = removedMetadata;
        }
    }
}