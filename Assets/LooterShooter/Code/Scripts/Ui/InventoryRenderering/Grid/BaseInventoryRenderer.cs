using LooterShooter.Framework.Inventories;
using LooterShooter.Framework.Inventories.Items;

namespace LooterShooter.Ui.InventoryRenderering.Grid
{
    /// <summary>
    /// Renders the player's "persistent" base inventory.
    /// </summary>
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

            _inventoryNameText.text = spatialInventory.Name;

            // Resize the slots image.
            _inventorySlotGrid.Initialize(spatialInventory);

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