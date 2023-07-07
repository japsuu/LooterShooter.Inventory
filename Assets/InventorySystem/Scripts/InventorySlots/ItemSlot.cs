using InventorySystem.Inventories;
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
        /// What types of items can be dropped to this slot. Leave empty to allow any items.
        /// </summary>
        protected abstract ItemType[] ItemTypeRestrictions { get; }
        
        //[Tooltip("Unique name of this slot. Used for saving it's contents.")]
        //protected string _uniqueSlotName = "gear";
        
        [SerializeField] private Image _assignedItemImage;
        
        public string Name => "slot_CHANGE_ME";

        protected InventoryItem AssignedItem;
        
        public override bool DoSnapHighlighterToGrid => false;


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
            draggableItem.InventoryItem.RequestMove(this, Vector2Int.zero, ItemRotation.DEG_0);
        }


        // protected virtual void OnValidate()
        // {
        //     _uniqueSlotName = _uniqueSlotName.Replace(' ', '_');
        // }
    }
}