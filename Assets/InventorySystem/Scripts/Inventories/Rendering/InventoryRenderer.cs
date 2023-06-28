using System.Collections.Generic;
using InventorySystem.Inventories.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryRenderer : MonoBehaviour
    {
        // Serialized fields.
        [SerializeField] private TMP_Text _inventoryNameText;
        [SerializeField] private LayoutElement _inventoryLayoutElement;
        [SerializeField] private RectTransform _entityRootTransform;
        [SerializeField] private InventorySlotsRenderer _slotsRenderer;
        [SerializeField] private InventoryEntity _entityPrefab;

        // Private fields.
        private Dictionary<ItemMetadata, InventoryEntity> _entities;
        
        // Public fields.
        public Inventory TargetInventory { get; private set; }
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


        public void RenderInventory(Inventory inventory, string inventoryName)
        {
            if (TargetInventory != null)
                StopRenderInventory();

            _inventoryNameText.text = inventoryName;
            
            TargetInventory = inventory;

            float width = TargetInventory.Bounds.Width * Utilities.INVENTORY_SLOT_SIZE;
            float height = TargetInventory.Bounds.Height * Utilities.INVENTORY_SLOT_SIZE;
            
            // Resize the grid.
            _inventoryLayoutElement.minWidth = width;
            _inventoryLayoutElement.minHeight = height;
            
            // Resize the slots image.
            _slotsRenderer.Initialize(this, width, height);

            foreach (ItemMetadata item in TargetInventory.GetItems())
            {
                CreateNewEntityForItem(item);
            }
        }


        public void CreateNewEntityForItem(ItemMetadata itemMetadata)
        {
            InventoryEntity entity = Instantiate(_entityPrefab, _entityRootTransform);

            entity.Initialize(itemMetadata, TargetInventory);
            
            _entities.Add(itemMetadata, entity);
        }


        public void RemoveEntityOfItem(ItemMetadata itemMetadata)
        {
            if (_entities.Remove(itemMetadata, out InventoryEntity entity))
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