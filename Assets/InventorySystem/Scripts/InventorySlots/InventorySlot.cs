using InventorySystem.Inventories;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;

namespace InventorySystem.InventorySlots
{
    public class InventorySlot : DraggableItemReceiverObject, IInventory
    {
        [SerializeField] private string _uniqueSlotName = "gear";
        
        [Tooltip("What types of items can be dropped to this slot. Leave empty to allow any items.")]
        [SerializeField] private ItemType[] _itemTypeRestrictions;
        
        public string Name => $"slot_{_uniqueSlotName}";

        private InventoryItem _assignedItem;


        public void RequestMoveItem(Vector2Int oldPosition, Vector2Int newPosition, ItemRotation newRotation, IInventory targetInventory)
        {
            targetInventory.ReceiveExistingInventoryItem(_assignedItem, )
        }


        public bool IsPositionInsideInventory(Vector2Int position) => true;


        public bool TryGetItemAtPosition(Vector2Int position, out InventoryItem item)
        {
            item = _assignedItem;
            return _assignedItem != null;
        }


        public bool IsItemBoundsValid(InventoryBounds itemBounds, InventoryBounds? existingBoundsToIgnore = null) => true;


        public InventoryItem ReceiveExistingInventoryItem(InventoryItem existingItem, InventoryBounds bounds, ItemRotation rotation)
        {
            _assignedItem = new InventoryItem(existingItem.Metadata, bounds, rotation, this);
            return _assignedItem;
        }


        public override bool CanDropDraggableItem(DraggableItem draggableItem)
        {
            foreach (ItemType restriction in _itemTypeRestrictions)
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


        private void OnValidate()
        {
            _uniqueSlotName = _uniqueSlotName.Replace(' ', '_');
        }
    }
}