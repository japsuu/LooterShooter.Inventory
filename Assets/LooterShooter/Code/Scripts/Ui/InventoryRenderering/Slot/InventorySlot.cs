using LooterShooter.Framework.Inventories;
using LooterShooter.Framework.Inventories.Items;
using UnityEngine;
using UnityEngine.UI;

namespace LooterShooter.Ui.InventoryRenderering.Slot
{
    [RequireComponent(typeof(Image))]
    public abstract class InventorySlot : DraggableItemDropTarget, IInventory    //TODO: Remove IInventory?
    {
        /// <summary>
        /// What types of items can be dropped to this slot. Leave empty to allow any items.
        /// </summary>
        protected abstract ItemType[] ItemTypeRestrictions { get; }
        
        protected abstract string Identifier { get; }
        
        [SerializeField] private Image _assignedItemImage;
        
        protected InventoryItem AssignedItem;

        public string Name => $"slot_{Identifier.ToLower()}";
        public ItemMetadata AssignedItemMetadata => AssignedItem.Metadata;
        public override bool DoSnapHighlighterToGrid => false;


        public virtual bool TryCreateNewInventoryItem(ItemMetadata metadata, Vector2Int position, InventoryItemRotation rotation, InventoryBounds? boundsToIgnore, out InventoryItem createdInventoryItem)
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
            if(AssignedItem == null)
                return;

            ItemMetadata removedItem = AssignedItem.Metadata;
            AssignedItem = null;

            OnItemRemoved(removedItem);
        }


        public virtual void AddItem(InventoryItem item)
        {
            AssignedItem = item;

            OnItemAdded();
        }


        protected virtual void OnItemRemoved(ItemMetadata itemMetadata)
        {
            _assignedItemImage.sprite = null;
            _assignedItemImage.color = Color.clear;
        }


        protected virtual void OnItemAdded()
        {
            _assignedItemImage.sprite = AssignedItem.Metadata.ItemData.Sprite;
            _assignedItemImage.preserveAspect = true;
            _assignedItemImage.color = Color.white;
        }


        public override bool CanDropDraggableItem(DraggableItem draggableItem)
        {
            if (AssignedItem != null)
                return false;
            
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
            draggableItem.InventoryItem.RequestMove(this, Vector2Int.zero, InventoryItemRotation.DEG_0);
        }


        // protected virtual void OnValidate()
        // {
        //     _uniqueSlotName = _uniqueSlotName.Replace(' ', '_');
        // }
    }
}