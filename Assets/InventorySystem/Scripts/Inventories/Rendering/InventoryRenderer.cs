﻿using System.Collections.Generic;
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
        [SerializeField] private float _slotSize = 100f;

        // Private fields.
        private Dictionary<InventoryItem, InventoryEntity> _entities;
        
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

            float width = TargetInventory.Bounds.Width * _slotSize;
            float height = TargetInventory.Bounds.Height * _slotSize;
            
            // Resize the grid.
            _inventoryLayoutElement.minWidth = width;
            _inventoryLayoutElement.minHeight = height;
            
            // Resize the slots image.
            _slotsRenderer.Initialize(this, width, height);

            foreach (InventoryItem item in TargetInventory.GetItems())
            {
                CreateNewEntityForItem(item);
            }
        }


        public void CreateNewEntityForItem(InventoryItem item)
        {
            InventoryEntity entity = Instantiate(_entityPrefab, _entityRootTransform);

            entity.Initialize(item, TargetInventory);
            
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