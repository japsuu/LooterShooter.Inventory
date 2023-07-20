using LooterShooter.Framework.Inventories.Items;

namespace LooterShooter.Framework.Inventories
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