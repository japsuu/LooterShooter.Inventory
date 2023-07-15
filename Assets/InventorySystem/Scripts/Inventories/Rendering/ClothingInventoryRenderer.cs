using InventorySystem.Clothing;
using InventorySystem.Inventories.Items;
using InventorySystem.InventorySlots;
using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(RectTransform))]
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
            if (clothingInventory == null || clothingInventory.Inventory == null)
            {
                StopRenderInventory();
                return;
            }

            //gameObject.name = $"{nameof(ClothingInventoryRenderer)}: {clothingInventory.Inventory.Name}";
            _inventoryNameText.text = clothingInventory.ClothingItem.ItemData.ItemName;

            // Resize the slots image.
            _itemGrid.Initialize(clothingInventory.Inventory);

            foreach (InventoryItem item in clothingInventory.Inventory.GetAllItems())
            {
                CreateNewDraggableItem(item);
            }
            
            clothingInventory.Inventory.AddedItem += OnInventoryAddedItem;
            clothingInventory.Inventory.RemovedItem += OnInventoryRemovedItem;
            
            _currentlyRenderedInventory = clothingInventory;
        }


        private void OnInventoryAddedItem(AddItemEventArgs obj) => CreateNewDraggableItem(obj.AddedItem);


        private void OnInventoryRemovedItem(RemoveItemEventArgs obj) => RemoveEntityOfItem(obj.RemovedItem);


        private void CreateNewDraggableItem(InventoryItem inventoryItem)
        {
            DraggableItem draggableItem = Instantiate(PrefabReferences.Singleton.DraggableItemPrefab, _draggableItemsRootTransform);

            draggableItem.Initialize(inventoryItem, false);
            
            DraggableItems.Add(inventoryItem, draggableItem);
            
            Logger.Log(LogLevel.DEBUG, gameObject.name, $"CreateNewDraggable '{inventoryItem.Metadata.ItemData.ItemName}'@{inventoryItem.Bounds.Position}");
        }


        private void RemoveEntityOfItem(InventoryItem inventoryItemSnapshot)
        {
            if (DraggableItems.Remove(inventoryItemSnapshot, out DraggableItem entity))
            {
                if(entity != null)
                    Destroy(entity.gameObject);
            }
        }


        private void StopRenderInventory()
        {
            if (_currentlyRenderedInventory == null)
                return;
            
            if(_currentlyRenderedInventory.Inventory != null)
            {
                _currentlyRenderedInventory.Inventory.AddedItem += OnInventoryAddedItem;
                _currentlyRenderedInventory.Inventory.RemovedItem += OnInventoryRemovedItem;
            }
            
            RemoveAllEntities();
        }


        private void RemoveAllEntities()
        {
            foreach (InventoryItem item in DraggableItems.Keys)
            {
                if(DraggableItems[item] != null)
                    Destroy(DraggableItems[item].gameObject);
            }
            
            DraggableItems.Clear();
        }
    }
}