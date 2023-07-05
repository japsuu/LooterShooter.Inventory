﻿using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.InventorySlots
{
    [RequireComponent(typeof(Image))]
    public abstract class ItemSlot : DraggableItemReceiverObject, IInventory
    {
        /// <summary>
        /// What types of items can be dropped to this slot. Leave empty to not allow any items.
        /// </summary>
        protected abstract ItemType[] ItemTypeRestrictions { get; }
        
        [Tooltip("Unique name of this slot. Used for saving it.")]
        [SerializeField] protected string _uniqueSlotName = "gear";
        
        [SerializeField] private Image _assignedItemImage;
        
        public string Name => $"slot_{_uniqueSlotName}";

        protected InventoryItem AssignedItem;


        public virtual bool TryCreateNewInventoryItem(ItemMetadata metadata, Vector2Int position, ItemRotation rotation, InventoryBounds? boundsToIgnore, out InventoryItem createdInventoryItem)
        {
            if (AssignedItem != null)
            {
                createdInventoryItem = null;
                return false;
            }

            InventoryBounds bounds = new(metadata.ItemData, position, rotation);
            createdInventoryItem = new InventoryItem(metadata, bounds, rotation, this);
            return true;
        }


        public virtual void RemoveItem(Vector2Int itemPosition)
        {
            AssignedItem = null;

            OnItemRemoved();
        }


        public virtual void AddItem(InventoryItem item)
        {
            AssignedItem = item;

            OnItemAdded();
        }


        protected virtual void OnItemRemoved()
        {
            _assignedItemImage.sprite = null;
            _assignedItemImage.color = Color.clear;
        }


        protected virtual void OnItemAdded()
        {
            _assignedItemImage.sprite = AssignedItem.Metadata.ItemData.Sprite;
            _assignedItemImage.color = Color.white;
        }


        public override bool CanDropDraggableItem(DraggableItem draggableItem)
        {
            if (ItemTypeRestrictions == null || ItemTypeRestrictions.Length == 0)
                return true;
            
            foreach (ItemType restriction in ItemTypeRestrictions)
            {
                if (restriction == draggableItem.InventoryItem.Metadata.ItemData.ItemType)
                    return true;
            }

            return false;
        }


        protected override void HandleDroppedDraggableItem(DraggableItem draggableItem)
        {
            draggableItem.InventoryItem.RequestMove(this, Vector2Int.zero, ItemRotation.DEG_0);
        }


        protected virtual void OnValidate()
        {
            _uniqueSlotName = _uniqueSlotName.Replace(' ', '_');
        }
    }
}