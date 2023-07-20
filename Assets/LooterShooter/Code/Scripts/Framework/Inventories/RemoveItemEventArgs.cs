using LooterShooter.Framework.Inventories.Items;

namespace LooterShooter.Framework.Inventories
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