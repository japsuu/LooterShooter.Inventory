using System.Collections.Generic;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Spatial;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(RectTransform))]
    public class SpatialInventoryRenderer : MonoBehaviour
    {
        // Serialized fields.
        [SerializeField] private TMP_Text _inventoryNameText;
        [SerializeField] private LayoutElement _inventoryLayoutElement;
        [SerializeField] private RectTransform _entityRootTransform;
        [SerializeField] private InventorySlotsRenderer _slotsRenderer;
        [SerializeField] private InventoryEntity _entityPrefab;

        // Private fields.
        private Dictionary<InventoryItem, InventoryEntity> _entities;
        
        // Public fields.
        public SpatialInventory TargetSpatialInventory { get; private set; }
        public RectTransform EntityRootTransform => _entityRootTransform;


        private void Awake()
        {
            _entities = new();
        }


        private void Start()
        {
            // Destroy all children >:).
            for (int i = _entityRootTransform.childCount - 1; i >= 0; i--)
            {
                Destroy(_entityRootTransform.GetChild(i).gameObject);
            }
        }


        public void RenderInventory(SpatialInventory spatialInventory, string inventoryName)
        {
            if (TargetSpatialInventory != null)
                StopRenderInventory();

            _inventoryNameText.text = inventoryName;
            
            TargetSpatialInventory = spatialInventory;

            float width = TargetSpatialInventory.Bounds.Width * Utilities.INVENTORY_SLOT_SIZE;
            float height = TargetSpatialInventory.Bounds.Height * Utilities.INVENTORY_SLOT_SIZE;
            
            // Resize the grid.
            _inventoryLayoutElement.minWidth = width;
            _inventoryLayoutElement.minHeight = height;
            
            // Resize the slots image.
            _slotsRenderer.Initialize(this, width, height);

            foreach (InventoryItem item in TargetSpatialInventory.GetItems())
            {
                CreateNewEntityForItem(item);
            }
        }


        public void CreateNewEntityForItem(InventoryItem item)
        {
            InventoryEntity entity = Instantiate(_entityPrefab, _entityRootTransform);

            entity.Initialize(item, _slotsRenderer);
            
            _entities.Add(item, entity);
        }


        public void RemoveEntityOfItem(InventoryItem item)
        {
            if (_entities.Remove(item, out InventoryEntity entity))
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