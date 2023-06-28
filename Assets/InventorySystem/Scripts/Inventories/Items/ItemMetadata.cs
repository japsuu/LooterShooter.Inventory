namespace InventorySystem.Inventories.Items
{
    public class ItemMetadata
    {
        public ItemData ItemDataReference { get; }
        public InventoryBounds Bounds { get; private set; }
        public ItemRotation RotationInInventory { get; private set; }
        
        
        public ItemMetadata(ItemData itemDataReference, InventoryBounds bounds, ItemRotation rotationInInventory)
        {
            ItemDataReference = itemDataReference;
            
            UpdateBounds(bounds, rotationInInventory);
        }


        public void UpdateBounds(InventoryBounds bounds, ItemRotation rotation)
        {
            Bounds = bounds;
            RotationInInventory = rotation;
        }
    }
}