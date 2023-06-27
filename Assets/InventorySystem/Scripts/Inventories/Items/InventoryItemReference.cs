namespace InventorySystem.Inventories.Items
{
    public class InventoryItemReference<T> where T : Inventory
    {
        public readonly InventoryItem Item;
        public readonly T ContainingInventory;
        public int CurrentInventoryIndex { get; private set; }
        
        
        public InventoryItemReference(InventoryItem item, T containingInventory, int currentInventoryIndex)
        {
            Item = item;
            ContainingInventory = containingInventory;
            CurrentInventoryIndex = currentInventoryIndex;
        }


        public void UpdateInventoryIndex(int newIndex)
        {
            CurrentInventoryIndex = newIndex;
        }
    }
}