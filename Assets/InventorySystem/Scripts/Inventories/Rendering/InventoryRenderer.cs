using System.Collections.Generic;
using InventorySystem.Inventories.Items;
using TMPro;
using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryRenderer : MonoBehaviour
    {
        // Serialized fields.
        [SerializeField] private TMP_Text _inventoryNameText;
        [SerializeField] private RectTransform _entityRootTransform;
        [SerializeField] private InventoryGrid _inventoryGrid;
        [SerializeField] private DraggableItem _draggableItemPrefab;

        // Private fields.
        private Dictionary<InventoryItem, DraggableItem> _entities;
        private Inventory _targetInventory;
        
        // Public fields.
        //public RectTransform EntityRootTransform => _entityRootTransform;


        private void Awake()
        {
            _entities = new();
            
            // Destroy all children >:).
            for (int i = _entityRootTransform.childCount - 1; i >= 0; i--)
            {
                Destroy(_entityRootTransform.GetChild(i).gameObject);
            }
        }


        public void RenderInventory(Inventory inventory, string inventoryName)
        {
            if (_targetInventory != null)
                StopRenderInventory();

            gameObject.name = $"InventoryRenderer: {inventoryName}";
            _inventoryNameText.text = inventoryName;
            
            _targetInventory = inventory;

            float width = _targetInventory.Bounds.Width * Utilities.INVENTORY_SLOT_SIZE;
            float height = _targetInventory.Bounds.Height * Utilities.INVENTORY_SLOT_SIZE;

            // Resize the slots image.
            _inventoryGrid.Initialize(_targetInventory, width, height);

            foreach (InventoryItem item in _targetInventory.GetAllItems())
            {
                CreateNewDraggableItem(item);
            }
        }


        public void CreateNewDraggableItem(InventoryItem inventoryItem)
        {
            DraggableItem draggableItem = Instantiate(_draggableItemPrefab, _entityRootTransform);

            draggableItem.Initialize(inventoryItem);
            
            _entities.Add(inventoryItem, draggableItem);
            
            Logger.Log(LogLevel.DEBUG, gameObject.name, $"CreateNewDraggable '{inventoryItem.ItemDataReference.Name}'@{inventoryItem.Bounds.Position}");
        }


        public void RemoveEntityOfItem(InventoryItem inventoryItemSnapshot)
        {
            if (_entities.Remove(inventoryItemSnapshot, out DraggableItem entity))
            {
                if(entity != null)
                    Destroy(entity.gameObject);
            }
        }


        private void StopRenderInventory()
        {
            RemoveAllEntities();
        }


        private void RemoveAllEntities()
        {
            foreach (InventoryItem item in _entities.Keys)
            {
                if(_entities[item] != null)
                    Destroy(_entities[item].gameObject);
            }
            
            _entities.Clear();
        }
    }
}