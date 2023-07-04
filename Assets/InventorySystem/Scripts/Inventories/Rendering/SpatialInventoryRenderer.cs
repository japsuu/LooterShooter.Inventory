using System.Collections.Generic;
using InventorySystem.Inventories.Items;
using TMPro;
using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(RectTransform))]
    public class SpatialInventoryRenderer : MonoBehaviour
    {
        // Serialized fields.
        [SerializeField] private TMP_Text _inventoryNameText;
        [SerializeField] private RectTransform _entityRootTransform;
        [SerializeField] private InventoryGrid _inventoryGrid;
        [SerializeField] private DraggableItem _draggableItemPrefab;

        // Private fields.
        private Dictionary<InventoryItem, DraggableItem> _entities;
        private SpatialInventory _targetSpatialInventory;


        private void Awake()
        {
            _entities = new();
            
            // Destroy all children >:).
            for (int i = _entityRootTransform.childCount - 1; i >= 0; i--)
            {
                Destroy(_entityRootTransform.GetChild(i).gameObject);
            }
        }


        public void RenderInventory(SpatialInventory spatialInventory, string inventoryName)
        {
            if (_targetSpatialInventory != null)
                StopRenderInventory();

            gameObject.name = $"InventoryRenderer: {inventoryName}";
            _inventoryNameText.text = inventoryName;
            
            _targetSpatialInventory = spatialInventory;

            float width = _targetSpatialInventory.Bounds.Width * Utilities.INVENTORY_SLOT_SIZE;
            float height = _targetSpatialInventory.Bounds.Height * Utilities.INVENTORY_SLOT_SIZE;

            // Resize the slots image.
            _inventoryGrid.Initialize(_targetSpatialInventory, width, height);

            foreach (InventoryItem item in _targetSpatialInventory.GetAllItems())
            {
                CreateNewDraggableItem(item);
            }
        }


        public void CreateNewDraggableItem(InventoryItem inventoryItem)
        {
            DraggableItem draggableItem = Instantiate(_draggableItemPrefab, _entityRootTransform);

            draggableItem.Initialize(inventoryItem);
            
            _entities.Add(inventoryItem, draggableItem);
            
            Logger.Log(LogLevel.DEBUG, gameObject.name, $"CreateNewDraggable '{inventoryItem.Metadata.ItemData.Name}'@{inventoryItem.Bounds.Position}");
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