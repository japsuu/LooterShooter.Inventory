using LooterShooter.Framework.Inventories.Items;
using UnityEngine;

namespace LooterShooter.Ui.InventoryRenderering.Slot
{
    public class TrashSlot : InventorySlot
    {
        protected override ItemType[] ItemTypeRestrictions => null;
        protected override string Identifier => "trash";


        public override bool CanDropDraggableItem(DraggableItem draggableItem) => true;


        public override void AddItem(InventoryItem item) { }


        public override void RemoveItem(Vector2Int itemPosition) { }
    }
}