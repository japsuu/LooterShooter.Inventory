using LooterShooter.Framework.Inventories;
using LooterShooter.Framework.Inventories.Items;
using UnityEngine;
using UnityEngine.UI;

namespace LooterShooter.Ui.InventoryRenderering.Grid
{
    /// <summary>
    /// Visual grid of inventory slots that <see cref="DraggableItem"/>s can be dropped on to.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventorySlotGrid : DraggableItemDropTarget
    {
        [SerializeField] private LayoutElement _inventoryLayoutElement;
        
        private Image _slotsImage;

        private SpatialInventory _targetSpatialInventory;


        public override bool DoSnapHighlighterToGrid => true;


        public void Initialize(SpatialInventory targetSpatialInventory)
        {
            _slotsImage = GetComponent<Image>();
            
            float width = targetSpatialInventory.Bounds.Width * InventoryUtilities.INVENTORY_SLOT_SIZE;
            float height = targetSpatialInventory.Bounds.Height * InventoryUtilities.INVENTORY_SLOT_SIZE;
            
            _targetSpatialInventory = targetSpatialInventory;
            
            // Resize the grid.
            _inventoryLayoutElement.minWidth = width;
            _inventoryLayoutElement.minHeight = height;
            
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }


        public override bool CanDropDraggableItem(DraggableItem draggableItem)
        {
            return _targetSpatialInventory.IsItemBoundsValid(draggableItem.GetBounds(RectTransform), draggableItem.InventoryItem.Bounds);
        }


        protected override void HandleDroppedDraggableItem(DraggableItem draggableItem)
        {
            Vector2 relativeTopLeftPosition = draggableItem.GetTopLeftCornerRelativeToRect(RectTransform);
            Vector2Int newPosition = InventoryUtilities.GetInventoryGridPosition(relativeTopLeftPosition);
            InventoryItemRotation newRotation = draggableItem.Rotation;
            SpatialInventory newSpatialInventory = _targetSpatialInventory;

            draggableItem.InventoryItem.RequestMove(newSpatialInventory, newPosition, newRotation);
        }
    }
}