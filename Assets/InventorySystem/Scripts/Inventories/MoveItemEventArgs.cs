using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories
{
    public readonly struct MoveItemEventArgs
    {
        public readonly InventoryItem OldItem;
        public readonly InventoryItem NewItem;


        public MoveItemEventArgs(InventoryItem oldItem, InventoryItem newItem)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }
    }
}