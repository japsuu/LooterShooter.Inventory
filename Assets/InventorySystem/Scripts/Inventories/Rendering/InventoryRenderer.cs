﻿using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryRenderer : MonoBehaviour
    {
        // Singleton.
        public static InventoryRenderer Singleton;

        // Serialized fields.
        [SerializeField] private RectTransform _entityRootTransform;
        [SerializeField] private InventoryEntity _entityPrefab;
        [SerializeField] private float _slotSize = 100f;

        // Private fields.
        private RectTransform _rectTransform;
        private Inventory _renderingInventory;
        private Dictionary<InventoryItem, InventoryEntity> _entities;


        private void Awake()
        {
            Singleton = this;

            _entities = new();
            _rectTransform = GetComponent<RectTransform>();
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
            _renderingInventory.MovedItem += OnMovedItem;
            _renderingInventory.RemovedItem += OnRemovedItem;
            
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _renderingInventory.Bounds.Width * _slotSize);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _renderingInventory.Bounds.Height * _slotSize);

            foreach (InventoryItem item in _renderingInventory.Contents)
            {
                if(item != null)
                    CreateNewEntityForItem(item);
            }
        }


        private void StopRenderInventory()
        {
            _renderingInventory.AddedItem -= OnAddedItem;
            _renderingInventory.MovedItem -= OnMovedItem;
            _renderingInventory.RemovedItem -= OnRemovedItem;

            RemoveAllEntities();
        }


        private void CreateNewEntityForItem(InventoryItem item)
        {
            InventoryEntity entity = Instantiate(_entityPrefab, _entityRootTransform);

            entity.Initialize(item, _renderingInventory, _slotSize);
            
            _entities.Add(item, entity);
        }


        private void RemoveEntityOfItem(InventoryItem item)
        {
            if (_entities.Remove(item, out InventoryEntity entity))
            {
                Destroy(entity.gameObject);
            }
        }


        private void RemoveAllEntities()
        {
            foreach (InventoryItem item in _entities.Keys)
            {
                Destroy(_entities[item].gameObject);
            }
            
            _entities.Clear();
        }


        private void OnAddedItem(InventoryItem item) => CreateNewEntityForItem(item);


        private void OnRemovedItem(InventoryItem item) => RemoveEntityOfItem(item);


        private void OnMovedItem(InventoryItem item, Inventory targetInventory, Vector2Int oldPos, Vector2Int newPos)
        {
            if (!_entities.TryGetValue(item, out InventoryEntity entity))
                return;
                
            // Relocate the entity.
            entity.UpdatePositionAndVisuals();
        }
    }
}