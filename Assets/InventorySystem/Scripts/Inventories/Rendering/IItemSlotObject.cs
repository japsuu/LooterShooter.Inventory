using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories.Rendering
{
    public interface IItemSlotObject
    {
        public bool IsBoundsValid(InventoryBounds itemBounds, ItemMetadata thisItemMetadata);
    }
}