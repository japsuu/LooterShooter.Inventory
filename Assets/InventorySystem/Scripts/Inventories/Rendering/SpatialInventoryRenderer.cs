using System.Collections.Generic;
using InventorySystem.Inventories.Items;
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
        [SerializeField] private RectTransform _floaterParentRectTransform;
        [SerializeField] private InventoryGridItemReceiver _slotsRenderer;
        [SerializeField] private Floater _floaterPrefab;

        // Private fields.
        private Dictionary<ItemMetadata, Floater> _entities;
        
        // Public fields.
        public InventoryX TargetInventoryX { get; private set; }
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


        public void RenderInventory(InventoryX inventoryX, string inventoryName)
        {
            if (TargetInventoryX != null)
                StopRenderInventory();

            _inventoryNameText.text = inventoryName;
            
            TargetInventoryX = inventoryX;

            float width = TargetInventoryX.Bounds.Width * Utilities.INVENTORY_SLOT_SIZE;
            float height = TargetInventoryX.Bounds.Height * Utilities.INVENTORY_SLOT_SIZE;
            
            // Resize the grid.
            _inventoryLayoutElement.minWidth = width;
            _inventoryLayoutElement.minHeight = height;
            
            // Resize the slots image.
            _slotsRenderer.Initialize(this, width, height);

            foreach (ItemMetadata item in TargetInventoryX.GetItems())
            {
                CreateNewEntityForItem(item);
            }
        }


        public void CreateNewEntityForItem(ItemMetadata data)
        {
            Floater floater = Instantiate(_floaterPrefab, _floaterParentRectTransform);

            floater.Initialize(data, _slotsRenderer);
            
            _entities.Add(data, floater);
        }


        public void RemoveEntityOfItem(ItemMetadata data)
        {
            if (_entities.Remove(data, out Floater entity))
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
            foreach (ItemMetadata item in _entities.Keys)
            {
                if(_entities[item] != null)
                    Destroy(_entities[item].gameObject);
            }
            
            _entities.Clear();
        }
    }
}