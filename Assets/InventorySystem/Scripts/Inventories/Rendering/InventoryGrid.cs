using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Rendering
{
    [RequireComponent(typeof(Image))]
    public class InventoryGrid : DraggableItemReceiverObject
    {
        [SerializeField] private LayoutElement _inventoryLayoutElement;
        
        private Image _slotsImage;

        public InventoryRenderer InventoryRenderer { get; private set; }


        private void Awake()
        {
            _slotsImage = GetComponent<Image>();
        }


        public void Initialize(InventoryRenderer inventoryRenderer, float width, float height)
        {
            InventoryRenderer = inventoryRenderer;
            
            // Resize the grid.
            _inventoryLayoutElement.minWidth = width;
            _inventoryLayoutElement.minHeight = height;
            
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }


        public override bool CanDropDraggableItem(DraggableItem draggableItem)
        {
            throw new System.NotImplementedException();
        }


        protected override void HandleDroppedDraggableItem(DraggableItem draggableItem)
        {
            // TODO: get draggable position, and convert it into an inventory position. -> Try to transfer to inventory.
            throw new System.NotImplementedException();
        }
    }
}