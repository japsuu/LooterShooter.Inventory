using System.Collections.Generic;
using InventorySystem.Inventories.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Spatial.Rendering
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
        private Dictionary<InventoryData, InventoryEntity> _entities;
        
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

            foreach (InventoryData item in TargetSpatialInventory.GetItems())
            {
                CreateNewEntityForItem(item);
            }
        }


        public void CreateNewEntityForItem(InventoryData data)
        {
            InventoryEntity entity = Instantiate(_entityPrefab, _entityRootTransform);

            entity.Initialize(data, _slotsRenderer);
            
            _entities.Add(data, entity);
        }


        public void RemoveEntityOfItem(InventoryData data)
        {
            if (_entities.Remove(data, out InventoryEntity entity))
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
            foreach (InventoryData item in _entities.Keys)
            {
                if(_entities[item] != null)
                    Destroy(_entities[item].gameObject);
            }
            
            _entities.Clear();
        }
    }
}