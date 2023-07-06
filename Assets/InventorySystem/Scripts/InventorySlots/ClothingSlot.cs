using InventorySystem.Clothing;
using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;

namespace InventorySystem.InventorySlots
{
    public class ClothingSlot : ItemSlot
    {
        protected override ItemType[] ItemTypeRestrictions => new[]
        {
            ItemType.CLOTHING
        };


        [SerializeField] private ClothingType _acceptedClothingType;


        public override bool CanDropDraggableItem(DraggableItem draggableItem)
        {
            if (!base.CanDropDraggableItem(draggableItem))
                return false;

            if (draggableItem.InventoryItem.Metadata.ItemData is not ClothingItem clothing)
                return false;
            
            return clothing.Type == _acceptedClothingType;
        }
    }
}