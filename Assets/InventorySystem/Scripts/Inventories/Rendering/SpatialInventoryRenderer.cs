using System;
using System.Collections.Generic;
using InventorySystem.Clothing;
using InventorySystem.Inventories.Items;
using TMPro;
using UnityEngine;

namespace InventorySystem.Inventories.Rendering
{
    public abstract class SpatialInventoryRenderer : MonoBehaviour
    {
        // Serialized fields.
        [SerializeField] protected TMP_Text _inventoryNameText;
        [SerializeField] protected RectTransform _draggableItemsRootTransform;
        [SerializeField] protected ItemGrid _itemGrid;

        // Private fields.
        protected Dictionary<InventoryItem, DraggableItem> DraggableItems;


        protected virtual void Awake()
        {
            DraggableItems = new();
            
            // Destroy all children >:).
            for (int i = _draggableItemsRootTransform.childCount - 1; i >= 0; i--)
            {
                Destroy(_draggableItemsRootTransform.GetChild(i).gameObject);
            }
        }


        private void Start()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }


        protected virtual void OnEnable()
        {
            PlayerInventoryManager.BaseInventoryChanged += OnBaseInventoryChanged;
            PlayerInventoryManager.EquippedClothesInventoryChanged += OnEquippedClothesInventoryChanged;
        }


        protected virtual void OnDisable()
        {
            PlayerInventoryManager.BaseInventoryChanged -= OnBaseInventoryChanged;
            PlayerInventoryManager.EquippedClothesInventoryChanged -= OnEquippedClothesInventoryChanged;
        }


        private void OnBaseInventoryChanged(SpatialInventory baseInventory)
        {
            if (!ShouldRenderBaseInventory())
                return;
            
            RenderNewBaseInventory();
            
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(baseInventory != null);
            }
        }


        private void OnEquippedClothesInventoryChanged(ClothingType clothingType, ClothingInventory clothingInventory)
        {
            if (!ShouldRenderClothingInventory(clothingType))
                return;
            
            RenderNewClothingInventory(clothingInventory);
            
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(clothingInventory != null);
            }
        }


        protected virtual bool ShouldRenderBaseInventory() => false;
        protected virtual bool ShouldRenderClothingInventory(ClothingType clothingType) => false;


        protected virtual void RenderNewBaseInventory() { }
        protected virtual void RenderNewClothingInventory(ClothingInventory clothingInventory) { }
    }
}