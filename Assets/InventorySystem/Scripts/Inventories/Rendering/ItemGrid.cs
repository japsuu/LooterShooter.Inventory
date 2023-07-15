using InventorySystem.Inventories.Items;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(Image))]
    public class ItemGrid : DraggableItemReceiverObject
    {
        [SerializeField] private LayoutElement _inventoryLayoutElement;
        
        private Image _slotsImage;

        private SpatialInventory _targetSpatialInventory;


        public override bool DoSnapHighlighterToGrid => true;


        protected override void Awake()
        {
            base.Awake();
            
            _slotsImage = GetComponent<Image>();
        }


        public void Initialize(SpatialInventory targetSpatialInventory)
        {
            float width = targetSpatialInventory.Bounds.Width * Utilities.INVENTORY_SLOT_SIZE;
            float height = targetSpatialInventory.Bounds.Height * Utilities.INVENTORY_SLOT_SIZE;
            
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
            Vector2Int newPosition = Utilities.GetInventoryGridPosition(relativeTopLeftPosition);
            ItemRotation newRotation = draggableItem.Rotation;
            SpatialInventory newSpatialInventory = _targetSpatialInventory;

            draggableItem.InventoryItem.RequestMove(newSpatialInventory, newPosition, newRotation);
        }
    }
}