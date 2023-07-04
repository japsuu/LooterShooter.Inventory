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


        public void RequestMoveItem(Vector2Int boundsPosition, Vector2Int newPos, ItemRotation newRotation, IInventory targetInventory)
        {
            throw new System.NotImplementedException();
        }


        public bool IsPositionInsideInventory(Vector2Int position)
        {
            throw new System.NotImplementedException();
        }


        public bool TryGetItemAtPosition(Vector2Int position, out InventoryItem item)
        {
            throw new System.NotImplementedException();
        }


        public bool IsItemBoundsValid(InventoryBounds itemBounds, InventoryBounds? existingBoundsToIgnore = null)
        {
            throw new System.NotImplementedException();
        }


        public InventoryItem TransferExistingInventoryItem(InventoryItem existingItem, InventoryBounds bounds, ItemRotation rotation)
        {
            _assignedItem = new InventoryItem(existingItem.Metadata, bounds, rotation);
            _assignedItem.AssignInventory(this);
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