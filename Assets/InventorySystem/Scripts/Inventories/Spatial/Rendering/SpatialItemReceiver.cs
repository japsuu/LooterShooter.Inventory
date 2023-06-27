using InventorySystem.Inventories.Items;
using InventorySystem.Inventories.Rendering;
using InventorySystem.Inventories.Spatial.Items;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Inventories.Spatial.Rendering
{
    [RequireComponent(typeof(Image))]
    public class SpatialItemReceiver : InventoryItemReceiver
    {
        private Image _slotsImage;
        private SpatialInventoryRenderer _inventoryRenderer;
        
        public override RectTransform FloaterParentRectTransform => _inventoryRenderer.FloaterParentRectTransform;


        private void Awake()
        {
            _slotsImage = GetComponent<Image>();
        }


        public void Initialize(SpatialInventoryRenderer spatialInventoryRenderer, float width, float height)
        {
            _inventoryRenderer = spatialInventoryRenderer;
            
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _slotsImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }


        public override bool CanDropFloater(SpatialFloater floater)
        {
            // Accept floater if their bounds do not collide with anything.
            return _inventoryRenderer.TargetSpatialInventory.IsBoundsValid(floater.GetBounds(FloaterParentRectTransform));
        }


        public override void HandleDroppedFloater(SpatialFloater floater)
        {
            if(!CanDropFloater(floater))
                return;
            
            floater.FloaterData.InventoryItem.ContainingInventory.TryMoveItem();
        }
    }
}