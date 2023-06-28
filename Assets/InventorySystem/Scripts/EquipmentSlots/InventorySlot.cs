using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;

namespace InventorySystem.EquipmentSlots
{
    public class InventorySlot : DraggableItemReceiverObject
    {
        [Tooltip("What types of items can be dropped to this slot. Leave empty to allow any items.")]
        [SerializeField] private ItemType[] _itemTypeRestrictions;


        public override bool CanDropDraggableItem(DraggableItem draggableItem)
        {
            foreach (ItemType restriction in _itemTypeRestrictions)
            {
                if (restriction == draggableItem.ItemReference.ItemDataReference.ItemType)
                    return true;
            }

            return false;
        }


        protected override void HandleDroppedDraggableItem(DraggableItem draggableItem)
        {
            throw new System.NotImplementedException();
        }
    }
}