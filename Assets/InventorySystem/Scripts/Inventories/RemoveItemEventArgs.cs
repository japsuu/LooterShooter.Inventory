using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories
{
    public readonly struct RemoveItemEventArgs
    {
        public readonly InventoryItem RemovedItem;


        public RemoveItemEventArgs(InventoryItem removedItem)
        {
            RemovedItem = removedItem;
        }
    }
}