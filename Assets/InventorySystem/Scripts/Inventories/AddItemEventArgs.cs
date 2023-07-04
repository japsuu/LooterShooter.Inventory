using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories
{
    public readonly struct AddItemEventArgs
    {
        public readonly InventoryItem AddedItem;


        public AddItemEventArgs(InventoryItem addedItem)
        {
            AddedItem = addedItem;
        }
    }
}