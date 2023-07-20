using LooterShooter.Framework.Clothing;
using LooterShooter.Framework.Inventories.Items;
using LooterShooter.Ui.InventoryRenderering.Slot;
using UnityEngine;

namespace LooterShooter.Ui.InventoryRenderering.Grid
{
    /// <summary>
    /// Renders player's currently equipped clothing inventory of the given type.
    /// </summary>
    public class ClothingInventoryRenderer : SpatialInventoryRenderer
    {
        [SerializeField] private ClothingSlot _linkedClothingSlot;

        private ClothingInventory _currentlyRenderedInventory;


        protected override bool ShouldRenderClothingInventory(ClothingType clothingType) => clothingType == _linkedClothingSlot.AcceptedClothingType;


        protected override void RenderNewClothingInventory(ClothingInventory clothingInventory)
        {
            // If we were rendering something before this change, stop it.
            if (_currentlyRenderedInventory != null)
                StopRenderInventory();

            // If the clothing type was completely removed, stop rendering and return.
            if (clothingInventory == null)
            {
                StopRenderInventory();
                return;
            }

            //gameObject.name = $"{nameof(ClothingInventoryRenderer)}: {clothingInventory.Inventory.Name}";
            _inventoryNameText.text = clothingInventory.ClothingItem.ItemData.ItemName;

            // Resize the slots image.
            _inventorySlotGrid.Initialize(clothingInventory.Inventory);

            foreach (InventoryItem item in clothingInventory.Inventory.GetAllItems())
            {
                CreateNewDraggableItem(item);
            }
            
            clothingInventory.Inventory.AddedItem += OnInventoryAddedItem;
            clothingInventory.Inventory.RemovedItem += OnInventoryRemovedItem;
            
            _currentlyRenderedInventory = clothingInventory;
        }


        private void StopRenderInventory()
        {
            if (_currentlyRenderedInventory == null)
                return;

            _currentlyRenderedInventory.Inventory.AddedItem += OnInventoryAddedItem;
            _currentlyRenderedInventory.Inventory.RemovedItem += OnInventoryRemovedItem;

            RemoveAllEntities();
        }
    }
}