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
        [SerializeField] private RectTransform _floaterParentRectTransform;
        [SerializeField] private SpatialItemReceiver _slotsRenderer;
        [SerializeField] private SpatialFloater _floaterPrefab;

        // Private fields.
        private Dictionary<InventoryItem<>, SpatialFloater> _entities;
        
        // Public fields.
        public SpatialInventory TargetSpatialInventory { get; private set; }
        public RectTransform FloaterParentRectTransform => _floaterParentRectTransform;


        private void Awake()
        {
            _entities = new();
        }


        private void Start()
        {
            // Destroy all children >:).
            for (int i = _floaterParentRectTransform.childCount - 1; i >= 0; i--)
            {
                Destroy(_floaterParentRectTransform.GetChild(i).gameObject);
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

            foreach (InventoryItem<> item in TargetSpatialInventory.GetItems())
            {
                CreateNewEntityForItem(item);
            }
        }


        public void CreateNewEntityForItem(InventoryItem<> data)
        {
            SpatialFloater floater = Instantiate(_floaterPrefab, _floaterParentRectTransform);

            floater.Initialize(data, _slotsRenderer);
            
            _entities.Add(data, floater);
        }


        public void RemoveEntityOfItem(InventoryItem<> data)
        {
            if (_entities.Remove(data, out SpatialFloater entity))
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
            foreach (InventoryItem<> item in _entities.Keys)
            {
                if(_entities[item] != null)
                    Destroy(_entities[item].gameObject);
            }
            
            _entities.Clear();
        }
    }
}