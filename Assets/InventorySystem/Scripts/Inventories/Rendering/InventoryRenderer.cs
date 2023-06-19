using System.Collections.Generic;
using InventorySystem.Inventories.Items;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryRenderer : MonoBehaviour
    {
        // Singleton. TODO: Remove. Add a InventoryManager with a static inventory to Rendering root. Methods: AddStartingInventory, AddClothes etc.
        public static InventoryRenderer Singleton;

        // Serialized fields.
        [SerializeField] private LayoutElement _inventoryLayoutElement;
        [SerializeField] private RectTransform _entityRootTransform;
        [SerializeField] private Image _slotsImage;
        [SerializeField] private InventoryEntity _entityPrefab;
        [SerializeField] private float _slotSize = 100f;

        // Private fields.
        private Inventory _renderingInventory;
        private Dictionary<InventoryItem, InventoryEntity> _entities;
        

        private void Awake()
        {
            Singleton = this;

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


        public void RenderInventory(Inventory inventory)
        {
            if (_renderingInventory != null)
                StopRenderInventory();
            
            StartRenderInventory(inventory);
        }


        private void StartRenderInventory(Inventory inventory)
        {
            _renderingInventory = inventory;
            _renderingInventory.AddedItem += OnAddedItem;
            _renderingInventory.RemovedItem += OnRemovedItem;

            float width = _renderingInventory.Bounds.Width * _slotSize;
            float height = _renderingInventory.Bounds.Height * _slotSize;
            
            // Resize the grid.
            _inventoryLayoutElement.minWidth = width;
            _inventoryLayoutElement.minHeight = height;
            
            // Resize the slots image.
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            foreach (InventoryItem item in _renderingInventory.GetItems())
            {
                CreateNewEntityForItem(item);
            }
        }


        private void StopRenderInventory()
        {
            _renderingInventory.AddedItem -= OnAddedItem;
            _renderingInventory.RemovedItem -= OnRemovedItem;

            RemoveAllEntities();
        }


        private void CreateNewEntityForItem(InventoryItem item)
        {
            InventoryEntity entity = Instantiate(_entityPrefab, _entityRootTransform);

            entity.Initialize(item, _renderingInventory);
            
            _entities.Add(item, entity);
        }


        private void RemoveEntityOfItem(InventoryItem item)
        {
            if (_entities.Remove(item, out InventoryEntity entity))
            {
                if(entity != null)
                    Destroy(entity.gameObject);
            }
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


        private void OnAddedItem(InventoryItem item) => CreateNewEntityForItem(item);


        private void OnRemovedItem(InventoryItem item) => RemoveEntityOfItem(item);
    }
}