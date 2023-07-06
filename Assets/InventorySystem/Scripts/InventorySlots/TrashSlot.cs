using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using UnityEngine;

namespace InventorySystem.InventorySlots
{
    public class TrashSlot : ItemSlot
    {
        protected override ItemType[] ItemTypeRestrictions => null;


        public override bool CanDropDraggableItem(DraggableItem draggableItem) => true;


        public override void AddItem(InventoryItem item) { }


        public override void RemoveItem(Vector2Int itemPosition) { }
    }
}