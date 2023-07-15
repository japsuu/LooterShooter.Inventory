using InventorySystem.Inventories.Items;

namespace InventorySystem.Inventories.Rendering
{
    public class BaseInventoryRenderer : SpatialInventoryRenderer
    {
        private SpatialInventory _currentlyRenderedInventory;
        
        
        protected override bool ShouldRenderBaseInventory() => true;


        protected override void RenderNewBaseInventory(SpatialInventory spatialInventory)
        {
            // If we were rendering something before this change, stop it.
            if (_currentlyRenderedInventory != null)
                StopRenderInventory();

            // If the clothing type was completely removed, stop rendering and return.
            if (spatialInventory == null)
            {
                StopRenderInventory();
                return;
            }

            //gameObject.name = $"{nameof(ClothingInventoryRenderer)}: {clothingInventory.Inventory.Name}";
            _inventoryNameText.text = spatialInventory.Name;

            // Resize the slots image.
            _itemGrid.Initialize(spatialInventory);

            foreach (InventoryItem item in spatialInventory.GetAllItems())
            {
                CreateNewDraggableItem(item);
            }
            
            spatialInventory.AddedItem += OnInventoryAddedItem;
            spatialInventory.RemovedItem += OnInventoryRemovedItem;
            
            _currentlyRenderedInventory = spatialInventory;
        }


        private void StopRenderInventory()
        {
            if (_currentlyRenderedInventory == null)
                return;

            _currentlyRenderedInventory.AddedItem += OnInventoryAddedItem;
            _currentlyRenderedInventory.RemovedItem += OnInventoryRemovedItem;

            RemoveAllEntities();
        }
    }
}